using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class MenuSettingModel
    {
        #region basic attributes
        public Nullable<long> menuSettingId { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<bool> sat { get; set; }
        public Nullable<bool> sun { get; set; }
        public Nullable<bool> mon { get; set; }
        public Nullable<bool> tues { get; set; }
        public Nullable<bool> wed { get; set; }
        public Nullable<bool> thur { get; set; }
        public Nullable<bool> fri { get; set; }
        public Nullable<decimal> preparingTime { get; set; }
        public Nullable<byte> isActive { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> createUserId { get; set; }
        public bool isExpired { get; set; }
        public int alertDays { get; set; }
     
        #endregion

        #region item attribute
        public Nullable<long> itemId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string details { get; set; }
        public Nullable<long> categoryId { get; set; }
        public Nullable<long> tagId { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public decimal price { get; set; }
        public decimal priceWithService { get; set; }
        public Nullable<int> isNew { get; set; }
        #endregion
    }
}