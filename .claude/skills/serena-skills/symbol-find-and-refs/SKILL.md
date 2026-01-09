---
name: symbol-find-and-refs
description: Locate symbols (defs/classes/functions) and their references using regex/glob search plus Serena-style tools; use when you need quick definition/usage discovery without full language server context.
---

# Symbol Find and Refs

Use this skill to rapidly locate where symbols are defined and referenced without depending on the Serena core. Run the bundled script directly.

All command examples assume:

```bash
PROJECT_ROOT="."
SKILLS_ROOT="$PROJECT_ROOT/.claude/skills/serena-skills"
```

## Script
- `scripts/find_symbol_refs.py`: extracts definitions/references via regex + glob filters (DOTALL+MULTILINE).

## Quick Workflow
1. Example (definition):
	 ```bash
	 python "$SKILLS_ROOT/symbol-find-and-refs/scripts/find_symbol_refs.py" "^class\\s+MyType" \
		 --root "$PROJECT_ROOT" \
		 --path src --include "**/*.py" --context-after 1
	 ```
2. Example (references):
	 ```bash
	 python "$SKILLS_ROOT/symbol-find-and-refs/scripts/find_symbol_refs.py" "MyType" \
		 --root "$PROJECT_ROOT" \
		 --path src --include "**/*.py" --context-after 1 --ignore-case
	 ```
3. Narrow scope with include/exclude globs (comma-separated allowed).

## Patterns
- Class: `^class\s+MyType\b`
- Function: `^def\s+handle_request\b`
- Method call: `\.handle_request\(` or `handle_request\(`
- Variable/const: `MY_CONST\b`

## Tips
- Always constrain by glob/path to cut noise (tests/vendor/build outputs).
- Prefer specific prefixes/suffixes (e.g., `\bMyType\b`) to avoid substring collisions.
- For multi-word names, escape underscores or use `[_ ]?` bridges if variants exist.
