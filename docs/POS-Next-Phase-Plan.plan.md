# MEOCAFE POS – Next Phase: Industry-Grade UI and Complete Feature Set

This document plans the **next phase** after the current Avalonia foundation: **professional UI** and **all features a POS must, should, and could have** (cafe/restaurant/retail). It does not replace the existing [modernize_meocafe_pos_77f5f227.plan.md](.cursor/plans/modernize_meocafe_pos_77f5f227.plan.md); it extends it.

**How to run this plan (Cursor agent)**  
Execute Part 6 (Granular task list) in order. Follow Part 5 (Cursor agent execution rules). Do not stop after a single phase unless the user asks; complete at least one full phase and prefer continuing to the next. Mark each task done as you complete it (e.g. change `- [ ]` to `- [x]` in a copy of the list or in the plan file if desired).

---

## Part 1: Complete POS Feature Checklist (ALL)

Every item below is a feature the POS **must** (M), **should** (S), or **could** (C) have. The next phase implements or explicitly defers each.

---

### 1. Sales and checkout (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 1.1 | Quick product grid with images and prices | M | Categories + products; tap to add. |
| 1.2 | Product customization (ice, sugar, size, toppings, note) | M | Per line; stored for receipt and kitchen. |
| 1.3 | Multiple open bills (tabs) with labels (Table X, Takeaway, etc.) | M | Switch, new bill, close bill. |
| 1.4 | Edit/remove line; quantity change | M | In current bill. |
| 1.5 | Order-level and line-level notes | M | For kitchen and receipt. |
| 1.6 | Subtotal, discount, tax, grand total | M | Configurable tax rules. |
| 1.7 | Apply discount: percentage, fixed amount, coupon code | M | Per bill or per line. |
| 1.8 | Price override (manager approval or role-based) | S | Audit log. |
| 1.9 | Void line / void entire bill (with reason and approval if required) | M | Soft/hard void; audit. |
| 1.10 | Split bill by item or by amount | S | Multiple payment methods per bill. |
| 1.11 | Merge bills (e.g. combine tables) | C | |
| 1.12 | Hold/suspend bill and recall later | S | Draft orders; recover after close. |
| 1.13 | Quick keys / shortcuts for top products | S | Configurable buttons. |
| 1.14 | Barcode scan to add product (if retail/some items) | S | Scanner as keyboard wedge or API. |
| 1.15 | Customer-facing display (second screen) | C | Show order total and items. |

---

### 2. Payment (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 2.1 | Cash payment: tendered amount, change calculation | M | |
| 2.2 | Card payment (terminal integration or manual entry) | M | payOS or similar. |
| 2.3 | Multiple payment methods per transaction (split payment) | S | Cash + card, etc. |
| 2.4 | Tip / gratuity (add to total or per payment method) | S | Tip adjustment, tip reporting. |
| 2.5 | Refund: full or partial; to original payment method or cash | M | Linked to original order. |
| 2.6 | Gift card: check balance, apply, reload | S | |
| 2.7 | Store credit / house account | C | |
| 2.8 | Payment confirmation and receipt trigger | M | Print/email/SMS. |
| 2.9 | Offline mode: queue payments, sync when online | C | |
| 2.10 | Cash drawer open (on cash payment or “No Sale”) | S | Hardware integration. |

---

### 3. Receipts and printing (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 3.1 | Print receipt (thermal ESC/POS or standard printer) | M | Header, lines, totals, thank you. |
| 3.2 | Email receipt | S | Customer email. |
| 3.3 | SMS receipt (optional) | C | |
| 3.4 | Reprint last receipt / reprint by order ID | M | |
| 3.5 | Receipt template customization (logo, footer, tax ID) | S | |
| 3.6 | Kitchen/bar ticket print (order ticket to printer) | M | By station or category. |
| 3.7 | Label printing (e.g. cup labels with options) | C | |
| 3.8 | End-of-day report print | S | |

---

### 4. Kitchen display and order routing (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 4.1 | Kitchen Display System (KDS): orders sent from POS to screen | S | Separate KDS app or second window. |
| 4.2 | Order routing by category (drinks → bar, food → kitchen) | S | |
| 4.3 | Order status: new, preparing, ready, served | S | Update from KDS or POS. |
| 4.4 | Fire course (send appetizer first, main later) | C | |
| 4.5 | Bump bar / mark item done | S | |
| 4.6 | Display of modifiers (ice, sugar, toppings, note) on KDS | M | |

---

### 5. Orders and service types (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 5.1 | Dine-in (with table assignment) | M | |
| 5.2 | Takeaway / to-go | M | |
| 5.3 | Delivery (address, driver, fee) | S | |
| 5.4 | Pickup (scheduled or ASAP) | S | |
| 5.5 | Table management: floor plan, table status (empty/occupied/reserved) | S | |
| 5.6 | Table merge / transfer items between tables | C | |
| 5.7 | Reservation list (time, guest count, table) | C | |
| 5.8 | Order types with different tax or pricing rules | S | |

---

### 6. Menu and products (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 6.1 | Categories and products (name, price, image, category) | M | |
| 6.2 | Product options/modifiers (ice, sugar, size, toppings) with optional price | M | |
| 6.3 | Combos / bundles (fixed price, component items) | M | |
| 6.4 | Product variants (e.g. size S/M/L as separate or one product with size option) | M | |
| 6.5 | Out-of-stock / hide or disable product when no inventory | S | |
| 6.6 | Recipe / ingredient linkage for inventory deduction | S | |
| 6.7 | Menu scheduling (time-based or day-part: breakfast/lunch/dinner) | C | |
| 6.8 | Happy hour or time-based pricing | C | |
| 6.9 | Barcode per product (for scan) | S | |
| 6.10 | Product import/export (Excel/CSV) | S | Already in prototype. |

---

### 7. Discounts and promotions (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 7.1 | Discount definitions: percentage or fixed; date range; active/disabled | M | |
| 7.2 | Apply discount to bill or to specific lines | M | |
| 7.3 | Coupon code (code → discount) | S | |
| 7.4 | Automatic discount by rule (e.g. 2nd drink half off) | C | |
| 7.5 | Manager approval for discount above threshold | S | |
| 7.6 | Discount reporting (by discount type) | S | |

---

### 8. Customers and loyalty (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 8.1 | Customer lookup by phone (or name/card) | M | |
| 8.2 | Customer profile: name, phone, email, address | M | |
| 8.3 | Points / loyalty: earn on purchase, redeem for discount or free item | S | |
| 8.4 | Customer purchase history | S | |
| 8.5 | Link order to customer (for history and loyalty) | M | |
| 8.6 | Customer notes / tags | C | |
| 8.7 | Birthday or visit-based offers | C | |
| 8.8 | SMS/email marketing (opt-in, segments) | C | |

---

### 9. Inventory (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 9.1 | Stock quantity per product (or per ingredient) | S | |
| 9.2 | Deduct on sale (or manual adjust) | S | |
| 9.3 | Low-stock alert and reorder point | S | |
| 9.4 | Receive stock (purchase order or manual receive) | S | |
| 9.5 | Inventory count / adjustment with reason | S | |
| 9.6 | Ingredient-level inventory and recipe (BOM) | C | |
| 9.7 | Multi-location stock (if multi-store) | C | |
| 9.8 | Waste / spill tracking | C | |

---

### 10. Staff and roles (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 10.1 | Login with username/password (or PIN) | M | |
| 10.2 | Roles: Manager, Cashier/Employee (and custom) | M | |
| 10.3 | Permissions: void, discount, price override, reports, settings | M | |
| 10.4 | Clock in / clock out (shift start and end) | M | |
| 10.5 | Shift assignment and registration (who works which shift) | M | |
| 10.6 | Sales per employee / per shift | S | |
| 10.7 | Commission or tip pool (optional) | C | |
| 10.8 | Employee list CRUD and basic info | M | |
| 10.9 | Audit log: who did what (void, override, refund) | S | |

---

### 11. Reporting and analytics (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 11.1 | Sales summary: by day, period, location | M | |
| 11.2 | Product mix (PMIX): quantity and revenue per product | M | |
| 11.3 | Category performance | S | |
| 11.4 | Top-selling items (and worst) | M | |
| 11.5 | Hourly/daily/weekly/monthly trends (charts) | M | |
| 11.6 | Comparison: period vs period (e.g. this week vs last) | S | |
| 11.7 | Employee performance (sales, transactions) | S | |
| 11.8 | Discount and promotion effectiveness | S | |
| 11.9 | End-of-day report (Z-report): sales, payments, taxes | M | |
| 11.10 | Tax report | S | |
| 11.11 | Export reports (PDF, Excel) | S | |
| 11.12 | Dashboard with KPIs (revenue, avg ticket, top items) | M | |
| 11.13 | Inventory valuation and movement report | C | |
| 11.14 | Customer analytics (frequency, spend) | C | |

---

### 12. Tax (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 12.1 | Tax rate configuration (e.g. 10% VAT) | M | |
| 12.2 | Tax included vs excluded pricing | M | |
| 12.3 | Tax by product category or product (e.g. food vs non-food) | S | |
| 12.4 | Tax-exempt customer or order | S | |
| 12.5 | Multiple tax rates (e.g. different regions) | C | |

---

### 13. Multi-location and multi-tenant (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 13.1 | Single store (current) | M | |
| 13.2 | Store/location selector at login or in app | S | |
| 13.3 | Consolidated reporting across locations | C | |
| 13.4 | Transfer stock or orders between locations | C | |

---

### 14. Hardware integration (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 14.1 | Receipt printer (ESC/POS over USB/network) | M | |
| 14.2 | Cash drawer (open on cash payment or No Sale) | S | |
| 14.3 | Barcode scanner (keyboard wedge) | S | |
| 14.4 | Customer display (second monitor) | C | |
| 14.5 | Card reader (integrated payment) | S | payOS or similar. |
| 14.6 | Scale (for weight-based items) | C | |

---

### 15. Settings and configuration (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 15.1 | Business info (name, address, tax ID, logo) | M | Receipt and reports. |
| 15.2 | Receipt header/footer text | S | |
| 15.3 | Tax rates and rules | M | |
| 15.4 | Payment method list (Cash, Card, etc.) | M | |
| 15.5 | Printer configuration (default receipt, kitchen printers) | M | |
| 15.6 | Database/connection settings (already in appsettings) | M | |
| 15.7 | Currency and locale (decimal, date format) | M | |
| 15.8 | Backup and restore (DB or config) | S | |
| 15.9 | License or activation (if commercial) | C | |

---

### 16. Order history and operations (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 16.1 | Order history list (date range, filters) | M | |
| 16.2 | Order detail view (lines, payments, customer) | M | |
| 16.3 | Reprint receipt from history | M | |
| 16.4 | Refund from history (open closed order for refund) | M | |
| 16.5 | Void from history (with reason) | S | |
| 16.6 | Search order by ID, customer, date | S | |

---

### 17. Localization and accessibility (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 17.1 | Multi-language UI (e.g. EN, VI) | S | Resource files. |
| 17.2 | RTL support (if needed) | C | |
| 17.3 | Accessibility: keyboard navigation, screen reader names | S | |
| 17.4 | Font size / density option | S | |
| 17.5 | High-contrast theme | C | |

---

### 18. Security and compliance (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 18.1 | Passwords hashed (not plain text) | M | |
| 18.2 | Session timeout / lock after inactivity | S | |
| 18.3 | Audit trail for sensitive actions | S | |
| 18.4 | PCI considerations (no card data stored if using external terminal) | M | |
| 18.5 | Role-based access (already in plan) | M | |

---

### 19. Integrations (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 19.1 | Payment gateway (payOS, etc.) | M | |
| 19.2 | Excel/CSV import export (menu, products, etc.) | S | |
| 19.3 | Accounting export (e.g. daily sales summary) | C | |
| 19.4 | Online ordering or delivery platform webhook | C | |
| 19.5 | QR code for payment or menu | S | QRCoder already. |

---

### 20. UX and performance (M/S/C)

| # | Feature | M/S/C | Notes |
|---|--------|-------|------|
| 20.1 | Fast load of menu and categories | M | |
| 20.2 | Responsive layout (different window sizes) | M | |
| 20.3 | Clear feedback (loading, success, error) | M | |
| 20.4 | Offline resilience (queue orders if DB down) | C | |
| 20.5 | Keyboard shortcuts for power users | S | |

---

## Part 2: Next Phase – Better UI (Industry-Standard)

**Goal:** Make the POS look and behave like a professional, industry-standard product (clean, consistent, fast, and suitable for all-day use).

### 2.1 Design system (apply everywhere)

- **FluentAvalonia or Material.Avalonia** as the single theme; **no** ad-hoc colors or fonts.
- **Design tokens:** spacing (4/8/16/24/32), corner radius (4/8), typography scale (caption, body, subtitle, title, large title). Define in `App.axaml` or `Styles/` and use via `StaticResource`/`ThemeResource`.
- **Colors:** Primary, secondary, background, surface, error, success; light and dark variants. All controls use these.
- **Icons:** Single icon set (Fluent or Material) and size scale (16/24/32).
- **Motion:** Short, consistent transitions (e.g. 150–200 ms) for panel open/close and content change; avoid flashy animations.

### 2.2 Shell and navigation

- **Shell:** Clear hierarchy: app bar (title, user, logout), **persistent nav** (sidebar or top tabs) with icons + labels, **content area**. No duplicate or buried nav.
- **Nav items:** Dashboard, Order (POS), Order history, [Manager: Menu, Employees, Discounts, Shifts, Reports, Settings]. Role-based visibility.
- **Breadcrumbs or context** where useful (e.g. “Order > Bill 2”).
- **Global actions:** Quick access to “New order” and “Current order” from shell if needed.

### 2.3 Order / POS screen (main work surface)

- **Layout:** Fixed, predictable grid: **left** = categories (vertical list or strip) + **product grid** (cards with image, name, price); **right** = **bill tabs** (horizontal) + **current bill** (lines, subtotal, discount, tax, total) + **actions** (Payment, Hold, Discount, etc.). No overlapping or hidden key areas.
- **Product cards:** Same size; image (or placeholder), name, price; optional badge (e.g. “Promo”). Tap = add (with customization if applicable). Clear selected state.
- **Bill panel:** One line per order line (product + modifiers summary, qty, price, line total); easy edit/remove; sticky subtotal/discount/tax/total; large “Pay” / “Payment” button.
- **Product customization:** Slide-out panel or dialog with **all** options (ice, sugar, size, toppings, note), quantity, and running total; primary “Add to order” button.
- **Multi-bill tabs:** Tabs or chips with label (e.g. “T1”, “Takeaway”, “Bill 3”); “+” for new bill; switch by click. Current tab clearly highlighted.
- **Loading and errors:** Skeleton or spinner for product load; inline or InfoBar for errors; no silent failures.

### 2.4 Forms and data entry (manager and settings)

- **Consistent form pattern:** Label, input (TextBox/NumberBox/DatePicker/ComboBox), optional helper text, validation message. Same spacing and alignment everywhere.
- **DataGrid/tables:** Sortable columns, optional search, row actions (edit/delete). Pagination or virtualizing for large lists.
- **Dialogs:** Modal for confirm (e.g. void, delete); clear primary/secondary actions; escape to cancel.
- **Empty states:** “No orders yet”, “No products in this category” with short guidance or action.

### 2.5 Dashboard and reports

- **Dashboard:** KPI cards (today’s sales, transactions, avg ticket); at least one chart (e.g. sales over time); optional top products. Use LiveChartsCore with theme colors.
- **Reports:** List of report types (sales summary, PMIX, end-of-day, etc.); date range picker; “Run” or “Export”; results in grid or chart. Same design tokens as rest of app.

### 2.6 Accessibility and polish

- **Focus order:** Tab through key controls in logical order.
- **Names:** All interactive elements have a name for screen readers.
- **Contrast:** Meet WCAG AA for text and controls.
- **Touch targets:** Minimum 44px for tap areas if touch is supported.
- **Performance:** Lazy load heavy views; avoid blocking UI on DB calls (async + progress).

---

## Part 3: Implementation Phases (Next Phase)

Break the next phase into **sub-phases** so progress is shippable and testable.

### Phase 6 – UI foundation and design system

- Introduce **design tokens** (spacing, radius, colors, type) in `App.axaml` and shared styles.
- Apply tokens to **Shell** (nav, app bar, content area) and **Login**.
- Ensure **FluentAvalonia** (or chosen theme) is applied consistently; remove any hard-coded colors/fonts.
- **Deliverable:** Shell and Login look consistent and on-brand; no new features yet.

### Phase 7 – Order/POS screen (industry layout + all checkout features)

- **Layout:** Implement the standard POS layout (categories + product grid | bill tabs + current bill + actions).
- **Product customization:** Full dialog/panel (ice, sugar, size, toppings, note); persist in order line; show on receipt and (later) KDS.
- **Bill operations:** Edit/remove line; apply discount (%, fixed, coupon); order note; subtotal, tax, grand total.
- **Payment flow:** Payment screen (cash, card, split); tendered amount and change (cash); record payment(s); complete order; print receipt; clear or hold bill.
- **Void:** Void line and void bill with reason (and role check); audit log.
- **Hold/recall:** Save current bill as draft; list of held bills; recall and continue.
- **Deliverable:** One complete path: add items with options → discount → pay (cash or card) → receipt → done. Void and hold working.

### Phase 8 – Receipts, printing, and kitchen

- **Receipt template:** Configurable header (logo, name, address), body (lines, options, totals), footer (thank you, tax ID). Print to ESC/POS (or system printer).
- **Reprint:** From order history or “last receipt”.
- **Kitchen ticket:** Send order to kitchen printer(s) by category/station; ticket shows modifiers and note.
- **KDS (optional):** Separate window or app that shows orders sent from POS; status (new/preparing/ready); bump. If deferred, at least kitchen print is in place.
- **Deliverable:** Receipt and kitchen print working; reprint working; KDS optional.

### Phase 9 – Customers, loyalty, and tax

- **Customer:** Lookup by phone; create/edit customer; link customer to order; store in DB.
- **Loyalty:** Points earn on sale; redeem points for discount or free item; balance on receipt or screen.
- **Tax:** Tax rate config; apply tax to order (included or excluded); tax line on receipt and in reports.
- **Deliverable:** Customer on order; points earn/redeem; tax on bill and in reports.

### Phase 10 – Inventory and product operations

- **Stock:** Quantity per product (or variant); deduct on sale; low-stock alert; receive and adjust with reason.
- **Out-of-stock:** Hide or disable product when quantity = 0; optional “allow oversell” setting.
- **Product import/export:** Excel/CSV for products and categories (already in prototype; wire to new UI).
- **Deliverable:** Inventory per product; deduct on sale; alerts; import/export.

### Phase 11 – Staff, permissions, and audit

- **Permissions:** By role (e.g. void, discount over X%, price override, reports, settings); enforce in UI and API.
- **Clock in/out:** Record start/end time; link to shift; show in shift report.
- **Audit log:** Log void, refund, price override, discount override; viewer (by date, user, action).
- **Deliverable:** Role-based permissions; clock in/out; audit log.

### Phase 12 – Reporting and dashboard (full set)

- **Reports:** Sales summary, PMIX, end-of-day (Z), tax report, employee sales, discount report; date range; export PDF/Excel.
- **Dashboard:** KPIs; sales trend chart; top products; optional comparison to previous period.
- **Deliverable:** All core reports and dashboard with charts.

### Phase 13 – Table management, delivery, and service types

- **Table management:** Floor plan (tables/zones); table status (empty, occupied, reserved); assign order to table; transfer items.
- **Service types:** Dine-in, takeaway, delivery, pickup; optional fee or tax by type.
- **Delivery:** Address, fee, driver (optional).
- **Deliverable:** Table map and assignment; service type on order; delivery fields.

### Phase 14 – Hardware, settings, and polish

- **Hardware:** Receipt printer (ESC/POS), cash drawer open, barcode scanner (keyboard wedge); config in settings.
- **Settings:** Business info, receipt template, tax, payment methods, printer list, DB config; save and apply.
- **Localization:** Resource files for EN/VI; use in all views.
- **Accessibility:** Focus order, names, contrast; high-contrast option.
- **Deliverable:** Printer and drawer working; settings UI complete; at least one language; a11y improved.

### Phase 15 – Refunds, split payment, gift card, and extras

- **Refund:** Full/partial; to original method or cash; from order history.
- **Split payment:** Multiple methods per order (e.g. cash + card).
- **Gift card:** Balance check, apply to order, reload (if supported by payment provider).
- **Tips:** Tip field; tip report by employee.
- **Deliverable:** Refund and split payment; optional gift card and tips.

---

## Part 4: Summary

- **Part 1** is the **master checklist** of every POS feature (must/should/could). Use it to track coverage and to decide what to build in each phase.
- **Part 2** defines **industry-standard UI**: design system, shell, POS layout, forms, reports, and accessibility.
- **Part 3** is the **next-phase implementation plan** in 10 sub-phases (6–15), from UI foundation through Order/POS, receipts/kitchen, customers/loyalty/tax, inventory, staff/audit, reporting, tables/delivery, hardware/settings, and refunds/split/tips.

Implement in order; at the end of the next phase, MEOCAFE POS will have a **professional UI** and **all core POS features** (and a clear list of “could have” for later).

---

## Part 5: Cursor agent execution rules (read this first when implementing)

- **Run duration:** When executing this plan, continue until the **entire current phase** (e.g. Phase 6, then Phase 7, …) is complete. Do **not** stop after implementing a single feature or a handful of tasks. Treat each phase as a **multi-hour batch** of work.
- **Task order:** Work through the **granular task list** (Part 6) in strict order. Mark tasks in progress and completed as you go. Do not skip tasks unless explicitly marked optional.
- **No early exit:** Do not summarize and stop after "Phase 6 started" or "design tokens added." Finish all tasks in the phase (e.g. all of Phase 6: tokens, Shell, Login, theme consistency, verification).
- **Checkpoints:** After each **phase** (not after each task), you may output a short phase summary and then **continue** to the next phase using the same task list.
- **Scope:** One execution session = at least one **full phase** (Phases 6–15). If the user says "run the plan," assume they want multiple phases executed in sequence until context or time limits apply.

---

## Part 6: Granular task list (per phase)

Work through these tasks in strict order. Mark each as done (e.g. `- [x]`) when complete. Phase N is complete when all N.x tasks are done.

### Phase 6 – UI foundation (design tokens, Shell, Login, theme)

- [x] 6.1 Create `Styles/Spacing.xaml` with design token keys: Spacing4, Spacing8, Spacing16, Spacing24, Spacing32.
- [x] 6.2 Create `Styles/Radius.xaml` with CornerRadius4, CornerRadius8 (and optionally CornerRadius12, CornerRadius16).
- [x] 6.3 Create `Styles/Colors.xaml` with Primary, Secondary, Background, Surface, Error, Success (light theme).
- [x] 6.4 Add dark variant resources for the same color keys.
- [x] 6.5 Create `Styles/Typography.xaml` with Caption, Body, Subtitle, Title, LargeTitle font sizes/weights.
- [x] 6.6 Merge all token dictionaries (Spacing, Radius, Colors, Typography) in `App.axaml`.
- [x] 6.7 Replace hard-coded margins in `ShellView.axaml` with token references.
- [x] 6.8 Replace hard-coded colors in `LoginView.axaml` with token references.
- [x] 6.9 Add app bar to Shell: title, current user name, logout button.
- [x] 6.10 Style nav items (icons + labels) using typography and spacing tokens.
- [x] 6.11 Apply radius tokens to all Card/Border components in Shell.
- [x] 6.12 Verify Login screen uses only theme/token colors; remove any inline colors.
- [x] 6.13 Add FluentAvalonia (or Material.Avalonia) and ensure no control uses ad-hoc styles.
- [x] 6.14 Document in plan: Phase 6 complete when all 6.x tasks are done.

### Phase 7 – Order/POS (layout, products, bill, customization, payment, void, hold)

- [x] 7.1 Create Order/POS view layout: main grid with regions for categories, products, and bill panel.
- [x] 7.2 Define column/row definitions and responsive breakpoints for Order view.
- [x] 7.3 Add category list control (vertical list or horizontal strip) bound to categories data.
- [x] 7.4 Add category selection command and selected state styling.
- [x] 7.5 Add product grid control bound to products for selected category.
- [x] 7.6 Bind product items to image, name, price; use design tokens for spacing and typography.
- [x] 7.7 Add command on product tap to add item to current bill (default quantity 1).
- [x] 7.8 Add product images with placeholder fallback and aspect ratio.
- [x] 7.9 Add bill panel container with header (e.g. "Current bill" or tab label).
- [x] 7.10 Add bill lines list: product name, quantity, unit price, line total, customization summary.
- [x] 7.11 Add subtotal row in bill panel.
- [x] 7.12 Add discount row (amount or percentage) and bind to order discount.
- [x] 7.13 Add tax row and bind to configured tax rules.
- [x] 7.14 Add grand total row and bind to order total.
- [x] 7.15 Add edit/remove line buttons per line and quantity up/down.
- [x] 7.16 Add "Customize" or "Options" button per line opening customization dialog.
- [x] 7.17 Create customization dialog view (ice, sugar, size, toppings, note).
- [x] 7.18 Add ice level control (e.g. None, Less, Normal, More) and bind to line option.
- [x] 7.19 Add sugar level control and bind to line option.
- [x] 7.20 Add size control (S/M/L or similar) and bind to line option and price modifier.
- [x] 7.21 Add toppings multi-select and note text field; bind to line.
- [x] 7.22 Add quantity and "Add to bill" (or "Update line") in dialog; close and refresh bill.
- [x] 7.23 Add "Pay" or "Checkout" button opening payment screen.
- [x] 7.24 Create payment view with Cash and Card (and optionally other methods).
- [x] 7.25 Add cash amount input and "Exact" / "Change" display.
- [x] 7.26 Add card payment option and optional amount split.
- [x] 7.27 Add "Complete payment" command; persist order and payment; close bill tab.
- [x] 7.28 Add payment success feedback and return to Order view.
- [x] 7.29 Add "Void line" action with confirmation and reason input.
- [x] 7.30 Persist void line (soft void) and refresh bill; optional manager approval flow.
- [x] 7.31 Add "Void bill" action with confirmation and reason.
- [x] 7.32 Persist void bill and audit log; close bill tab.
- [x] 7.33 Add optional manager approval for void (role-based).
- [x] 7.34 Add "Hold" button to suspend current bill and clear bill panel.
- [x] 7.35 Persist held order with label (e.g. Table X, Takeaway); add to held list.
- [x] 7.36 Add "Recall" UI to list and select a held order.
- [x] 7.37 Recall command loads held order into current bill and removes from held list.
- [x] 7.38 Add multiple open bills (tabs) with labels; switch tab updates bill panel.
- [x] 7.39 Add "New bill" tab and "Close bill" (with payment or void).
- [x] 7.40 Document: Phase 7 complete when all 7.x tasks are done.

### Phase 8 – Receipts and kitchen display

- [x] 8.1 Add receipt template resource (e.g. XAML or text template) with placeholders for header, lines, totals.
- [x] 8.2 Implement receipt data model (header, line items, totals, footer).
- [x] 8.3 Add receipt generation service: build receipt content from completed order.
- [x] 8.4 Add business name, address, and optional logo to receipt header.
- [x] 8.5 Add line items with name, quantity, price, options to receipt body.
- [x] 8.6 Add subtotal, discount, tax, total and payment method to receipt.
- [x] 8.7 Add optional QR or barcode for order reference on receipt.
- [x] 8.8 Add print receipt command after payment (queue or direct to printer).
- [x] 8.9 Add "Reprint" from order history with same template.
- [x] 8.10 Create kitchen display view (full-screen or second screen).
- [x] 8.11 Add kitchen order list: new orders appear with timestamp and items.
- [x] 8.12 Add per-line options (ice, sugar, size, note) on kitchen display.
- [x] 8.13 Add "Mark prepared" or "Send to kitchen" state and filter by status.
- [x] 8.14 Add sound or visual alert for new kitchen orders (optional).
- [x] 8.15 Add kitchen display settings: font size, columns, auto-refresh interval.
- [x] 8.16 Persist order state for kitchen (sent, in progress, done).
- [x] 8.17 Add reprint kitchen ticket from order detail.
- [x] 8.18 Document: Phase 8 complete when all 8.x tasks are done.

### Phase 9 – Customers, loyalty, and tax

- [x] 9.1 Add customer lookup field or button on Order view (optional link to bill).
- [x] 9.2 Create customer search/lookup dialog (by name, phone, or ID).
- [x] 9.3 Bind selected customer to current order; display name on bill panel.
- [x] 9.4 Add "New customer" quick form: name, phone, optional email/address.
- [x] 9.5 Persist customer and link to order.
- [x] 9.6 Add loyalty points balance display when customer is selected.
- [x] 9.7 Add "Redeem points" option and points-to-discount conversion.
- [x] 9.8 Add points accrual on payment (configurable rate) and persist.
- [x] 9.9 Add tax configuration model (rate, name, applicable to product categories).
- [x] 9.10 Add tax calculation service: compute tax from order lines and rules.
- [x] 9.11 Apply tax to bill panel and receipt; support multiple tax rates per order.
- [x] 9.12 Add tax-exempt flag per customer or per order (optional).
- [x] 9.13 Add customer list view (manager): search, edit, view order history.
- [x] 9.14 Add customer detail: contact info, loyalty balance, recent orders.
- [x] 9.15 Document: Phase 9 complete when all 9.x tasks are done.

### Phase 10 – Inventory and product management

- [x] 10.1 Add inventory entity: product/sku, quantity on hand, reorder level, unit.
- [x] 10.2 Add inventory repository and list/update methods.
- [x] 10.3 Add "Decrease stock" on order completion for linked products (e.g. by recipe or direct link).
- [x] 10.4 Add low-stock warning when quantity below reorder level (dashboard or alert).
- [x] 10.5 Add inventory list view (manager): product, current qty, reorder level, last updated.
- [x] 10.6 Add inventory adjust form: quantity change, reason, audit log.
- [x] 10.7 Add product–ingredient or product–inventory link model (optional for cafe).
- [x] 10.8 Add stock-in form: receive quantity, supplier reference, date.
- [x] 10.9 Add product management list: name, category, price, active, image.
- [x] 10.10 Add product add/edit form: name, category, base price, options (size price modifiers), image upload.
- [x] 10.11 Add category management: list, add, edit, reorder.
- [x] 10.12 Add category drag-and-drop or numeric sort for display order.
- [x] 10.13 Add product import (CSV/Excel) with mapping and validation.
- [x] 10.14 Add product export (CSV/Excel) for backup or transfer.
- [x] 10.15 Document: Phase 10 complete when all 10.x tasks are done.

### Phase 11 – Staff, shifts, and audit

- [x] 11.1 Add shift entity: employee, start/end time, opening/closing cash amount.
- [x] 11.2 Add shift repository: start shift, end shift, get current shift, list by date.
- [x] 11.3 Add shift register view: clock in/out, opening cash input, closing cash and reconciliation.
- [x] 11.4 Add shift summary: sales count, total amount, payment breakdown.
- [x] 11.5 Add employee list view (manager): name, role, status, last login.
- [x] 11.6 Add employee add/edit form: name, login, role, pin (if used).
- [x] 11.7 Add role-based permissions: map roles to allowed screens and actions.
- [x] 11.8 Enforce permission checks on navigation and sensitive actions (void, discount override, settings).
- [x] 11.9 Add audit log entity: user, action, entity type, entity id, timestamp, details.
- [x] 11.10 Log order create, payment, void, discount override, price override.
- [x] 11.11 Add audit log list view: filter by user, date, action type.
- [x] 11.12 Add audit log export (CSV) for compliance.
- [x] 11.13 Add "Current user" and "Switch user" or logout in shell; show current shift.
- [x] 11.14 Add manager override dialog for void/discount/price with reason and PIN.
- [x] 11.15 Document: Phase 11 complete when all 11.x tasks are done.

### Phase 12 – Reporting

- [x] 12.1 Add report data service: sales by date range, by category, by product.
- [x] 12.2 Add sales summary report: total revenue, order count, average ticket, date range filter.
- [x] 12.3 Add sales by category report: category name, quantity, revenue.
- [x] 12.4 Add sales by product report: product name, quantity sold, revenue.
- [x] 12.5 Add payment method breakdown report: cash, card, other; totals and counts.
- [x] 12.6 Add hourly/daily trend data for charts (e.g. sales by hour, by day).
- [x] 12.7 Create reports list view: links or tabs to each report type.
- [x] 12.8 Add date range picker (today, yesterday, last 7 days, custom range) for all reports.
- [x] 12.9 Add report results view: table or grid with sortable columns.
- [x] 12.10 Add export report to CSV or PDF (optional).
- [x] 12.11 Add dashboard widgets: today sales, top products, low stock (if Phase 10 done).
- [x] 12.12 Add shift report: per-shift sales and payment summary for selected date.
- [x] 12.13 Add employee sales report: sales attributed to employee by shift (optional).
- [x] 12.14 Add discount report: total discounts given by type and date range.
- [x] 12.15 Add tax report: tax collected by period for filing.
- [x] 12.16 Document: Phase 12 complete when all 12.x tasks are done.

### Phase 13 – Tables and delivery

- [x] 13.1 Add table entity: name/code, capacity, zone, status (empty, occupied, reserved).
- [x] 13.2 Add table repository: list, get by id, update status.
- [x] 13.3 Add table map view: grid or list of tables with status and current order indicator.
- [x] 13.4 Add tap table to assign or create order for that table; open Order view with table context.
- [x] 13.5 Add table assignment to order (table id or name) and show on bill tab.
- [x] 13.6 Add service type on order: Dine-in, Takeaway, Delivery (enum or config).
- [x] 13.7 Add delivery fields to order: address, fee, optional driver/notes.
- [x] 13.8 Add delivery address form or lookup when service type is Delivery.
- [x] 13.9 Add delivery fee calculation (fixed or by zone/distance) and display on bill.
- [ ] 13.10 Add table management view (manager): add/edit/remove tables, zones, capacity.
- [ ] 13.11 Add merge tables action: combine two orders (e.g. merge tables) with confirmation.
- [ ] 13.12 Add reserve table: set status to reserved with optional time/customer.
- [ ] 13.13 Document: Phase 13 complete when all 13.x tasks are done.

### Phase 14 – Hardware, settings, and polish

- [x] 14.1 Add receipt printer service: send ESC/POS commands to configured printer.
- [ ] 14.2 Add printer configuration: name, port/network path, paper width, test print.
- [ ] 14.3 Add cash drawer open command (ESC/POS or dedicated command) and link to payment.
- [ ] 14.4 Add barcode scanner support (keyboard wedge): parse scan input and add product or search.
- [x] 14.5 Add settings view: sections for Business, Receipt, Tax, Payment methods, Printers, Database.
- [x] 14.6 Add business info settings: name, address, phone, logo; save and use in receipts.
- [x] 14.7 Add receipt template settings: header/footer text, show/hide options.
- [x] 14.8 Add tax settings: default rate(s), names, applicability.
- [ ] 14.9 Add payment method list: name, type (cash, card, etc.), active; save.
- [ ] 14.10 Add printer list settings: add, edit, remove, set default; test print.
- [ ] 14.11 Add database/config connection settings and save; apply on restart if needed.
- [ ] 14.12 Add localization: resource files for EN and VI; load by current culture.
- [ ] 14.13 Replace all user-facing strings in Shell and Login with resource keys.
- [ ] 14.14 Replace all user-facing strings in Order, Dashboard, and Manager views with resource keys.
- [ ] 14.15 Add accessibility: focus order and accessible names on key controls.
- [ ] 14.16 Add high-contrast theme option and apply in App.axaml or theme selector.
- [ ] 14.17 Add contrast and font size checks for key screens; document a11y status.
- [ ] 14.18 Document: Phase 14 complete when all 14.x tasks are done.

### Phase 15 – Refunds, split payment, gift card, and extras

- [x] 15.1 Add refund entity: original order, amount, method (cash/card), reason, timestamp.
- [x] 15.2 Add refund from order history: select order, full or partial amount, reason.
- [x] 15.3 Add refund to original payment method or cash; persist and update order state.
- [ ] 15.4 Add split payment: multiple payment methods per order (e.g. cash + card).
- [ ] 15.5 Add payment split UI: add payment 1 (cash), payment 2 (card), total must match; complete.
- [ ] 15.6 Persist multiple payment records per order and show on receipt.
- [ ] 15.7 Add gift card entity (if supported): code, balance, expiry.
- [ ] 15.8 Add gift card balance check (dialog or field) and apply to order as discount.
- [ ] 15.9 Add gift card reload (if supported by provider); persist balance change.
- [ ] 15.10 Add tip field on payment screen: amount or percentage; optional.
- [ ] 15.11 Persist tip per order and link to employee if applicable.
- [ ] 15.12 Add tip report by employee and date range (manager report).
- [ ] 15.13 Add coupon/discount code field on bill; validate and apply discount.
- [ ] 15.14 Add price override with manager approval and audit log (if not already in Phase 7/11).
- [ ] 15.15 Document: Phase 15 complete when all 15.x tasks are done.
