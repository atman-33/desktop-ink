#!/usr/bin/env python3
"""
List all registered projects
"""
import argparse
import json
import sys
from pathlib import Path


def get_projects_file():
    """Get path to projects registry file"""
    return Path.home() / ".serena" / "projects.json"


def list_projects():
    """List all registered projects"""
    projects_file = get_projects_file()
    
    if not projects_file.exists():
        return []
    
    with open(projects_file, 'r', encoding='utf-8') as f:
        projects = json.load(f)
    
    return projects


def main():
    parser = argparse.ArgumentParser(description="List registered projects")
    args = parser.parse_args()
    
    try:
        projects = list_projects()
        print(json.dumps(projects, indent=2))
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
