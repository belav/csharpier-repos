using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ItemSalePurModel
    {
        public long? itemId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string details { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public decimal taxes { get; set; }
        public byte isActive { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public Nullable<long> categoryId { get; set; }
        public string categoryName { get; set; }
        public Nullable<long> parentId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> minUnitId { get; set; }
        public Nullable<long> maxUnitId { get; set; }
        public int itemCount { get; set; }
        public Boolean canDelete { get; set; }
        public Nullable<long> tagId { get; set; }
        public decimal avgPurchasePrice { get; set; }

        public string notes { get; set; }
        public string categoryString { get; set; }

        // offer item
        public Nullable<decimal> desPrice { get; set; }
        public int isNew { get; set; }
        public int isOffer { get; set; }
        public string forAgent { get; set; }

        // unit item
        public decimal price { get; set; }
        public Nullable<decimal> basicPrice { get; set; }
        public decimal priceWithService { get; set; }
        public Nullable<long> unitId { get; set; }
        public string unitName { get; set; }
        public string offerName { get; set; }
        public Nullable<long> offerId { get; set; }
        public string discountType { get; set; }
        public Nullable<decimal> discountValue { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public byte isActiveOffer { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public short defaultSale { get; set; }
        public decimal priceTax { get; set; }
        public short defaultPurchase { get; set; }
        public int used { get; set; }
        public string parentName { get; set; }
        public string barcode { get; set; }
        public string minUnitName { get; set; }
        public string maxUnitName { get; set; }
        public bool isExpired { get; set; }
        public int alertDays { get; set; }
    }
}
