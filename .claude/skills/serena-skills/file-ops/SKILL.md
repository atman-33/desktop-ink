---
name: file-ops
description: File and directory operations for code projects - read files, list directories, search patterns, and manage file structures. Use for exploring codebases, finding files, and reading content.
---

# File Operations

Core file system operations for code project navigation and management.

## Core Capabilities

### 1. Read File

Read file contents or specific line ranges.

```bash
python3 {skill_path}/scripts/read_file.py \
  --project-root /path/to/project \
  --file src/module.py \
  --start-line 10 \
  --end-line 50
```

**Parameters:**
- `--project-root`: Absolute path to project
- `--file`: Relative path to file
- `--start-line`: Starting line (0-based, optional)
- `--end-line`: Ending line (0-based, optional)

### 2. List Directory

List files and directories with optional recursion.

```bash
python3 {skill_path}/scripts/list_dir.py \
  --project-root /path/to/project \
  --path src \
  --recursive
```

**Parameters:**
- `--project-root`: Absolute path to project
- `--path`: Relative directory path ("." for root)
- `--recursive`: Scan subdirectories
- `--skip-ignored`: Skip gitignored files

**Output:** JSON with `directories` and `files` arrays

### 3. Find Files

Find files matching a pattern.

```bash
python3 {skill_path}/scripts/find_file.py \
  --project-root /path/to/project \
  --mask "*.py" \
  --path src
```

**Parameters:**
- `--mask`: Filename or pattern (* and ? wildcards)
- `--path`: Search within relative path
- `--project-root`: Absolute path to project

**Output:** JSON array of matching file paths

### 4. Search Pattern

Search for text patterns in files using regex.

```bash
python3 {skill_path}/scripts/search_pattern.py \
  --project-root /path/to/project \
  --pattern "class.*:" \
  --path src \
  --context 2
```

**Parameters:**
- `--pattern`: Regex pattern to search
- `--path`: Restrict to directory/file (optional)
- `--context`: Lines of context before/after match
- `--code-only`: Search only code files (skip assets)
- `--include-glob`: Include pattern (e.g., "*.py")
- `--exclude-glob`: Exclude pattern (e.g., "*test*")

**Output:** JSON mapping file paths to match contexts

## Pattern Matching

### File Masks
- `*.py` - All Python files
- `test_*.py` - Files starting with test_
- `**/config.json` - config.json in any directory

### Regex Patterns
- Use Python regex syntax
- DOTALL flag enabled (`.` matches newlines)
- Case-insensitive matching
- Use `.*?` for non-greedy matching

## Ignore Handling

Scripts respect `.gitignore` and common ignore patterns:
- `node_modules/`, `__pycache__/`, `.git/`
- `*.pyc`, `*.class`, `*.o`
- Build directories: `build/`, `dist/`, `target/`

Use `--skip-ignored` to enforce ignore rules.

## Usage Patterns

**Explore unknown codebase:**
```bash
# 1. List top-level structure
./list_dir.py --project-root /code --path . --recursive

# 2. Find configuration files
./find_file.py --project-root /code --mask "*.json" --path .

# 3. Search for specific patterns
./search_pattern.py --project-root /code --pattern "TODO|FIXME" --code-only
```

**Read specific sections:**
```bash
# Read imports section
./read_file.py --project-root /code --file main.py --start-line 0 --end-line 20
```
