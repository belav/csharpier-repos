using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/couponsInvoices")]
    public class couponsInvoicesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all couponsInvoices
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var couponsInvoicesList = (
                        from c in entity.couponsInvoices
                        where c.InvoiceId == invoiceId
                        join b in entity.coupons on c.couponId equals b.cId into lj
                        from x in lj.DefaultIfEmpty()
                        select new CouponInvoiceModel()
                        {
                            id = c.id,
                            couponId = c.couponId,
                            InvoiceId = c.InvoiceId,
                            createDate = c.createDate,
                            updateDate = c.updateDate,
                            createUserId = c.createUserId,
                            updateUserId = c.updateUserId,
                            discountValue = c.discountValue,
                            discountType = c.discountType,
                            couponCode = x.code,
                            name = x.name,
                            forAgents = x.forAgents,
                        }
                    ).ToList();
                    return TokenManager.GenerateToken(couponsInvoicesList);
                }
            }
        }

        [HttpPost]
        [Route("GetOriginal")]
        public string GetOriginal(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var couponsInvoicesList = (
                        from c in entity.couponsInvoices
                        where c.InvoiceId == invoiceId
                        join b in entity.coupons on c.couponId equals b.cId into lj
                        from x in lj.DefaultIfEmpty()
                        where x.startDate <= datenow && x.endDate >= datenow && x.remainQ > 0
                        select new CouponInvoiceModel()
                        {
                            id = c.id,
                            couponId = c.couponId,
                            InvoiceId = c.InvoiceId,
                            createDate = c.createDate,
                            updateDate = c.updateDate,
                            createUserId = c.createUserId,
                            updateUserId = c.updateUserId,
                            discountValue = x.discountValue,
                            discountType = x.discountType,
                            couponCode = x.code,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(couponsInvoicesList);
                }
            }
        }

        // GET api/<controller>  Get couponsInvoices By ID
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long id = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        id = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var couponsInvoices = entity.couponsInvoices
                        .Where(c => c.id == id)
                        .Select(
                            c =>
                                new
                                {
                                    c.id,
                                    c.couponId,
                                    c.InvoiceId,
                                    c.createDate,
                                    c.updateDate,
                                    c.createUserId,
                                    c.updateUserId
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(couponsInvoices);
                }
            }
        }

        // add or update couponsInvoices
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string couponsInvoicesObject = "";
                long invoiceId = 0;
                string invType = "";
                List<couponsInvoices> Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        couponsInvoicesObject = c.Value.Replace("\\", string.Empty);
                        couponsInvoicesObject = couponsInvoicesObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<List<couponsInvoices>>(
                            couponsInvoicesObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        //break;
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    if (invType == "sd" || invType == "qd")
                    {
                        var oldList = entity.couponsInvoices.Where(p => p.InvoiceId == invoiceId);
                        if (oldList.Count() > 0)
                        {
                            entity.couponsInvoices.RemoveRange(oldList);
                        }
                        if (Object.Count() > 0)
                        {
                            foreach (couponsInvoices coupon in Object)
                            {
                                coupon.InvoiceId = invoiceId;
                                if (coupon.createDate == null)
                                {
                                    coupon.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    coupon.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    coupon.updateUserId = coupon.createUserId;
                                }
                                else
                                {
                                    coupon.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                }
                            }
                            entity.couponsInvoices.AddRange(Object);
                        }
                        entity.SaveChanges();
                    }
                    else
                    {
                        var oldList = entity.couponsInvoices
                            .Where(x => x.InvoiceId == invoiceId)
                            .Select(x => new { x.couponId, x.id })
                            .ToList();
                        for (int i = 0; i < oldList.Count; i++) // loop to remove not exist coupon
                        {
                            int exist = 0;
                            coupons c = entity.coupons.Find(oldList[i].couponId);

                            int couponId = (int)oldList[i].couponId;
                            var tci = entity.couponsInvoices
                                .Where(x => x.couponId == couponId && x.InvoiceId == invoiceId)
                                .FirstOrDefault();
                            couponsInvoices ci = entity.couponsInvoices.Find(tci.id);

                            if (Object != null && Object.Count > 0)
                            {
                                var isExist = Object.Find(x => x.couponId == oldList[i].couponId);
                                if (isExist == null)
                                    exist = 0;
                                else
                                    exist = 1;
                            }
                            //return exist;
                            if (exist == 0) // remove coupon from invoice
                            {
                                c.remainQ++;
                                entity.couponsInvoices.Remove(ci);
                            }
                            else // edit previously added coupons
                            {
                                ci.discountType = c.discountType;
                                ci.discountValue = c.discountValue;
                                ci.forAgents = c.forAgents;
                                ci.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            }
                            entity.SaveChanges();
                        }
                        foreach (couponsInvoices coupon in Object) // loop to add new coupons
                        {
                            Boolean isInList = false;
                            if (oldList != null)
                            {
                                var old = oldList.ToList().Find(x => x.couponId == coupon.couponId);
                                if (old != null)
                                    isInList = true;
                            }
                            if (!isInList)
                            {
                                if (coupon.updateUserId == 0 || coupon.updateUserId == null)
                                {
                                    Nullable<long> id = null;
                                    coupon.updateUserId = id;
                                }
                                if (coupon.createUserId == 0 || coupon.createUserId == null)
                                {
                                    Nullable<long> id = null;
                                    coupon.createUserId = id;
                                }
                                coupon.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                coupon.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                coupon.updateUserId = coupon.createUserId;

                                entity.couponsInvoices.Add(coupon);
                                entity.SaveChanges();
                                coupons c = entity.coupons.Find(coupon.couponId);
                                c.remainQ--;
                                entity.SaveChanges();
                            }
                        }
                        try
                        {
                            entity.SaveChanges();
                        }
                        catch
                        {
                            message = "0";
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                message = "1";
                return TokenManager.GenerateToken(message);
            }
        }
    }
}
