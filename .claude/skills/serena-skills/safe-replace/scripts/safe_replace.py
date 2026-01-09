#!/usr/bin/env python3
"""
Safe replace utility: literal or regex replacements with optional dry-run and backups.
Backups stored under .tmp/.serena-skills/backups/<relative path> by default.
"""
import argparse
import json
import os
import re
from pathlib import Path
from typing import Tuple

BACKUP_ROOT = Path(".tmp/.serena-skills/backups")


def ensure_inside_root(root: Path, target: Path) -> Path:
    target = target.resolve()
    root = root.resolve()
    try:
        target.relative_to(root)
    except ValueError:
        raise SystemExit("target must stay inside root")
    if not target.exists():
        raise SystemExit(f"target not found: {target}")
    return target


def load_file(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="ignore")


def save_backup(root: Path, file_path: Path, content: str) -> Path:
    rel = file_path.relative_to(root)
    backup_path = BACKUP_ROOT / rel
    backup_path.parent.mkdir(parents=True, exist_ok=True)
    backup_path.write_text(content, encoding="utf-8")
    return backup_path


def perform_replace(content: str, needle: str, repl: str, mode: str, allow_multi: bool) -> Tuple[str, int]:
    if mode == "literal":
        pattern = re.escape(needle)
        flags = 0
    elif mode == "regex":
        pattern = needle
        flags = re.DOTALL | re.MULTILINE
    else:
        raise SystemExit("mode must be literal or regex")

    compiled = re.compile(pattern, flags)

    def replace_fn(match: re.Match) -> str:
        # expand $!1 style backrefs
        def backref(m: re.Match) -> str:
            idx = int(m.group(1))
            try:
                return match.group(idx) or ""
            except IndexError:
                return m.group(0)

        return re.sub(r"\$!(\d+)", backref, repl)

    result, count = compiled.subn(replace_fn, content)
    if count == 0:
        raise SystemExit("no matches found")
    if not allow_multi and count > 1:
        raise SystemExit(f"pattern matched {count} occurrences; set --allow-multiple to proceed")
    return result, count


def main() -> None:
    p = argparse.ArgumentParser(description="Safe replace with backups and optional dry-run")
    p.add_argument("file", help="Target file (relative or absolute)")
    p.add_argument("needle", help="Pattern (literal or regex)")
    p.add_argument("repl", help="Replacement text; $!1 style backrefs if regex mode")
    p.add_argument("--mode", choices=["literal", "regex"], default="regex")
    p.add_argument("--root", default=".", help="Project root for safety checks")
    p.add_argument("--allow-multiple", action="store_true", help="Allow replacing multiple occurrences")
    p.add_argument("--dry-run", action="store_true", help="Do not write; just report planned changes")
    p.add_argument("--no-backup", action="store_true", help="Skip writing backup file")
    args = p.parse_args()

    root = Path(args.root).resolve()
    target = ensure_inside_root(root, Path(args.file))
    original = load_file(target)

    try:
        updated, count = perform_replace(original, args.needle, args.repl, args.mode, args.allow_multiple)
    except SystemExit as e:
        raise
    except Exception as exc:
        raise SystemExit(f"replace failed: {exc}")

    if args.dry_run:
        print(json.dumps({"dry_run": True, "matches": count}, ensure_ascii=False, indent=2))
        return

    if not args.no_backup:
        backup_path = save_backup(root, target, original)
    else:
        backup_path = None

    target.write_text(updated, encoding="utf-8")
    print(json.dumps({"dry_run": False, "matches": count, "backup": str(backup_path) if backup_path else None}, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
