using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite;

[NonController]
public class SqlDataController
{
    public int TruncateAllDbRecords()
    {
        // Return no. of tables truncated
        return 7;
    }
}
