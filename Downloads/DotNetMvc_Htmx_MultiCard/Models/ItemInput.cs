namespace DotNetMvc_Htmx_MultiCard.Models;

public class ItemInput
{
    public string? Type { get; set; } // product | service | note

    // product
    public string? Name { get; set; }
    public int? Qty { get; set; }
    public decimal? Price { get; set; }
    public string? Sku { get; set; }
    public string? Category { get; set; }

    // service
    public string? Desc { get; set; }
    public decimal? Hours { get; set; }
    public decimal? Rate { get; set; }
    public decimal? Tax { get; set; } // percent

    // note
    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Content { get; set; }

    // computed (for display)
    public decimal LineTotal { get; set; }
}
