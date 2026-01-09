#!/usr/bin/env python3
"""
Minimal project config initializer (independent of Serena). Writes JSON to .tmp/.serena-skills/config.json by default.
"""
import argparse
import json
from pathlib import Path

DEFAULT_OUT = Path(".tmp/.serena-skills/config.json")


def main() -> None:
    p = argparse.ArgumentParser(description="Initialize lightweight project config")
    p.add_argument("--root", default=".", help="Project root")
    p.add_argument("--languages", default="python", help="Comma-separated languages (free text)")
    p.add_argument("--encoding", default="utf-8")
    p.add_argument("--ignore", default=".git,node_modules,dist,build", help="Comma-separated ignore patterns")
    p.add_argument("--output", default=str(DEFAULT_OUT), help="Output JSON path")
    args = p.parse_args()

    root = Path(args.root).resolve()
    out_path = Path(args.output)
    if not out_path.is_absolute():
        out_path = (root / out_path).resolve()

    out_path.parent.mkdir(parents=True, exist_ok=True)

    cfg = {
        "root": str(root),
        "languages": [s for s in args.languages.split(",") if s],
        "encoding": args.encoding,
        "ignore": [s for s in args.ignore.split(",") if s],
    }
    out_path.write_text(json.dumps(cfg, indent=2, ensure_ascii=False), encoding="utf-8")
    print(f"Wrote config to {out_path}")


if __name__ == "__main__":
    main()
