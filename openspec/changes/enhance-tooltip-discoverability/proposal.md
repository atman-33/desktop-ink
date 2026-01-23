# Proposal: Enhance Control Palette Tooltip Discoverability

## Context

The Control Palette provides visual buttons for core drawing operations. Currently, the **Toggle Draw** button's tooltip shows only "Toggle Draw (Win+Shift+D)", which informs users about the keyboard shortcut and the button's basic function.

However, the application supports an important secondary interaction: **Alt + Double-click** on the Toggle Draw button activates **Temporary Draw Mode**. This mode allows users to quickly enter drawing mode while holding Shift, which is distinct from the permanent draw mode toggled by the button or Win+Shift+D.

**Problem:** Users are unlikely to discover the Alt + Double-click interaction because:
1. The tooltip does not mention this capability
2. There is no visual or textual hint about this advanced interaction
3. Users may miss a valuable feature that enhances their workflow

## Proposed Change

Enhance the Toggle Draw button's tooltip to include information about the Alt + Double-click interaction for Temporary Draw Mode.

**Current tooltip:**
```
Toggle Draw (Win+Shift+D)
```

**Proposed tooltip:**
```
Toggle Draw (Win+Shift+D)
Alt+Double-click for temporary mode
```

## Benefits

1. **Improved Discoverability:** Users will learn about Temporary Draw Mode through natural UI exploration (hovering over the button)
2. **Better User Experience:** Users can leverage both permanent and temporary draw modes effectively
3. **Minimal Implementation Effort:** Simple text change with no logic modifications required
4. **Maintains Simplicity:** Two-line tooltip remains concise and readable

## Scope

- **In scope:** Update the Toggle Draw button's tooltip text in the Control Palette
- **Out of scope:** 
  - Adding UI-based tutorial or help system
  - Modifying other tooltips (can be addressed separately)
  - Changing the Alt + Double-click behavior itself

## Alternatives Considered

1. **Keep tooltip as-is:** Rejected because it fails to address the discoverability problem
2. **Add in-app help system:** Deferred as a longer-term enhancement; requires more design and implementation effort
3. **Show hint on first launch:** Deferred; requires persistence logic and additional UI
4. **Add to README only:** Insufficient; users may not read documentation

## Success Criteria

- Tooltip displays two lines of text clearly
- Alt + Double-click interaction is mentioned explicitly
- Tooltip remains readable and uncluttered
- No performance or visual regression introduced
