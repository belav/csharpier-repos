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
    [RoutePrefix("api/invoicesClass")]
    public class invoicesClassController : ApiController
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
                    var List = entity.invoicesClass.Select(S => new invoicesClassModel
                    {
                        invClassId = S.invClassId,
                        minInvoiceValue = S.minInvoiceValue,
                        maxInvoiceValue = S.maxInvoiceValue,
                        discountValue = S.discountValue,
                        discountType = S.discountType,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        updateUserId = S.updateUserId,
                        createUserId = S.createUserId,
                        notes = S.notes,
                        isActive = S.isActive,
                        name = S.name,


                    })
                    .ToList();

                    if (List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            canDelete = false;
                            if (List[i].isActive == 1)
                            {
                                int invClassId = (int)List[i].invClassId;
                                var items5 = entity.invoicesClassMemberships.Where(x => x.invClassId == invClassId).Select(b => new { b.invClassMemberId }).FirstOrDefault();

                                if ((items5 is null))
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
                long invClassId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invClassId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.invoicesClass
                   .Where(S => S.invClassId == invClassId)
                   .Select(S => new
                   {
                       S.invClassId,
                       S.minInvoiceValue,
                       S.maxInvoiceValue,
                       S.discountValue,
                       S.discountType,
                       S.createDate,
                       S.updateDate,
                       S.updateUserId,
                       S.createUserId,
                       S.notes,
                       S.isActive,
                        S.name,

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
                string invClassId = "";
                invoicesClass newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        invClassId = c.Value.Replace("\\", string.Empty);
                        invClassId = invClassId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoicesClass>(invClassId, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
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
                        invoicesClass tmpObject = new invoicesClass();
                        var bankEntity = entity.Set<invoicesClass>();
                        if (newObject.invClassId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.invClassId.ToString(); ;
                        }
                        else
                        {
                            tmpObject = entity.invoicesClass.Where(p => p.invClassId == newObject.invClassId).FirstOrDefault();

                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.invClassId = newObject.invClassId;
                            tmpObject.minInvoiceValue = newObject.minInvoiceValue;
                            tmpObject.maxInvoiceValue = newObject.maxInvoiceValue;
                            tmpObject.discountValue = newObject.discountValue;
                            tmpObject.discountType = newObject.discountType;
                            tmpObject.createDate = newObject.createDate;

                            tmpObject.updateUserId = newObject.updateUserId;
                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.isActive = newObject.isActive;
                            tmpObject.name = newObject.name;



                            entity.SaveChanges();
                            message = tmpObject.invClassId.ToString();

                        }
                        return TokenManager.GenerateToken(message);
                    }
                }

                catch (Exception ex)
                {
                     
                    return TokenManager.GenerateToken(ex.ToString());
                    //message = "0";
                    //return TokenManager.GenerateToken(message);
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
                long invClassId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invClassId = long.Parse(c.Value);
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

                            invoicesClass objDelete = entity.invoicesClass.Find(invClassId);
                            entity.invoicesClass.Remove(objDelete);
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

                            invoicesClass objDelete = entity.invoicesClass.Find(invClassId);
                            objDelete.isActive = 0;
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


        [HttpPost]
        [Route("GetInvclassByMembershipId")]
        public string GetInvclassByMembershipId(string token)
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
                    var List = (from S in entity.invoicesClassMemberships
                                join B in entity.invoicesClass on S.invClassId equals B.invClassId into JB
                                join U in entity.memberships on S.membershipId equals U.membershipId into JU
                                from JBB in JB.DefaultIfEmpty()
                                from JUU in JU.DefaultIfEmpty()
                                where S.membershipId == membershipId
                                select new invoicesClassModel()
                                {
                                    invClassId = JBB.invClassId,
                                    minInvoiceValue = JBB.minInvoiceValue,
                                    maxInvoiceValue = JBB.maxInvoiceValue,
                                    discountValue = JBB.discountValue,
                                    discountType = JBB.discountType,
                                    createDate = JBB.createDate,
                                    updateDate = JBB.updateDate,
                                    updateUserId = JBB.updateUserId,
                                    createUserId = JBB.createUserId,
                                    notes = JBB.notes,
                                    isActive = JBB.isActive,
                                   
                                    invClassMemberId = S.invClassMemberId,

                                    membershipId = S.membershipId,
                                    name= JBB.name,

                                }).ToList();
                    return TokenManager.GenerateToken(List);

               
                }
            }
        }

    }
}
