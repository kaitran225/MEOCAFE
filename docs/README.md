# HienXanh POS — docs

| Document | What it covers |
| --- | --- |
| [`design-system.md`](design-system.md) | Tokens, buttons, shared components, and how to add to the system |
| [`../.rules/design.md`](../.rules/design.md) | The design rules a screen must satisfy before it is "done" |
| [`../.report/09-11-26-report_03.md`](../.report/09-11-26-report_03.md) | UI redesign and component unification: what changed, defects fixed, verification, open items |
| [`../.report/09-11-26-report_01.md`](../.report/09-11-26-report_01.md) | Checkout / sales screen review (P0–P2 findings) |
| [`../.report/09-11-26-report_02.md`](../.report/09-11-26-report_02.md) | Database layer review (migrations, repositories, timezone) |

New to the UI code? Read `design-system.md` first, then `.rules/design.md`, then
the relevant report.

Project conventions:

- Reports are dated and live in `.report/` (`DD-MM-YY-report_NN.md`).
- Rules that must hold for every screen live in `.rules/`.
- Reference documentation lives here, in `docs/`.
