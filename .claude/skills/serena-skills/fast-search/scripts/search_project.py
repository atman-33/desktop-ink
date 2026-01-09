#!/usr/bin/env python3
"""
Fast project-wide regex search with glob filters and optional .gitignore support.
"""
import argparse
import json
import os
import re
import fnmatch
from pathlib import Path
from typing import Iterable, List, Optional, Tuple

from joblib import Parallel, delayed
import pathspec

DEFAULT_CODE_EXTS = {
    ".py",
    ".ts",
    ".tsx",
    ".js",
    ".jsx",
    ".java",
    ".kt",
    ".kts",
    ".go",
    ".rs",
    ".cpp",
    ".cc",
    ".c",
    ".h",
    ".hpp",
    ".cs",
    ".swift",
    ".rb",
    ".php",
    ".html",
    ".css",
    ".scss",
    ".sql",
    ".yaml",
    ".yml",
    ".json",
    ".toml",
    ".ini",
    ".md",
}


def expand_braces(pattern: str) -> List[str]:
    patterns = [pattern]
    while any("{" in p for p in patterns):
        new_patterns: List[str] = []
        for p in patterns:
            match = re.search(r"\{([^{}]+)\}", p)
            if match:
                prefix = p[: match.start()]
                suffix = p[match.end() :]
                options = match.group(1).split(",")
                for option in options:
                    new_patterns.append(f"{prefix}{option}{suffix}")
            else:
                new_patterns.append(p)
        patterns = new_patterns
    return patterns


def glob_match(pattern: str, path: str) -> bool:
    pattern = pattern.replace("\\", "/")
    path = path.replace("\\", "/")
    if "**" in pattern:
        translated = re.compile(fnmatch.translate(pattern))
        if translated.match(path):
            return True
        if "/**/" in pattern:
            zero_dir_pattern = pattern.replace("/**/", "/")
            if re.compile(fnmatch.translate(zero_dir_pattern)).match(path):
                return True
        if pattern.startswith("**/"):
            zero_dir_pattern = pattern[3:]
            if re.compile(fnmatch.translate(zero_dir_pattern)).match(path):
                return True
        return False
    return fnmatch.fnmatch(path, pattern)


def load_ignore_spec(root: Path, enable_gitignore: bool) -> Optional[pathspec.PathSpec]:
    if not enable_gitignore:
        return None
    gitignore_path = root / ".gitignore"
    if not gitignore_path.exists():
        return None
    with gitignore_path.open() as fh:
        lines = [ln.strip() for ln in fh if ln.strip() and not ln.startswith("#")]
    return pathspec.PathSpec.from_lines(pathspec.patterns.GitWildMatchPattern, lines)


def list_files(
    root: Path,
    relative: Path,
    include_patterns: Optional[List[str]],
    exclude_patterns: Optional[List[str]],
    ignore_spec: Optional[pathspec.PathSpec],
    restrict_code: bool,
) -> List[Path]:
    files: List[Path] = []
    start = (root / relative).resolve()
    for current_root, dirs, filenames in os.walk(start, followlinks=True):
        rel_dir = Path(current_root).relative_to(root)
        # skip ignored directories
        dirs[:] = [d for d in dirs if not (ignore_spec and ignore_spec.match_file(str(rel_dir / d)))]
        for name in filenames:
            rel_path = rel_dir / name
            if ignore_spec and ignore_spec.match_file(str(rel_path)):
                continue
            if include_patterns and not any(glob_match(p, str(rel_path)) for p in include_patterns):
                continue
            if exclude_patterns and any(glob_match(p, str(rel_path)) for p in exclude_patterns):
                continue
            if restrict_code and rel_path.suffix.lower() not in DEFAULT_CODE_EXTS:
                continue
            files.append(rel_path)
    return files


def search_text(
    pattern: str,
    content: str,
    ctx_before: int,
    ctx_after: int,
) -> List[Tuple[int, int, List[Tuple[int, str, str]]]]:
    compiled = re.compile(pattern, re.DOTALL)
    lines = content.splitlines()
    total = len(lines)
    matches: List[Tuple[int, int, List[Tuple[int, str, str]]]] = []
    for m in compiled.finditer(content):
        start_pos, end_pos = m.start(), m.end()
        start_line = content[:start_pos].count("\n") + 1
        end_line = content[:end_pos].count("\n") + 1
        ctx_start = max(1, start_line - ctx_before)
        ctx_end = min(total, end_line + ctx_after)
        context_block: List[Tuple[int, str, str]] = []
        for idx in range(ctx_start, ctx_end + 1):
            if idx < start_line:
                kind = "before"
            elif idx > end_line:
                kind = "after"
            else:
                kind = "match"
            context_block.append((idx, lines[idx - 1], kind))
        matches.append((start_line, end_line, context_block))
    return matches


def search_file(
    root: Path,
    rel_path: Path,
    pattern: str,
    ctx_before: int,
    ctx_after: int,
) -> Tuple[str, List[dict]]:
    abs_path = root / rel_path
    try:
        content = abs_path.read_text(encoding="utf-8", errors="ignore")
    except Exception as exc:  # pragma: no cover - defensive
        return str(rel_path), [{"error": f"read_failed: {exc}"}]
    results = []
    for start_line, end_line, ctx_lines in search_text(pattern, content, ctx_before, ctx_after):
        results.append(
            {
                "span": [start_line, end_line],
                "lines": [{"line": ln, "text": txt, "type": kind} for ln, txt, kind in ctx_lines],
            }
        )
    return str(rel_path), results


def main() -> None:
    parser = argparse.ArgumentParser(description="Fast regex search with glob filters")
    parser.add_argument("pattern", help="Regex pattern (DOTALL enabled)")
    parser.add_argument("--root", default=".", help="Project root")
    parser.add_argument("--path", dest="relative_path", default=".", help="Relative path to limit search")
    parser.add_argument("--include", dest="include_glob", default="", help="Glob of files to include")
    parser.add_argument("--exclude", dest="exclude_glob", default="", help="Glob of files to exclude")
    parser.add_argument("--context-before", type=int, default=0, help="Context lines before match")
    parser.add_argument("--context-after", type=int, default=0, help="Context lines after match")
    parser.add_argument("--restrict-code", action="store_true", help="Only search common code/text extensions")
    parser.add_argument("--no-gitignore", action="store_true", help="Do not honor .gitignore patterns")
    parser.add_argument("--workers", type=int, default=-1, help="joblib workers (-1 uses all cores)")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    if not root.exists():
        raise SystemExit(f"root not found: {root}")
    rel_base = Path(args.relative_path)
    target = (root / rel_base).resolve()
    if not target.exists():
        raise SystemExit(f"relative path not found: {rel_base}")
    try:
        target.relative_to(root)
    except ValueError:
        raise SystemExit("relative path must stay inside root")

    include_patterns = expand_braces(args.include_glob) if args.include_glob else None
    exclude_patterns = expand_braces(args.exclude_glob) if args.exclude_glob else None
    ignore_spec = load_ignore_spec(root, enable_gitignore=not args.no_gitignore)

    if target.is_file():
        candidate_files = [rel_base]
    else:
        candidate_files = list_files(
            root=root,
            relative=rel_base,
            include_patterns=include_patterns,
            exclude_patterns=exclude_patterns,
            ignore_spec=ignore_spec,
            restrict_code=args.restrict_code,
        )

    jobs = (delayed(search_file)(root, rel_path, args.pattern, args.context_before, args.context_after) for rel_path in candidate_files)
    results = Parallel(n_jobs=args.workers, backend="threading")(jobs)

    hits = {path: matches for path, matches in results if matches}
    summary = {"files_scanned": len(candidate_files), "files_with_matches": len(hits)}
    output = {"summary": summary, "matches": hits}
    print(json.dumps(output, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
