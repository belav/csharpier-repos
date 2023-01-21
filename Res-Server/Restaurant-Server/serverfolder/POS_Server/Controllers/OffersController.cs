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
    [RoutePrefix("api/Offers")]
    public class OffersController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
            Boolean canDelete = false;
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var offersList = entity.offers
                    .Select(L => new OfferModel
                    {
                        offerId = L.offerId,
                        name = L.name,
                        code = L.code,
                        isActive = L.isActive,
                        discountType = L.discountType,
                        discountValue = L.discountValue,
                        startDate = L.startDate,
                        endDate = L.endDate,
                        createDate = L.createDate,
                        updateDate = L.updateDate,
                        createUserId = L.createUserId,
                        updateUserId = L.updateUserId,
                        notes = L.notes,
                        forAgents = L.forAgents,
                    })
                    .ToList();

                    if (offersList.Count > 0)
                    {
                        for (int i = 0; i < offersList.Count; i++)
                        {
                            if (offersList[i].isActive == 1)
                            {
                                long offerId = (long)offersList[i].offerId;
                                var offerItems = entity.itemsOffers.Where(x => x.offerId == offerId).Select(b => new { b.offerId }).FirstOrDefault();
                               
                                if (offerItems is null) 
                                    canDelete = true;
                            }
                            offersList[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(offersList);
                }
            }
        }
        // GET api/<controller>
        [HttpPost]
        [Route("GetOfferByID")]
        public string GetOfferByID(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long offerId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        offerId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var offer = entity.offers
                   .Where(u => u.offerId == offerId)
                   .Select(L => new
                   {
                       L.offerId,
                       L.name,
                       L.code,
                       L.isActive,
                       L.discountType,
                       L.discountValue,
                       L.startDate,
                       L.endDate,
                       L.createDate,
                       L.updateDate,
                       L.createUserId,
                       L.updateUserId,
                       L.notes,
                      L.forAgents,
                   })
                   .FirstOrDefault();
                    return TokenManager.GenerateToken(offer);
                }
            }
        }
        // add or update offer
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
                string offerObject = "";
                offers newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        offerObject = c.Value.Replace("\\", string.Empty);
                        offerObject = offerObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<offers>(offerObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }

                 if (newObject.updateUserId == 0 || newObject.updateUserId == null)
                {
                    Nullable<long> id = null;
                    newObject.updateUserId = id;
                }
                if (newObject.createUserId == 0 || newObject.createUserId == null)
                {
                    Nullable<long> id = null;
                    newObject.createUserId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        offers oldObject = new offers();
                        var offerEntity = entity.Set<offers>();
                        if (newObject.offerId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            
                            newObject.updateDate = newObject.createDate;
                            oldObject = offerEntity.Add(newObject);
                            entity.SaveChanges();
                            message = oldObject.offerId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            oldObject = entity.offers.Where(p => p.offerId == newObject.offerId).FirstOrDefault();
                            oldObject.name = newObject.name;
                            oldObject.code = newObject.code;
                            oldObject.discountType = newObject.discountType;
                            oldObject.discountValue = newObject.discountValue;
                            oldObject.startDate = newObject.startDate;
                            oldObject.endDate = newObject.endDate;
                            oldObject.updateDate = newObject.updateDate;
                            oldObject.updateUserId = newObject.updateUserId;
                            oldObject.notes = newObject.notes;
                            oldObject.isActive = newObject.isActive;
                            oldObject.forAgents = newObject.forAgents;
                            entity.SaveChanges();
                            message = oldObject.offerId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
            }
            return TokenManager.GenerateToken(message);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
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
                long offerId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        offerId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "final")
                    {
                        final = bool.Parse(c.Value);
                    }
                }
                if (final)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            offers offerObj = entity.offers.Find(offerId);

                            entity.offers.Remove(offerObj);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
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
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            offers offerObj = entity.offers.Find(offerId);

                            offerObj.isActive = 0;
                            offerObj.updateUserId = userId;
                            offerObj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
            }
        }


        [HttpPost]
        [Route("GetOffersByMembershipId")]
        public string GetOffersByMembershipId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                long membershipId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (from S in entity.membershipsOffers
                                join B in entity.offers on S.offerId equals B.offerId into JB
                                join U in entity.memberships on S.membershipId equals U.membershipId into JU
                                from JBB in JB.DefaultIfEmpty()
                                from JUU in JU.DefaultIfEmpty()
                                where S.membershipId == membershipId
                                select new OfferModel()
                                {
                                    offerId = JBB.offerId,
                                    name = JBB.name,
                                    code = JBB.code,
                                    isActive = JBB.isActive,
                                    discountType = JBB.discountType,
                                    discountValue = JBB.discountValue,
                                    startDate = JBB.startDate,
                                    endDate = JBB.endDate,
                                    createDate = JBB.createDate,
                                    updateDate = JBB.updateDate,
                                    createUserId = JBB.createUserId,
                                    updateUserId = JBB.updateUserId,
                                    notes = JBB.notes,

                                    membershipOfferId = S.membershipOfferId,

                                    membershipId = S.membershipId,
                                    forAgents = JBB.forAgents,

                                }).ToList();
                    return TokenManager.GenerateToken(List);


                }
            }
        }

    }
}