# Five plans: commonize the app, make the data flow seamless

Scope: the whole desktop app (`POS.Avalonia`, `POS.Core`, `POS.Data`), 27 views,
33 view models, 21 repository contracts, 18 implementations, 22 modules.

These are plans, not tasks: each one is independently shippable, has a first
slice that fits in a day, and ends with checks that can fail. The five are
ordered so each unblocks the next, but Plan 3 can run beside Plan 2.

## Where the app stands today

Measured from source, not estimated:

| Symptom | Count | Where |
| --- | --- | --- |
| Module lists that must agree | **3 separate lists of 22** | `ShellViewModel.BuildMenu`, `BentoHomeViewModel.BuildTiles`, `Program.cs` registrations |
| Screens that load from their constructor | **20** | `_ = LoadAsync();` in 20 view models |
| Screens with their own `IsLoading` flag | **17** | no shared busy/error state |
| Load cancellations in flight | **0** | no `CancellationTokenSource` anywhere |
| Payment writes in one transaction | **0** | `OrderSelectionViewModel.CompletePaymentAsync`, 6+ separate `await`s |
| Service-locator calls | 1 | `AppNavigator` → `Program.Host` |
| Dead shell (view + view model) | 1 | `ShellView.axaml` / `ShellViewModel.cs`, never instantiated |
| Views still declaring `Button.Styles` | 2 | Menu Management, Order History |
| Buttons outside the style system | 28 | see report_03 §3 |
| Inline `StringFormat` in views | 56 | money/date formatting repeated per screen |

What is already good, and should stay:

- **Zero** `async void`, **zero** `Task.Run`, **zero** `.Result`/`.Wait()`.
- `CancellationToken` appears **201** times; repository contracts already take a
  token and return `IReadOnlyList<T>` / `T?`.
- `CommunityToolkit.Mvvm` 8.2.1 (`[ObservableProperty]`, `[RelayCommand]`) is used
  consistently; no hand-rolled INPC.
- Data and Core have no reference to the UI layer.
- SQL is confined to `POS.Data` (91 statements), not leaked into view models.

So the problem is not sloppy async. It is that **the same concept is expressed
several times**: a module, a screen load, a page of data, a sale, and a "this
changed" signal each have 2–22 unofficial definitions.

## The five plans at a glance

| # | Plan | Commonizes | Makes data flow seamless by |
| --- | --- | --- | --- |
| 1 | **Module catalog** | the definition of a module (route, title, icon, VM, DI) | giving navigation and DI one key to agree on |
| 2 | **Screen lifecycle** | how a screen loads, fails, retries and cancels | one load path, so every screen refreshes the same way |
| 3 | **Queries and transactions** | how data is read and written | correct paging in SQL, one atomic sale, no half-writes |
| 4 | **Change bus and session state** | how screens learn that data changed | paying a bill updates every screen that cares, once |
| 5 | **Frame and vocabulary** | shell, navigation, error containment, leftover UI pieces | one place to navigate, one place to fail, one way to format |

Dependencies:

```
Plan 1 ──> Plan 2 ──> Plan 4
   │           │         ^
   └──> Plan 3 ┘─────────┘
   └──> Plan 5 (after Plan 1; independent of 3/4)
```

---

# Plan 1 — One module catalog, three surfaces derived from it

**Outcome:** a module is defined once. The home wall, the side menu, DI and
routing cannot disagree, because they read the same records.

### Evidence

Three lists enumerate the same 22 modules by hand, and they have already drifted:

- `POS.Avalonia/ViewModels/ShellViewModel.cs` — `BuildMenu()` adds 22 `NavItem`s
  with localized labels and icons such as `IconPackage`, `IconTag`.
- `POS.Avalonia/ViewModels/BentoHomeViewModel.cs` — `BuildTiles()` adds 22 tiles
  with hand-written Vietnamese titles and different icons (`IconInventory`,
  `IconDiscount`), plus subtitles that exist nowhere else.
- `POS.Avalonia/Program.cs` — 22 individual `AddTransient<XViewModel>()` calls.
- `MainViewModel.ShowModule(Type viewModelType, string title)` takes the title as
  a string, so every caller invents it.
- `ShellView.axaml` + `ShellViewModel` (127 lines) are never instantiated —
  a fourth, dead definition of the menu that still compiles.

A new screen therefore needs four edits (view, VM, catalog-ish list, DI), and
forgetting one is silent: the tile opens an empty host instead of failing.

### Target design

```csharp
// POS.Avalonia/Modules/ModuleDefinition.cs
public sealed record ModuleDefinition(
    ModuleKey Key,
    Type ViewModelType,
    string TitleKey,          // "Nav_Inventory"
    string TitleFallback,     // "Kho hàng"
    string SubtitleKey,       // tile caption; null for menu-only
    string IconKey,           // resource key in Styles/Icons.axaml
    int Order,
    ModuleGroup Group,        // Sell, Operate, Manage, System
    bool DeveloperOnly = false);

// POS.Avalonia/Modules/ModuleCatalog.cs
public static class ModuleCatalog
{
    public static IReadOnlyList<ModuleDefinition> All { get; } = new[] { ... };
    public static ModuleDefinition Get(ModuleKey key);
    public static IEnumerable<ModuleDefinition> Visible(bool isDeveloper);
}
```

`ModuleKey` is an enum, not a `Type`, so navigation, telemetry, permissions and
deep links all speak one vocabulary. `Type` stays a property of the definition.

Surfaces become projections:

- **Home:** `BentoHomeViewModel` iterates `ModuleCatalog.All` ordered by `Order`.
- **Menu:** `ShellViewModel` (if kept) or a thin `NavigationMenuViewModel` maps
  `Group` to section headers — one place, not two.
- **DI:** `foreach (var m in ModuleCatalog.All) services.AddTransient(m.ViewModelType);`
- **Routing:** `ShowModule(ModuleKey)` resolves title/icon from the catalog.
- **Developer modules** (`Debug`, `StyleGuide`) get `DeveloperOnly = true` and are
  filtered unless the build is Debug — they currently ship in the home wall.

### Work items

1. Add `ModuleKey`, `ModuleGroup`, `ModuleDefinition`, `ModuleCatalog`.
2. Encode all 22 modules once; keep the current home-wall order as `Order`.
3. Delete `ShellView.axaml` + `ShellViewModel.cs` (unreferenced), or reduce the
   menu to a projection of the catalog — decide once, not both.
4. Rewrite `BentoHomeViewModel.BuildTiles()` into a projection; keep the
   `BentoTile` display record as a view model concern only.
5. Replace the 22 `AddTransient` lines with the loop; assert count == 22 at startup.
6. Change `ShowModule(Type, string)` → `ShowModule(ModuleKey)` and update the
   navigator, `ModuleHostViewModel`, and `BentoHomeViewModel`.
7. Remove the per-caller titles from `AppNavigator.OpenModule`; titles come from
   the catalog, translated through `IStringLocalizer`.
8. Add resource keys for the six titles that only exist as literals in tiles.
9. Test: reflection over `ModuleCatalog` asserts, for every entry, (a) VM derives
   from `ViewModelBase`, (b) `{ViewModelType.Name without "ViewModel"}View` exists,
   (c) the icon key resolves, (d) `TitleKey` resolves in both cultures, (e) DI
   resolves the type.
10. Test: home wall count == menu count == DI resolution count.

### Acceptance

- Adding a module means one record + one view; the three-surface test fails if any
  surface disagrees.
- No hand-written module list remains (`grep -c AddTransient<.*ViewModel> Program.cs`
  is 0; module types are registered in one loop).
- Opening every `ModuleKey` through the catalog in a harness yields 22/22 non-null
  view models and 22/22 constructible views.
- `ShellView`/`ShellViewModel` are gone or are the only menu definition.

### Risks

- Localized titles rely on resource keys; missing keys currently fall back to
  `viewModelType.Name`, which is why drift is invisible. The test in item 9 fixes
  that permanently.
- Removing `ShellView` may surface stale XAML references — build catches them.

**Effort:** 3–4 dev-days. **Depends on:** nothing. **Unblocks:** 2, 4, 5.

**First slice (½ day):** catalog with the 22 records + DI loop + home projection,
leaving `ShowModule(Type,string)` in place; ship that alone.

---

# Plan 2 — One screen lifecycle: load once, cancel on leave, fail visibly

**Outcome:** a screen cannot forget to load, cannot load twice at once, cannot
show stale data as if it were fresh, and cannot crash the process from a failed
query.

### Evidence

- `ViewModelBase` (`POS.Avalonia/ViewModels/ViewModelBase.cs`) has only
  `FormError`, `StatusMessage` and `RunCrudAsync` — no load concept.
- **20** view models start work from their constructor: `_ = LoadAsync();`. The
  task is unobserved, so an exception is swallowed or crashes on finalization,
  and nothing can await the first paint.
- **17** view models declare their own `IsLoading`; there is no `IsBusy`, no
  `ErrorMessage`, no `HasLoaded`, and **0** `CancellationTokenSource`. Navigate
  away mid-query and the work continues, then writes into a screen nobody sees.
- `ViewModelCache` (`Services/ViewModelCache.cs`) resolves each view model once
  per app session and never invalidates. Revisit a screen and you see the data
  from first open, with no indication it is old. This is the largest single cause
  of "the numbers are wrong" in daily use.
- Replace-after-save is ad hoc: some commands reload, some mutate the list, some
  do neither.

### Target design

```csharp
public abstract partial class ScreenViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _hasLoaded;
    [ObservableProperty] private string? _loadError;
    [ObservableProperty] private DateTimeOffset? _loadedAt;

    public bool IsStale => LoadedAt is { } at && DateTimeOffset.Now - at > StaleAfter;
    protected virtual TimeSpan StaleAfter => TimeSpan.FromMinutes(2);

    public async Task EnsureLoadedAsync(bool force = false, CancellationToken ct = default)
    {
        if (IsBusy) return;                       // no double load
        if (HasLoaded && !force && !IsStale) return;
        // cancel the previous run, then run LoadCoreAsync under one policy
    }

    protected abstract Task LoadCoreAsync(CancellationToken ct);

    [RelayCommand] private Task RefreshAsync() => EnsureLoadedAsync(force: true);
}
```

Policy lives in exactly one method:

- `IsBusy` true for the duration; UI binds to it (`ProgressBar`, disabled rows).
- Exceptions become `LoadError` with the existing `MapCrudError` text; the screen
  shows `EmptyState` + "Thử lại" bound to `RefreshCommand`. No swallowed tasks.
- The token is cancelled in `OnNavigatedFrom` (Plan 5) and on a second load.
- `Invalidate()` marks `HasLoaded = false` for the cases where a write makes the
  screen's data obsolete (this is the hook Plan 4 will pull).
- Navigation calls `EnsureLoadedAsync()`; constructors no longer touch data.

`ViewModelCache` stops being the freshness answer: it still keeps POS state
(cart, filters) alive, but the base class decides when to reload.

### Work items

1. Add `ScreenViewModel`; keep `ViewModelBase` for non-screen VMs (dialogs, models).
2. Implement `EnsureLoadedAsync` with one `SemaphoreSlim`/flag, one try/catch, one
   `CancellationTokenSource`.
3. Add `LoadError` + retry UI to `EmptyState` usage on the four screens that
   already have empty states, then to the rest.
4. Convert the 20 constructor loads, one screen per commit, highest-use first:
   Bán hàng, Tổng quan, Sơ đồ bàn, Kho, Lịch sử đơn, then the rest.
5. Delete the 17 local `IsLoading` fields; bind to `IsBusy`.
6. Add `RefreshCommand` to every screen's header (the `PageTitle` slot exists).
7. Replace ad-hoc post-save reloads with `Invalidate()` + refresh (interim until
   Plan 4's bus).
8. Unsubscribing: screens with timers/subscriptions override `OnNavigatedFrom`.
9. Test harness: a fake repository that blocks on a `TaskCompletionSource` proves
   (a) a second `EnsureLoadedAsync` does not start a second query, (b) navigating
   away cancels, (c) `force` reloads, (d) a thrown exception lands in `LoadError`
   and leaves `IsBusy == false`.
10. Test: after startup, every screen's constructor performs no I/O (assert with a
    counting repository).

### Acceptance

- `grep -rn "_ = Load.*Async()" POS.Avalonia/ViewModels` → 0.
- Only `ScreenViewModel` declares `IsBusy`; view models declare none.
- Every screen renders three states from the same properties: busy, error+retry,
  content/empty.
- Cancelling navigation mid-load produces no `ObjectDisposedException` and no
  write into a detached screen (harness test).
- Revisiting a screen after `StaleAfter` reloads automatically; revisiting within
  it is instant.

### Risks

- Highest regression risk in checkout: a cancelled load during payment would be
  bad. Rule: payment and other writes never run inside `LoadCoreAsync`; they are
  commands with their own state and are not cancelled by navigation.
- `ConfigureAwait(true)` is used today to stay on the UI thread; keep it inside
  the base class so view models stop thinking about it.

**Effort:** 5–6 dev-days. **Depends on:** Plan 1 (routing key). **Unblocks:** 4.

**First slice (1 day):** base class + convert Bán hàng and Tổng quan; both are the
screens people open all day.

---

# Plan 3 — Queries that page in SQL, and one atomic sale

**Outcome:** reading a list asks the database for one page; writing a sale either
happens completely or not at all.

### Evidence

- `InventoryViewModel` loads **all** menu items, then filters, sorts and pages in
  memory (`ApplyFilter`, `ApplyPaging`, `FilteredItems`, `PagedItems`). Fine at 80
  items, wrong at 8,000 — and the same shape will be copied to the next list.
- `OrderSelectionViewModel.CompletePaymentAsync` (lines ~503–585) writes: order →
  each detail → each option (3–4 per detail) → stock → shift, as separate awaits.
  No transaction. Kill the app mid-payment and the database keeps a half order
  that still appears in reports.
- Those writes also pass `default` instead of the caller's `CancellationToken`
  (`AddAsync(order, default)`), so a cancelled checkout still writes.
- Repositories return whole entities; screens then do joins and totals in memory
  (`AnalyticsRepository` returns a `SalesSummaryDto` living in `POS.Core.Models`,
  mixing read model and domain model in one folder).
- 21 contracts, ~57 methods, each hand-written; no shared paging, filter or count
  convention, so each new screen invents one.

### Target design

Read side — explicit queries with one paging contract:

```csharp
public sealed record PageRequest(int Page = 1, int PageSize = 25, string? Search = null);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize)
{
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(Total / (double)PageSize));
    public bool HasPrev => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public sealed record InventoryQuery(string? Search, int? CategoryId, bool LowStockOnly, PageRequest Page);
Task<PagedResult<MenuItem>> QueryAsync(InventoryQuery query, CancellationToken ct);
```

`LIMIT @take OFFSET @skip` plus one `COUNT(*)` with the same `WHERE`; search and
category filters move into SQL and are indexed.

Write side — a use case per transaction:

```csharp
public interface IUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<IWriteSession, CancellationToken, Task<T>> work, CancellationToken ct);
}
var orderId = await _uow.ExecuteAsync(async (tx, ct) =>
{
    var id = await tx.Orders.AddAsync(order, ct);
    foreach (var line in bill.Lines) { ... tx.Details.AddAsync(...); tx.Options.AddAsync(...); }
    await tx.Stock.ApplySaleAsync(id, ct);
    await tx.Shifts.AddSaleAsync(shiftId, total, ct);
    return id;
}, ct);   // one connection, one transaction, rollback on any throw
```

`SaleCommand`/`SaleResult` become the single sale shape, so the POS screen, the
AI assistant and any future kiosk call the same path.

### Work items

1. Add `PageRequest`, `PagedResult<T>`, and a small `Sql` helper for paging +
   count (one place building the two statements).
2. Add `IUnitOfWork`/`IWriteSession` over the existing `IDbConnectionFactory`;
   begin/commit/rollback, `CancellationToken` threaded.
3. Extract `CompleteSaleAsync(SaleCommand, ct)` out of `OrderSelectionViewModel`
   into `POS.Avalonia/Services/CheckoutService.cs` (or a Core use-case), keeping
   the screen a caller.
4. Convert the payment path: order + details + options + stock + shift + audit in
   one transaction; remove `default` tokens; propagate failures as one message.
5. Add `InventoryQuery` and move paging/filtering into SQL; delete
   `FilteredItems`/`PagedItems`/`ApplyPaging` from the view model.
6. Repeat for the next three lists with the same shape (Order History, Audit Log,
   Problem Reports), then stop — no big-bang rewrite.
7. Standardize read models in `POS.Core/ReadModels/` (`SalesSummary`,
   `TopItemRow`, `ShiftTotals`), separate from `Models/`.
8. Add indexes for the columns the queries filter on (`orders.created_at`,
   `order_details.order_id`, `menu_items.category_id`, `menu_items.stock`).
9. Test: fault injection inside `ExecuteAsync` after the order insert leaves zero
   rows in `orders` and zero in `order_details`.
10. Test: 10,000 seeded inventory rows return one page in bounded time; the app
    never materializes the full table (assert repository called once with LIMIT).
11. Test: `CompleteSaleAsync` under a cancelled token writes nothing.

### Acceptance

- No view model sorts/pages a full table: `grep -rn "ApplyPaging\|FilteredItems"` → 0.
- A sale is one transaction: kill the process mid-way in a harness, database has
  either the whole sale or nothing.
- Cancellation aborts a write and rolls back.
- All list queries take `PageRequest` and return `PagedResult<T>`.

### Risks

- The sale path is the app's most valuable code and currently untested. Build the
  transaction test **before** refactoring the method, using the current behaviour
  as the fixture.
- SQLite + `Microsoft.Data.Sqlite` needs `PRAGMA journal_mode=WAL` and a busy
  timeout for concurrent reads during a write transaction; verify both.
- Don't convert all 18 repositories at once; only the lists that page.

**Effort:** 6–8 dev-days. **Depends on:** Plan 1 lightly. **Parallel with:** 2.

**First slice (1–2 days):** `IUnitOfWork` + `CompleteSaleAsync` moved behind it
with a rollback test; no query changes yet. This alone removes money errors.

---

# Plan 4 — Say what changed, once, so every screen can be fresh

**Outcome:** a sale, a stock adjustment or a closed shift updates every open
screen without reopening the app, and "current shift / current table / open bill"
have one owner.

### Evidence

- After `CompletePaymentAsync`, nothing notifies anyone: Dashboard, Table Map,
  Inventory, Daily Sales and Shift Register all hold whatever they loaded first.
- `ViewModelCache` keeps one instance per screen for the whole session, so those
  values never refresh by themselves.
- Cross-screen intent is passed through a mutable helper: `PendingTableService`
  plus `AppNavigator.OpenOrderForTable` and a `NavigateToOrderRequested` event in
  the dead `ShellViewModel`. Opening a table's bill means "set a field somewhere,
  then navigate, then read it".
- `CurrentUserService` exists, but the current shift, the selected table and the
  open bill are each re-derived or re-read per screen.
- `IDatabaseSyncService` only checkpoints WAL for git sync — there is no writer
  and reader coordination beyond SQLite itself.

### Target design

One typed bus, provided by the MVVM toolkit already on hand:

```csharp
public static class ChangeBus
{
    private static readonly WeakReferenceMessenger Bus = new();
    public static void Publish<T>(T message) where T : class => Bus.Send(message);
    public static void Subscribe<T>(object recipient, Action<T> handler) where T : class
        => Bus.Register<T>(recipient, (_, m) => handler(m));
}

public sealed record OrderPaidMessage(int OrderId, int? TableId, decimal Total);
public sealed record StockChangedMessage(IReadOnlyCollection<int> MenuItemIds, string Reason);
public sealed record ShiftChangedMessage(int ShiftId, string Status);
public sealed record MenuChangedMessage(MenuChangeKind Kind, int EntityId);
```

- Published **after** the unit of work commits (Plan 3), never before, so a rolled
  back sale cannot announce itself.
- Screens subscribe once and respond by `Invalidate()` + refresh if visible, or
  just `Invalidate()` to reload on next visit. This is what makes "seamless"
  cheap: no polling, no per-screen refresh plumbing.
- `WeakReferenceMessenger` means a cached screen cannot leak a subscription.

Session state becomes explicit:

```csharp
public sealed partial class SessionContext : ObservableObject   // singleton
{
    public int? CurrentShiftId { get; }
    public Table? SelectedTable { get; }
    public BillViewModel? OpenBill { get; }      // one cart, not per screen
    public bool IsShiftOpen => CurrentShiftId is not null;
    public void BeginOrder(Table table);         // replaces PendingTableService
}
```

Screens read it; they do not copy it. `AppNavigator.OpenOrderForTable` becomes
`navigate(ModuleKey.Order, new OrderRoute(tableId))`, and the POS screen opens
that table's bill from session state.

### Work items

1. Add `ChangeBus` + message records (no new package).
2. Publish `OrderPaidMessage` from `CompleteSaleAsync` after commit.
3. Publish `StockChangedMessage` from the stock-adjustment path; `MenuChanged` from
   menu/category edits; `ShiftChanged` from open/close shift.
4. Subscribe in Dashboard, Table Map, Inventory, Daily Sales, Shift Register,
   Order History: `Invalidate()` and, when visible, debounced refresh.
5. Add `SessionContext`; move shift id, selected table and open bill into it.
6. Delete `PendingTableService` and its event; route the table through the nav
   parameter + session context.
7. Add a shell status strip: open shift, unsynced writes, database path — the
   operator's single "is the system healthy" line.
8. Debounce refreshes (250–400 ms) so a multi-line sale publishes once per screen,
   not once per line.
9. Test: pay a bill → assert exactly one `OrderPaidMessage`; Dashboard receives it
   once; a second subscription does not double-fire; a rolled-back sale publishes
   nothing.
10. Test: `SessionContext` is the only holder of `CurrentShiftId` (grep for
   `CurrentShiftId` outside it must be zero in view models).

### Acceptance

- Pay a bill, then visit Dashboard, Table Map, Inventory: all reflect it without
  restarting, with no manual refresh.
- One commit produces one message per concern.
- No view model stores its own shift/table/bill copy.
- `PendingTableService` is deleted.

### Risks

- Over-refreshing: debounce and only refresh visible screens. Screens that are not
  visible just mark stale (Plan 2) and load on entry.
- Message ordering vs transactions: publish only after commit; never inside.
- Weak subscriptions are easy to get wrong with lambdas capturing `this`; use
  `bus.Register<T>(this, ...)` form and a test that a closed screen is collected.

**Effort:** 5–7 dev-days. **Depends on:** Plans 2 and 3.

**First slice (1 day):** `OrderPaidMessage` + Dashboard + Table Map only. That is
the demo people notice ("I paid, the tile updated").

---

# Plan 5 — One frame: navigation with parameters, per-module containment, shared vocabulary

**Outcome:** navigation is a service with a real entry point and parameters, one
screen crashing cannot take the app down, and the last repeated UI pieces are
shared rather than re-drawn.

### Evidence

- `MainViewModel.ShowModule(Type, string)` + `AppNavigator` (which reaches
  `Program.Host` for `PendingTableService`) is the whole navigation story: no
  parameters, no back stack beyond "GoHome", no way to deep-link a table or an
  order.
- `ModuleHostView` is a header with a back button and a `ContentControl`.
- A failed load inside a module today either shows nothing or throws on the UI
  thread — the crash that killed Orders (report_03) is exactly this class of bug.
- **28** buttons still style themselves; 2 views declare `Button.Styles`; 56
  inline `StringFormat`s repeat money and date formatting; `StyleGuideView` is
  stale (report_03 §3, "Still open" 2–3).
- `ShellView`/`ShellViewModel` are dead but compile into the app.

### Target design

Navigation as a service with typed routes:

```csharp
public interface INavigationService
{
    Task NavigateAsync(ModuleKey key, object? parameter = null, CancellationToken ct = default);
    Task GoBackAsync(CancellationToken ct = default);
    bool CanGoBack { get; }
}

public interface INavigationAware
{
    Task OnNavigatedToAsync(object? parameter, CancellationToken ct);
    Task OnNavigatedFromAsync();
}
```

`ModuleHostViewModel` becomes a host only: resolve from catalog, call
`OnNavigatedToAsync(parameter)`, observe `LoadError`, offer retry. It does not
know about orders or tables.

Containment:

- `ModuleHostViewModel` catches `LoadError` from the hosted screen and renders
  `EmptyState` + "Thử lại" + "Về trang chủ" — a broken module is a bad page, not a
  dead process.
- App-level: log `TaskScheduler.UnobservedTaskException` and
  `AppDomain.CurrentDomain.UnhandledException` with the payload, then show one
  recovery dialog. (Report_03's two P0s were both "UI thread exception = exit".)

Vocabulary (finishing commonization):

- `row` button variant; delete both `Button.Styles` blocks; retire the 12
  converter-driven chips into one `FilterChip` control.
- `MoneyText` control/converter (`{0:N0} ₫`) and `DateText`; remove the 56 inline
  formats so currency and dates cannot diverge per screen.
- `FormField` wrapper (label + input + error) for the 6 hand-built forms.
- Regenerate `StyleGuideView` from tokens + variants, or delete it.

### Work items

1. Add `INavigationService` + `NavigationService` (stack, parameter passing, no
   `Program.Host`); delete the service-locator call.
2. `ModuleHostViewModel` hosts only; title/icon from catalog; `INavigationAware`
   hooks replace the `OrderSelectionViewModel` special case
   (`ApplyPendingTableIfAny`).
3. Add `LoadError` → retry/home UI in the host; keep the header's back button.
4. Register `AppDomain`/`TaskScheduler` handlers with structured logging.
5. Delete `ShellView.axaml`/`ShellViewModel.cs` if Plan 1 kept the catalog only.
6. Add `row` variant + `FilterChip`; migrate the two `Button.Styles` blocks and the
   12 chips; then the other 16 unclassed buttons.
7. Add `MoneyText`/`DateText`; migrate views in batches, asserting the same output.
8. Add `FormField`; migrate the six forms with the most duplicated label/error XAML.
9. Test: navigate to all 22 modules through `INavigationService`, then back through
   22 — no exceptions, `CanGoBack` correct, each screen got its parameter.
10. Test: a module whose load throws renders the error panel and the shell stays
    interactive (harness).
11. Test: `MoneyText` output equals `{0:N0} ₫` for 0, 1.000, 1.234.567.

### Acceptance

- `grep -rn "Program.Host" POS.Avalonia` → 0.
- Navigating to every module and back leaves zero unhandled exceptions, and one
  failing module does not exit the process.
- Zero `Button.Styles` in views; zero unconverted chips; zero inline money formats.
- `StyleGuideView` either reflects the live theme or is gone.

### Risks

- Navigation rewrite touches every screen; do it behind `INavigationService` while
  keeping `ShowModule` as a shim, delete the shim at the end.
- Money formatting changes must not change printed receipts: assert receipt output
  before/after on a fixed bill.
- UI churn is where regressions hide; each of items 6–8 is layout-only and should
  be reviewed with the style guide open.

**Effort:** 4–5 dev-days. **Depends on:** Plan 1; the UI items can run in parallel.

**First slice (1 day):** `row` variant + delete the two local `Button.Styles`
blocks. Pure win, no navigation risk.

---

# Sequencing

| Phase | Days | Ship | Exit check |
| --- | --- | --- | --- |
| 0 | 0.5 | Test harness (fake repos, view construction) + baseline counts | the metrics below are reproducible |
| 1 | 3–4 | Plan 1 | 22/22 modules from one catalog; drift test green |
| 2a | 5–6 | Plan 2 | zero constructor loads; every screen has busy/error |
| 2b | 6–8 | Plan 3 (parallel with 2a) | sale is atomic; lists page in SQL |
| 3 | 5–7 | Plan 4 | paid bill updates Dashboard/Table Map/Inventory |
| 4 | 4–5 | Plan 5 | one nav service, one error boundary, vocabulary done |

Total **24–30 dev-days**. Plan 3 is the one that protects money, so it may jump
ahead: it needs only the catalog's `ModuleKey` type, not the full Plan 1.

# Gate metrics (run in CI or before each release)

| Gate | Today | Target |
| --- | --- | --- |
| Module lists | 3 (22 each) | 1 catalog |
| `_ = Load…Async()` in view models | 20 | 0 |
| View models declaring `IsLoading` | 17 | 0 |
| Payment writes per transaction | 6+ | 1 |
| List screens paging in memory | 1 (Inventory) | 0 |
| `Program.Host` uses | 1 | 0 |
| Dead shell pairs | 1 | 0 |
| `Button.Styles` in views | 2 | 0 |
| Buttons outside the style system | 28 | 0 |
| Inline money `StringFormat` in views | 56 | 0 |
| `async void` / `.Result` / `Task.Run` | 0 | 0 (keep) |
| Repository methods without `CancellationToken` | 0 | 0 (keep) |

# What these plans deliberately do not do

- No new UI framework, no MVVM library swap, no ReactiveUI.
- No database engine change, no ORM. SQL stays in `POS.Data`; only its shape
  (queries, transactions) changes.
- No rewrite of `OrderSelectionViewModel` beyond moving the sale out of it. It
  stays large until a separate, measured refactor.
- No offline/replication layer. `IDatabaseSyncService` keeps its current job.
- No new module until Plans 1 and 2 are done — adding a screen should be cheap
  afterwards, and expensive in the current three-list state.

# Definition of "commonized"

One concept, one definition, enforced by a check:

| Concept | One definition | Enforced by |
| --- | --- | --- |
| A module | `ModuleCatalog` | drift test + DI loop |
| A screen load | `ScreenViewModel.EnsureLoadedAsync` | grep gate |
| A page of data | `PageRequest`/`PagedResult<T>` | signature review |
| A sale | `CompleteSaleAsync` in one transaction | rollback test |
| A data change | `ChangeBus` message after commit | publish-count test |
| A route | `ModuleKey` + `INavigationService` | harness navigation test |
| A button | `Buttons.axaml` | style-free view gate |
| Money / date text | `MoneyText` / `DateText` | grep gate |
