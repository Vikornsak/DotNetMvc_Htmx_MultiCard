using System.Collections.Generic;

namespace DotNetMvc_Htmx_MultiCard.Models;

public class CreateItemsRequest
{
    public List<ItemInput> Items { get; set; } = new();
}
