---
name: code-editor
description: Safe code editing operations using symbol-level replacements and file-level edits. Use for modifying code precisely with LSP-aware symbol replacement and text-based find/replace operations.
---

# Code Editor

Precise code editing with symbol-level and file-level operations.

## Symbol-Level Editing

### 1. Replace Symbol Body

Replace entire symbol definition using LSP.

```bash
python3 {skill_path}/scripts/replace_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "MyClass/my_method" \
  --body "def my_method(self):
    return 42"
```

**Parameters:**
- `--file`: File containing the symbol
- `--symbol`: Name path (e.g., "Class/method")
- `--body`: New symbol body (full definition)
- `--language`: Language backend

**Important:** Body includes the full definition (signature + implementation).

### 2. Insert After Symbol

Add code after a symbol.

```bash
python3 {skill_path}/scripts/insert_after_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "MyClass" \
  --body "
def new_method(self):
    pass"
```

Use for adding new methods, classes, or functions.

### 3. Insert Before Symbol

Add code before a symbol.

```bash
python3 {skill_path}/scripts/insert_before_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "MyClass" \
  --body "from typing import Optional
"
```

Use for adding imports or preceding definitions.

### 4. Rename Symbol

Rename symbol across entire codebase.

```bash
python3 {skill_path}/scripts/rename_symbol.py \
  --project-root /path/to/project \
  --file src/module.py \
  --symbol "old_name" \
  --new-name "new_name"
```

**Features:**
- Updates all references automatically
- Maintains code integrity
- Works across files

## File-Level Editing

### 1. Replace Content

Safe find/replace in files using exact string matching.

```bash
python3 {skill_path}/scripts/replace_content.py \
  --project-root /path/to/project \
  --file src/module.py \
  --old "def old_function():
    pass" \
  --new "def new_function():
    return True"
```

**Safety Features:**
- Matches exact string (whitespace-sensitive)
- Fails if old string not found
- Reports line numbers for verification
- Supports regex mode

**Parameters:**
- `--old`: Exact text to replace (include context)
- `--new`: Replacement text
- `--mode`: `literal` (default) or `regex`
- `--expected-count`: Expected number of matches (optional)

### 2. Create File

Create new file with content.

```bash
python3 {skill_path}/scripts/create_file.py \
  --project-root /path/to/project \
  --file src/new_module.py \
  --content "# New Module

def hello():
    print('Hello, World!')
"
```

**Note:** Overwrites existing files.

### 3. Delete Lines

Delete specific line range from a file.

```bash
python3 {skill_path}/scripts/delete_lines.py \
  --project-root /path/to/project \
  --file src/old_code.py \
  --start-line 10 \
  --end-line 20
```

### 4. Replace Lines

Replace specific lines with new content.

```bash
python3 {skill_path}/scripts/replace_lines.py \
  --project-root /path/to/project \
  --file src/config.py \
  --start-line 5 \
  --end-line 7 \
  --new-content "NEW_CONFIG = True"
```

### 5. Insert At Line

Insert content at a specific line number.

```bash
python3 {skill_path}/scripts/insert_at_line.py \
  --project-root /path/to/project \
  --file src/module.py \
  --line 10 \
  --content "import logging"
```

**Note:** All line numbers are 1-based (first line = 1).

## Best Practices

### Symbol-Level Editing

**Prefer when:**
- Modifying entire functions/methods/classes
- Need LSP awareness (references, types)
- Working with well-defined symbols

**Example workflow:**
```bash
# 1. Find symbol to understand current implementation
find_symbol.py --project-root /code --pattern "MyClass/process"

# 2. Replace symbol body
replace_symbol.py --project-root /code --file src/processor.py \
  --symbol "MyClass/process" --body "..."
```

### File-Level Editing

**Prefer when:**
- Small targeted changes
- Editing comments/strings/docs
- Working with non-code files
- Editing across multiple locations

**Example workflow:**
```bash
# Replace with context for safety
replace_content.py --project-root /code --file config.py \
  --old "DEBUG = False
LOG_LEVEL = 'INFO'
TIMEOUT = 30" \
  --new "DEBUG = True
LOG_LEVEL = 'DEBUG'
TIMEOUT = 60"
```

### Context Rules

**For replace_content:**
- Include 3-5 lines before/after target
- Match whitespace exactly
- Use `--expected-count` to verify

**Example:**
```python
# Good - includes context
--old "    def process(self):
        # Process data
        return self.data

    def validate(self):"

# Bad - too specific, might not match
--old "return self.data"
```

## Error Handling

Scripts exit with code 1 on errors:
- Symbol not found
- Old string not found or multiple matches
- File not found
- Invalid syntax

Use exit codes in automation:
```bash
if replace_content.py --file src/config.py --old "..." --new "..."; then
    echo "Success"
else
    echo "Failed"
fi
```
