# Design rules — HienXanh POS

Read this before adding or changing any screen. When a rule and a personal
preference disagree, the rule wins; when two rules disagree, the one closer to
"fewer elements, clearer information" wins.

Reference point of view: a premium, quiet, editorial storefront (see the Solara
Halo S1 reference at awwwards). Near-black warm canvas, one accent, generous
negative space, no decorative chrome. The product is an operator's tool used for
a whole shift, so "premium" here means calm and legible, never dark-and-glowy.

---

## 1. Foundations

### 1.1 Color

Cool slate base, high-contrast ink, one deep-teal brand accent, slate-blue for
secondary data, amber reserved for attention. Values live only in
`Styles/Theme.axaml`; screens reference the semantic brushes with `DynamicResource`
and never hardcode a hex.

| Role | Dark | Light | Use for |
| --- | --- | --- | --- |
| Primary | `#4FB3A6` | `#0F6B62` | brand accent, active state, primary text-on-dark accent |
| OnPrimary | `#0A1214` | `#FFFFFF` | text/icons drawn on a Primary fill |
| Background | `#0F1316` | `#F5F7F9` | app canvas behind everything |
| Surface | `#161B1F` | `#FFFFFF` | cards, tiles, panels, dialogs |
| SurfaceMuted | `#1E242A` | `#EDF1F5` | icon plates, table headers, inset areas |
| Border | `#2B333A` | `#D5DDE5` | hairlines (1px) separating surface from canvas |
| BorderStrong | `#5E6B78` | `#8794A2` | inputs, dividers inside a surface (3:1 on Surface) |
| OnSurface | `#EDF2F6` | `#101820` | primary text |
| OnSurfaceMuted | `#A3AFBA` | `#55636F` | secondary text, captions |
| Secondary | `#93A8C6` | `#3D5A78` | secondary data series, quieter emphasis |
| Error | `#EC8B82` | `#A63429` | destructive, failure |
| Success | `#6FC08A` | `#2C6E45` | completed, in stock |
| Warning | `#E4AF66` | `#7C530C` | pending, low stock |
| Info | `#7FB2E0` | `#2C5E86` | neutral notices |
| Focus | `#7FD3C6` | `#0F6B62` | keyboard focus ring only |

Rules:

- One accent per screen. Primary marks the single most likely action; everything
  else is neutral. If a screen has three green buttons, two of them are wrong.
- Status color is information, never decoration. A card border is never tinted
  to "add life".
- Text on any fill must reach 4.5:1, including text on its own 20% status tint.
  `OnPrimary` exists because a light Primary in dark mode cannot carry white
  text — use it instead of `#FFFFFF`.
- Never introduce a new hex in a view. If a role is missing, add a token to both
  variants of `Theme.axaml` in the same commit.
- No gradients as background decoration. No glow. No colored drop shadows.
- Shadows are structural (dialogs, menus, the app bar), soft and near-invisible
  on light surfaces; prefer a 1px border over a shadow on cards.

### 1.2 Typography

One family for the whole product (system sans: Inter / Segoe UI). Personality
comes from scale, weight and spacing, not from a second typeface.

| Role | Size | Weight | Notes |
| --- | --- | --- | --- |
| Screen title | 28 | Bold | one per screen, sentence case |
| Section title | 20 | SemiBold | groups content, no rule under it |
| Tile / card title | 16 | SemiBold | wraps, max 2 lines |
| Body | 14 | Regular | line-height 1.45 |
| Caption / meta | 12 | Regular | `OnSurfaceMuted`, never the only place a number lives |
| Numeric KPI | 28–32 | Bold | tabular figures, unit in a smaller caption |

- Sentence case everywhere. **No ALL-CAPS labels**, no tracked-out eyebrows, no
  letterspacing tricks to signal a "section".
- Vietnamese diacritics need room: line-height ≥ 1.4, no tight leading on
  multi-line labels, avoid `TextTrimming` on titles that carry meaning.
- Numbers: right-aligned in tables, tabular, `{0:N0} ₫` for money, thousands
  separator by culture, never a raw float.
- Do not accent a single word inside a sentence with color or italics to create
  hierarchy. Change the whole line or the size.

### 1.3 Spacing, radius, shape

- 4-point scale: 4 / 8 / 12 / 16 / 24 / 32 / 40. No 5, 7, 15, 18.
- Radii: 8 controls (button, input, small chip), 12 icon plates and inner
  surfaces, 16 tiles and cards, 20 dialogs. Never mix 4 and 16 on one screen.
- Page padding 32 (24 when width < 900). Gap between cards 16, between sections 28.
- Borders are 1px `Border`; a card has exactly one of (border, shadow), not both.

### 1.4 Layout

- Structure is carried by whitespace and alignment, not by furniture. **No
  section dividers, group headers or labels like "VẬN HÀNH / QUẢN LÝ / HỆ
  THỐNG"** on the home screen — the wall of modules is one continuous thing.
- Home is a single grid of 1:1 square tiles: `BentoGridPanel` with
  `SquareCells="True"`, 2 columns under 560, 3 under 880, 4 under 1280, 5 under
  1720, 6 above. Every tile is the same size; hierarchy comes from order, not
  from spans.
- Content is left-aligned. Centering is for empty states and dialog bodies only.
- One dominant element per screen. Everything else steps down in weight.
- Text lines stay under ~80 characters; use `MaxWidth` on prose, not on tables.
- Wide tables scroll horizontally inside their card; the page itself never
  scrolls sideways.

---

## 2. Components

### 2.0 Shared library — compose, never re-draw

Visual decisions live in one place. A screen composes these; it does not paint
its own buttons, cards, headers or empty states.

| Need | Use | Notes |
| --- | --- | --- |
| Button | `Styles/Buttons.axaml` via `Classes` | variants `primary` / `secondary` / `ghost` / `danger` / `link`; sizes `sm` / `lg` / `icon` |
| Card / panel | `controls:Card` | surface, hairline, radius 12, soft shadow |
| Screen header | `controls:PageTitle` | `Title`, `Subtitle`, right-hand `ActionContent` |
| Empty list | `controls:EmptyState` | `Icon`, `Title`, `Hint`, optional action |
| KPI | `controls:KpiCard` | label, value, accent, optional trend |
| Status chip | `controls:StatusBadge` | one word, tinted by its brush |
| Dialog | `controls:ModalOverlay` | `IsOpen`, `Title`, body content |
| Inputs / lists | `Styles/AppControls.axaml` | TextBox, ComboBox, ListBox, tab lists |

Rules:

- Choose a button by variant, never by colour: `<Button Classes="primary"/>`.
  Need a different look? Add a variant to `Buttons.axaml`; never override in a view.
- Inline `Background`, `BorderBrush`, `CornerRadius` or `Padding` on a `Button`
  is a defect. A view's buttons should grep to zero `Background=`.
- Two screens needing the same block (stat row, filter bar, detail panel) means
  extract a control under `Controls/` with styled properties — not a second copy.
- Sizes come from classes too: `sm` for toolbars and table rows, `lg` for the one
  commit action on a screen, `icon` for a single glyph.

**Buttons.** One primary per view. Order: primary, then secondary (outline),
then text. Label is a verb in sentence case ("Thanh toán", "Lưu thay đổi"), not
a noun and not "OK". Minimum height 40, padding 16×10, radius 8.

**Inputs.** Height 40, 1px `BorderStrong`, radius 8, label above (12,
`OnSurfaceMuted`), placeholder is an example not a label. Focus shows the Focus
brush on the border plus a 1px outer ring — never a color swap of the whole field.

**Cards / tiles.** Surface fill, 1px Border, radius 16, padding 20. On hover:
border becomes Primary and fill steps to SurfaceMuted. On press: no shrink. On
keyboard focus: 2px Focus border. Cursor is a hand only when the whole surface is
clickable.

**Badges.** 12px text, radius 6, tinted fill at ≤ 20% alpha of its status color
with the solid status color as text. A badge never appears without a word.

**Tables.** Header 12 `OnSurfaceMuted` sentence case, row height 44 minimum, 1px
`Border` row separators only (no vertical lines), hover row uses SurfaceMuted,
selected row uses Primary at 12% alpha with an unchanged text color.

**Dialogs.** Radius 20, padding 24, title (20 SemiBold) + one line of context +
actions bottom-right. Destructive action is the Secondary/Error variant and says
exactly what is deleted.

**Empty states.** One sentence saying what is missing, one action to fix it.
Never a decorative illustration that pushes the action below the fold.

---

## 3. States and behavior

Every interactive element must define: default, hover, pressed, focus-visible,
disabled. Disabled keeps 3:1 against its background and never relies on opacity
alone to look inert.

- Loading: skeleton or a single spinner in the content area; never a full-screen
  block for a partial refresh.
- Error: say what failed and what to do next, in the interface's voice, no
  apology, no stack trace. Keep the user's input.
- Commands that depend on state (pay, complete payment) must re-evaluate their
  enabled state when that state changes — a button stuck disabled is a bug.
- Selection controls (payment method, service type, table status) always show a
  bound selected state, not just a click animation.

---

## 4. Motion

- 120–180 ms, ease-out, opacity and small offsets (≤ 8px) only.
- Motion answers an action (open, confirm, expand) or explains a change.
- No entrance animation on every card, no parallax, no looping decoration.
- A single orchestrated moment per screen at most — usually none in this product.
- Respect reduced motion: with it on, transitions collapse to instant.

---

## 5. Copy

- Vietnamese, sentence case, plain verbs, no filler ("Vui lòng", "Xin chào" only
  where a human would say it).
- Name things by what the user controls, not by the table or service behind it.
- One action keeps one name through the flow: the button "Thanh toán" produces
  "Đã thanh toán".
- Money `{0:N0} ₫`. Quantity gets a unit. Time is `HH:mm`, date is `dd/MM/yyyy`.
- Never show a label with no value, or a value with no unit.
- Empty and error copy is direction, not mood: what happened, what to do.

---

## 6. Icons

- Phosphor Icons v2, fill weight, 256 viewBox, keyed `Icon*` in
  `Styles/Icons.axaml`.
- One size per context: 22 in tile plates, 18 in buttons, 16 inline in text.
- Icons inherit `Foreground`; never bake a color into a geometry.
- An icon never replaces a label that carries meaning for a first-time user.

---

## 7. Data honesty

- Never present sample, cached or zeroed data as if it were live. A failed query
  shows an error state, not `0 ₫`.
- Show the real source's timestamp ("Cập nhật 14:32") when a number can be stale.
- Round only at display, never before summing.
- Do not invent comparison percentages ("+12% so với hôm qua") unless the data
  actually supports them.

---

## 8. Negative list (these read as templated or generated)

- ALL-CAPS eyebrows, tracked labels, `A · B · C` meta strings, `WORD — fragment`.
- Numbered markers (01 / 02 / 03) on content that is not a sequence.
- A section divider line between groups of cards.
- Identical radius and identical shadow on every element regardless of hierarchy.
- Three or more accent-colored buttons on one screen.
- Gradient washes, glassmorphism, neon accent on near-black as a "premium" look.
- `→` appended to every link and button.
- Emoji as iconography.

---

## 9. Checklist before calling a screen done

1. Every tile in the home wall is 1:1 and the same size as its neighbours.
2. No group label or divider splits the layout.
3. One primary action; primary color appears on nothing else that competes.
4. All colors come from `Theme.axaml` tokens; `dotnet build` reports no
   `StaticResource` on a brush and no unknown key.
5. Dark and Light both checked for 4.5:1 text contrast and visible focus.
6. Tab order reaches every control; focus ring is visible on each.
7. Disabled/enabled logic re-evaluates with the data it depends on.
8. Numbers are formatted `{0:N0} ₫`, dates `dd/MM/yyyy`, no raw floats.
9. Empty, loading and error states exist for every list and KPI.
10. Removing any single decorative element would not make the screen worse — if
    it would not, remove it.
