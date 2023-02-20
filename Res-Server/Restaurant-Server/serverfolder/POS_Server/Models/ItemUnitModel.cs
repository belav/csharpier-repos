using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ItemUnitModel
    {
        public long itemUnitId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> unitId { get; set; }
        public Nullable<int> unitValue { get; set; }
        public short defaultSale { get; set; }
        public short defaultPurchase { get; set; }
        public decimal price { get; set; }
        public decimal priceWithService { get; set; }
        public string barcode { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> subUnitId { get; set; }
        public decimal purchasePrice { get; set; }
        public Nullable<long> storageCostId { get; set; }
        public byte isActive { get; set; }

        public string mainUnit { get; set; }
        public string smallUnit { get; set; }
        public string countSmallUnit { get; set; }

        public string itemName { get; set; }
        public string itemCode { get; set; }
        public string unitName { get; set; }
        public string type { get; set; }
        public Nullable<long> categoryId { get; set; }
        public Boolean canDelete { get; set; }

        public Nullable<decimal> taxes { get; set; }
        public Nullable<decimal> cost { get; set; }
    }
}
