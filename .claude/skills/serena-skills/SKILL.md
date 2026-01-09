---
name: serena-skills
description: Routing guide for Serena helper skills; use this to choose the right skill (search, replace, symbols, config, memories, workflows) before loading specifics.
---

# Serena Skill Index

Use this index to pick the right Serena skill quickly. These skills are standalone (no dependency on the Serena core) and store data under `.tmp/.serena-skills` when needed.

## Quick Setup (Portable)

These skills are meant to be reused across projects. Use these two variables in examples:

- `PROJECT_ROOT`: the repository you are working on (usually `.`)
- `SKILLS_ROOT`: where this folder lives inside that repository (usually `.claude/skills/serena-skills`)

Example shell setup:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

All script examples below assume those variables.

## Skill Map
- Search: `fast-search` — fast regex/glob search with context, .gitignore support.
- Symbols: `symbol-find-and-refs` — locate defs/usages via regex + scoped globs.
- Replace: `safe-replace` — safe literal/regex replacements with ambiguity checks.
- Config: `project-init-config` — set languages, encoding, ignore patterns; outputs `.tmp/.serena-skills/config.json`.
- Memories: `memories-manager` — read/write/list/delete `.tmp/.serena-skills/memories/*.md`.
- Workflows: `workflow-runner` — list/run JSON workflows under `.tmp/.serena-skills/workflows/`.

## Decision Matrix (Avoid Search-Only)

Use this table as the default router after reading the user task.

- If you need to *locate* code/text across the repo → use **Search**.
- If you need to *find likely definitions/usages* quickly (without LSP) → use **Symbols**.
- If you need to *change* files → use **Replace** (start with dry-run).
- If you will repeat the same command sequence → use **Workflows**.
- If you learn project-specific knowledge worth reusing → use **Memories**.
- If you need to set/record repo scope conventions → use **Config**.

Rule of thumb:

- After any Search/Symbols step, decide whether the next step is **Replace**, **Workflow**, or **Memory**.
- Do not stop after search results unless the user explicitly asked for “just locate it”.
## Important Note
⚠️ **Do not use regular Serena MCP tools when working with serena-skills.** These skills are standalone and designed to work independently. Using both simultaneously may cause conflicts or unexpected behavior.
## How to Use
1. Identify task type (search, symbols, replace, config, memories, workflow).
2. Load the corresponding skill above.
3. Follow that skill's quick workflow; keep scope narrow (paths/globs) and avoid unsafe edits.

## Recommended Workflows

### A) Investigate → Fix
1. Search for the relevant code:
	 ```bash
	 python "$SKILLS_ROOT/fast-search/scripts/search_project.py" "PATTERN" \
		 --root "$PROJECT_ROOT" \
		 --restrict-code \
		 --context-before 2 --context-after 2
	 ```
2. If a change is needed, run safe replace (dry-run first):
	 ```bash
	 python "$SKILLS_ROOT/safe-replace/scripts/safe_replace.py" "path/to/file" "NEEDLE" "REPL" \
		 --root "$PROJECT_ROOT" \
		 --mode regex \
		 --dry-run
	 ```
3. If the dry-run count is correct, rerun without `--dry-run`.

### B) Repeated Repo Task → Workflow
If you need to do the same thing multiple times (e.g., “search → run tests → search again”), create a workflow JSON and run it:

```bash
python "$SKILLS_ROOT/workflow-runner/scripts/run_workflow.py" list --root "$PROJECT_ROOT"
python "$SKILLS_ROOT/workflow-runner/scripts/run_workflow.py" run <name> --root "$PROJECT_ROOT"
```

### C) Capture Project Knowledge → Memory
When you discover durable information (build commands, important folders, conventions), write it once:

```bash
python "$SKILLS_ROOT/memories-manager/scripts/memories.py" write build-and-test "<short notes>" \
	--dir "$PROJECT_ROOT/.tmp/.serena-skills/memories"
```

## Tips
- Default to search-first (`fast-search`) to gather evidence before edits.
- Keep project operations within the repo root; respect ignore patterns.
- Log useful invocations as memories for reuse.

## Limitations (Compared to Serena MCP)

- These skills do not provide LSP-backed semantic operations (e.g., precise symbol references). The **Symbols** skill is regex-based.
- The goal is practical efficiency without MCP: fast search, safe replacement, lightweight memories, and repeatable workflows.
