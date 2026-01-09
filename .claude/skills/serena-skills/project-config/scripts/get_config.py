#!/usr/bin/env python3
"""
Get current configuration
"""
import argparse
import json
import os
import sys
from pathlib import Path


def get_config():
    """Get current configuration"""
    
    # Load projects
    projects_file = Path.home() / ".serena" / "projects.json"
    projects = {}
    if projects_file.exists():
        with open(projects_file, 'r', encoding='utf-8') as f:
            projects = json.load(f)
    
    # Get current directory as potential active project
    cwd = os.getcwd()
    active_project = None
    for name, info in projects.items():
        if info["path"] == cwd or cwd.startswith(info["path"]):
            active_project = name
            break
    
    config = {
        "active_project": active_project,
        "current_directory": cwd,
        "registered_projects": list(projects.keys()),
        "projects": projects,
        "serena_home": str(Path.home() / ".serena")
    }
    
    return config


def main():
    parser = argparse.ArgumentParser(description="Get current configuration")
    args = parser.parse_args()
    
    try:
        config = get_config()
        print(json.dumps(config, indent=2))
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
