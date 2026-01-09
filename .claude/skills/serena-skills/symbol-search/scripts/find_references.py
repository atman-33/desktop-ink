#!/usr/bin/env python3
"""
Find references to a symbol using LSP
"""
import argparse
import json
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


def find_references(project_root: str, file: str, symbol: str, language: str = "python"):
    """Find all references to a symbol"""
    
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
        # First find the symbol definition
        full_path = os.path.join(project_root, file)
        uri = Path(full_path).as_uri()
        
        # Get document symbols to find the target
        doc_symbols = ls.get_document_symbols(uri)
        
        target_sym = None
        for sym in doc_symbols:
            if sym.name == symbol or symbol.endswith(f"/{sym.name}"):
                target_sym = sym
                break
        
        if not target_sym:
            return []
        
        # Find references
        position = target_sym.range.start
        references = ls.find_references(uri, position, include_declaration=True)
        
        results = []
        for ref in references:
            ref_path = Path(ref.uri).relative_to(project_root) if hasattr(ref, 'uri') else None
            ref_line = ref.range.start.line if hasattr(ref, 'range') else None
            
            # Get code snippet around reference
            snippet = ""
            if ref_path and ref_line is not None:
                ref_file_path = os.path.join(project_root, str(ref_path))
                if os.path.exists(ref_file_path):
                    with open(ref_file_path, 'r', encoding='utf-8') as f:
                        lines = f.readlines()
                        start = max(0, ref_line - 1)
                        end = min(len(lines), ref_line + 2)
                        snippet = "".join(lines[start:end])
            
            results.append({
                "relative_path": str(ref_path) if ref_path else None,
                "line": ref_line,
                "snippet": snippet
            })
        
        return results
        
    finally:
        ls.stop()


def main():
    parser = argparse.ArgumentParser(description="Find references to a symbol")
    parser.add_argument("--project-root", required=True, help="Absolute path to project root")
    parser.add_argument("--file", required=True, help="File containing the symbol")
    parser.add_argument("--symbol", required=True, help="Symbol name path")
    parser.add_argument("--language", default="python", help="Programming language (default: python)")
    
    args = parser.parse_args()
    
    try:
        results = find_references(
            args.project_root,
            args.file,
            args.symbol,
            args.language
        )
        print(json.dumps(results, indent=2))
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
