using Microsoft.AspNetCore.Mvc;
using DotNetMvc_Htmx_MultiCard.Models;

namespace DotNetMvc_Htmx_MultiCard.Controllers;

public class ItemsController : Controller
{
    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateItemsRequest
        {
            Items = new List<ItemInput> { new ItemInput { Type = "product" } }
        };
        ViewBag.NextIndex = 1;
        return View(model);
    }

    [HttpGet]
    public IActionResult NewRow(int ix = 0, string variant = "product")
    {
        var vm = new ItemRowVm { Index = ix, Variant = variant };
        return View(vm);
    }

    [HttpGet]
    public IActionResult RemoveRow() => Content(string.Empty, "text/html");

    [HttpPost]
    public IActionResult Create(CreateItemsRequest request)
    {
        decimal grand = 0m;
        foreach (var it in request.Items)
        {
            if (string.IsNullOrWhiteSpace(it?.Type)) continue;
            switch (it.Type)
            {
                case "product":
                    var qty = it.Qty ?? 0;
                    var price = it.Price ?? 0m;
                    it.LineTotal = qty * price;
                    grand += it.LineTotal;
                    break;
                case "service":
                    var hours = it.Hours ?? 0m;
                    var rate = it.Rate ?? 0m;
                    var baseLine = hours * rate;
                    var tax = it.Tax ?? 0m;
                    it.LineTotal = baseLine * (1 + tax / 100m);
                    grand += it.LineTotal;
                    break;
                default:
                    it.LineTotal = 0m;
                    break;
            }
        }

        if (Request.Headers.TryGetValue("HX-Request", out var hx) && hx.ToString() == "true")
        {
            var vm = new CreateItemsResultVm { Items = request.Items, Total = grand };
            return PartialView("_ResultFragment", vm);
        }

        ViewBag.Total = grand;
        return View(request);
    }
}
