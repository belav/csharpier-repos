using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class TablesStatisticsModel
    {
        public long branchId { get; set; }
        public string branchName { get; set; }
        public int openedCount { get; set; }
        public int emptyCount { get; set; }
        public int reservedCount { get; set; } // count for reserved but not opened tables


    }
}