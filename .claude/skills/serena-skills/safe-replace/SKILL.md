---
name: safe-replace
description: Perform safe regex/literal replacements with ambiguity checks, context review, and optional dry-run before writing changes.
---

# Safe Replace

Use this skill to replace code/text safely with regex or literal patterns. It does not depend on the Serena core; use the bundled script.

All command examples assume:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

## Script
- `scripts/safe_replace.py`: supports literal/regex replacement, dry-run, and backups.

## Workflow
1. Scope first: use a search (e.g., python-fast-search) to confirm locations and counts.
2. Replace examples:
    - Regex:
       ```bash
       python "$SKILLS_ROOT/safe-replace/scripts/safe_replace.py" path/to/file "BEGIN.*?END" "replacement" \
          --root "$PROJECT_ROOT" \
          --mode regex
       ```
    - Literal:
       ```bash
       python "$SKILLS_ROOT/safe-replace/scripts/safe_replace.py" path/to/file "old" "new" \
          --root "$PROJECT_ROOT" \
          --mode literal
       ```
3. Default is single replacement; allow multiple with `--allow-multiple`.
4. Dry-run: `--dry-run` to report match count without writing.
5. Backup: saved to `.tmp/.serena-skills/backups/...` by default; skip with `--no-backup`.

## Tips
- Avoid spanning huge blocks unless necessary; use narrower sentinels instead of `.*`.
- Use capturing groups with `$!1`-style backreferences only after verifying the match groups.
- Keep backups or rely on VCS to revert if needed.
