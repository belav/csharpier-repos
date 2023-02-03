using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class InvoiceStatusModel
    {
        public long invStatusId { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }


        public Nullable<System.DateTime> date { get; set; }
        public Nullable<long> userId { get; set; }
        public string updateUserName { get; set; }

    }
}
