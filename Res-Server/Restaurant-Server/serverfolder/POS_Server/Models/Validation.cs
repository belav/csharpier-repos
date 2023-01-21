using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Controllers
{
    public class Validation
    {
        public string APIKey = "1234";
        public bool CheckApiKey(string apiKey)
        {
            if (apiKey.Equals(APIKey))
                return true;
            else
                return false;
        }
    }
}