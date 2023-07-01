using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

public class CustomerCommentsDTO
{
    [FromQuery]
    public string ShippingInstructions { get; set; }

    public string Feedback { get; set; }
}
