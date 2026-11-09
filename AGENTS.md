# HienXanh POS

Desktop point-of-sale for a café: orders and tables, payment, inventory, menu,
shifts, reporting, and an AI chat. Avalonia 11.3 UI on .NET 9, SQLite storage
with DbUp migrations. Three projects:

- `POS.Avalonia` — views, view models, styles, shared controls, services
- `POS.Core` — models and repository contracts (must stay UI-free)
- `POS.Data` — SQLite repositories and schema migrations

UI copy is Vietnamese, sentence case. Money `{0:N0} ₫`, dates `dd/MM/yyyy`.

## Build & run

```bash
dotnet build HienXanh.sln
dotnet run --project POS.Avalonia
```

There is no test project and no test/lint command — the solution contains only
the three app projects.

## Dev environment

- Requires the .NET 9 SDK; the app is a WinExe desktop target.
- Config: `appsettings.json` (+ optional `appsettings.Local.json`) loaded in
  `POS.Avalonia/Program.cs`; `appsettings.Local.json` is gitignored, absence is
  fine.
- DB: `data/hienxanh.db` (gitignored with `*.db-wal` / `*.db-shm`), found by
  walking up from the output directory. DbUp migrations run on startup; a
  failed init shows the error in the window instead of the UI
  (`Program.DatabaseStartupError`).
- `**/bin/` and `**/obj/` are gitignored.

## Conventions

- MVVM via CommunityToolkit.Mvvm: view models extend
  `ViewModelBase : ObservableObject` using `[ObservableProperty]` fields and
  `[RelayCommand]` methods (source-generated, not hand-written setters).
- CRUD mutations go through `RunCrudAsync` and surface failures in `FormError`;
  `MapCrudError` turns SqliteException codes (19, 787, 1811) into Vietnamese
  messages. New view models should follow this rather than raw try/catch.
- Repositories implement `POS.Core.Contracts.*` in `POS.Data/Repositories/` and
  are registered as singletons in `Program.cs` — no newing up repos in VMs.
- Navigation runs through `Services.IAppNavigator` / `AppNavigator`; view models
  resolve via `IViewModelResolver` (`ViewModelCache`).
- UI: every colour token lives in `POS.Avalonia/Styles/Theme.axaml` (Dark and
  Light variants); views use semantic brushes (`PrimaryBrush`, `SurfaceBrush`)
  and never a hex literal. Buttons come from `Styles/Buttons.axaml` via Classes
  (variants `primary`/`secondary`/`ghost`/`danger`/`link`, sizes
  `sm`/`lg`/`icon`/`choice`) — never set `Background`/`BorderBrush`/
  `CornerRadius` on a Button in a view. Two screens sharing a block means a new
  control under `Controls/`. See `.rules/design.md` and `docs/design-system.md`.
- Migrations: new files in `POS.Data/Migrations` named strictly
  `V<number>__description.sql` (parsed by `DatabaseMigrationRunner`).

## Docs

- `.rules/design.md` — hard rules every screen must satisfy before done.
- `docs/design-system.md` — tokens, button variants, shared components.
- `.report/` — dated reviews/change reports, `DD-MM-YY-report_NN.md`.

## Pitfalls

- Migrations are read from the output dir (`bin/.../Migrations`), so after
  adding `V<next>__...sql` you must rebuild or the runner throws "No database
  migrations found".
- Never edit an already-applied migration to change schema; deployed DBs
  record the old script. Add a new numbered script instead.
- Startup uses the state of `data/hienxanh.db` — a stale local DB with an
  incompatible schema fails migration. Delete the file to start fresh.
- POS.Core must not reference Avalonia types — keep `DynamicResource`,
  `AvaloniaObject`, etc. out of `POS.Core` and `POS.Data`.
