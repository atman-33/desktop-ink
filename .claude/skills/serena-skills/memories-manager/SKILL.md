---
name: memories-manager
description: Manage Serena project memories (.serena/memories/*.md): create, read, list, and delete concise memory files for project-specific guidance.
---

# Memories Manager

Use this skill to handle project memory files stored under `.tmp/.serena-skills/memories` (independent of the Serena core).

All command examples assume:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

## Script
- `scripts/memories.py`: provides list/read/write/delete.
  - Example:
    ```bash
    python "$SKILLS_ROOT/memories-manager/scripts/memories.py" list \
      --dir "$PROJECT_ROOT/.tmp/.serena-skills/memories"
    ```
  - Example:
    ```bash
    python "$SKILLS_ROOT/memories-manager/scripts/memories.py" write release-notes "Notes here" \
      --dir "$PROJECT_ROOT/.tmp/.serena-skills/memories"
    ```
  - Example:
    ```bash
    python "$SKILLS_ROOT/memories-manager/scripts/memories.py" read release-notes \
      --dir "$PROJECT_ROOT/.tmp/.serena-skills/memories"
    ```
  - Example:
    ```bash
    python "$SKILLS_ROOT/memories-manager/scripts/memories.py" delete release-notes \
      --dir "$PROJECT_ROOT/.tmp/.serena-skills/memories"
    ```

## Actions
- Create/update: write Markdown to `{name}.md` (keep it concise).
- Read: load only when relevant; avoid unnecessary memories.
- List: enumerate available notes to choose what to read.
- Delete: remove stale or unneeded notes.

## Practices
- One topic per memory file; prefer short, scannable bullets.
- Avoid secrets and volatile data; store stable facts and workflows.
- Normalize names (lowercase, hyphen/underscore) to reduce duplicates.
