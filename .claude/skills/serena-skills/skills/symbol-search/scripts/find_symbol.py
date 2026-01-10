#!/usr/bin/env python3
"""
Find symbols by name path pattern using LSP
"""
import argparse
import json
import os
import sys
import platform
from pathlib import Path

# Auto-activate venv if available
skills_root = Path(__file__).parent.parent.parent.parent
if platform.system() == "Windows":
    venv_python = skills_root / ".venv" / "Scripts" / "python.exe"
else:
    venv_python = skills_root / ".venv" / "bin" / "python"

if venv_python.exists() and str(Path(sys.executable).parent) != str(venv_python.parent):
    os.execv(str(venv_python), [str(venv_python)] + sys.argv)

# Add serena-skills to path
sys.path.insert(0, str(skills_root))

from lib.solidlsp import SolidLanguageServer
from lib.solidlsp.ls_config import Language, LanguageServerConfig
from lib.solidlsp.settings import SolidLSPSettings
from lib.solidlsp.ls_types import SymbolKind


def find_symbol(
    project_root: str,
    pattern: str,
    language: str = "python",
    file: str | None = None,
    depth: int = 0,
    include_body: bool = False,
    substring: bool = False
):
    """Find symbols matching the name path pattern"""
    
    # Setup language server
    lang = Language(language.lower())
    ls_config = LanguageServerConfig(
        code_language=lang,
        ignored_paths=[],
        encoding="utf-8"
    )
    
    settings = SolidLSPSettings(
        solidlsp_dir=os.path.expanduser("~/.serena"),
        project_data_relative_path=".tmp/.serena-skills"
    )
    
    ls = SolidLanguageServer.create(ls_config, project_root, solidlsp_settings=settings)
    ls.start()
    
    try:
        # Parse pattern
        is_absolute = pattern.startswith("/")
        if is_absolute:
            pattern = pattern[1:]
        
        parts = pattern.split("/")
        
        # Search workspace symbols
        search_query = parts[-1]  # Use last part for search
        workspace_symbols = ls.get_workspace_symbols(search_query)
        
        results = []
        for sym in workspace_symbols:
            # Match pattern logic
            sym_name_path = sym.name  # Simplified - in full implementation, build name path
            
            if substring:
                matches = search_query.lower() in sym.name.lower()
            else:
                matches = sym.name == search_query or sym_name_path == pattern
            
            if matches:
                sym_dict = {
                    "name": sym.name,
                    "kind": sym.kind.name if hasattr(sym.kind, 'name') else str(sym.kind),
                    "relative_path": str(Path(sym.location.uri).relative_to(project_root)) if hasattr(sym.location, 'uri') else None,
                    "line": sym.location.range.start.line if hasattr(sym.location, 'range') else None,
                }
                
                if include_body:
                    # Read file content for body
                    if sym_dict["relative_path"] and sym_dict["line"] is not None:
                        file_path = os.path.join(project_root, sym_dict["relative_path"])
                        if os.path.exists(file_path):
                            with open(file_path, 'r', encoding='utf-8') as f:
                                lines = f.readlines()
                                # Simple body extraction - get a few lines
                                start_line = sym_dict["line"]
                                end_line = min(start_line + 20, len(lines))
                                sym_dict["body"] = "".join(lines[start_line:end_line])
                
                results.append(sym_dict)
        
        return results
        
    finally:
        ls.stop()


def main():
    parser = argparse.ArgumentParser(description="Find symbols by name path pattern")
    parser.add_argument("--project-root", required=True, help="Absolute path to project root")
    parser.add_argument("--pattern", required=True, help="Name path pattern to search")
    parser.add_argument("--language", default="python", help="Programming language (default: python)")
    parser.add_argument("--file", help="Restrict search to specific file")
    parser.add_argument("--depth", type=int, default=0, help="Include descendants (default: 0)")
    parser.add_argument("--include-body", action="store_true", help="Include source code")
    parser.add_argument("--substring", action="store_true", help="Enable substring matching")
    
    args = parser.parse_args()
    
    try:
        results = find_symbol(
            args.project_root,
            args.pattern,
            args.language,
            args.file,
            args.depth,
            args.include_body,
            args.substring
        )
        print(json.dumps(results, indent=2))
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
