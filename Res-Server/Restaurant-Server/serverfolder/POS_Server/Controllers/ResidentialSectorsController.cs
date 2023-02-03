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
    [RoutePrefix("api/ResidentialSectors")]
    public class ResidentialSectorsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        // GET api/<controller>
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
                    var  List = entity.residentialSectors.Select(S => new ResidentialSectorsModel
                    {
                        residentSecId = S.residentSecId,
                        name = S.name,
                        notes = S.notes,
                        isActive = S.isActive,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,
                    })
                    .ToList();


                    /*
  

                     * */

                    return TokenManager.GenerateToken(List);

                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GeById")]
        public string GeById(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var Item = entity.residentialSectors
                   .Where(S => S.residentSecId == itemId)
                   .Select(S => new
                   {
                       S.residentSecId,
                       S.name,
                       S.notes,
                       S.isActive,
                       S.createDate,
                       S.updateDate,
                       S.createUserId,
                       S.updateUserId,

                   })
                   .FirstOrDefault();
                    return TokenManager.GenerateToken(Item);
                }
            }
        }

        // add or update  
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
                string residentSecId = "";
                residentialSectors newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        residentSecId = c.Value.Replace("\\", string.Empty);
                        residentSecId = residentSecId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<residentialSectors>(residentSecId, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
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
                        residentialSectors tmpObject = new residentialSectors();
                        var Entity = entity.Set<residentialSectors>();
                        if (newObject.residentSecId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = Entity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.residentSecId.ToString();
                        }
                        else
                        {
                            tmpObject = entity.residentialSectors.Where(p => p.residentSecId == newObject.residentSecId).FirstOrDefault();
                            tmpObject.residentSecId = newObject.residentSecId;
                            tmpObject.name = newObject.name;
                            tmpObject.notes = newObject.notes;
                            tmpObject.isActive = newObject.isActive;
                            tmpObject.createDate = newObject.createDate;
                            tmpObject.updateDate = newObject.updateDate;
                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;

                            entity.SaveChanges();
                            message = tmpObject.residentSecId.ToString();

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
                long residentSecId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        residentSecId = long.Parse(c.Value);
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

                            residentialSectors DeleteObj = entity.residentialSectors.Find(residentSecId);
                            entity.residentialSectors.Remove(DeleteObj);
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

                            residentialSectors Obj = entity.residentialSectors.Find(residentSecId);
                            Obj.isActive = 0;
                            Obj.updateUserId = userId;
                            Obj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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
        [Route("GetResSectorsByUserId")]
        public string GetResSectorsByUserId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (from S in entity.residentialSectorsUsers
                                join B in entity.residentialSectors on S.residentSecId equals B.residentSecId into JB
                                join U in entity.users on S.userId equals U.userId into JU
                                from JBB in JB.DefaultIfEmpty()
                                from JUU in JU.DefaultIfEmpty()
                                where S.userId == userId
                                select new ResidentialSectorsModel()
                                {
                                    residentSecId = JBB.residentSecId,
                                    name = JBB.name,
                                    notes = JBB.notes,
                                    isActive = JBB.isActive,
                                    createDate = JBB.createDate,
                                    updateDate = JBB.updateDate,
                                    createUserId = JBB.createUserId,
                                    updateUserId = JBB.updateUserId,

                                }).ToList();
                    return TokenManager.GenerateToken(List);


                }
            }
        }
    }
}
