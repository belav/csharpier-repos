using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

public class OrderDetailsDTO
{
    [FromForm]
    public int Quantity { get; set; }

    [FromForm]
    public Product Product { get; set; }
}
