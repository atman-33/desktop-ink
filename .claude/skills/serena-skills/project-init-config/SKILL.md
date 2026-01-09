---
name: project-init-config
description: Set up Serena project config (languages, encoding, ignores) and validate paths; use when bootstrapping or adjusting a project for Serena tools.
---

# Project Init and Config

Use this skill to bootstrap or adjust project settings independently (no Serena core). Outputs a lightweight JSON config under `.tmp/.serena-skills/` by default.

All command examples assume:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

## Script
- `scripts/init_config.py`: defaults to `.tmp/.serena-skills/config.json`.
  - Example:
    ```bash
    python "$SKILLS_ROOT/project-init-config/scripts/init_config.py" \
      --root "$PROJECT_ROOT" \
      --languages python,ts \
      --ignore ".git,node_modules,dist,build"
    ```

## Steps
1. Decide project root and languages (e.g., Python, TS, Java). Encoding: UTF-8 unless required otherwise.
2. List ignore patterns (e.g., `.git,node_modules,dist,build`).
3. Run the script to emit JSON (override path with `--output` if needed).
4. Edit/share the generated file as needed.

## Tips
- Keep ignore lists minimal but effective (build/dist/node_modules/.git, etc.).
- When adding languages, ensure only relevant files are in scope (use globs/search to confirm).
- Use python-fast-search to validate the search scope before heavy operations.
