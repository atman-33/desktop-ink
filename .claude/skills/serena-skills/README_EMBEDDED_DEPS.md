# Serena Skills - Embedded solidlsp Dependencies

This directory contains an embedded copy of Serena's `solidlsp` package and its dependencies to enable standalone operation of serena-skills.

## Directory Structure

```
serena-skills/
├── SKILL.md                    # Main skill documentation
├── README_EMBEDDED_DEPS.md     # Setup guide (English)
├── DEPENDENCIES_ja.md          # Setup guide (Japanese)
├── .venv/                      # Virtual environment
│
├── lib/                        # Shared implementation libraries
│   ├── common/                # Common utilities
│   ├── serena_deps/           # Serena dependencies
│   └── solidlsp/              # LSP implementation
│
└── skills/                     # Sub-skills
    ├── code-editor/           # Code editing operations
    ├── symbol-search/         # LSP-based symbol operations
    ├── file-ops/              # File system operations
    ├── memory-manager/        # Project memory management
    ├── project-config/        # Project configuration
    ├── shell-executor/        # Shell command execution
    └── workflow-assistant/    # Workflow automation
```

Each sub-skill follows the standard structure:
```
skill-name/
├── SKILL.md          # Skill documentation
└── scripts/          # Executable scripts
```

## Required Python Packages

To use serena-skills with the embedded solidlsp, you need to install dependencies in a virtual environment.

### Initial Setup (Required Once Per Environment)

The scripts automatically detect and use the virtual environment if available.

#### Linux/WSL/macOS

```bash
# Install uv if not already installed
curl -LsSf https://astral.sh/uv/install.sh | sh

# Navigate to serena-skills directory
cd ~/.codex/skills/serena-skills

# Create virtual environment
uv venv

# Install required packages
uv pip install pathspec requests pyright joblib pyyaml psutil overrides python-dotenv
```

#### Windows (PowerShell)

```powershell
# Install uv if not already installed
irm https://astral.sh/uv/install.ps1 | iex

# Navigate to serena-skills directory
cd ~\.codex\skills\serena-skills

# Create virtual environment
uv venv

# Install required packages
uv pip install pathspec requests pyright joblib pyyaml psutil overrides python-dotenv
```

### How It Works

- Scripts automatically detect the `.venv` directory and use it
- No need to manually activate the virtual environment
- Cross-platform: works on Linux, WSL, macOS, and Windows
- Portable: copy serena-skills folder to any project (re-run `uv venv` on different OS)

### Package Purpose

- **pathspec**: Pattern matching for file ignores
- **requests**: HTTP client (provides charset-normalizer as dependency)
- **pyright**: Python language server
- **joblib**: Parallel processing utilities
- **pyyaml**: YAML configuration parsing

## Language-Specific Requirements

For language server support beyond Python, install the appropriate language servers:

- **TypeScript/JavaScript**: `npm install -g typescript typescript-language-server`
- **Go**: Install `gopls`
- **Rust**: Install `rust-analyzer`
- See Serena documentation for other languages

## Usage

Scripts automatically add the serena-skills root to Python path, so imports work correctly:

```python
from solidlsp import SolidLanguageServer
from solidlsp.ls_config import Language, LanguageServerConfig
```

## Maintenance

This is a snapshot copy. To update from Serena:

1. Copy `/src/solidlsp` from Serena repo
2. Copy required files from `/src/serena` (text_utils.py, constants.py, util/file_system.py)
3. Update imports to use `serena_deps` instead of `serena`
4. Ensure sensai imports use `serena_deps.sensai_shim`

## Notes

- This embedded approach trades code duplication for standalone operation
- Updates to Serena's solidlsp won't automatically reflect here
- Consider using Serena directly if you need the latest features
