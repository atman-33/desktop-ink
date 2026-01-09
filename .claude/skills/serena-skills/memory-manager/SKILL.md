---
name: memory-manager
description: Project-specific memory management for storing and retrieving context, knowledge, and notes about codebases. Use for maintaining project information across sessions, documenting findings, and building knowledge bases.
---

# Memory Manager

Store and retrieve project-specific knowledge and context.

## Core Capabilities

### 1. Write Memory

Save information to project memory.

```bash
python3 {skill_path}/scripts/write_memory.py \
  --project-root /path/to/project \
  --name "architecture" \
  --content "The system uses microservices architecture..."
```

**Parameters:**
- `--project-root`: Absolute path to project
- `--name`: Memory name (without .md extension)
- `--content`: Content to save (Markdown format)

### 2. Read Memory

Retrieve saved memory.

```bash
python3 {skill_path}/scripts/read_memory.py \
  --project-root /path/to/project \
  --name "architecture"
```

### 3. List Memories

Show all available memories.

```bash
python3 {skill_path}/scripts/list_memories.py \
  --project-root /path/to/project
```

**Output:** JSON array of memory names

### 4. Delete Memory

Remove a memory file.

```bash
python3 {skill_path}/scripts/delete_memory.py \
  --project-root /path/to/project \
  --name "old-notes"
```

### 5. Edit Memory

Update existing memory content.

```bash
python3 {skill_path}/scripts/edit_memory.py \
  --project-root /path/to/project \
  --name "architecture" \
  --needle "microservices" \
  --replace "event-driven microservices" \
  --mode literal
```

**Modes:**
- `literal`: Exact string match
- `regex`: Regular expression pattern (Python re module)

## Storage Location

Memories are stored in `.tmp/.serena-skills/memories/` within the project root:
```
project/
└── .tmp/
    └── .serena-skills/
        └── memories/
            ├── architecture.md
            ├── coding-style.md
            └── api-endpoints.md
```

## Use Cases

**Document Architecture:**
```bash
./write_memory.py --project-root /code --name architecture \
  --content "# Architecture

## Components
- API Server (port 8000)
- Database (PostgreSQL)
- Cache (Redis)

## Data Flow
..."
```

**Track API Changes:**
```bash
./write_memory.py --project-root /code --name api-changes \
  --content "# API Changes

## 2026-01-10
- Added /v2/users endpoint
- Deprecated /v1/users
..."
```

**Store Code Patterns:**
```bash
./write_memory.py --project-root /code --name patterns \
  --content "# Common Patterns

## Error Handling
All API endpoints use ErrorHandler middleware...
..."
```

## Best Practices

- Use descriptive memory names (kebab-case recommended)
- Write in Markdown format for better readability
- Keep memories focused on specific topics
- Update memories when project knowledge changes
- List memories periodically to avoid duplication
