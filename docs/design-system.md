# Design system reference — HienXanh POS

Companion to `.rules/design.md` (the rules) and `.report/09-11-26-report_03.md`
(the change history). This file is the practical reference: what exists, what to
write in a view, and where a value lives.

## Where things live

| File | Owns |
| --- | --- |
| `POS.Avalonia/Styles/Theme.axaml` | colour tokens and semantic brushes, per `Dark` / `Light` variant |
| `POS.Avalonia/Styles/Buttons.axaml` | the only shared button definition: variants, sizes, states |
| `POS.Avalonia/Styles/AppControls.axaml` | TextBox, ComboBox, ListBox, tab lists |
| `POS.Avalonia/Styles/Icons.axaml` | `Icon*` geometries (Phosphor v2, fill) |
| `POS.Avalonia/Controls/` | shared components (Card, PageTitle, EmptyState, …) |
| `POS.Avalonia/App.axaml` | spacing / radius / type size tokens, style includes, converters |

Rule of thumb: a view may choose *which* token or variant to use, never *what*
it looks like. If nothing fits, add a variant here — the system is where
exceptions are allowed to live.

## Colour

Screens reference the semantic **brush** (`PrimaryBrush`), never the raw
`Color` token (`PrimaryColor`) and never a hex literal.

| Role | Dark | Light | Brush |
| --- | --- | --- | --- |
| Brand accent | `#9BAF7D` | `#52653D` | `PrimaryBrush` |
| Ink on accent | `#12130F` | `#FBFBF7` | `OnPrimaryBrush` |
| Canvas | `#141413` | `#FAF9F5` | `BackgroundBrush` |
| Card / panel | `#1C1B19` | `#FFFFFF` | `SurfaceBrush` |
| Inset / icon plate | `#262523` | `#F0EEE7` | `SurfaceMutedBrush` |
| Hairline | `#353330` | `#E3E0D5` | `BorderBrush` |
| Input / divider | `#4A4741` | `#C9C5B6` | `BorderStrongBrush` |
| Primary text | `#FAF9F5` | `#141413` | `OnSurfaceBrush` |
| Secondary text | `#B0AEA5` | `#6B675E` | `OnSurfaceMutedBrush` |
| Attention | `#D97757` | `#A8552F` | `SecondaryBrush` |
| Destructive | `#E07A6E` | `#B0392C` | `ErrorBrush` |
| Success | `#8FBF7B` | `#4C7042` | `SuccessBrush` |
| Pending | `#E0B267` | `#96631C` | `WarningBrush` |
| Notice | `#82B1DC` | `#3D6C93` | `InfoBrush` |
| Focus ring | `#CFE0B4` | `#52653D` | `FocusBrush` |

Adding a role means adding it to **both** variants in `Theme.axaml`, plus a
`Color` token and a `SolidColorBrush` alias, in the same change.

## Typography and shape tokens

| Token | Value | Use |
| --- | --- | --- |
| `CaptionFontSize` | 12 | meta, table headers, hints |
| `BodyFontSize` | 14 | default text and buttons |
| `SubtitleFontSize` | 16 | card titles |
| `TitleFontSize` | 20 | section titles, dialog titles |
| `LargeTitleFontSize` | 24 | screen titles (`PageTitle`) |
| `Spacing4/8/12/16/24/32` | 4–32 | the only allowed gaps |
| `CornerRadius4/8/12/16` | 4–16 | controls 8, plates 12, cards 12–16 |

Shadows are `BoxShadows` tokens in `Theme.axaml` (`ShadowCard`, `ShadowDialog`,
…). They are structural only — dialogs and the app bar.

## Buttons

Choose a **variant** by meaning and a **size** only when the default does not
fit. Never set `Background`, `BorderBrush`, `CornerRadius` or `Padding` on a
button in a view, and never declare `Button.Styles` inside a view — a row that
needs its own hover fill wants a `row` variant here instead (two views still
do it the old way, see report_03).

| Class | Use for |
| --- | --- |
| `primary` | the single most likely action on the screen |
| `secondary` | normal actions next to the primary (default-looking) |
| `ghost` | toolbars, icon-only actions, low-emphasis rows |
| `danger` | irreversible action (void, delete, refund) |
| `link` | inline navigation inside text |
| `muted` | modifier: quiet text on any variant |
| `sm` | height 32 — toolbars, table rows, dense panels |
| `lg` | height 48 — the commit action (CHARGE) |
| `icon` | square 40×40 (32×32 with `sm`) for a single glyph |
| `choice` | selectable tile (payment method, service type) |
| `selected` | state class on a `choice`, bound to the current value |

```xml
<!-- the one commit action -->
<Button Classes="primary lg" Content="Thanh toán" Command="{Binding PayCommand}"/>

<!-- secondary action beside it -->
<Button Classes="secondary" Content="In tạm tính"/>

<!-- icon-only, quiet -->
<Button Classes="ghost icon" ToolTip.Tip="Đóng" Command="{Binding CloseCommand}">
  <PathIcon Data="{StaticResource IconClose}" Width="18" Height="18"/>
</Button>

<!-- destructive, small -->
<Button Classes="danger sm" Content="Hủy đơn" Command="{Binding VoidCommand}"/>
```

### Selectable group

`choice` is a tile; the `selected` class carries the state, driven from the
bound value with `StringEqualsConverter`:

```xml
<Button Classes="choice"
        Command="{Binding SetPaymentMethodCommand}" CommandParameter="Cash"
        Classes.selected="{Binding SelectedPaymentMethod,
                           Converter={StaticResource StringEqualsConverter},
                           ConverterParameter=Cash}">
  <StackPanel Spacing="6" HorizontalAlignment="Center">
    <PathIcon Data="{StaticResource IconCash}" Width="20" Height="20"/>
    <TextBlock Text="Cash" FontSize="12" FontWeight="SemiBold"/>
  </StackPanel>
</Button>
```

Behaviour contract, already implemented in `Buttons.axaml`:

- hover lifts the fill and (for `secondary`) strengthens the border;
- pressed deepens it, no scaling;
- disabled is a neutral fill with muted ink — legible, never opacity-washed;
- keyboard focus draws a 2 px `FocusBrush` ring on the presenter;
- radius 8 for text buttons, 12 for `choice` tiles.

## Shared components

### `Card`

Content wrapper: `SurfaceBrush`, 1 px border, radius 12, padding 24, soft shadow.

```xml
<controls:Card>
  <StackPanel Spacing="12">…</StackPanel>
</controls:Card>
```

### `PageTitle`

| Property | Type | Notes |
| --- | --- | --- |
| `Title` | `string` | screen title, 24 bold |
| `Subtitle` | `string` | one line of context, optional |
| `ActionContent` | `object` | `[Content]` — right-aligned actions |

```xml
<controls:PageTitle Title="Lịch sử đơn" Subtitle="Tra cứu và in lại hoá đơn">
  <Button Classes="secondary sm" Content="Xuất CSV"/>
</controls:PageTitle>
```

### `EmptyState`

| Property | Type | Notes |
| --- | --- | --- |
| `Icon` | `Geometry` | optional, 32 px, muted |
| `Title` | `string` | what is missing |
| `Hint` | `string` | the way out, optional |
| `ActionContent` | `object` | `[Content]` — one action |

```xml
<controls:EmptyState Icon="{StaticResource IconHistory}"
                     Title="No orders found"
                     Hint="Thử đổi khoảng ngày hoặc từ khoá tìm kiếm."
                     IsVisible="{Binding !DisplayedOrders.Count}"/>
```

### `KpiCard`

`Title`, `Value`, `Subtitle`, `AccentBrush`, `Icon`, `TrendText`.

### `StatusBadge`

`Text`, `BadgeBrush`. One word, tinted at ~15% of its brush.

### `ModalOverlay`

| Property | Type | Notes |
| --- | --- | --- |
| `IsOpen` | `bool` | visibility |
| `Title` | `string` | dialog title |
| `DialogContent` | `object` | `[Content]` — body |

## Layout helper

`controls:BentoGridPanel` — flow grid with fixed columns and square cells:

```xml
<controls:BentoGridPanel Columns="4" Gap="16" SquareCells="True"/>
```

`SquareCells` makes the row height follow the measured column width, so a tile
can never become a rectangle.

## Adding something new

1. **Need a new look for a button?** Add a variant or size to `Buttons.axaml`.
   Do not style it in the view.
2. **Need a new block used twice?** Create `Controls/YourThing.axaml` +
   `.axaml.cs` with `StyledProperty<T>` for its inputs (`Title`, `Icon`, content
   slot), then use it from both screens.
3. **Need a colour that does not exist?** Add the role to both variants of
   `Theme.axaml`; do not hardcode.
4. **Then** run: build, and confirm the new view still constructs (the UI
   harness pattern in the report).

## Review checklist for a UI change

- No inline `Background` / `CornerRadius` / `Foreground` on buttons.
- No hex literal outside `Theme.axaml` (the style guide's swatch *labels* are
  text, not colours — and that page still needs rebuilding).
- One `primary` per screen; destructive actions use `danger`.
- Selection visible wherever the app remembers a choice.
- Empty, loading and error states use the shared components.
- Disabled controls are readable, not faded to nothing.
- Money `{0:N0} ₫`, dates `dd/MM/yyyy`.
