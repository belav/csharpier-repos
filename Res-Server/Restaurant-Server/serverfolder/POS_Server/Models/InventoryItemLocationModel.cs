using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class InventoryItemLocationModel
    {
        public long id { get; set; }
        public bool isDestroyed { get; set; }
        public int amount { get; set; }
        public int amountDestroyed { get; set; }
        public int realAmount { get; set; }
        public Nullable<long> itemLocationId { get; set; }
        public Nullable<long> inventoryId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<byte> isActive { get; set; }
        public string notes { get; set; }
        public string cause { get; set; }
        public bool isFalls { get; set; }
        public string fallCause { get; set; }

      
        public int sequence { get; set; }
      
        public int quantity { get; set; }  //realAmount
      
        public Boolean canDelete { get; set; }
        public string itemName { get; set; }
        public long itemId { get; set; }
        public long unitId { get; set; }
        public long itemUnitId { get; set; }
        public string location { get; set; }
        public string section { get; set; }
        public string unitName { get; set; }
        public string inventoryNum { get; set; }
        public Nullable<System.DateTime> inventoryDate { get; set; }
        public string itemType { get; set; }


        public Nullable<decimal> avgPurchasePrice { get; set; }
    }
}