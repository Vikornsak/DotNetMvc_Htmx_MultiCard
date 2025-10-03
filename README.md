 **.NET 9 MVC + Bootstrap 5 + htmx (Multi-Card Rows)** 

## What it does

* Lets you **add multiple rows** (as Bootstrap cards) to a form.
* Each row can be one of several **variants**: `product`, `service`, or `note`.
* You can **mix variants** in one form, **remove** any row, and **submit all rows** at once.
* The server **model-binds** everything into `List<ItemInput>` and computes line totals + a grand total.

## How it works (flow)

1. **Create page** (`GET /Items/Create`)

   * Renders the form with one initial row (a `product` card).
   * Keeps a hidden `#ix` field that tracks the **next index** for new rows.

2. **Add row** buttons

   * Buttons call `hx-get` to `/Items/NewRow?variant=product|service|note` and include `#ix` (`hx-include="#ix"`).
   * The server returns:

     * The **row partial** (`_ItemRow.cshtml`) rendered with the current index and chosen variant.
     * An **OOB** (`hx-swap-oob`) update that **increments `#ix`** to avoid duplicate indexes.

3. **Remove row**

   * Each card has a “close” button that calls `hx-get="/Items/RemoveRow"` with:

     * `hx-target="closest .item-row"`
     * `hx-swap="outerHTML"`
   * The server returns empty HTML; htmx removes that row from the DOM.

4. **Submit**

   * The form uses `hx-post="/Items/Create"` and targets `#result`.
   * The controller receives a `CreateItemsRequest` with `Items: List<ItemInput>`.
   * Totals are calculated:

     * `product`: `Qty × Price`
     * `service`: `(Hours × Rate) * (1 + Tax%)`
     * `note`: `0`
   * If it’s an htmx request, the server returns `_ResultFragment.cshtml` (a summary table + grand total); otherwise it renders the full page.

## Why model binding “just works”

Each input name follows **array-style** notation that MVC understands:

* `Items[0].Type`, `Items[0].Name`, `Items[0].Qty` …
* `Items[1].Type`, `Items[1].Desc`, `Items[1].Hours` …
  This maps cleanly to `List<ItemInput>` in the POST action.

## Key files (where to look)

* **Controllers/ItemsController.cs**

  * `Create` (GET): shows page with one row.
  * `NewRow` (GET): returns the next row partial + OOB index bump.
  * `RemoveRow` (GET): returns empty HTML for removal.
  * `Create` (POST): binds rows, computes totals, returns fragment or full page.
* **Models/**

  * `ItemInput.cs`: unified input model for all variants (product/service/note).
  * `CreateItemsRequest.cs`: wrapper with `List<ItemInput> Items`.
  * `ItemRowVm.cs`: view model for the row partial (index + variant).
  * `CreateItemsResultVm.cs`: data for the result fragment.
* **Views/Items/**

  * `Create.cshtml`: main form and “Add row” buttons.
  * `_ItemRow.cshtml`: **one partial** that switches UI by `Variant` and sets correct field names.
  * `NewRow.cshtml`: returns `_ItemRow` + the OOB hidden `#ix` update.
  * `_ResultFragment.cshtml`: summary table after submit.
* **Views/Shared/_Layout.cshtml**

  * Loads **Bootstrap 5** and **htmx** via CDN.

## Run it

```bash
cd DotNetMvc_Htmx_MultiCard
dotnet restore
dotnet run
# open the URL shown (e.g., http://localhost:5000)
```

## Customizing / extending

* **Add a new card type** (e.g., `discount`, `bundle`):

  1. Extend `ItemInput.cs` with the new fields.
  2. Add a new UI branch in `_ItemRow.cshtml` (use `Variant == "discount"`).
  3. Update the POST compute logic in `ItemsController.Create`.
  4. Add a button for `NewRow?variant=discount`.
* **Persist to DB**: Inject a DbContext and, in `Create (POST)`, save `request.Items`.
* **Validation**: You can add DataAnnotations to `ItemInput` and plug in ASP.NET Core client-side validation (jQuery Validate + unobtrusive) if you want inline validation messages.
* **CSRF/Anti-forgery**: If you enable it, include the anti-forgery token in htmx requests (e.g., via a hidden input + `hx-headers` or `hx-include`).

If you want, I can add a fourth card type or wire this up to EF Core with a sample database schema.
