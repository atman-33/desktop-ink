#!/usr/bin/env python3
"""
Remove a registered project
"""
import argparse
import json
import sys
from pathlib import Path


def get_projects_file():
    """Get path to projects registry file"""
    return Path.home() / ".serena" / "projects.json"


def remove_project(name: str):
    """Remove a project from registry"""
    projects_file = get_projects_file()
    
    if not projects_file.exists():
        raise ValueError("No projects registered")
    
    with open(projects_file, 'r', encoding='utf-8') as f:
        projects = json.load(f)
    
    if name not in projects:
        raise ValueError(f"Project not found: {name}")
    
    del projects[name]
    
    with open(projects_file, 'w', encoding='utf-8') as f:
        json.dump(projects, f, indent=2)
    
    return f"Project removed: {name}"


def main():
    parser = argparse.ArgumentParser(description="Remove a project")
    parser.add_argument("--name", required=True, help="Project name")
    
    args = parser.parse_args()
    
    try:
        result = remove_project(args.name)
        print(result)
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
