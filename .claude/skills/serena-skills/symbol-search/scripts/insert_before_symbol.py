#!/usr/bin/env python3
"""
Insert code before a symbol using LSP
"""
import argparse
import os
import sys
from pathlib import Path

# Add serena to path if running standalone
serena_root = Path(__file__).parent.parent.parent.parent.parent.parent
if serena_root.exists():
    sys.path.insert(0, str(serena_root / "src"))

from solidlsp import SolidLanguageServer
from solidlsp.ls_config import Language, LanguageServerConfig
from solidlsp.settings import SolidLSPSettings


def insert_before_symbol(project_root: str, file: str, symbol: str, body: str, language: str = "python"):
    """Insert code before symbol"""
    
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
        # Find symbol location
        full_path = os.path.join(project_root, file)
        uri = Path(full_path).as_uri()
        
        doc_symbols = ls.get_document_symbols(uri)
        
        target_sym = None
        for sym in doc_symbols:
            if sym.name == symbol or symbol.endswith(f"/{sym.name}"):
                target_sym = sym
                break
        
        if not target_sym:
            raise ValueError(f"Symbol not found: {symbol}")
        
        # Read file
        with open(full_path, 'r', encoding='utf-8') as f:
            lines = f.readlines()
        
        # Insert before symbol (before start line)
        start_line = target_sym.range.start.line
        
        # Insert body before the symbol
        new_lines = lines[:start_line] + [body if body.endswith('\n') else body + '\n'] + lines[start_line:]
        
        # Write back
        with open(full_path, 'w', encoding='utf-8') as f:
            f.writelines(new_lines)
        
        return f"Code inserted before symbol: {symbol}"
        
    finally:
        ls.stop()


def main():
    parser = argparse.ArgumentParser(description="Insert code before symbol")
    parser.add_argument("--project-root", required=True, help="Absolute path to project root")
    parser.add_argument("--file", required=True, help="File containing symbol")
    parser.add_argument("--symbol", required=True, help="Symbol name path")
    parser.add_argument("--body", required=True, help="Code to insert")
    parser.add_argument("--language", default="python", help="Programming language")
    
    args = parser.parse_args()
    
    try:
        result = insert_before_symbol(args.project_root, args.file, args.symbol, args.body, args.language)
        print(result)
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
