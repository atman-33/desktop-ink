#!/usr/bin/env python3
"""
Simple memories manager storing Markdown files under .tmp/.serena-skills/memories.
"""
import argparse
from pathlib import Path

DEFAULT_DIR = Path(".tmp/.serena-skills/memories")


def ensure_dir(base: Path) -> None:
    base.mkdir(parents=True, exist_ok=True)


def cmd_list(base: Path) -> None:
    ensure_dir(base)
    names = sorted([p.stem for p in base.glob("*.md") if p.is_file()])
    for n in names:
        print(n)


def cmd_read(base: Path, name: str) -> None:
    ensure_dir(base)
    path = base / f"{name}.md"
    if not path.exists():
        raise SystemExit(f"not found: {path}")
    print(path.read_text(encoding="utf-8"))


def cmd_write(base: Path, name: str, content: str) -> None:
    ensure_dir(base)
    path = base / f"{name}.md"
    path.write_text(content, encoding="utf-8")
    print(f"wrote {path}")


def cmd_delete(base: Path, name: str) -> None:
    ensure_dir(base)
    path = base / f"{name}.md"
    if not path.exists():
        raise SystemExit(f"not found: {path}")
    path.unlink()
    print(f"deleted {path}")


def main() -> None:
    p = argparse.ArgumentParser(description="Manage memories under .tmp/.serena-skills/memories")
    p.add_argument("command", choices=["list", "read", "write", "delete"], help="Action to perform")
    p.add_argument("name", nargs="?", help="Memory name (without extension)")
    p.add_argument("content", nargs="?", help="Content when writing")
    p.add_argument("--dir", dest="base", default=str(DEFAULT_DIR), help="Memories directory")
    args = p.parse_args()

    base = Path(args.base)
    if args.command == "list":
        cmd_list(base)
    elif args.command == "read":
        if not args.name:
            raise SystemExit("name is required for read")
        cmd_read(base, args.name)
    elif args.command == "write":
        if not args.name or args.content is None:
            raise SystemExit("name and content are required for write")
        cmd_write(base, args.name, args.content)
    elif args.command == "delete":
        if not args.name:
            raise SystemExit("name is required for delete")
        cmd_delete(base, args.name)


if __name__ == "__main__":
    main()
