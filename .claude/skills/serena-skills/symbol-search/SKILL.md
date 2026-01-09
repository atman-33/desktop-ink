---
name: symbol-search
description: Search and analyze code symbols (classes, functions, methods) in codebases using Language Server Protocol. Use for discovering code structure, finding definitions, analyzing references, and understanding symbol relationships.
---

# Symbol Search

Efficient code navigation through semantic symbol search using LSP backends.

## Core Capabilities

### 1. Get Symbols Overview

Get high-level structure of a file's symbols.

```bash
python3 {skill_path}/scripts/get_symbols_overview.py \
  --project-root /path/to/project \
  --file src/module.py \
  --depth 1
```

**Parameters:**
- `--project-root`: Absolute path to project
- `--file`: Relative path to file
- `--depth`: Descendant depth (0=top-level only, 1=include children)
- `--language`: Language (python, typescript, java, etc.)

**Output:** JSON with symbols grouped by kind (Class, Function, Method, etc.)

### 2. Find Symbol

Search for symbols by name pattern.

```bash
python3 {skill_path}/scripts/find_symbol.py \
  --project-root /path/to/project \
  --pattern "MyClass/my_method" \
  --include-body
```

**Name Path Patterns:**
- Simple name: `method` (matches any symbol named "method")
- Relative path: `Class/method` (matches path suffix)
- Absolute path: `/Class/method` (exact match within file)
- With index: `Class/method[1]` (specific overload)

**Parameters:**
- `--pattern`: Name path pattern
- `--file`: Restrict to specific file (optional)
- `--depth`: Include descendants
- `--include-body`: Include source code
- `--substring`: Enable substring matching
- `--language`: Language backend

**Output:** JSON array of matching symbols with locations

### 3. Find References

Find all code locations referencing a symbol.

```bash
python3 {skill_path}/scripts/find_references.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "MyClass/method"
```

**Parameters:**
- `--symbol`: Name path of target symbol
- `--file`: File containing the symbol
- `--language`: Language backend

**Output:** JSON array of references with code snippets

### 4. Insert After Symbol

Add code after a symbol definition.

```bash
python3 {skill_path}/scripts/insert_after_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "MyClass" \
  --body "
    def new_method(self):
        pass"
```

### 5. Insert Before Symbol

Add code before a symbol definition.

```bash
python3 {skill_path}/scripts/insert_before_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "MyClass" \
  --body "from typing import Optional
"
```

### 6. Rename Symbol

Rename symbol throughout the entire codebase.

```bash
python3 {skill_path}/scripts/rename_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "old_name" \
  --new-name "new_name"
```

**Features:**
- Updates all references automatically
- Works across multiple files
- Maintains code integrity

## Symbol Kinds

LSP symbol kinds (for filtering):
- 5=Class, 6=Method, 12=Function, 13=Variable
- 4=Package, 2=Module, 3=Namespace
- 10=Enum, 11=Interface, 23=Struct

Use `--include-kinds` and `--exclude-kinds` for filtering.

## Language Support

Supports any LSP-compatible language:
- Python (Pylance, Pyright, Jedi)
- TypeScript/JavaScript (tsserver)
- Java (jdtls)
- Go (gopls)
- Rust (rust-analyzer)
- C/C++ (clangd)

Configure via `--language` parameter.

## Integration Notes

Scripts use `solidlsp` library (Serena's LSP handler). Install requirements:

```bash
pip install -e /path/to/serena
```

Or use the bundled LSP client directly (see scripts for implementation).
