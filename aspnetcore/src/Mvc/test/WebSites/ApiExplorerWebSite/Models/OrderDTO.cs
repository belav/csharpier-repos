using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

public class OrderDTO
{
    public string CustomerId { get; set; }

    [FromHeader(Name = "Referrer")]
    public string ReferrerId { get; set; }

    public OrderDetailsDTO Details { get; set; }

    [FromForm]
    public CustomerCommentsDTO Comments { get; set; }
}
