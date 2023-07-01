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
    [RoutePrefix("api/ItemsExtra")]
    public class ItemsExtraController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        [HttpPost]
        [Route("GetExtraByItemId")]
        public string GetExtraByItemId(string token)
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = (
                            from IE in entity.itemsExtra
                            join I in entity.items on IE.itemId equals I.itemId
                            join E in entity.items on IE.extraId equals E.itemId

                            where I.itemId == itemId
                            select new ItemModel
                            {
                                // itemExtraId = IE.itemExtraId,
                                //itemId = IE.itemId,
                                //extraId = IE.extraId,
                                notes = E.notes,
                                createDate = E.createDate,
                                updateDate = E.updateDate,
                                createUserId = E.createUserId,
                                updateUserId = E.updateUserId,
                                //itemcode=I.code,
                                //=I.itemsUnits.First().barcode,
                                //itemName = I.name,
                                //itemType = I.type,
                                //itemImage=I.image ,
                                itemId = E.itemId,
                                code = E.code,
                                name = E.name,
                                type = E.type,
                                //extraImage= E.image,
                            }
                        ).ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        #region
        [HttpPost]
        [Route("UpdateExtraByItemId")]
        public string UpdateExtraByItemId(string token)
        {
            //newplist
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                List<itemsExtra> newObj = new List<itemsExtra>();
                string newlist = "";

                long userId = 0;
                long itemId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        newlist = c.Value.Replace("\\", string.Empty);
                        newlist = newlist.Trim('"');
                        newObj = JsonConvert.DeserializeObject<List<itemsExtra>>(
                            newlist,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    int res = 0;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var oldList = entity.itemsExtra.Where(p => p.itemId == itemId);
                        if (oldList.Count() > 0)
                        {
                            entity.itemsExtra.RemoveRange(oldList);
                        }
                        if (newObj.Count() > 0)
                        {
                            foreach (itemsExtra row in newObj)
                            {
                                row.itemId = itemId;

                                if (row.createUserId == null || row.createUserId == 0)
                                {
                                    row.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    row.updateDate = row.createDate;

                                    row.createUserId = userId;
                                    row.updateUserId = userId;
                                }
                                else
                                {
                                    row.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    row.updateUserId = userId;
                                }
                            }
                            entity.itemsExtra.AddRange(newObj);
                        }
                        res = entity.SaveChanges();

                        // return res;
                        return TokenManager.GenerateToken(res.ToString());
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }
        #endregion
    }
}
