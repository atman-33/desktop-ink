#!/usr/bin/env python3
"""
Rename symbol across codebase using LSP
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


def rename_symbol(project_root: str, file: str, symbol: str, new_name: str, language: str = "python"):
    """Rename symbol throughout codebase"""
    
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
        
        # Get rename edits from LSP
        position = target_sym.range.start
        workspace_edit = ls.rename(uri, position, new_name)
        
        if not workspace_edit or not workspace_edit.changes:
            return f"No changes needed for renaming {symbol} to {new_name}"
        
        # Apply edits to files
        files_changed = 0
        for file_uri, edits in workspace_edit.changes.items():
            file_path = Path(file_uri.replace('file://', ''))
            
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            
            # Apply edits in reverse order (to preserve positions)
            sorted_edits = sorted(edits, key=lambda e: (e.range.start.line, e.range.start.character), reverse=True)
            
            lines = content.split('\n')
            for edit in sorted_edits:
                start_line = edit.range.start.line
                start_char = edit.range.start.character
                end_line = edit.range.end.line
                end_char = edit.range.end.character
                
                # Simple single-line edit
                if start_line == end_line:
                    line = lines[start_line]
                    lines[start_line] = line[:start_char] + edit.newText + line[end_char:]
            
            new_content = '\n'.join(lines)
            
            with open(file_path, 'w', encoding='utf-8') as f:
                f.write(new_content)
            
            files_changed += 1
        
        return f"Symbol renamed: {symbol} -> {new_name} ({files_changed} file(s) changed)"
        
    finally:
        ls.stop()


def main():
    parser = argparse.ArgumentParser(description="Rename symbol across codebase")
    parser.add_argument("--project-root", required=True, help="Absolute path to project root")
    parser.add_argument("--file", required=True, help="File containing symbol")
    parser.add_argument("--symbol", required=True, help="Symbol name path")
    parser.add_argument("--new-name", required=True, help="New symbol name")
    parser.add_argument("--language", default="python", help="Programming language")
    
    args = parser.parse_args()
    
    try:
        result = rename_symbol(args.project_root, args.file, args.symbol, args.new_name, args.language)
        print(result)
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
