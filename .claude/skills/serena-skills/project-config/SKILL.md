---
name: project-config
description: Manage project registration, activation, and configuration settings. Use for setting up projects, switching contexts, and managing Serena project metadata.
---

# Project Configuration

Manage Serena project settings and registration.

## Core Capabilities

### 1. Activate Project

Register and activate a project for Serena operations.

```bash
python3 {skill_path}/scripts/activate_project.py \
  --project-path /path/to/project \
  --name "my-project"
```

**Parameters:**
- `--project-path`: Absolute path to project directory
- `--name`: Project name (optional, defaults to directory name)

**Storage:** Project info saved to `~/.serena/projects.json`

### 2. List Projects

Show all registered projects.

```bash
python3 {skill_path}/scripts/list_projects.py
```

**Output:** JSON array of project names and paths

### 3. Remove Project

Unregister a project.

```bash
python3 {skill_path}/scripts/remove_project.py \
  --name "my-project"
```

### 4. Get Current Config

Display current configuration and active project.

```bash
python3 {skill_path}/scripts/get_config.py
```

**Output:** JSON with:
- Active project
- Available projects
- Configuration paths
- Language settings

### 5. Get Project Config

Read project-specific configuration from project.yml.

```bash
python3 {skill_path}/scripts/get_project_config.py \
  --project-root /path/to/project
```

**Output:** JSON with configuration values from `.tmp/.serena-skills/project.yml`

### 6. Update Project Config

Update a configuration value in project.yml.

```bash
python3 {skill_path}/scripts/update_project_config.py \
  --project-root /path/to/project \
  --key "read_only" \
  --value "true"
```

**Supported keys**: `languages`, `encoding`, `ignore_all_files_in_gitignore`, `ignored_paths`, `read_only`, `excluded_tools`, `initial_prompt`

## Project Structure

### Serena Metadata

When a project is activated, Serena Skills creates:

```
project/
└── .tmp/
    └── .serena-skills/
        ├── project.yml        # Project configuration (main)
        ├── memories/          # Project memories
        └── onboarding.md      # Project onboarding info
```

### Global Configuration

```
~/.serena/
├── projects.json          # Registered projects
└── solidlsp/              # LSP server data
```

## Configuration Files

### project.yml Format

Located at `.tmp/.serena-skills/project.yml`:

```yaml
# List of programming languages
languages: ["python"]

# File encoding
encoding: "utf-8"

# Use gitignore patterns
ignore_all_files_in_gitignore: true

# Additional paths to ignore
ignored_paths: []

# Read-only mode (disables editing tools)
read_only: false

# Tools to exclude
excluded_tools: []

# Initial prompt for the project
initial_prompt: ""
```

### projects.json Format

```json
{
  "my-project": {
    "path": "/absolute/path/to/project",
    "language": "python",
    "encoding": "utf-8"
  }
}
```

## Usage Patterns

### Initial Setup

```bash
# 1. Activate project (creates project.yml)
./activate_project.py --project-path /code/myproject

# 2. Verify registration
./list_projects.py

# 3. Check configuration
./get_project_config.py --project-root /code/myproject

# 4. Update configuration if needed
./update_project_config.py \
  --project-root /code/myproject \
  --key "read_only" \
  --value "false"
```

### Switching Projects

```bash
# List available projects
./list_projects.py

# Activate different project
./activate_project.py --project-path /code/another-project
```

### Cleanup

```bash
# Remove old project
./remove_project.py --name "old-project"
```

## Integration Notes

- Projects must exist on filesystem
- Activation validates directory exists
- Language auto-detected from file extensions
- LSP servers initialized per language
- Ignore patterns follow `.gitignore` format

## Best Practices

1. **Use descriptive names** for projects
2. **Keep project paths absolute** for consistency
3. **Register projects once** at project root
4. **Clean up unused** registrations periodically
5. **Check config** after activation to verify settings
