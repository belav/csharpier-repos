using Microsoft.AspNetCore.Mvc.Filters;

namespace BasicWebSite;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class VndErrorAttribute : Attribute, IFilterMetadata { }
