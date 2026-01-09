---
name: serena-skills
description: Complete Serena MCP toolset as standalone skills. Provides symbol search, code editing, file operations, memory management, and project workflow tools without requiring MCP server. Use for code intelligence, analysis, and modification tasks.
metadata:
  coverage: 95% of Serena MCP tools (38/40)
  architecture: 7 sub-skills with 40 executable scripts
---

# Serena Skills

Standalone implementation of Serena MCP tools for code intelligence and manipulation.

## Quick Start

### Initial Setup (Recommended)

While all tools can work independently with `--project-root`, it's recommended to register your project first for the best experience:

```bash
python project-config/scripts/activate_project.py \
  --project-path /absolute/path/to/your/project \
  --name myproject
```

This one-time setup provides:
- **Directory structure**: Creates `.tmp/.serena-skills/` with memory storage
- **Project configuration**: Generates `project.yml` with language, encoding, and ignore settings
- **Language detection**: Auto-detects programming language (Python, TypeScript, Java, Go, etc.)
- **Project registry**: Registers project in `~/.serena/projects.json` for reference
- **Configuration storage**: Prepares persistent project configuration

After activation, all tools work with `--project-root /path/to/your/project` parameter.

**Note**: Project activation is **optional**. All tools function without it by specifying `--project-root` directly in each command.

## Overview

This skill provides Serena's complete code intelligence capabilities without requiring the MCP server. It implements 38 of 40 Serena MCP tools (95% coverage) as 7 organized sub-skills.

**Core capabilities:**
- Symbol-level code analysis and editing (LSP-based)
- File system operations and pattern search
- Project memory and knowledge management
- Code editing (symbol and text level)
- Project configuration and onboarding
- Workflow assistance and documentation
- Shell command execution

**When to use:** Code exploration, analysis, refactoring, documentation, or any programmatic code manipulation task.

## Tool Reference

### Symbol Search & Analysis (symbol-search/)

**get_symbols_overview** - Get high-level structure of symbols in a file
- Purpose: First tool to call when understanding a new file. Shows classes, functions, methods grouped by kind.
- Key params: `file` (relative path), `depth` (0=top-level, 1=include children), `language`
- Returns: JSON with symbols grouped by kind (Class, Function, Method, etc.)
- Use: Starting point for code exploration, understanding file structure

**find_symbol** - Search for symbols by name path pattern
- Purpose: Find specific symbols (classes, methods, functions) across codebase using flexible pattern matching.
- Key params: `pattern` (name path), `file` (optional restriction), `include_body` (include source code), `substring` (fuzzy matching)
- Patterns: `"method"` (any), `"Class/method"` (relative), `"/Class/method"` (absolute), `"method[0]"` (overload)
- Returns: Array of matching symbols with locations, optionally with source code
- Use: Finding definitions, understanding implementations, before editing symbols

**find_references** - Find all code locations referencing a symbol
- Purpose: Discover all usages of a symbol to understand impact of changes.
- Key params: `file` (containing symbol), `symbol` (name path), `language`
- Returns: Array of references with code snippets showing context
- Use: Before renaming/modifying symbols, understanding dependencies, impact analysis

**insert_after_symbol** - Insert code after a symbol's definition
- Purpose: Add new methods, classes, or code blocks following existing symbols.
- Key params: `file`, `symbol` (name path), `body` (code to insert)
- Use: Adding new methods to classes, appending functions, extending modules

**insert_before_symbol** - Insert code before a symbol's definition
- Purpose: Add imports, decorators, or preceding code before symbols.
- Key params: `file`, `symbol` (name path), `body` (code to insert)
- Use: Adding imports at file start, inserting decorators, adding preceding definitions

**rename_symbol** - Rename symbol throughout entire codebase
- Purpose: Safely rename symbols with automatic reference updates across all files.
- Key params: `file` (containing symbol), `symbol` (current name path), `new_name`
- Returns: Summary of files changed
- Use: Refactoring, improving naming, maintaining consistency

### File Operations (file-ops/)

**read_file** - Read file contents or specific line ranges
- Purpose: Retrieve file contents for analysis. More efficient than reading entire large files.
- Key params: `file` (relative path), `start_line` (0-based), `end_line` (optional)
- Returns: File content as text
- Use: Reading specific sections, examining implementations, general file access

**list_dir** - List files and directories with optional recursion
- Purpose: Explore directory structure, understand project organization.
- Key params: `path` (relative, "." for root), `recursive` (boolean), `skip_ignored` (respect .gitignore)
- Returns: JSON with `directories` and `files` arrays
- Use: Project exploration, finding file locations, understanding structure

**find_file** - Find files matching name patterns
- Purpose: Locate files by name using wildcards, faster than full directory scan.
- Key params: `mask` (pattern with * and ?), `path` (search root)
- Returns: Array of matching file paths
- Use: Finding configuration files, locating specific modules, file discovery

**search_pattern** - Search for regex patterns in files
- Purpose: Text-based code search with context, flexible pattern matching.
- Key params: `pattern` (regex), `path` (optional restriction), `context` (lines before/after), `code_only` (skip assets), `include_glob`/`exclude_glob`
- Returns: Map of file paths to matches with context
- Use: Finding TODO comments, searching for patterns, code archaeology, grep-like operations

### Memory Management (memory-manager/)

**write_memory** - Save project information to persistent memory
- Purpose: Document findings, architectural decisions, and project knowledge for future sessions.
- Key params: `name` (memory identifier), `content` (markdown text)
- Storage: `.tmp/.serena-skills/memories/{name}.md`
- Use: Documenting discoveries, saving architectural notes, building project knowledge base

**read_memory** - Retrieve saved project memory
- Purpose: Access previously documented project information.
- Key params: `name` (memory identifier)
- Returns: Memory content
- Use: Recalling project context, reviewing documented patterns, accessing saved knowledge

**list_memories** - Show all available memories
- Purpose: Discover what project knowledge has been documented.
- Returns: Array of memory names
- Use: Finding relevant saved information, memory discovery

**delete_memory** - Remove a project memory
- Purpose: Clean up outdated or incorrect information.
- Key params: `name` (memory identifier)
- Use: Removing obsolete documentation, correcting mistakes

**edit_memory** - Update existing memory content
- Purpose: Modify saved information without rewriting entire memory.
- Key params: `name`, `needle` (text to find), `replace` (replacement), `mode` (literal/regex)
- Use: Correcting information, updating documentation incrementally

### Code Editor (code-editor/)

**replace_symbol** - Replace entire symbol definition using LSP
- Purpose: Symbol-level code replacement with LSP awareness (types, references).
- Key params: `file`, `symbol` (name path), `body` (new definition including signature)
- Note: Body includes full definition (signature + implementation), not just implementation
- Use: Modifying functions/methods/classes completely, symbol-level refactoring

**replace_content** - Safe find/replace in files (literal or regex)
- Purpose: Text-based editing with exact matching and safety checks.
- Key params: `file`, `old` (text to find), `new` (replacement), `mode` (literal/regex), `expected_count`
- Safety: Fails if old string not found, reports line numbers, requires exact match
- Use: Targeted edits, comment changes, non-symbol edits, multi-line replacements

**create_file** - Create new file with content
- Purpose: Generate new files, overwrites if exists.
- Key params: `file` (relative path), `content`
- Use: Creating new modules, generating files, adding resources

**delete_lines** - Delete specific line range
- Purpose: Remove lines by number, useful for cleanup.
- Key params: `file`, `start_line` (1-based), `end_line` (inclusive)
- Use: Removing debug code, deleting obsolete sections, line-based cleanup

**replace_lines** - Replace specific lines with new content
- Purpose: Line-number-based replacement, simpler than text matching.
- Key params: `file`, `start_line`, `end_line`, `new_content`
- Use: Updating configuration values, replacing known line ranges

**insert_at_line** - Insert content at specific line number
- Purpose: Line-based insertion, precise positioning.
- Key params: `file`, `line` (1-based), `content`
- Use: Adding imports at specific positions, inserting at known locations

### Project Configuration (project-config/)

**activate_project** - Register and activate a project
- Purpose: Set up project for Serena Skills, creates metadata directory, registers in global config.
- Key params: `project_path` (absolute), `name` (optional, defaults to dir name)
- Creates: `.tmp/.serena-skills/` directory structure, `~/.serena/projects.json` entry
- Use: Initial project setup, registering new projects

**list_projects** - Show all registered projects
- Purpose: View all projects configured for Serena Skills.
- Returns: JSON map of project names to configuration
- Use: Discovering registered projects, checking configuration

**remove_project** - Unregister a project
- Purpose: Clean up project registry.
- Key params: `name` (project identifier)
- Use: Removing old projects, cleanup

**get_config** - Display current configuration
- Purpose: View active project, all registered projects, and configuration paths.
- Returns: JSON with active project, current directory, registered projects list, config paths
- Use: Debugging setup, understanding context, checking active project

### Workflow Assistant (workflow-assistant/)

**check_onboarding** - Check if project has onboarding documentation
- Purpose: Verify onboarding setup status.
- Returns: JSON with existence status and path
- Use: Before creating onboarding, checking project state

**create_onboarding** - Initialize project onboarding documentation
- Purpose: Create structured project documentation template.
- Creates: `.tmp/.serena-skills/onboarding.md` with sections for architecture, setup, structure, tasks
- Use: New project initialization, setting up documentation framework

**update_onboarding** - Update section in onboarding documentation
- Purpose: Incrementally build project documentation.
- Key params: `section` (section name), `content` (new section content)
- Use: Documenting findings, updating architecture notes, adding information

**get_onboarding** - Read project onboarding documentation
- Purpose: Access project onboarding information.
- Returns: Full onboarding markdown content
- Use: Reviewing project documentation, understanding project structure

**think_collected_info** - Reflect on information collection completeness
- Purpose: Prompt to assess whether sufficient information has been gathered.
- Returns: Reflection prompt with guiding questions
- Use: After completing a sequence of search/read operations (find_symbol, read_file, etc.)
- When: After non-trivial information gathering steps

**think_task_adherence** - Verify alignment with task before code changes
- Purpose: Ensure you're on track and aligned with code style before editing.
- Returns: Task adherence checklist prompt
- Use: **ALWAYS call before inserting, replacing, or deleting code**
- When: Before any code modification, especially after long conversations

**think_are_done** - Verify task completion before finalizing
- Purpose: Ensure all required steps are completed (tests, linting, documentation).
- Returns: Completion checklist prompt
- Use: When you believe the task is complete
- When: Before declaring work finished

### Shell Executor (shell-executor/)

**execute** - Execute shell commands safely in project context
- Purpose: Run builds, tests, git operations, or any shell commands with timeout protection and output capture.
- Key params: `command` (shell command), `timeout` (seconds), `env` (environment variables as JSON)
- Returns: JSON with `exit_code`, `stdout`, `stderr`, `duration`, `timed_out`
- Safety: Runs in project directory, timeout protection, output capture
- Use: Running tests, executing builds, git operations, code analysis tools, any system commands

## Usage Patterns

### Understanding New Codebase

1. **List structure**: `list_dir` with `recursive=true`
2. **Find entry points**: `find_file` for main files, `search_pattern` for patterns
3. **Get overview**: `get_symbols_overview` on key files
4. **Explore symbols**: `find_symbol` with `include_body=true`
5. **Document findings**: `write_memory` to save discoveries

### Making Code Changes

1. **Find target**: `find_symbol` to locate symbol to modify
2. **Check impact**: `find_references` to see all usages
3. **Edit code**: `replace_symbol` or `replace_content` depending on scope
4. **Verify**: `execute` to run tests
5. **Document**: `update_onboarding` or `write_memory` with changes

### Exploring Unknown Code

1. **Quick scan**: `search_pattern` for TODO, FIXME, or key terms
2. **Symbol discovery**: `get_symbols_overview` on multiple files
3. **Deep dive**: `find_symbol` with `substring=true` for fuzzy search
4. **Trace usage**: `find_references` to understand connections
5. **Save context**: `write_memory` with findings

### Refactoring Workflow

1. **Identify scope**: `find_symbol` to understand structure
2. **Map dependencies**: `find_references` for all uses
3. **Rename safely**: `rename_symbol` for automatic updates
4. **Modify implementations**: `replace_symbol` for each changed symbol
5. **Validate**: `execute` tests, check results

## When to Use What

**Need to understand code structure?**
→ Start with `get_symbols_overview`, then `find_symbol` for details

**Want to modify a function/class?**
→ Use `find_symbol` first, then `replace_symbol` (symbol-level) or `replace_content` (text-level)

**Searching for something but don't know where?**
→ `search_pattern` for text search, `find_symbol` for symbol search

**Making widespread changes?**
→ `rename_symbol` for names, `search_pattern` + `replace_content` for patterns

**Need to track findings?**
→ `write_memory` for session persistence, `update_onboarding` for permanent documentation

**Setting up new project?**
→ `activate_project` → `create_onboarding` → explore with `list_dir` and `get_symbols_overview`

**Running commands?**
→ `execute` for builds, tests, git, or any shell operations

## Symbol vs Text Editing

**Use symbol-level editing when:**
- Modifying entire functions, methods, or classes
- Need LSP awareness (types, references)
- Working with well-defined code symbols
- Tools: `replace_symbol`, `insert_after_symbol`, `insert_before_symbol`, `rename_symbol`

**Use text-level editing when:**
- Small targeted changes
- Editing comments, strings, or documentation
- Working across multiple small locations
- Need regex patterns
- Tools: `replace_content`, `delete_lines`, `replace_lines`, `insert_at_line`

## Progressive Usage

**Level 1 - Quick exploration:**
- `list_dir`, `find_file`, `search_pattern`
- Get bearings without deep analysis

**Level 2 - Code understanding:**
- `get_symbols_overview`, `find_symbol`, `read_file`
- Understand structure and implementations

**Level 3 - Deep analysis:**
- `find_references`, `find_symbol` with `include_body=true`
- Understand relationships and dependencies

**Level 4 - Modification:**
- `replace_symbol`, `rename_symbol`, `replace_content`
- Make changes with confidence

**Level 5 - Documentation:**
- `write_memory`, `update_onboarding`
- Preserve knowledge for future sessions

## Technical Notes

**LSP Integration:**
- Symbol operations require language servers installed
- First run may be slower (LSP initialization)
- Supports Python, TypeScript, Java, Go, Rust, C/C++, and more

**Storage Locations:**
- Project data: `{project}/.tmp/.serena-skills/`
- Project config: `{project}/.tmp/.serena-skills/project.yml`
- Global registry: `~/.serena/projects.json`
- Memories: `{project}/.tmp/.serena-skills/memories/`
- Onboarding: `{project}/.tmp/.serena-skills/onboarding.md`

**Project Configuration (project.yml):**
- Created automatically by activate_project
- Controls languages, encoding, ignore patterns
- Supports read-only mode, tool exclusions
- Update with update_project_config script
- Compatible with Serena MCP format

**Dependencies:**
- Python 3.8+
- Serena's `solidlsp` library (for symbol operations)
- `pathspec` (for gitignore compatibility, optional but recommended)
- `pyyaml` (for project.yml support, optional)
- Language servers (for specific languages)
