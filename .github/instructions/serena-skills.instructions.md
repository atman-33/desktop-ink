---
applyTo: '**'
---
Enforce the use of the `serena-skills` skill for code investigation and implementation work in this repository.

## Serena Skills (Required for Investigation & Implementation)

When doing code investigation (searching/locating behavior) or implementation (making edits/refactors), you MUST use the `serena-skills` skill as the router and follow its instructions.

- First, load the `serena-skills` index and choose the appropriate helper:
	- `fast-search` for repo-wide search
	- `symbol-find-and-refs` for likely definitions/usages
	- `safe-replace` for file edits (dry-run first)
	- `workflow-runner` for repeated sequences
	- `memories-manager` to capture durable repo knowledge
	- `project-init-config` to set/record repo scope conventions
- Prefer the default flow: **Investigate → Fix** (search/symbols, then safe replace).
- Do not stop after search results unless the user explicitly asked for “just locate it”.
- IMPORTANT: Do not use regular Serena MCP tools at the same time as `serena-skills`.