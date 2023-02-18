using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using POS_Server.Models;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;
using Newtonsoft.Json.Converters;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/ItemsOffers")]
    public class ItemsOffersController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        int newdays = -15;

        //[HttpPost]
        //[Route("Getall")]
        //public IHttpActionResult Getall()
        //{



        //    var re = Request;
        //    var headers = re.Headers;
        //    string token = "";
        //    if (headers.Contains("APIKey"))
        //    {
        //        token = headers.GetValues("APIKey").First();
        //    }
        //    Validation validation = new Validation();
        //    bool valid = validation.CheckApiKey(token);

        //    if (valid) // APIKey is valid
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var ioList = entity.itemsOffers

        //           .Select(c => new ItemOfferModel() {
        //            iuId=   c.iuId,
        //               offerId= c.offerId,

        //               ioId= c.ioId,
        //               createUserId= c.createUserId,
        //               updateUserId=  c.updateUserId,
        //               createDate=   c.createDate,
        //               updateDate=   c.updateDate,


        //           })
        //           .ToList();

        //            if (ioList == null)
        //                return NotFound();
        //            else
        //                return Ok(ioList);
        //        }
        //    }
        //    //else
        //    return NotFound();
        //}


        #region
        [HttpPost]
        [Route("UpdateItemsByOfferId")]
        public string UpdateItemsByOfferId(string token)
        {
            //string  newitoflist


            //string itemLocationObject
            string message = "";
            long offerId = 0;
            long userId = 0;

            token = TokenManager.readToken(HttpContext.Current.Request);
            if (TokenManager.GetPrincipal(token) == null) //invalid authorization
            {
                return TokenManager.GenerateToken("-7");
            }
            else
            {
                int res = 0;
                string Object = "";
                List<itemsOffers> newObject = new List<itemsOffers>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsOffers>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "offerId")
                    {
                        offerId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                if (newObject != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var iuoffer = entity.itemsOffers.Where(p => p.offerId == offerId);
                            if (iuoffer.Count() > 0)
                            {
                                entity.itemsOffers.RemoveRange(iuoffer);
                            }
                            if (newObject.Count() > 0)
                            {
                                foreach (itemsOffers newitofrow in newObject)
                                {
                                    newitofrow.offerId = offerId;

                                    if (
                                        newitofrow.createUserId == null
                                        || newitofrow.createUserId == 0
                                    )
                                    {
                                        newitofrow.createDate = coctrlr.AddOffsetTodate(
                                            DateTime.Now
                                        );
                                        newitofrow.updateDate = coctrlr.AddOffsetTodate(
                                            DateTime.Now
                                        );

                                        newitofrow.createUserId = userId;
                                        newitofrow.updateUserId = userId;
                                    }
                                    else
                                    {
                                        newitofrow.updateDate = coctrlr.AddOffsetTodate(
                                            DateTime.Now
                                        );
                                        newitofrow.updateUserId = userId;
                                    }
                                    newitofrow.used = 0;
                                }
                                entity.itemsOffers.AddRange(newObject);
                            }
                            res = entity.SaveChanges();

                            // return res;
                            return TokenManager.GenerateToken(res.ToString());
                        }
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //long userId =0;
            //long offerId = 0;
            //var re = Request;
            //var headers = re.Headers;
            //int res = 0;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //if (headers.Contains("offerId"))
            //{
            //    offerId = Convert.ToInt32(headers.GetValues("offerId").First());
            //}
            //if (headers.Contains("userId"))
            //{
            //    userId = Convert.ToInt32(headers.GetValues("userId").First());
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);
            //newitoflist = newitoflist.Replace("\\", string.Empty);
            //newitoflist = newitoflist.Trim('"');
            //List<itemsOffers> newitofObj = JsonConvert.DeserializeObject<List<itemsOffers>>(newitoflist, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var iuoffer= entity.itemsOffers.Where(p => p.offerId == offerId);
            //     if(iuoffer.Count()> 0)
            //        {
            //        entity.itemsOffers.RemoveRange(iuoffer);
            //        }
            //        if (newitofObj.Count() > 0)
            //        {
            //            foreach (itemsOffers newitofrow in newitofObj)
            //            {
            //                newitofrow.offerId = offerId;

            //                if (newitofrow.createUserId == null || newitofrow.createUserId ==0 )
            //                {
            //                newitofrow.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newitofrow.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

            //                    newitofrow.createUserId = userId;
            //                    newitofrow.updateUserId =userId;
            //                }
            //                else
            //                {
            //                    newitofrow.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                    newitofrow.updateUserId = userId;

            //                }

            //            }
            //            entity.itemsOffers.AddRange(newitofObj);
            //        }
            //      res  =entity.SaveChanges();

            //            return res;

            //    }

            //}
            //else
            //{
            //    return -1;
            //}
        }
        #endregion


        #region
        [HttpPost]
        [Route("GetItemsByOfferId")]
        public string GetItemsByOfferId(string token)
        {
            //string itemLocationObject
            string message = "";
            long offerId = 0;

            token = TokenManager.readToken(HttpContext.Current.Request);
            if (TokenManager.GetPrincipal(token) == null) //invalid authorization
            {
                return TokenManager.GenerateToken("-7");
            }
            else
            {
                int res = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "offerId")
                    {
                        offerId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var iuoffer = (
                            from itofr in entity.itemsOffers
                            join itunit in entity.itemsUnits on itofr.iuId equals itunit.itemUnitId
                            join item in entity.items on itunit.itemId equals item.itemId
                            join ofr in entity.offers on itofr.offerId equals ofr.offerId
                            join un in entity.units on itunit.unitId equals un.unitId
                            select new ItemOfferModel()
                            {
                                offerId = itofr.offerId,
                                offerName = ofr.name,
                                unitId = un.unitId,
                                unitName = un.name,
                                itemId = item.itemId,
                                itemName = item.name,
                                iuId = itunit.itemUnitId,
                                quantity = itofr.quantity,
                                createDate = itofr.createDate,
                                updateDate = itofr.updateDate,
                                createUserId = itofr.createUserId,
                                updateUserId = itofr.updateUserId,

                                //code = item.code,
                            }
                        ).Where(p => p.offerId == offerId).ToList();

                        return TokenManager.GenerateToken(iuoffer);
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
            //        long offerId = 0;
            //        var re = Request;
            //        var headers = re.Headers;
            //        string token = "";
            //        if (headers.Contains("APIKey"))
            //        {
            //            token = headers.GetValues("APIKey").First();
            //        }
            //        if (headers.Contains("offerId"))
            //        {
            //            offerId = Convert.ToInt32(headers.GetValues("offerId").First());
            //        }
            //        Validation validation = new Validation();
            //        bool valid = validation.CheckApiKey(token);

            //        if (valid)
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                var iuoffer = (from itofr in entity.itemsOffers
            //                               join itunit in entity.itemsUnits on itofr.iuId equals itunit.itemUnitId
            //                               join item in entity.items on itunit.itemId equals item.itemId
            //                               join ofr in entity.offers on itofr.offerId equals ofr.offerId
            //                               join un in entity.units on itunit.unitId equals un.unitId
            //                               select new ItemOfferModel()
            //                               {
            //                                   offerId = itofr.offerId,
            //                                   offerName = ofr.name,

            //                                   unitId=un.unitId,
            //                                   unitName = un.name,
            //                                   itemId = item.itemId,
            //                                   itemName = item.name,
            //                                   iuId = itunit.itemUnitId,
            //                                   quantity= itofr.quantity,
            //                                   createDate = itofr.createDate,
            //                                   updateDate= itofr.updateDate,
            //                                   createUserId= itofr.createUserId,
            //                                   updateUserId= itofr.updateUserId,

            //    //code = item.code,
            //}).Where(p => p.offerId == offerId).ToList();


            //                if (iuoffer == null)
            //                    return NotFound();
            //                else
            //                    return Ok(iuoffer);


            //            }

            //        }
            //        else
            //        {
            //            return NotFound();
            //        }
        }
        #endregion
        #region
        [HttpPost]
        [Route("getRemain")]
        public string getRemain(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            if (TokenManager.GetPrincipal(token) == null) //invalid authorization
            {
                return TokenManager.GenerateToken("-7");
            }
            else
            {
                long offerId = 0;
                long itemUnitId = 0;
                int remain = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "offerId")
                    {
                        offerId = long.Parse(c.Value);
                    }
                    else if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var iuoffer = (
                            from itofr in entity.itemsOffers
                            where itofr.offerId == (long)offerId && itofr.iuId == (long)itemUnitId
                            select new ItemOfferModel()
                            {
                                offerId = itofr.offerId,
                                quantity = itofr.quantity,
                                used = itofr.used,
                                createDate = itofr.createDate,
                                updateDate = itofr.updateDate,
                                createUserId = itofr.createUserId,
                                updateUserId = itofr.updateUserId,
                            }
                        ).FirstOrDefault();

                        if (iuoffer != null)
                        {
                            if (iuoffer.used == null)
                                iuoffer.used = 0;
                            remain = (int)iuoffer.quantity - (int)iuoffer.used;
                        }

                        return TokenManager.GenerateToken(remain);
                    }
                }
                catch
                {
                    message = "10";
                    return TokenManager.GenerateToken(message);
                }
            }
        }
        #endregion

        public int getRemain(long offerId, long itemUnitId)
        {
            int remain = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var iuoffer = (
                    from itofr in entity.itemsOffers
                    where itofr.offerId == (long)offerId && itofr.iuId == (long)itemUnitId
                    select new ItemOfferModel()
                    {
                        offerId = itofr.offerId,
                        quantity = itofr.quantity,
                        used = itofr.used,
                        createDate = itofr.createDate,
                        updateDate = itofr.updateDate,
                        createUserId = itofr.createUserId,
                        updateUserId = itofr.updateUserId,
                    }
                ).FirstOrDefault();

                if (iuoffer != null)
                {
                    if (iuoffer.used == null)
                        iuoffer.used = 0;
                    remain = (int)iuoffer.quantity - (int)iuoffer.used;
                }
            }
            return remain;
        }
    }
}
