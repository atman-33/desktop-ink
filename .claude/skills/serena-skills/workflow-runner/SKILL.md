---
name: workflow-runner
description: Discover and run Serena workflows; use to list available workflows and execute the right one with validated inputs.
---

# Workflow Runner

Use this skill to find and execute predefined workflows stored independently (JSON under `.tmp/.serena-skills/workflows`).

All command examples assume:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

## Script
- `run_workflow.py`: list/run workflows defined as JSON.
	- List:
	  ```bash
	  python "$SKILLS_ROOT/workflow-runner/scripts/run_workflow.py" list --root "$PROJECT_ROOT"
	  ```
	- Run:
	  ```bash
	  python "$SKILLS_ROOT/workflow-runner/scripts/run_workflow.py" run build --root "$PROJECT_ROOT"
	  ```
	  (expects `$PROJECT_ROOT/.tmp/.serena-skills/workflows/build.json`)

## Quick Workflow
1. Place a workflow JSON at `.tmp/.serena-skills/workflows/<name>.json` (list commands in the `steps` array).
2. Use `list` to confirm it exists.
3. Run with `run <name>`. Each step is restricted to the project root; non-zero exits stop execution.
4. Review logs and rerun with adjusted parameters if needed.

## Tips
- Use `shell: true` only when required (e.g., pipes/redirection) and review commands carefully.
- Avoid long-running or interactive commands, or split them into smaller steps.
- Record successful invocations/parameters in memories for reuse.
