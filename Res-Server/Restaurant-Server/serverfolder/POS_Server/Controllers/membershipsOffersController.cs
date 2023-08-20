using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/membershipsOffers")]
    public class membershipsOffersController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List1 = entity.membershipsOffers.ToList();
                    var List = List1
                        .Select(
                            S =>
                                new membershipsOffers
                                {
                                    membershipOfferId = S.membershipOfferId,
                                    membershipId = S.membershipId,
                                    offerId = S.offerId,
                                    notes = S.notes,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                }
                        )
                        .ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        /*
       public long membershipOfferId { get; set; }
        public Nullable<long> membershipId { get; set; }
        public Nullable<int> offerId { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
         * */
        [HttpPost]
        [Route("GetById")]
        public string GetById(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long membershipOfferId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        membershipOfferId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.membershipsOffers
                        .Where(S => S.membershipOfferId == membershipOfferId)
                        .Select(
                            S =>
                                new
                                {
                                    S.membershipOfferId,
                                    S.membershipId,
                                    S.offerId,
                                    S.notes,
                                    S.createDate,
                                    S.updateDate,
                                    S.createUserId,
                                    S.updateUserId,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(bank);
                }
            }
        }

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
                string membershipOfferId = "";
                membershipsOffers newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        membershipOfferId = c.Value.Replace("\\", string.Empty);
                        membershipOfferId = membershipOfferId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<membershipsOffers>(
                            membershipOfferId,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
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
                if (newObject.membershipId == 0 || newObject.membershipId == null)
                {
                    Nullable<long> id = null;
                    newObject.membershipId = id;
                }
                if (newObject.offerId == 0 || newObject.offerId == null)
                {
                    Nullable<long> id = null;
                    newObject.offerId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        membershipsOffers tmpObject = new membershipsOffers();
                        var bankEntity = entity.Set<membershipsOffers>();
                        if (newObject.membershipOfferId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.membershipOfferId.ToString();
                            ;
                        }
                        else
                        {
                            tmpObject = entity.membershipsOffers
                                .Where(p => p.membershipOfferId == newObject.membershipOfferId)
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.membershipOfferId = newObject.membershipOfferId;
                            tmpObject.membershipId = newObject.membershipId;
                            tmpObject.offerId = newObject.offerId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.createDate = newObject.createDate;

                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;

                            entity.SaveChanges();
                            message = tmpObject.membershipOfferId.ToString();
                        }
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
                long membershipOfferId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        membershipOfferId = long.Parse(c.Value);
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
                            membershipsOffers objDelete = entity.membershipsOffers.Find(
                                membershipOfferId
                            );
                            entity.membershipsOffers.Remove(objDelete);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            membershipsOffers objDelete = entity.membershipsOffers.Find(
                                membershipOfferId
                            );

                            objDelete.updateUserId = userId;
                            objDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
            }
        }

        //update branches list by userId
        [HttpPost]
        [Route("UpdateOffersByMembershipId")]
        public string UpdateOffersByMembershipId(string token)
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
                string strObject = "";
                List<membershipsOffers> newListObj = null;
                long membershipId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<membershipsOffers>>(
                            strObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "membershipId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                    else if (c.Type == "updateUserId")
                    {
                        updateUserId = long.Parse(c.Value);
                    }
                }

                List<membershipsOffers> items = null;
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity.membershipsOffers
                        .Where(x => x.membershipId == membershipId)
                        .ToList();
                    if (items != null)
                    {
                        entity.membershipsOffers.RemoveRange(items);
                        try
                        {
                            entity.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            message = "-2";
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        for (int i = 0; i < newListObj.Count; i++)
                        {
                            if (
                                newListObj[i].updateUserId == 0
                                || newListObj[i].updateUserId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].updateUserId = id;
                            }
                            if (
                                newListObj[i].createUserId == 0
                                || newListObj[i].createUserId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].createUserId = id;
                            }
                            if (
                                newListObj[i].membershipId == 0
                                || newListObj[i].membershipId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].membershipId = id;
                            }
                            if (newListObj[i].offerId == 0 || newListObj[i].offerId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].offerId = id;
                            }
                            var branchEntity = entity.Set<membershipsOffers>();

                            newListObj[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newListObj[i].updateDate = newListObj[i].createDate;
                            newListObj[i].updateUserId = updateUserId;
                            newListObj[i].membershipId = membershipId;
                            branchEntity.Add(newListObj[i]);
                            entity.SaveChanges();
                        }

                        entity.SaveChanges();
                    }

                    message = "1";
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }
    }
}
