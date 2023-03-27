using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ItemsExtraModel
    {
        public long itemExtraId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> extraId { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
 
        // item parent
    
        public string itemcode { get; set; }
        public string  itemName { get; set; }
      
        public string itemType { get; set; }
        public string itemImage { get; set; }



        //Extra  
        public string extraCode { get; set; }
        public string extraName { get; set; }

        public string extraType { get; set; }
        public string extraImage { get; set; }



    }
}