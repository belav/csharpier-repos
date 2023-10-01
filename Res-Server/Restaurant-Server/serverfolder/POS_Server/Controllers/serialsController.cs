using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    [RoutePrefix("api/serials")]
    public class serialsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
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
                    var serialsList = entity
                        .serials
                        .Where(S => S.itemId == itemId)
                        .Select(
                            S =>
                                new
                                {
                                    S.serialId,
                                    S.itemId,
                                    S.serialNum,
                                    S.isActive,
                                    S.createUserId,
                                    S.createDate,
                                    S.updateUserId,
                                    S.updateDate,
                                }
                        )
                        .ToList();
                    return TokenManager.GenerateToken(serialsList);
                }
            }
        }

        // add or update location
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
                string serialObject = "";
                serials newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        serialObject = c.Value.Replace("\\", string.Empty);
                        serialObject = serialObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<serials>(
                            serialObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        serials tmpSerial = new serials();
                        var serialEntity = entity.Set<serials>();
                        if (newObject.serialId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;

                            tmpSerial = serialEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpSerial.serialId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            tmpSerial = entity
                                .serials
                                .Where(p => p.serialId == newObject.serialId)
                                .FirstOrDefault();
                            tmpSerial.itemId = newObject.itemId;
                            tmpSerial.serialNum = newObject.serialNum;
                            tmpSerial.isActive = newObject.isActive;
                            tmpSerial.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpSerial.updateUserId = newObject.createUserId;
                            entity.SaveChanges();
                            message = tmpSerial.serialId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
            }
            return message;
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
                long serialId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        serialId = long.Parse(c.Value);
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
                            serials serialObj = entity.serials.Find(serialId);

                            entity.serials.Remove(serialObj);
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
                            serials serialObj = entity.serials.Find(serialId);

                            serialObj.isActive = 0;
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
    }
}
