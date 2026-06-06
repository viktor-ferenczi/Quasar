# Quasar/Components/Pages/Configs.razor.css

**Module:** Quasar.Components  **Kind:** CSS  **Tier:** 3

## Summary
Scoped stylesheet for `Configs.razor`. Implements the two-column sticky-sidebar layout, styled template-tile list, one-line plugin-catalog description preview layout, option cards with search-highlight states, and Workshop thumbnail sizing.

## Structure
Key rule groups:
- `.configs-page-shell` — `MudGrid` wrapper; on screens >= 1280 px sets `height: calc(100vh - 11rem)` so the shell fills the viewport.
- `.configs-sidebar` / `.configs-sidebar-inner` / `.configs-template-list` — flex column layout; the template list scrolls independently (`overflow-y: auto`) while the sidebar header stays fixed. On large screens the sidebar is `position: sticky`.
- `.configs-main-column` / `.configs-editor-scroll` — on large screens scrolls independently (`overflow-y: auto; height: 100%`); on mobile collapses to `height: auto; overflow: visible`.
- `.config-template-tile` — background using `--mud-palette-background-gray`; smooth color/transform transition.
- `.config-template-selected` — primary palette background + text for the active template.
- `.config-template-button` / `.config-template-button-selected` — borderless full-height left-aligned button; selected state adds a left inset box shadow (`inset 4px 0 0 currentColor`).
- `.config-secondary` — `opacity: 0.78` for caption/helper text.
- `.workshop-thumbnail` — 80×80 px `object-fit: cover` image with rounded border.
- `.configs-search-field` — `flex: 1 1 20rem; min-width: min(22rem, 100%)`.
- `.plugin-description-cell` / `.plugin-description-line` / `.plugin-description-more` — keeps plugin catalog descriptions on one line with a fixed action button; preview cutoff is character-count based in `Configs.razor`.
- `.configs-add-button` — fixed 56 px height to align with outlined text fields.
- `.configs-dev-folder-path-row` / `.configs-dev-folder-path-field` / `.configs-dev-folder-browse` — flex row for path + browse button; collapses to column on mobile.
- `.configs-expansion-panels` — CSS grid with 0.5 rem gap.
- `.configs-expansion-panel` — custom card style (background, border, border-radius, no shadow); hover adds `background-color: --mud-palette-action-default-hover`.
- `.config-option-card` — option card background with `scroll-margin-top: 6rem` for jump-to scroll.
- `.config-option-highlight` — yellow-tinted background + left inset shadow for search matches.
- `.config-option-first` — primary-color left inset for the first match.
- `.configs-dev-folder-row-selected` — amber-tinted highlight for the dev-folder row being edited.

## Dependencies
- [`Quasar/Components/Pages/Configs.razor`](Configs.razor.md) (scoped to this component)
