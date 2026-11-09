# HienXanh POS

Desktop point-of-sale for a café: orders and tables, payment, inventory, menu,
shifts, reporting and an AI assistant. Avalonia UI on .NET 9, SQLite storage with
DbUp migrations.

## Build and run

```bash
dotnet build HienXanh.sln
dotnet run --project POS.Avalonia
```

The app resolves its database from `data/hienxanh.db` (walking up from the output
directory) and applies any pending migrations on startup. If initialisation
fails, the window shows the error instead of the UI.

## Layout

| Project | Responsibility |
| --- | --- |
| `POS.Avalonia` | views, view models, styles, shared controls, services |
| `POS.Core` | models and repository contracts |
| `POS.Data` | SQLite repositories and schema migrations |

Inside `POS.Avalonia`:

- `Views/` — screens; `Controls/` — reusable components (`Card`, `PageTitle`,
  `EmptyState`, `KpiCard`, `StatusBadge`, `ModalOverlay`, …)
- `Styles/` — `Theme.axaml` (colour tokens per variant), `Buttons.axaml` (the
  single button definition), `AppControls.axaml` (inputs and lists), `Icons.axaml`
- `Services/` — navigation, receipts/printing, kitchen display, theme, settings

## Documentation

- [`docs/design-system.md`](docs/design-system.md) — tokens, button variants and
  sizes, shared components, how to extend the system
- [`docs/README.md`](docs/README.md) — documentation index
- [`.rules/design.md`](.rules/design.md) — the rules every screen must satisfy
- [`.report/`](.report/) — dated reviews and change reports

## Conventions

- Colour and shape come from `Styles/Theme.axaml`; a view never hardcodes a hex.
- A button is chosen by variant (`primary`, `secondary`, `ghost`, `danger`,
  `link`) and size (`sm`, `lg`, `icon`) — never painted inline.
- UI copy is Vietnamese, sentence case, with `{0:N0} ₫` for money and
  `dd/MM/yyyy` for dates.
