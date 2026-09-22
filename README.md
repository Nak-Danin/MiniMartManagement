# MiniMart Management System — Step 1: Project Setup

## What this step contains

A working, buildable Windows Forms skeleton with the layered folder
structure the project will grow into. Nothing about products, sales,
or login is implemented yet — that comes in later phases. This step
only proves the architecture "wires up": Forms → Services → Repositories →
Database, with a real `.sln`/`.csproj` you can open in Visual Studio.

```
MiniMartManagement/
├── MiniMartManagement.sln
├── MiniMartManagement.csproj
├── appsettings.json          ← SQL Server connection string lives here
├── Program.cs                ← application entry point
├── Presentation/
│   ├── Forms/
│   │   └── PlaceholderForm.cs   ← temporary screen, replaced by LoginForm later
│   └── Controls/                ← reusable custom controls go here later
├── Models/                      ← domain classes (empty until Phase 3)
├── Services/                    ← business logic (empty until Phase 5)
├── Repositories/                ← ADO.NET data access (empty until Phase 4)
├── Interfaces/                  ← repository contracts (empty until Phase 4)
├── Database/
│   └── DbConnectionFactory.cs   ← single place that creates SqlConnections
└── Utilities/
    └── AppSettings.cs           ← reads appsettings.json
```

## Why these files exist already

- **`DbConnectionFactory`** — repositories will ask this class for a
  connection instead of each repository knowing the connection string
  itself. This keeps the connection string in exactly one place.
- **`AppSettings`** — a tiny config reader so the connection string
  isn't hardcoded in source. In a real company project you'd likely use
  `Microsoft.Extensions.Configuration`; for an OOAD course project a
  plain JSON reader is easier to explain in your report and has no
  extra dependencies.
- **`PlaceholderForm`** — exists purely so `dotnet build`/`F5` gives you
  something to see right now. It gets deleted once `LoginForm` exists.

## How to open and run this (Visual Studio)

1. Copy the `MiniMartManagement` folder anywhere on your machine.
2. Double-click `MiniMartManagement.sln` to open it in Visual Studio 2022.
3. Visual Studio will restore the `Microsoft.Data.SqlClient` NuGet
   package automatically on first build (needs internet the first time).
4. Press **F5**. You should see a window titled
   *"MiniMart Management System - Project Skeleton"*.

## How to build/run from the command line (if you have the .NET 8 SDK)

```bash
dotnet restore
dotnet build
dotnet run
```

Note: Windows Forms apps only run on Windows, so `dotnet run` must be
executed on a Windows machine (or Windows VM) — not Linux/macOS.

## Configuring your database connection

Edit `appsettings.json`. Two common setups:

**SQL Server Express (default in this file):**
```json
"Server=localhost\\SQLEXPRESS;Database=MiniMartDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

**LocalDB (comes bundled with Visual Studio, no separate install):**
```json
"Server=(localdb)\\MSSQLLocalDB;Database=MiniMartDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Tell me which one you have and I'll set the default accordingly in
Step 2, when we create the actual database and tables.

## Step 2 — Database

SQL scripts live in `Database/Scripts/`, run in order:

1. **`01_CreateDatabase.sql`** — creates the `MiniMartDb` database.
   Run this connected to `master`.
2. **`02_CreateTables.sql`** — creates all 7 tables (`Users`,
   `Employees`, `Categories`, `Products`, `Inventory`, `Sales`,
   `SaleItems`) with primary keys, foreign keys, `CHECK` constraints,
   and indexes on foreign-key columns.
3. **`03_SeedData.sql`** — inserts test data: one Admin login, one
   Employee login, 3 categories, 4 products, and starting inventory
   (deliberately includes one low-stock and one out-of-stock product
   so Phase 7 reports have something to show).

### How to run them

**In SQL Server Management Studio (SSMS) or Azure Data Studio:**
Open each `.sql` file and execute in order (`01` → `02` → `03`).

**From the command line with `sqlcmd`:**
```bash
sqlcmd -S localhost -E -i Database/Scripts/01_CreateDatabase.sql
sqlcmd -S localhost -E -i Database/Scripts/02_CreateTables.sql
sqlcmd -S localhost -E -i Database/Scripts/03_SeedData.sql
```
Replace `-S localhost` with your server name/instance (e.g.
`localhost\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`, or a remote server
name), and swap `-E` (Windows auth) for `-U user -P password` if you're
using SQL authentication.

### Design decisions worth knowing about

- **`Employees.user_id` is 0-or-1 per `Users` row, not strictly 1-1**
  (see the note above the scripts) — Admins never get an `Employees`
  row; only Employee-role users do.
- **`Inventory.product_id` has a `UNIQUE` constraint** — this is what
  actually enforces "a product should have one inventory record"
  at the database level, not just by convention.
- **`Sales.payment >= total`** and **`SaleItems.quantity > 0`** etc.
  are enforced with `CHECK` constraints — so even if a bug slipped
  past the Service layer, the database itself won't accept bad data.
- **`SaleItems` cascades on delete from `Sales`** — deleting a sale
  (rare, but useful for admin corrections) automatically removes its
  line items instead of leaving orphans.
- **Seed admin login:** username `admin`, password `Admin123!` — the
  password hash in the seed script was generated with the exact same
  PBKDF2 algorithm/parameters (`SHA256`, 100,000 iterations, 16-byte
  salt) that `AuthService` will use in Phase 6, so it will log in
  correctly once authentication code exists.

## Next step

**Step 3 — Models**: the domain classes (`User`, `Admin`, `Employee`,
`Category`, `Product`, `Inventory`, `Sale`, `SaleItem`) with proper
encapsulation, the `User` → `Admin`/`Employee` inheritance hierarchy,
and validation inside the models themselves (e.g. a `Product` refusing
a negative price).

## Step 3 — Models

Domain classes now live in `Models/`. Nothing here touches the database
or the UI — these are plain C# classes whose entire job is to keep
their own data valid.

```
Models/
├── UserRole.cs         enum: Admin | Employee
├── EmployeeStatus.cs   enum: Active | Inactive
├── User.cs             abstract base class
├── Admin.cs            : User
├── Employee.cs         : User
├── Category.cs
├── Product.cs
├── Inventory.cs
└── Sale.cs / SaleItem.cs
```

### Where each OOAD requirement (section 6 of the spec) shows up

- **Inheritance** — `Admin : User` and `Employee : User`. Shared fields
  (`Username`, `PasswordHash`, `IsActive`, ...) live once, in `User`.
- **Polymorphism** — `User.Role`, `GetDashboardTitle()`,
  `CanManageEmployees()`, etc. are `virtual`/`abstract` and overridden
  differently by `Admin` and `Employee`. Later, `DashboardForm` will call
  `currentUser.GetDashboardTitle()` without caring which subclass it
  actually has — the correct behavior happens automatically.
- **Encapsulation** — every property has a `private set` (or `internal
  set` for repository-only fields like `Id`). The only way to change a
  `Product`'s price or an `Inventory`'s quantity is through a method
  that validates the change (`UpdateDetails`, `Increase`, `Decrease`).
  Nothing outside a model can put it into an invalid state.
- **Abstraction** — `User` is `abstract`; you can never instantiate a
  bare `User`, only a concrete `Admin` or `Employee`. (The repository
  *interfaces* from section 6 come in Step 4.)
- **Composition** — `Sale` owns a private `List<SaleItem>`, exposed only
  as read-only (`IReadOnlyList<SaleItem>`). A `SaleItem` has no meaning
  outside the `Sale` it belongs to, and callers can only add/remove
  items through `Sale.AddItem()` / `RemoveItem()`.
- **Association** — `Employee`/`Admin` and `Sale` are linked by
  `Sale.EmployeeId`, without `Sale` owning the `Employee` (an employee
  exists independently of any particular sale) — a plain reference
  relationship rather than composition.

### Business rules already enforced at the model level

| Rule | Where |
|---|---|
| Rule 5 / 11 — stock can't go negative, `NewStock = CurrentStock - QuantitySold` | `Inventory.Decrease()` |
| Rule 6 — a sale needs at least one item | `Sale.EnsureCanComplete()` |
| Rule 7 — quantity > 0 | `SaleItem` constructor |
| Rule 8 — prices can't be negative | `Product` constructor / `UpdateDetails()` |
| Rule 9 — payment ≥ total | `Sale.SetPayment()` |
| Rule 10 — change = payment − total | `Sale.ChangeAmount` (computed property) |
| Rule 12 — low-stock warning at MinStock | `Inventory.IsLowStock(int minStockLevel)` |
| Rule 13 — inactive employees can't log in | `Employee.IsAvailableForLogin()` |

Rules that depend on *other* data (rule 1 - must be logged in, rule 2/3
- who's allowed to do what, rule 4/14 - active product checks against
the database, rule 15 - transaction safety) can't live in a model by
themselves — those belong in the **Service layer**, coming in Step 5,
after Repositories exist to fetch the data those checks need.

### A note on `Id` properties

`Id` (and `EmployeeId` on `Employee`) use `internal set` rather than
`private set`. `internal` means "settable from anywhere in this same
project" — so once we write `Repositories/` in Step 4, a repository can
assign the database-generated ID after an `INSERT`, but Presentation
code (Forms) still can't tamper with it. This is a deliberate, narrow
exception to encapsulation, not a loophole — worth mentioning in your
report if your instructor asks about it.

## Step 4 — Repositories & Interfaces

```
Interfaces/
├── IUserRepository.cs
├── IEmployeeRepository.cs
├── ICategoryRepository.cs
├── IProductRepository.cs
├── IInventoryRepository.cs
└── ISaleRepository.cs

Repositories/
├── UserRepository.cs
├── EmployeeRepository.cs
├── CategoryRepository.cs
├── ProductRepository.cs
├── InventoryRepository.cs
└── SaleRepository.cs
```

This is the **only** layer that contains `SqlConnection`/`SqlCommand`
code. Every query is parameterized (`command.Parameters.AddWithValue(...)`)
— no string concatenation of SQL anywhere, per your security requirement.

### How repositories map database rows back to models

Since model constructors validate their input, repositories reuse those
same constructors when reading from the database (a saved row is
already known-valid), then use the `internal set` properties (`Id`,
`EmployeeId`) to attach the database-generated primary key afterward.
`UserRepository.MapUser()` is the one interesting case: it reads the
`role` column and decides whether to construct an `Admin` or an
`Employee` — this is the repository layer participating in the same
polymorphism the models set up in Step 3. `EmployeeRepository` reuses
that exact method (via `internal`) instead of duplicating the mapping.

### The two transactional repositories

- **`EmployeeRepository.Add()`** — creating an employee touches two
  tables (`Users` for login, `Employees` for profile). Both inserts
  happen inside one `SqlTransaction`; if the second insert fails, the
  first is rolled back, so you can never end up with a login that has
  no profile.
- **`SaleRepository.CompleteSale()`** — the important one, implementing
  section 12 of your spec exactly: insert `Sales`, insert each
  `SaleItems` row, decrease `Inventory` — all in one transaction. The
  stock decrease is written as
  `UPDATE Inventory SET quantity = quantity - @qty WHERE product_id = @productId AND quantity >= @qty`
  — the stock check and the decrement happen as a single atomic
  database operation, so even under concurrent access the quantity can
  never go negative. If `0` rows are affected (not enough stock), the
  whole transaction rolls back and nothing is written.

### Note on `SetStatus` for employees

"Activate/deactivate employee" updates **two** columns in one
transaction: `Employees.status` and the linked `Users.is_active`. This
keeps `Employee.IsAvailableForLogin()` (from Step 3, which checks both)
reliably in sync — deactivating an employee always blocks their login,
not just half the time depending on which field got updated.

## Step 5 — Services

```
Services/
├── SessionContext.cs      tracks who's logged in (rule 1)
├── AuthService.cs         login / logout / change password (rules 1, 13)
├── EmployeeService.cs     Admin-only employee management (rule 2)
├── CategoryService.cs     Admin-only category management (rule 3)
├── ProductService.cs      Admin-only product management (rule 3)
├── InventoryService.cs    stock viewing + manual adjustment
├── SaleService.cs         POS checkout orchestration (rules 4,5,6,7,9,10,14,15)
├── ReportService.cs       Admin-only reporting
└── Exceptions/
    ├── AuthenticationException.cs
    └── BusinessRuleException.cs
```

Two small additions were also made to files from earlier steps:
`Utilities/PasswordHasher.cs` (PBKDF2, matching the seed data exactly),
and a `CanManageInventory()` permission hook added to `User`/`Admin`
(section 5 of your spec calls out "Manage inventory" as its own Admin
capability, separate from products/categories).

### How authorization works

Every Admin-only method takes the acting `User` as its first parameter
and calls one of the polymorphic checks from Step 3
(`CanManageEmployees()`, `CanManageProductsAndCategories()`,
`CanManageInventory()`, `CanViewAllSalesReports()`) — this is the same
`User` → `Admin`/`Employee` polymorphism doing real work now, not just
sitting in the model layer. An `Employee` object always returns `false`
from all of these, so calling `employeeService.AddEmployee(someEmployee, ...)`
throws `UnauthorizedAccessException` automatically — there's no
separate "if role == Admin" check scattered through the services to
forget.

**Forms will pass `SessionContext.CurrentUser` as the acting user** for
every Admin-only call, once Phase 6/7 wires up the login screen — so an
Employee genuinely cannot reach these code paths, satisfying "Employees
MUST NOT access Admin functions" at the service layer, before a Form
even exists.

### Two design notes worth knowing about

- **`SaleService.StartSale()` takes an `Employee`, not a `User`.**
  `Sales.employee_id` is a foreign key to `Employees`, and only
  `Employee` objects have an `EmployeeId` — `Admin` doesn't. This means
  an Admin physically cannot start a sale; the type system (backed by
  the database schema from Step 2) enforces "employees process sales"
  without needing an explicit role check in `SaleService` at all.
- **Exception types split failures by cause**: `AuthenticationException`
  for login problems, `BusinessRuleException` for things like
  duplicate usernames or insufficient stock, and plain
  `UnauthorizedAccessException` for permission failures. Phase 7/8
  Forms will catch these separately to show the right kind of message
  (a login form shouldn't say "insufficient stock", etc.).

## Step 6 — Authentication (Forms)

The app is wired end-to-end now: **UI → Service → Repository →
Database**. `PlaceholderForm` is gone, replaced by a real login flow.

```
Services/
└── AppServices.cs          composition root - builds every repo + service once

Presentation/Forms/
├── LoginForm.cs             username/password, calls AuthService.Login()
└── DashboardForm.cs         shared shell, sidebar driven by role permissions

Program.cs                   runs a Login -> Dashboard -> Login loop
```

### How the flow works

1. `Program.cs` creates one `AppServices` instance (this builds the
   `DbConnectionFactory`, every repository, and every service exactly
   once) and passes it to `LoginForm`.
2. `LoginForm` is shown with `ShowDialog()`. On success,
   `AuthService.Login()` sets `SessionContext.CurrentUser` and
   `LoginForm.LoggedInUser` is set; the form closes with
   `DialogResult.OK`.
3. `Program.cs` then shows `DashboardForm`, passing in the logged-in
   `User`. **This is the payoff from Step 3/5**: `DashboardForm` never
   checks `if (user.Role == "Admin")` anywhere — it just calls
   `currentUser.CanManageEmployees()`, `CanManageProductsAndCategories()`,
   etc., and only adds sidebar buttons for what comes back `true`. An
   `Employee` object returns `false` from all the Admin checks
   automatically, so the Admin buttons never even get created for an
   Employee's dashboard — this is the "clearly distinguish Admin and
   Employee functionality" requirement (section 9) enforced by
   polymorphism rather than a big `switch` statement.
4. Clicking a sidebar button right now shows a "coming in a later step"
   message — the real screens (`EmployeeManagementForm`, `POSForm`,
   etc.) are Phase 7/8. This step is only about proving login, role
   detection, and role-based navigation work.
5. **Logout** sets `DialogResult.OK` and closes `DashboardForm`;
   `Program.cs` then calls `AuthService.Logout()` (clearing
   `SessionContext`) and loops back to `LoginForm`. Closing the
   dashboard via the window's **X** button does the same cleanup — the
   session is always cleared on the way back to login, regardless of
   how the dashboard closed.

### Try it

Build and run in Visual Studio. You should be able to log in with
either seeded account from Step 2:

| Username | Password | Role |
|---|---|---|
| `admin` | `Admin123!` | Admin — sidebar shows Employee/Category/Product/Inventory/Reports |
| `jdoe` | *(see note below)* | Employee — sidebar shows POS/Product List/My Sales |

**Note on `jdoe`:** the Step 2 seed script inserted a placeholder hash
for this account that doesn't correspond to a real password (it was
just there to prove the schema, before `PasswordHasher` existed). It
will **not** log in as-is. Easiest fix: once you're logged in as
`admin`, we'll wire up "Add Employee" in Step 7 and you can create a
real employee through the app instead — or tell me now and I'll give
you a corrected seed script with a proper `jdoe` password hash to
re-run.

## Step 7 — Admin

All five Admin sidebar buttons now open real, working screens instead
of "coming soon" placeholders.

```
Presentation/Forms/
├── EmployeeManagementForm.cs   list, search, add, edit, activate/deactivate employees
├── EmployeeEditDialog.cs        shared Add/Edit dialog (hides username/password in Edit mode)
├── CategoryManagementForm.cs    list, add, edit, activate/deactivate categories
├── CategoryEditDialog.cs
├── ProductManagementForm.cs     list, search, add, edit, activate/deactivate products
├── ProductEditDialog.cs         category dropdown; product code locked once created
├── InventoryForm.cs             stock list, low/out-of-stock filters, manual adjustment
├── StockAdjustDialog.cs
└── SalesReportForm.cs           tabbed: Sales by Date, Best Sellers, Low Stock, Out of Stock
```

### Pattern used across all four management forms

Every list form follows the same shape: a `DataGridView` populated from
a `Service.GetAll()`/`Search()` call, with the underlying model object
stashed in each row's `Tag` (so Edit/Activate don't need a second
lookup), and Add/Edit buttons opening a small dialog whose result is
handed straight to the matching `Service` method. Every service call is
wrapped in a `try/catch` for `BusinessRuleException` /
`UnauthorizedAccessException` — so if a rule is violated (duplicate
product code, duplicate username, empty required field), the person
sees the exact message the Service layer wrote, not a raw exception or
a crash. Nothing here talks to the database directly — you can check
that by searching these files for `SqlCommand`: there isn't one.

### `SalesReportForm`

Built as a `TabControl` with four tabs, each backed by a different
`ReportService` method: **Sales by Date** (with a date range and a
running total), **Best Sellers**, **Low Stock**, and **Out of Stock**.
This is the one form where all the data is loaded once when the form
opens (except the date-range tab, which re-runs on demand) rather than
needing a refresh button.

### A couple of small decisions worth knowing about

- **Product code becomes read-only when editing** — `ProductService`
  never accepts a code change in `UpdateProduct()`, since changing a
  product's identity code after it's been sold under that code would
  be confusing for reports. The dialog just reflects that.
- **Inventory adjustment is a "set to" not "add/remove"** —
  `AdjustStock(productId, newQuantity)` is meant for manual corrections
  (restocking, damage write-offs, stock counts), which is different
  from the automatic `Increase`/`Decrease` that happens during a sale
  (that logic lives in `SaleRepository.CompleteSale`, built in Step 4,
  and isn't touched by this form at all).

## Step 8 — Employee, and a full visual redesign

Every remaining screen from the spec is now built, and the entire app
was restyled to look like a modern desktop tool instead of default
gray WinForms.

```
Presentation/
├── UiTheme.cs                   shared color palette + styling helpers
└── Forms/
    ├── POSForm.cs                the POS screen - search, cart, checkout, receipt
    ├── ProductViewForm.cs        read-only product browsing + live stock
    ├── MySalesForm.cs            an employee's own sales history
    └── ReceiptForm.cs            printable receipt (System.Drawing.Printing)
```

### `UiTheme` — one place for the whole app's look

Rather than hand-styling every button and grid individually, `UiTheme`
holds the color palette (a blue accent, dark sidebar, white cards) and
a handful of helpers: `StyleForm`, `StylePrimaryButton`,
`StyleSecondaryButton`, `StyleDangerButton`, `StyleGrid`,
`StyleTextBox`, `CreateCard`. Every form calls into these instead of
setting `BackColor`/`Font`/`FlatStyle` inline — change the palette in
one file and the whole app updates. This is still plain WinForms (no
external UI library, no extra NuGet package), just applied
consistently — in keeping with "do not over-engineer" from section 18.

**What changed visually:**
- **`LoginForm`** — full redesign: a dark brand panel on the left, a
  white card with the login fields on the right, instead of a plain
  gray dialog.
- **`DashboardForm`** — dark sidebar (matches modern admin-panel
  conventions), flat nav buttons with hover highlighting, a welcome
  "card" in the content area instead of a bare label.
- **Every list screen** — `DataGridView`s now have a flat white
  background, a light gray header row, subtle alternating row
  stripes, and no default gridlines; action buttons are colored by
  intent (blue for the primary action, outlined gray for secondary,
  red reserved for destructive/logout actions).

### `POSForm` — the payoff screen

Implements the exact workflow from section 10 of your spec: search →
select → quantity → **`SaleService.AddItemToCart`** (checks active +
stock) → cart updates live → discount via
**`SaleService.ApplyDiscount`** → payment via **`SaleService.SetPayment`**
→ **`SaleService.CompleteSale`** (the atomic transaction from Step 4) →
`ReceiptForm` opens automatically → cart resets for the next customer,
and the product list refreshes so the just-sold stock is reflected
immediately.

### `ReceiptForm`

A plain-text receipt built from the completed `Sale`, shown in a
read-only textbox with a working **Print** button
(`System.Drawing.Printing.PrintDocument` — sends to whatever printer
is configured in Windows, or "Print to PDF" if that's all that's
available on your machine to test with). Reused by both `POSForm`
(right after checkout) and `MySalesForm` (reviewing a past sale).

### Every sidebar button now opens a real screen

`DashboardForm.OpenFeature()` no longer has a "coming soon" fallback —
all eight items (5 Admin + 3 Employee) route to a finished form. That
completes the full form list from section 9 of your spec.

## The application is now feature-complete

All 19 functional requirements from section 3, all 15 business rules
from section 4, and every form from section 9 are implemented and
wired end-to-end: Forms → Services → Repositories → SQL Server.

What's left from your original spec is mostly *process*, not code:

- **Section 16 — Testing**: writing the actual test cases (correct/
  incorrect login, insufficient stock, sale rollback, etc.) against
  what's built. I can help set up a test project (xUnit/MSTest) if
  you'd like — say the word and we can treat that as Step 9.
- **Section 17 — UML/OOAD diagrams**: Use Case, Class, Activity,
  Sequence, ERD, Component. These should now be *drawn from* the
  finished code rather than the other way around — I can help write
  out the diagram contents (actors, relationships, sequences) in
  whatever tool/notation your course expects.

Try the full flow end-to-end: log in as `admin`, add a category and a
product, log out, log back in (or create a real employee via Employee
Management and log in as them), and run a full sale through the new
POS screen. Let me know how it looks and whether you'd like to tackle
testing or the UML diagrams next.

## Step 9 — UI reinnovation: embedded navigation, layout fixes, product photos

This step doesn't add new business features - it reworks how the existing
screens are presented, based on four pieces of feedback after trying
Step 8 as a real user.

### 1. Sidebar navigation no longer pops up separate windows

Every screen that used to be `ShowDialog()`'d from the sidebar
(Employee/Category/Product Management, Inventory, Sales Reports, POS,
Product List, My Sales History) is now a `UserControl` instead of a
`Form`, and `DashboardForm` swaps it directly into a content panel to
the right of the sidebar:

```
Presentation/Forms/
├── DashboardForm.cs             sidebar + a persistent content host on the right
├── DashboardHomePanel.cs        new "Dashboard" landing page (stats + quick actions)
├── EmployeeManagementPanel.cs   was EmployeeManagementForm
├── CategoryManagementPanel.cs   was CategoryManagementForm
├── ProductManagementPanel.cs    was ProductManagementForm - now an ecommerce photo grid
├── InventoryPanel.cs            was InventoryForm
├── SalesReportPanel.cs          was SalesReportForm
├── POSPanel.cs                  was POSForm
├── ProductViewPanel.cs          was ProductViewForm - now an ecommerce photo grid
└── MySalesPanel.cs              was MySalesForm
```

Clicking a sidebar item now just replaces the control docked in the
content area (`DashboardForm.Navigate`) - the window itself never
changes, and the clicked item gets a highlighted background so it's
obvious which page you're on. The small Add/Edit dialogs
(`ProductEditDialog`, `EmployeeEditDialog`, `CategoryEditDialog`,
`StockAdjustDialog`, `ReceiptForm`) are still real popups - that's a
normal, expected pattern for a short "fill in a form and confirm"
action, and is different from a whole screen popping up like Step 8 did.

### 2. Fixed the actual causes of "half-hidden" controls

Two concrete, reproducible bugs were causing this, both now fixed:

- **`EmployeeEditDialog` and `ProductEditDialog` were shorter than
  their own content.** `EmployeeEditDialog`'s Add-mode fields added up
  to roughly 550px of stacked fields, inside a dialog whose
  `ClientSize.Height` was hardcoded to 420 - so Save/Cancel rendered
  below the visible window. Same issue in `ProductEditDialog`. Both
  dialogs are now sized generously, `AutoScroll = true`, and resizable,
  so nothing depends on a hand-guessed pixel height ever again.
- **`SalesReportForm` placed "From"/"To" labels at `Y = -5`** - a
  negative coordinate that clips a label's top half against its
  container's edge. Now at a normal positive `Y` inside a taller
  header panel.

As a general hardening pass, every place that used to stack multiple
`Dock = Top` (or `Bottom`) siblings by hand - which silently reverses
your intended order if you get the add-order wrong - was rebuilt with
a top-down `FlowLayoutPanel`, where the visual order always matches
the order controls are added, with no ambiguity.

### 3. The receipt and POS checkout no longer clip the totals

- **`ReceiptForm`** used to render everything (items *and* totals) as
  one block of monospace text in a fixed-size `TextBox`, so Subtotal
  could get pushed out of view. It's now split into an item grid (the
  only part that scrolls) and a fixed, non-scrolling totals panel
  docked to the bottom (Subtotal/Discount/Total/Payment/Change), so
  the totals are physically incapable of being scrolled out of sight.
  Printing (`System.Drawing.Printing`) still works exactly as before.
- **`POSPanel`**'s checkout section used to be a fixed-height
  `TableLayoutPanel` with 6 equal rows, which clipped labels whenever
  a row's shared height didn't fit that label's font. It's now a
  top-down `FlowLayoutPanel` where each row (Subtotal, Discount,
  Total, Payment, Change, Complete Sale) sizes itself to its own
  content and the whole block is docked to the bottom of the cart, so
  it can never be squeezed by the product/cart grids above it.
- **Product photos added.** `Products` gained an `image_path` column
  (`Product.ImagePath`), set from `ProductEditDialog` via a "Choose
  Image..." button (`Utilities/ProductImageStore` copies the picked
  file into a local `ProductImages/` folder next to the executable).
  `ProductManagementPanel` and `ProductViewPanel` now show products as
  a scrollable grid of photo cards (`Presentation/Controls/ProductCard.cs`)
  instead of a plain data table - name, photo, price, stock badge, and
  (for Admin) Edit/Activate buttons - matching an ecommerce-style
  catalog. `POSPanel`'s product list also gained a small thumbnail
  column. Products without a photo show a simple drawn placeholder
  icon rather than a broken image.

### 4. Other UI improvements

- **A real "Dashboard" home page** (`DashboardHomePanel`) instead of a
  static welcome label - a few at-a-glance stat cards (today's revenue,
  low/out-of-stock counts, employee count for Admin; your sales today
  for Employee) and one-click "Quick Actions" buttons, all scoped to
  what the signed-in role can actually see via the same `CanXxx()`
  checks the rest of the app already uses.
- **Active sidebar item highlighting** so it's always obvious which
  screen you're on.
- **Status badges** (green "In Stock" / amber "Low Stock" / red "Out of
  Stock" / gray "Inactive") drawn as small rounded pills
  (`UiTheme.CreateBadge`) instead of plain text, used on both product
  grids.

### Running this version

Everything from Step 8 still applies (open the `.sln`, restore NuGet,
F5). One extra step **only if you already have a database created from
an earlier step**: run the new migration script to add the photo
column to your existing `Products` table:

```bash
sqlcmd -S localhost -E -i Database/Scripts/04_AddProductImage.sql
```

(A brand-new database created from `02_CreateTables.sql` already
includes this column - you don't need to run `04` in that case.)

### What I'd extend next

- **Persist the discount/payment inputs' currency formatting** - they're
  plain `NumericUpDown`s right now; a masked currency display would be
  a small but nice polish item.
- **Multiple photos per product** (currently one) - would mean a
  separate `ProductImages` table instead of a single column, plus a
  small image-carousel control on the product card.
- **Drag-and-drop image upload** in `ProductEditDialog`, instead of
  only a file-picker button.
- **Undo the "Clear Cart" button in POSPanel** - right now it silently
  discards the in-progress sale; a confirmation prompt (like the
  Activate/Deactivate actions already have) would prevent accidental
  data loss for a cashier mid-sale.
- **Keyboard shortcuts in POSPanel** (barcode-scanner-style Enter-to-add,
  F-keys for common actions) - the workflow is built around mouse
  clicks right now, and a busy cashier would benefit from not needing
  the mouse for the common path.
