using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ItemOfferModel
    {
        public long ioId { get; set; }
        public Nullable<long> iuId { get; set; }
        public Nullable<long> offerId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public int quantity { get; set; }
        public int used { get; set; }

 
       
        public string offerName { get; set; }
        public string unitName { get; set; }
        public string code { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long>  unitId { get; set; }
    
        public string itemName { get; set; }



    }
}