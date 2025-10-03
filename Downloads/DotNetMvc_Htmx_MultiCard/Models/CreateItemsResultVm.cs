using System.Collections.Generic;

namespace DotNetMvc_Htmx_MultiCard.Models;

public class CreateItemsResultVm
{
    public List<ItemInput> Items { get; set; } = new();
    public decimal Total { get; set; }
}
