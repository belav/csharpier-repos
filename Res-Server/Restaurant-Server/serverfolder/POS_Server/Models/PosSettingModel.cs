using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class PosSettingModel
    {
        public long  posSettingId { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<long> saleInvPrinterId { get; set; }
        public Nullable<long> reportPrinterId { get; set; }
        public Nullable<long> saleInvPapersizeId { get; set; }
        public string posSerial { get; set; }
        public Nullable<long> docPapersizeId { get; set; }
        public string posDeviceCode { get; set; }
        public Nullable<long> posSerialId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }

       

        public Nullable<long> repprinterId { get; set; }
        public string repname { get; set; }
        public string repprintFor { get; set; }

        public Nullable<long> salprinterId { get; set; }
        public string salname { get; set; }
        public string salprintFor { get; set; }

        public Nullable<long> sizeId { get; set; }
        public string paperSize1 { get; set; }

        public string docPapersize { get; set; }
        public string saleSizeValue { get; set; }
        public string docSizeValue { get; set; }

        public Nullable<long> kitchenPrinterId { get; set; }
        public Nullable<long> kitchenPapersizeId { get; set; }
        public string kitchenPrinter { get; set; }
        public string kitchenPapersize { get; set; }
        public string kitchenprintFor { get; set; }
        public string kitchenSizeValue { get; set; }

    }
}