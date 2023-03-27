using LinqKit;
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
using System.Web;
using System.Web.Http;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/menuSettings")]
    public class menuSettingsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
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
                long itemUnitId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<menuSettings>();
                    searchPredicate.And(x => x.isActive == 1);
                    if (itemUnitId != 0)
                        searchPredicate.And(x => x.itemUnitId == itemUnitId);

                    var menuList = entity.menuSettings.Where(searchPredicate).ToList();
                   
                    return TokenManager.GenerateToken(menuList);
                }
            }
        }
        // add or update menu settings 
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
                string itemObject = "";
                menuSettings Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<menuSettings>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        menuSettings tmpObject = new menuSettings();
                        if (Object.menuSettingId == 0)
                        {
                            Object.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            entity.menuSettings.Add(Object);
                        }
                        else
                        {
                            tmpObject = entity.menuSettings.Find(Object.menuSettingId);
                            tmpObject.preparingTime = Object.preparingTime;
                            tmpObject.sat = Object.sat;
                            tmpObject.sun = Object.sun;
                            tmpObject.mon = Object.mon;
                            tmpObject.tues = Object.tues;
                            tmpObject.wed = Object.wed;
                            tmpObject.thur = Object.thur;
                            tmpObject.fri = Object.fri;
                            tmpObject.isActive = Object.isActive;
                            tmpObject.updateUserId = Object.updateUserId;
                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        }
                        message = entity.SaveChanges().ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch { return TokenManager.GenerateToken("0"); }
            }
        }
    }
}
