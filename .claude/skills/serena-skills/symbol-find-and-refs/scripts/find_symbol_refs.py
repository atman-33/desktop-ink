#!/usr/bin/env python3
"""
Lightweight symbol finder: search regex across files with glob filters.
Stores nothing; runs on current repo tree.
"""
import argparse
import json
import os
import re
from fnmatch import fnmatch
from pathlib import Path
from typing import List, Tuple

DEFAULT_CODE_GLOB = "**/*"


def iter_files(root: Path, rel: Path, include: List[str] | None, exclude: List[str] | None) -> List[Path]:
    files: List[Path] = []
    start = (root / rel).resolve()
    for dirpath, _dirs, filenames in os.walk(start, followlinks=True):
        rel_dir = Path(dirpath).relative_to(root)
        for name in filenames:
            rel_path = rel_dir / name
            rel_str = str(rel_path)
            if include and not any(fnmatch(rel_str, pat) for pat in include):
                continue
            if exclude and any(fnmatch(rel_str, pat) for pat in exclude):
                continue
            files.append(rel_path)
    return files


def search_one(root: Path, rel_path: Path, pattern: re.Pattern, ctx_before: int, ctx_after: int) -> Tuple[str, List[dict]]:
    abs_path = root / rel_path
    try:
        content = abs_path.read_text(encoding="utf-8", errors="ignore")
    except Exception as exc:  # pragma: no cover
        return str(rel_path), [{"error": f"read_failed: {exc}"}]

    lines = content.splitlines()
    total = len(lines)
    results: List[dict] = []
    for m in pattern.finditer(content):
        start_pos, end_pos = m.start(), m.end()
        start_line = content[:start_pos].count("\n") + 1
        end_line = content[:end_pos].count("\n") + 1
        ctx_start = max(1, start_line - ctx_before)
        ctx_end = min(total, end_line + ctx_after)
        context = []
        for idx in range(ctx_start, ctx_end + 1):
            if idx < start_line:
                kind = "before"
            elif idx > end_line:
                kind = "after"
            else:
                kind = "match"
            context.append({"line": idx, "text": lines[idx - 1], "type": kind})
        results.append({"span": [start_line, end_line], "lines": context})
    return str(rel_path), results


def main() -> None:
    p = argparse.ArgumentParser(description="Find symbols/refs via regex with glob filters")
    p.add_argument("pattern", help="Regex (DOTALL enabled)")
    p.add_argument("--root", default=".")
    p.add_argument("--path", dest="rel_path", default=".")
    p.add_argument("--include", default=DEFAULT_CODE_GLOB, help="Glob include (comma-separated for multiple)")
    p.add_argument("--exclude", default="", help="Glob exclude (comma-separated)")
    p.add_argument("--context-before", type=int, default=0)
    p.add_argument("--context-after", type=int, default=0)
    p.add_argument("--ignore-case", action="store_true")
    args = p.parse_args()

    root = Path(args.root).resolve()
    target = (root / Path(args.rel_path)).resolve()
    if not target.exists():
        raise SystemExit(f"path not found: {args.rel_path}")
    try:
        target.relative_to(root)
    except ValueError:
        raise SystemExit("path must stay inside root")

    include = [s for s in args.include.split(",") if s] if args.include else None
    exclude = [s for s in args.exclude.split(",") if s] if args.exclude else None
    flags = re.DOTALL | re.MULTILINE
    if args.ignore_case:
        flags |= re.IGNORECASE
    try:
        pattern = re.compile(args.pattern, flags)
    except re.error as exc:
        raise SystemExit(f"invalid regex: {exc}")

    files = [Path(args.rel_path)] if target.is_file() else iter_files(root, Path(args.rel_path), include, exclude)
    hits = {}
    for rel in files:
        path_str, matches = search_one(root, rel, pattern, args.context_before, args.context_after)
        if matches:
            hits[path_str] = matches
    summary = {"files_scanned": len(files), "files_with_matches": len(hits)}
    print(json.dumps({"summary": summary, "matches": hits}, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
