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
    [RoutePrefix("api/subscriptionFees")]
    public class subscriptionFeesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
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
                    var List1 = entity.subscriptionFees.ToList();
                    var List = List1
                        .Select(
                            S =>
                                new subscriptionFeesModel
                                {
                                    subscriptionFeesId = S.subscriptionFeesId,
                                    membershipId = S.membershipId,
                                    monthsCount = S.monthsCount,
                                    Amount = S.Amount,
                                    notes = S.notes,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    isActive = S.isActive,
                                }
                        )
                        .ToList();

                    if (List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            canDelete = false;
                            if (List[i].isActive == 1)
                            {
                                long subscriptionFeesId = (long)List[i].subscriptionFeesId;
                                var items5 = entity
                                    .agentMembershipCash
                                    .Where(x => x.subscriptionFeesId == subscriptionFeesId)
                                    .Select(b => new { b.agentMembershipCashId })
                                    .FirstOrDefault();

                                if (items5 is null)
                                    canDelete = true;
                            }
                            List[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(List);
                }
            }
        }

        /*
   public int subscriptionFeesId { get; set; }
        public Nullable<long> subscriptionFeesId { get; set; }
        public int monthsCount { get; set; }
        public decimal Amount { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }
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
                long subscriptionFeesId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        subscriptionFeesId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity
                        .subscriptionFees
                        .Where(S => S.subscriptionFeesId == subscriptionFeesId)
                        .Select(
                            S =>
                                new
                                {
                                    S.subscriptionFeesId,
                                    S.membershipId,
                                    S.monthsCount,
                                    S.Amount,
                                    S.notes,
                                    S.createDate,
                                    S.updateDate,
                                    S.createUserId,
                                    S.updateUserId,
                                    S.isActive,
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
                string subscriptionFeesId = "";
                subscriptionFees newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        subscriptionFeesId = c.Value.Replace("\\", string.Empty);
                        subscriptionFeesId = subscriptionFeesId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<subscriptionFees>(
                            subscriptionFeesId,
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        subscriptionFees tmpObject = new subscriptionFees();
                        var bankEntity = entity.Set<subscriptionFees>();
                        if (newObject.subscriptionFeesId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.subscriptionFeesId.ToString();
                            ;
                        }
                        else
                        {
                            tmpObject = entity
                                .subscriptionFees
                                .Where(p => p.subscriptionFeesId == newObject.subscriptionFeesId)
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.subscriptionFeesId = newObject.subscriptionFeesId;
                            tmpObject.membershipId = newObject.membershipId;
                            tmpObject.monthsCount = newObject.monthsCount;
                            tmpObject.Amount = newObject.Amount;
                            tmpObject.notes = newObject.notes;
                            //  tmpObject.createDate = newObject.createDate;

                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;
                            tmpObject.isActive = newObject.isActive;

                            entity.SaveChanges();
                            message = tmpObject.subscriptionFeesId.ToString();
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
                long subscriptionFeesId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        subscriptionFeesId = long.Parse(c.Value);
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
                            subscriptionFees objDelete = entity
                                .subscriptionFees
                                .Find(subscriptionFeesId);
                            entity.subscriptionFees.Remove(objDelete);
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
                            subscriptionFees objDelete = entity
                                .subscriptionFees
                                .Find(subscriptionFeesId);
                            objDelete.isActive = 0;
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

        public long Save(subscriptionFees newObject)
        {
            long message = 0;

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
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    subscriptionFees tmpObject = new subscriptionFees();
                    var bankEntity = entity.Set<subscriptionFees>();
                    if (newObject.subscriptionFeesId == 0)
                    {
                        newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateUserId = newObject.createUserId;
                        tmpObject = bankEntity.Add(newObject);
                        entity.SaveChanges();
                        message = tmpObject.subscriptionFeesId;
                    }
                    else
                    {
                        tmpObject = entity
                            .subscriptionFees
                            .Where(p => p.subscriptionFeesId == newObject.subscriptionFeesId)
                            .FirstOrDefault();

                        tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                        tmpObject.subscriptionFeesId = newObject.subscriptionFeesId;
                        tmpObject.membershipId = newObject.membershipId;
                        tmpObject.monthsCount = newObject.monthsCount;
                        tmpObject.Amount = newObject.Amount;
                        tmpObject.notes = newObject.notes;
                        //  tmpObject.createDate = newObject.createDate;

                        tmpObject.createUserId = newObject.createUserId;
                        tmpObject.updateUserId = newObject.updateUserId;
                        tmpObject.isActive = newObject.isActive;

                        entity.SaveChanges();
                        message = tmpObject.subscriptionFeesId;
                    }
                    return message;
                }
            }
            catch
            {
                message = 0;
                return message;
            }
        }

        public long DeleteByMembershipId(long membershipId)
        {
            long message = 0;
            List<subscriptionFees> items = null;
            // delete old invoice items
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity
                        .subscriptionFees
                        .Where(x => x.membershipId == membershipId)
                        .ToList();
                    if (items != null)
                    {
                        entity.subscriptionFees.RemoveRange(items);

                        message = entity.SaveChanges();
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                message = -2;
                return message;
            }
        }

        public subscriptionFees GetById(long subscriptionFeesId)
        {
            subscriptionFees item = new subscriptionFees();
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var list = entity.subscriptionFees.ToList();

                    item = list.Where(S => S.subscriptionFeesId == subscriptionFeesId)
                        .Select(
                            S =>
                                new subscriptionFees
                                {
                                    subscriptionFeesId = S.subscriptionFeesId,
                                    membershipId = S.membershipId,
                                    monthsCount = S.monthsCount,
                                    Amount = S.Amount,
                                    notes = S.notes,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    isActive = S.isActive,
                                }
                        )
                        .FirstOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
                //item.notes = ex.ToString();
                return item;
            }
        }
    }
}
