using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mvc.RoutingWebSite.Controllers
{
    public class DynamicOrderController : Controller
    {
        private readonly TestResponseGenerator _generator;

        public DynamicOrderController(TestResponseGenerator generator)
        {
            _generator = generator;
        }

        [HttpGet("attribute-dynamic-order/{**slug}", Name = "AttributeRouteSlug")]
        public IActionResult Get(string slug)
        {
            return _generator.Generate(Url.RouteUrl("AttributeRouteSlug", new { slug }));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return _generator.Generate(Url.RouteUrl(null, new { controller = "DynamicOrder", action = "Index" }));
        }
    }
}
