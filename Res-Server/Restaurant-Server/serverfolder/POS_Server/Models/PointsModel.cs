using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class PointsModel
    {
        public long pointId { get; set; }
        public decimal Cash { get; set; }
        public int CashPoints { get; set; }
        public int invoiceCount { get; set; }
        public int invoiceCountPoints { get; set; }
        public decimal CashArchive { get; set; }
        public int CashPointsArchive { get; set; }
        public int invoiceCountArchive { get; set; }
        public int invoiceCountPoinstArchive { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }
        public Nullable<long> agentId { get; set; }

     
       
        public bool canDelete { get; set; }
    }
}