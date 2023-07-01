using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CorsWebSite;

[Route("Cors/[action]")]
[EnableCors("AllowAnySimpleRequest")]
public class BlogController : Controller
{
    public IEnumerable<string> GetBlogComments(int id)
    {
        return new[] { "comment1", "comment2", "comment3" };
    }

    [EnableCors("AllowSpecificOrigin")]
    public IEnumerable<string> GetUserComments(int id)
    {
        return new[] { "usercomment1", "usercomment2", "usercomment3" };
    }

    [DisableCors]
    [AcceptVerbs("HEAD", "GET", "POST")]
    public string GetExclusiveContent()
    {
        return "exclusive";
    }

    [EnableCors("WithCredentialsAndOtherSettings")]
    public string EditUserComment(int id, string userComment)
    {
        return userComment;
    }
}
