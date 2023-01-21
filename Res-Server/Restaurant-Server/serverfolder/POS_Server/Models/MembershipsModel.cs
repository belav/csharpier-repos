using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class MembershipsModel
    {
        public long membershipId { get; set; }
        public string name { get; set; }
       
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }
        public string subscriptionType { get; set; }
        public string code { get; set; }
        public bool isFreeDelivery { get; set; }
        public decimal deliveryDiscountPercent { get; set; }

        public Nullable<decimal> subscriptionFee { get; set; }
        public bool canDelete { get; set; }

        public int customersCount { get; set; }
        public int couponsCount { get; set; }
        public int offersCount { get; set; }
        public int invoicesClassesCount { get; set; }



    }
}