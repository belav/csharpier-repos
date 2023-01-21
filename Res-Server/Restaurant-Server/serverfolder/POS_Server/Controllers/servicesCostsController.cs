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
    [RoutePrefix("api/servicesCosts")]
    public class servicesCostsController : ApiController
    {
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
                int itemId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        itemId = int.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var servicesList = entity.servicesCosts
                        .Where(S => S.itemId == itemId)
                    .Select(S => new
                    {
                        S.costId,
                        S.name,
                        S.itemId,
                        S.costVal,
                        S.createDate,
                        S.updateDate,
                        S.updateUserId,
                        S.createUserId
                    })
                    .ToList();
                     
                    return TokenManager.GenerateToken(servicesList);
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
                string serviceObject = "";
                servicesCosts newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        serviceObject = c.Value.Replace("\\", string.Empty);
                        serviceObject = serviceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<servicesCosts>(serviceObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }

 
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var serviceEntity = entity.Set<servicesCosts>();
                        if (newObject.costId == 0)
                        {
                            newObject.createDate = DateTime.Now;
                            newObject.updateDate = DateTime.Now;
                            newObject.updateUserId = newObject.createUserId;

                            newObject = serviceEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.costId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            var tmpSerial = entity.servicesCosts.Where(p => p.costId == newObject.costId).FirstOrDefault();
                            tmpSerial.name = newObject.name;
                            tmpSerial.costVal = newObject.costVal;
                            tmpSerial.updateDate = DateTime.Now;
                            tmpSerial.updateUserId = newObject.updateUserId;

                            entity.SaveChanges();
                            message = tmpSerial.costId.ToString();
                            return TokenManager.GenerateToken(message);

                        }
                    }
                }
                catch
                {
                    message = "0";
                    return message;
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
                try
                {
                    int costId = 0;
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "itemId")
                        {
                            costId = int.Parse(c.Value);
                        }
                    }
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        servicesCosts serviceObj = entity.servicesCosts.Find(costId);
                        entity.servicesCosts.Remove(serviceObj);
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