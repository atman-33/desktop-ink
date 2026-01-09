---
name: fast-search
description: Fast regex/glob project search using Python with joblib concurrency, .gitignore awareness, and context lines. Use when you need efficient keyword/file search across a repository with include/exclude globs or code-only filtering.
---

# Python Fast Search

Use this skill to run a fast, configurable project-wide search with Python. It mirrors Serena's `search_files` and `SearchForPatternTool` behaviors (regex with DOTALL, glob include/exclude, .gitignore support) while remaining standalone.

All command examples assume:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

## What You Get
- `scripts/search_project.py`: CLI for parallel regex search with glob filters, optional code-only restriction, context lines, and .gitignore handling.
- Behavior inspired by [src/serena/text_utils.py](src/serena/text_utils.py) and [src/serena/tools/file_tools.py](src/serena/tools/file_tools.py): brace-expanded globs, DOTALL regex, joblib threading.

## Quick Start
1. Run the script from the repo root (or pass `--root`):
   ```bash
  python "$SKILLS_ROOT/fast-search/scripts/search_project.py" "class\s+Project" \
     --root "$PROJECT_ROOT" \
     --context-after 2
   ```
2. Narrow scope with `--path` and globs:
   ```bash
  python "$SKILLS_ROOT/fast-search/scripts/search_project.py" "search_files" \
     --root "$PROJECT_ROOT" \
     --path src \
     --include "**/*.py" --exclude "**/tests/**"
   ```
3. Search only common code/text extensions:
   ```bash
  python "$SKILLS_ROOT/fast-search/scripts/search_project.py" "MatchedConsecutiveLines" \
     --root "$PROJECT_ROOT" \
     --restrict-code
   ```

### Dependencies
- Requires `joblib` and `pathspec`. If missing, install via apt: `sudo apt-get install -y python3-joblib python3-pathspec` (or inside your venv: `python -m pip install joblib pathspec`).

## Options Cheatsheet
- `pattern` (required): Regex, evaluated with `re.DOTALL` (so `.` spans newlines).
- `--root`: Project root (default: current directory).
- `--path`: Subpath or file to search (default: `.`). Must stay inside root.
- `--include` / `--exclude`: Glob patterns with brace expansion (e.g., `**/*.{py,ts}`), matched on relative paths.
- `--context-before` / `--context-after`: Lines of context to keep around each hit.
- `--restrict-code`: Keep only files with common code/text extensions (see `DEFAULT_CODE_EXTS` in the script).
- `--no-gitignore`: Skip .gitignore filtering (enabled by default).
- `--workers`: joblib workers (`-1` uses all cores).

## Usage Tips
- Prefer non-greedy regex (`.*?`) when matches could span large regions.
- Use `--include` to avoid scanning vendor/build dirs; use `--exclude` for tests/fixtures when noise is high.
- When chasing symbol usages, pair `pattern` with `--restrict-code` and `--include "**/*.py"` for tighter scans.
- The output is JSON with a summary and per-file matches; pipe to `jq` when you need compact views.

## When to Use This Skill
- You need fast, repeatable repo-wide substring/regex search with context lines.
- You want Serena-style filtering (globs + .gitignore + multi-file parallelism) outside the agent runtime.
- You need a portable script you can run from CI or ad-hoc shells without loading the full agent.
