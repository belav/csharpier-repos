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
    [RoutePrefix("api/invoicesClassMemberships")]
    public class invoicesClassMembershipsController : ApiController
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
                    var List1 = entity.invoicesClassMemberships.ToList();
                    var List = List1.Select(S => new invoicesClassMemberships
                    {
                        invClassMemberId = S.invClassMemberId,
                        membershipId = S.membershipId,
                        invClassId = S.invClassId,
                        notes = S.notes,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,



                    })
                    .ToList();

                    return TokenManager.GenerateToken(List);

                }
            }
        }
        /*
   public long invClassMemberId { get; set; }
        public Nullable<long> membershipId { get; set; }
        public Nullable<int> invClassId { get; set; }
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
                long invClassMemberId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invClassMemberId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.invoicesClassMemberships
                   .Where(S => S.invClassMemberId == invClassMemberId)
                   .Select(S => new
                   {
                       S.invClassMemberId,
                       S.membershipId,
                       S.invClassId,
                       S.notes,
                       S.createDate,
                       S.updateDate,
                       S.createUserId,
                       S.updateUserId,


                   })
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
                string invClassMemberId = "";
                invoicesClassMemberships newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        invClassMemberId = c.Value.Replace("\\", string.Empty);
                        invClassMemberId = invClassMemberId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoicesClassMemberships>(invClassMemberId, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
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
                if (newObject.invClassId == 0 || newObject.invClassId == null)
                {
                    Nullable<long> id = null;
                    newObject.invClassId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        invoicesClassMemberships tmpObject = new invoicesClassMemberships();
                        var bankEntity = entity.Set<invoicesClassMemberships>();
                        if (newObject.invClassMemberId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.invClassMemberId.ToString(); ;
                        }
                        else
                        {
                            tmpObject = entity.invoicesClassMemberships.Where(p => p.invClassMemberId == newObject.invClassMemberId).FirstOrDefault();

                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.invClassMemberId = newObject.invClassMemberId;
                            tmpObject.membershipId = newObject.membershipId;
                            tmpObject.invClassId = newObject.invClassId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.createDate = newObject.createDate;

                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;




                            entity.SaveChanges();
                            message = tmpObject.invClassMemberId.ToString();

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
                long invClassMemberId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invClassMemberId = long.Parse(c.Value);
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

                            invoicesClassMemberships objDelete = entity.invoicesClassMemberships.Find(invClassMemberId);
                            entity.invoicesClassMemberships.Remove(objDelete);
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

                            invoicesClassMemberships objDelete = entity.invoicesClassMemberships.Find(invClassMemberId);

                            objDelete.updateUserId = userId;
                            objDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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
        [Route("UpdateInvclassByMembershipId")]
        public string UpdateInvclassByMembershipId(string token)
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
                List<invoicesClassMemberships> newListObj = null;
                long membershipId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<invoicesClassMemberships>>(strObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "membershipId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                    else
                  if (c.Type == "updateUserId")
                    {
                        updateUserId = long.Parse(c.Value);
                    }
                }

                List<invoicesClassMemberships> items = null;
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity.invoicesClassMemberships.Where(x => x.membershipId == membershipId).ToList();
                    if (items != null)
                    {
                        entity.invoicesClassMemberships.RemoveRange(items);
                        try
                        { entity.SaveChanges(); }
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
                            if (newListObj[i].updateUserId == 0 || newListObj[i].updateUserId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].updateUserId = id;
                            }
                            if (newListObj[i].createUserId == 0 || newListObj[i].createUserId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].createUserId = id;
                            }
                            if (newListObj[i].membershipId == 0 || newListObj[i].membershipId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].membershipId = id;
                            }
                            if (newListObj[i].invClassId == 0 || newListObj[i].invClassId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].invClassId = id;
                            }
                            var branchEntity = entity.Set<invoicesClassMemberships>();

                            newListObj[i].createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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