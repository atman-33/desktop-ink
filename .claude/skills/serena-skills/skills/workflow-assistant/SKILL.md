---
name: workflow-assistant
description: Project onboarding, task tracking, and workflow guidance tools. Use for initializing new projects, documenting progress, and maintaining workflow state.
---

# Workflow Assistant

Tools for project onboarding and workflow management.

## Core Capabilities

### 1. Check Onboarding

Check if project has been onboarded.

```bash
python3 {skill_path}/scripts/check_onboarding.py \
  --project-root /path/to/project
```

**Output:** Boolean indicating onboarding status

### 2. Create Onboarding

Initialize project onboarding documentation.

```bash
python3 {skill_path}/scripts/create_onboarding.py \
  --project-root /path/to/project
```

**Creates:** `.tmp/.serena-skills/onboarding.md` with project structure template

### 3. Update Onboarding

Update onboarding documentation with new information.

```bash
python3 {skill_path}/scripts/update_onboarding.py \
  --project-root /path/to/project \
  --section "Architecture" \
  --content "Uses microservices pattern..."
```

### 4. Get Onboarding

Read project onboarding documentation.

```bash
python3 {skill_path}/scripts/get_onboarding.py \
  --project-root /path/to/project
```

### 5. Think About Collected Information

Prompt reflection on information collection completeness. Call after search operations.

```bash
python3 {skill_path}/scripts/think_collected_info.py
```

### 6. Think About Task Adherence

Verify alignment with task before code changes. **ALWAYS call before editing code.**

```bash
python3 {skill_path}/scripts/think_task_adherence.py
```

### 7. Think Are Done

Verify task completion. Call when you believe work is complete.

```bash
python3 {skill_path}/scripts/think_are_done.py
```

## Onboarding Template

When created, onboarding.md includes:

```markdown
# Project Onboarding

## Overview
[Project description]

## Architecture
[System architecture and design patterns]

## Key Components
[Main modules and their responsibilities]

## Development Setup
[Prerequisites and setup instructions]

## Code Structure
[Directory organization and conventions]

## Common Tasks
[Frequent operations and workflows]

## References
[Important files and documentation links]
```

## Usage Patterns

### Initial Project Setup

```bash
# 1. Activate project
activate_project.py --project-path /code/myproject

# 2. Check if onboarded
./check_onboarding.py --project-root /code/myproject

# 3. Create onboarding if needed
./create_onboarding.py --project-root /code/myproject

# 4. Update with findings
./update_onboarding.py --project-root /code/myproject \
  --section "Architecture" \
  --content "# Architecture
REST API with PostgreSQL backend
Event-driven microservices..."
```

### Reading Onboarding

```bash
# Get full onboarding
./get_onboarding.py --project-root /code/myproject
```

## Integration with Memory

Onboarding complements project memories:

- **Onboarding**: Static project structure and setup
- **Memories**: Dynamic findings and evolving knowledge

**Best Practice:** 
- Use onboarding for foundational project info
- Use memories for specific discoveries and patterns

## Workflow Tracking

### Task Progress

While Serena MCP has "Think" tools for reflection, this skill focuses on concrete documentation:

**Document Progress:**
```bash
./update_onboarding.py --project-root /code \
  --section "Recent Changes" \
  --content "$(date): Implemented user authentication"
```

**Track Key Decisions:**
```bash
./update_onboarding.py --project-root /code \
  --section "Design Decisions" \
  --content "Chose JWT for auth (vs sessions) - better for microservices"
```

## Best Practices

1. **Create onboarding early** in project lifecycle
2. **Update incrementally** as you learn
3. **Keep sections focused** and organized
4. **Link to files** using relative paths
5. **Document why**, not just what
6. **Review periodically** to keep current

## File Locations

```
project/
└── .tmp/
    └── .serena-skills/
        ├── onboarding.md       # This skill manages this
        └── memories/
            ├── patterns.md      # Memories managed separately
            └── api-notes.md
```
