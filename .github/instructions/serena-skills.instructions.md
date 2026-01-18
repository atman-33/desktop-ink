---
name: Serena Skills Usage Policy
description: Policy for using serena-skills for code intelligence and symbol-level operations
applyTo: '**'
---

# Serena Skills Usage Policy

## CRITICAL: Primary Reference

This project uses the **Serena Skills Agent Skill** for all code intelligence and symbol-level operations.

**🚨 MANDATORY: Read `skills/serena-skills/SKILL.md` for complete documentation:**
- All available tools and their usage
- Setup and activation instructions
- Usage patterns and examples
- Troubleshooting guide

## Core Policy

When working with code in this project:

1. **ALWAYS use Serena Skills tools** for:
   - Finding/analyzing symbols (classes, methods, functions)
   - Modifying code at symbol level
   - Understanding code structure
   - Cross-file operations

2. **DO NOT use standard text-based tools** (semantic_search, grep_search, read_file) for symbol operations

3. **ALWAYS consult SKILL.md** for:
   - Which tool to use
   - How to use it
   - When to use it

## Required First Step

Before any code operation: **Read `skills/serena-skills/SKILL.md`**

The SKILL.md is the single source of truth for all Serena Skills usage.
