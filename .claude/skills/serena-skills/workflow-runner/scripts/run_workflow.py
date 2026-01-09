#!/usr/bin/env python3
"""
Minimal workflow runner using JSON workflow files under .tmp/.serena-skills/workflows.
Format example:
{
  "steps": [
    {"run": "echo hello", "shell": true},
    {"run": ["python3", "script.py"], "cwd": "subdir"}
  ]
}
"""
import argparse
import json
import subprocess
from pathlib import Path
from typing import Any, List

WORKFLOW_DIR = Path(".tmp/.serena-skills/workflows")


def list_workflows(directory: Path) -> List[str]:
    directory.mkdir(parents=True, exist_ok=True)
    return sorted([p.stem for p in directory.glob("*.json") if p.is_file()])


def load_workflow(directory: Path, name: str) -> dict:
    path = directory / f"{name}.json"
    if not path.exists():
        raise SystemExit(f"workflow not found: {path}")
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        raise SystemExit(f"invalid JSON in {path}: {exc}")


def run_step(root: Path, step: dict[str, Any]) -> None:
    if "run" not in step:
        raise SystemExit("step missing 'run'")
    cmd = step["run"]
    cwd = step.get("cwd")
    shell = bool(step.get("shell", False))

    if cwd:
        cwd_path = (root / cwd).resolve()
        try:
            cwd_path.relative_to(root)
        except ValueError:
            raise SystemExit("cwd must stay inside root")
        if not cwd_path.exists():
            raise SystemExit(f"cwd not found: {cwd_path}")
    else:
        cwd_path = root

    if not shell and isinstance(cmd, str):
        # split naive? better require list if shell False and str
        raise SystemExit("When shell is false, 'run' must be a list of argv")

    print(f"==> run: {cmd} (cwd={cwd_path}) shell={shell}")
    res = subprocess.run(cmd, cwd=cwd_path, shell=shell, check=False, text=True, capture_output=True)
    if res.stdout:
        print(res.stdout.rstrip())
    if res.stderr:
        print(res.stderr.rstrip())
    if res.returncode != 0:
        raise SystemExit(f"step failed with code {res.returncode}")


def main() -> None:
    p = argparse.ArgumentParser(description="Run simple JSON workflows")
    p.add_argument("command", choices=["list", "run"])
    p.add_argument("name", nargs="?")
    p.add_argument("--root", default=".")
    p.add_argument("--workflows", default=str(WORKFLOW_DIR), help="Directory containing workflow JSON files")
    args = p.parse_args()

    root = Path(args.root).resolve()
    wf_dir = Path(args.workflows)
    if not wf_dir.is_absolute():
        wf_dir = (root / wf_dir).resolve()

    if args.command == "list":
        names = list_workflows(wf_dir)
        for n in names:
            print(n)
        return

    if args.command == "run":
        if not args.name:
            raise SystemExit("workflow name required")
        workflow = load_workflow(wf_dir, args.name)
        steps = workflow.get("steps")
        if not isinstance(steps, list):
            raise SystemExit("workflow 'steps' must be a list")
        for step in steps:
            run_step(root, step)
        print("workflow completed")


if __name__ == "__main__":
    main()
