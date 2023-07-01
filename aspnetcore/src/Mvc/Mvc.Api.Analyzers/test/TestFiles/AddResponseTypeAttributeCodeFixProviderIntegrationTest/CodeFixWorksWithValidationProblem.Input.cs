using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Api.Analyzers._INPUT_
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CodeFixWorksWithValidationProblem : ControllerBase
    {
        public IActionResult GetProblem()
        {
            return ValidationProblem();
        }
    }
}
