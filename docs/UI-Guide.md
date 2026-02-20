# MEOCAFE POS – UI Guide

Design system and component usage for the MEOCAFE Point of Sale app. Inspired by modern coffee-shop POS UIs (e.g. [Beans & Bills](https://dribbble.com/shots/25024080-Beans-Bills-Point-of-Sales), [Kopinan](https://ar.pinterest.com/pin/853995148122449838/), [POS System for Coffee Shop](https://dribbble.com/shots/24504375-UI-POS-System-for-Coffee-Shop)).

---

## 1. Design principles

- **Warm and professional** – Cafe-friendly palette (greens, browns, cream) and clear hierarchy.
- **Touch- and mouse-friendly** – Adequate hit targets (min 36px height), spacing, and contrast.
- **Consistent components** – Use shared controls and styles so every screen feels part of the same app.
- **Scannable layout** – Clear sections: categories → products → current bill → actions.

---

## 2. Global theme (colors only from here)

**All colors and shadows come from `Styles/Theme.axaml`.** Views must use `{StaticResource KeyName}` only; no hardcoded hex in XAML.

- **Merge order:** App.axaml merges `Theme.axaml` first, then `Components.axaml`. Spacing, radius, and typography remain in App.axaml.
- **To change the look:** Edit only `Theme.axaml` (and typography in App.axaml if needed).

### Color palette (Theme.axaml) – Black 30% | White 60% | Brown 20% | Matcha 10%

| Ratio | Tokens | Usage |
|-------|--------|--------|
| **White 60%** | BackgroundBrush, SurfaceBrush, SurfaceMutedBrush, BorderBrush | Main backgrounds, cards, panels |
| **Black 30%** | BlackBrush, BlackSurfaceBrush, SidebarBrush, OnSurfaceBrush, OnSurfaceMutedBrush | App bar, sidebar, body text |
| **Brown 20%** | SecondaryBrush, SecondaryLightBrush | Prices, secondary actions, warmth |
| **Matcha 10%** | PrimaryBrush, PrimaryLightBrush, SuccessBrush, NavSelectedBrush, NavSelectedFgBrush | Primary CTAs, success, nav selected |

Other: **ErrorBrush** / **ErrorBgBrush**, **OnPrimaryBrush** (white on dark), **AppBar*** brushes, **OverlayDimBrush**, **SidebarBorderBrush**.

### Shadows (Theme.axaml)

| Token | Use |
|-------|-----|
| **ShadowAppBar** | Header bar |
| **ShadowContent** | Main content card |
| **ShadowCard** | KPI / section cards |
| **ShadowCardLight** | Light card shadow |
| **ShadowDialog** | Modal dialog |
| **ShadowLogin** | Login card |

---

## 3. Typography

| Style | Size | Weight | Use |
|-------|------|--------|-----|
| **Caption** | 12px | Normal | Hints, metadata, secondary info |
| **Body** | 14px | Normal | Default text, list items |
| **Subtitle** | 16px | SemiBold | Section titles, emphasis |
| **Title** | 20px | Bold | Page/section headers |
| **LargeTitle** | 24px | Bold | App/screen title |

Use static resources: `{StaticResource CaptionFontSize}`, `{StaticResource TitleFontWeight}`, etc.

---

## 4. Spacing and radius

- **Spacing:** 4, 8, 16, 24, 32 (use `Spacing4`–`Spacing32` and `Margin4`–`Margin32`).
- **Radius:** 4 (chips, tags), 8 (buttons, inputs, list items), 12 (cards, panels), 16 (large cards).

---

## 5. Common components

All screens should use these shared building blocks. Add this namespace to use controls:

```xml
xmlns:controls="using:POS.Avalonia.Controls"
```

Static resources (templates, tokens) are in `Application.Resources` and merged `Styles/Components.axaml`.

### 5.1 PageTitle

- **Control:** `POS.Avalonia.Controls.PageTitle`
- **Use:** Top of every page.
- **Props:** `Title` (string), optional `Action` (e.g. Refresh button) via content or attached property.
- **Example:**
  ```xml
  <controls:PageTitle Title="Order / POS">
    <Button Content="Refresh" Command="{Binding LoadCommand}"/>
  </controls:PageTitle>
  ```

### 5.2 Card

- **Control:** `POS.Avalonia.Controls.Card` (ContentControl)
- **Use:** Any grouped content (KPI blocks, lists, forms). Renders as a rounded surface with border and shadow.
- **Example:**
  ```xml
  <controls:Card>
    <StackPanel> ... </StackPanel>
  </controls:Card>
  ```

### 5.3 ModalOverlay

- **Control:** `POS.Avalonia.Controls.ModalOverlay`
- **Use:** Dialogs (payment, customization, void, customer search).
- **Props:** `IsOpen` (bool), `Title` (string). Content is the body of the dialog.
- **Example:**
  ```xml
  <controls:ModalOverlay IsOpen="{Binding IsPaymentOpen}" Title="Payment">
    <StackPanel> ... </StackPanel>
  </controls:ModalOverlay>
  ```

### 5.4 Buttons

- **Primary:** `Classes="primary"` – main actions (Login, Pay, Save).
- **Default:** Standard `Button` – secondary actions (Cancel, Hold, Recall).
- **Destructive:** Use `Foreground="{StaticResource ErrorBrush}"` for Void, Delete.

### 5.5 ProductCard template

- **Resource:** `ProductCardTemplate` (in `Styles/Components.axaml`), `DataType="models:MenuItem"`.
- **Use:** Any `ListBox` or `ItemsControl` that displays menu items (category product list, quick keys).
- **Example:**
  ```xml
  <ListBox ItemsSource="{Binding FilteredMenuItems}"
          ItemTemplate="{StaticResource ProductCardTemplate}"
          .../>
  ```
- **CategoryItemTemplate** is also provided for category list items.

### 5.6 Bill summary block

- **Pattern:** Card with rows: Subtotal, Discount, Tax, then Total (emphasized). Use `BillSummaryCard` or the shared layout in `OrderSelectionView`.

### 5.7 App bar and navigation

- **App bar:** Logo/title left, page title center, user + Logout right (see `ShellView`).
- **Nav:** Vertical list of nav items; selected state uses `NavSelectedBrush`.

---

## 6. Layout patterns

- **POS main:** Categories (narrow column) | Product grid/list | Bill panel (fixed width ~400px).
- **Dashboard / reports:** Header row (title + action) then grid of cards.
- **Management pages:** PageTitle, then toolbar (filters, New), then list/grid inside a Card.

---

## 7. Accessibility and UX

- Minimum touch target height: 36px for buttons and list items.
- Sufficient contrast for text (OnSurface on Surface, OnPrimary on Primary).
- Loading: use `ProgressBar IsIndeterminate="True"` and hide main content while loading.
- Errors: show inline with `ErrorBrush` and clear copy.

---

## 8. Reference links

- [Beans & Bills – Point of Sales](https://dribbble.com/shots/25024080-Beans-Bills-Point-of-Sales)
- [Kopinan – POS for Coffee Shop](https://ar.pinterest.com/pin/853995148122449838/)
- [POS System App](https://dribbble.com/shots/18977642-POS-System-App)
- [UI POS System for Coffee Shop](https://dribbble.com/shots/24504375-UI-POS-System-for-Coffee-Shop)

Use these for inspiration only; implement with the shared palette and components above for consistency.
