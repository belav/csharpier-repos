using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class BankModel
    {
        public long bankId { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string accNumber { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }

       
        public Boolean canDelete { get; set; }
    }
}