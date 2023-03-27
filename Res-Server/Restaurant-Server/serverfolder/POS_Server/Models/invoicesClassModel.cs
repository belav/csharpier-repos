using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class invoicesClassModel
    {
        public long invClassId { get; set; }
        public decimal minInvoiceValue { get; set; }
        public decimal maxInvoiceValue { get; set; }
        public decimal discountValue { get; set; }
        public byte discountType { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> createUserId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
     
        public bool canDelete { get; set; }

        public long invClassMemberId { get; set; }
        public Nullable<long> membershipId { get; set; }
        public string name { get; set; }
        public long invClassDiscountId { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public Nullable<decimal> finalDiscount { get; set; }

    }
}