#!/usr/bin/env python3
"""
Get symbols overview from a file using LSP
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

# Add common utilities
common_path = Path(__file__).parent.parent.parent / "common"
if common_path.exists():
    sys.path.insert(0, str(common_path.parent))

from solidlsp import SolidLanguageServer
from solidlsp.ls_config import Language, LanguageServerConfig
from common.utils import (
    create_lsp_settings,
    get_project_language,
    limit_output_length,
    format_error
)


def get_symbols_overview(
    project_root: str,
    relative_file_path: str,
    depth: int = 0,
    language: str | None = None,
    max_answer_chars: int = -1
):
    """Get symbols overview for a file"""
    
    # Auto-detect language if not provided
    if language is None:
        language = get_project_language(project_root)
    
    # Setup language server
    lang = Language(language.lower())
    ls_config = LanguageServerConfig(
        code_language=lang,
        ignored_paths=[],
        encoding="utf-8"
    )
    
    settings = create_lsp_settings(project_root)
    
    ls = SolidLanguageServer.create(ls_config, project_root, solidlsp_settings=settings)
    ls.start()
    
    try:
        # Get document symbols
        full_path = os.path.join(project_root, relative_file_path)
        
        if not os.path.exists(full_path):
            raise FileNotFoundError(f"File not found: {relative_file_path}")
        
        uri = Path(full_path).as_uri()
        symbols = ls.get_document_symbols(uri)
        
        # Transform to compact format
        def transform_symbols(syms, current_depth=0):
            result = {}
            for sym in syms:
                kind_name = sym.kind.name if hasattr(sym.kind, 'name') else str(sym.kind)
                if kind_name not in result:
                    result[kind_name] = []
                
                if current_depth < depth and hasattr(sym, 'children') and sym.children:
                    children_dict = transform_symbols(sym.children, current_depth + 1)
                    result[kind_name].append({sym.name: children_dict})
                else:
                    result[kind_name].append(sym.name)
            
            return result
        
        overview = transform_symbols(symbols)
        return overview
        
    finally:
        ls.stop()


def main():
    parser = argparse.ArgumentParser(description="Get symbols overview from a file")
    parser.add_argument("--project-root", required=True, help="Absolute path to project root")
    parser.add_argument("--file", required=True, help="Relative path to file")
    parser.add_argument("--depth", type=int, default=0, help="Descendant depth (default: 0)")
    parser.add_argument("--language", default=None, help="Programming language (auto-detected if not specified)")
    parser.add_argument("--max-answer-chars", type=int, default=-1, help="Max output chars (-1 for default)")
    
    args = parser.parse_args()
    
    try:
        overview = get_symbols_overview(
            args.project_root,
            args.file,
            args.depth,
            args.language,
            args.max_answer_chars
        )
        
        # Apply output limit
        output = limit_output_length(overview, args.project_root, args.max_answer_chars)
        print(output)
        
    except Exception as e:
        context = {
            "project_root": args.project_root,
            "file": args.file,
            "operation": "get_symbols_overview"
        }
        print(format_error(e, context), file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
