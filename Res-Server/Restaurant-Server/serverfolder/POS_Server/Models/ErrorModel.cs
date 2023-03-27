using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ErrorModel
    {
        public long errorId { get; set; }
        public string num { get; set; }
        public string msg { get; set; }
        public string stackTrace { get; set; }
        public string targetSite { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<long> createUserId { get; set; }

        public string source { get; set; }
        public string method { get; set; }

    }
}