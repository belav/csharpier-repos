using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class PosModel
    {
        public long posId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public decimal balance { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
        public decimal balanceAll { get; set; }
        public string boxState { get; set; }


        public string branchName { get; set; }
        public string branchCode { get; set; }
      
        public Boolean canDelete { get; set; }
   
    }
}