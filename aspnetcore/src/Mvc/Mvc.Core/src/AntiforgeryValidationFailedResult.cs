using Microsoft.AspNetCore.Mvc.Core.Infrastructure;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// A <see cref="BadRequestResult"/> used for antiforgery validation
/// failures. Use <see cref="IAntiforgeryValidationFailedResult"/> to
/// match for validation failures inside MVC result filters.
/// </summary>
public class AntiforgeryValidationFailedResult
    : BadRequestResult,
        IAntiforgeryValidationFailedResult { }
