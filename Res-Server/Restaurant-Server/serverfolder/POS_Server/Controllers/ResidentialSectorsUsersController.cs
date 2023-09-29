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
    [RoutePrefix("api/ResidentialSectorsUsers")]
    public class ResidentialSectorsUsersController : ApiController
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var List1 = entity.residentialSectorsUsers.ToList();
                        var List = List1
                            .Select(
                                S =>
                                    new residentialSectorsUsers
                                    {
                                        id = S.id,
                                        residentSecId = S.residentSecId,
                                        userId = S.userId,
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
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        /*
    public long id { get; set; }
        public Nullable<long> residentSecId { get; set; }
        public Nullable<long> userId { get; set; }
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
                    var bank = entity.residentialSectorsUsers
                        .Where(S => S.id == id)
                        .Select(
                            S =>
                                new
                                {
                                    S.id,
                                    S.residentSecId,
                                    S.userId,
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
                string strObject = "";
                residentialSectorsUsers newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<residentialSectorsUsers>(
                            strObject,
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
                if (newObject.residentSecId == 0 || newObject.residentSecId == null)
                {
                    Nullable<long> id = null;
                    newObject.residentSecId = id;
                }
                if (newObject.userId == 0 || newObject.userId == null)
                {
                    Nullable<long> id = null;
                    newObject.userId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        residentialSectorsUsers tmpObject = new residentialSectorsUsers();
                        var bankEntity = entity.Set<residentialSectorsUsers>();
                        if (newObject.id == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.id.ToString();
                            ;
                        }
                        else
                        {
                            tmpObject = entity.residentialSectorsUsers
                                .Where(p => p.id == newObject.id)
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.id = newObject.id;
                            tmpObject.residentSecId = newObject.residentSecId;
                            tmpObject.userId = newObject.userId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.createDate = newObject.createDate;

                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;

                            entity.SaveChanges();
                            message = tmpObject.id.ToString();
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
                long id = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        id = long.Parse(c.Value);
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
                            residentialSectorsUsers objDelete = entity.residentialSectorsUsers.Find(
                                id
                            );
                            entity.residentialSectorsUsers.Remove(objDelete);
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
                            residentialSectorsUsers objDelete = entity.residentialSectorsUsers.Find(
                                id
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

        [HttpPost]
        [Route("UpdateResSectorsByUserId")]
        public string UpdateResSectorsByUserId(string token)
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
                List<residentialSectorsUsers> newListObj = null;
                long userId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<residentialSectorsUsers>>(
                            strObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "updateUserId")
                    {
                        updateUserId = long.Parse(c.Value);
                    }
                }

                List<residentialSectorsUsers> items = null;
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity.residentialSectorsUsers.Where(x => x.userId == userId).ToList();
                    if (items != null)
                    {
                        entity.residentialSectorsUsers.RemoveRange(items);
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
                                newListObj[i].residentSecId == 0
                                || newListObj[i].residentSecId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].residentSecId = id;
                            }
                            if (newListObj[i].userId == 0 || newListObj[i].userId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].userId = id;
                            }
                            var branchEntity = entity.Set<residentialSectorsUsers>();

                            newListObj[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newListObj[i].updateDate = newListObj[i].createDate;
                            newListObj[i].updateUserId = updateUserId;
                            newListObj[i].userId = userId;
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
