using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class WebDashBoardModel
    {
        public int branchId { get; set; }
        public int salesCount { get; set; }
        public int tablesCount { get; set; }
        public int diningHallCount { get; set; }
        public int takeawayCount { get; set; }
        public int selfServiceCount { get; set; }
        public int reservationsCount { get; set; }
        public int onLineUsersCount { get; set; }

        public int customersCount { get; set; }
        public decimal balance { get; set; }

    }
}