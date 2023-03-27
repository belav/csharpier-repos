using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class itemsTransferIngredientsModel
    {
        public long itemsTransIngredId { get; set; }
        public Nullable<long> itemsTransId { get; set; }
        public Nullable<long> dishIngredId { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
        public string itemName { get; set; }
        public string DishIngredientName { get; set; }
        public bool isBasic { get; set; }
    }
}