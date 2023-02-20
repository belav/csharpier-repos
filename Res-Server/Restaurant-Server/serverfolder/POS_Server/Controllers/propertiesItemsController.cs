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
    [RoutePrefix("api/propertiesItems")]
    public class propertiesItemsController : ApiController
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
                    var propertiesList = entity.propertiesItems
                        .Select(
                            p =>
                                new PropertiesItemModel
                                {
                                    propertyItemId = p.propertyItemId,
                                    propertyId = p.propertyId,
                                    propertyItemName = p.name,
                                    isDefault = p.isDefault,
                                    createDate = p.createDate,
                                    createUserId = p.createUserId,
                                    updateDate = p.updateDate,
                                    updateUserId = p.updateUserId,
                                    isActive = p.isActive,
                                }
                        )
                        .ToList();

                    if (propertiesList.Count > 0)
                    {
                        for (int i = 0; i < propertiesList.Count; i++)
                        {
                            canDelete = false;
                            if (propertiesList[i].isActive == 1)
                            {
                                long propertyItemId = (long)propertiesList[i].propertyItemId;
                                var Itemsprop = entity.itemsProp
                                    .Where(x => x.propertyItemId == propertyItemId)
                                    .Select(b => new { b.itemPropId, b.itemId })
                                    .ToList();

                                if (Itemsprop.Count == 0)
                                    canDelete = true;
                            }
                            propertiesList[i].canDelete = canDelete;
                        }
                    }

                    return TokenManager.GenerateToken(propertiesList);
                }
            }
        }

        //********************************************************
        //****************** get values of property
        [HttpPost]
        [Route("GetPropertyItems")]
        public string GetPropertyItems(string token)
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
                long propertyId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        propertyId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var propertiesList = entity.propertiesItems
                        .Where(p => p.propertyId == propertyId)
                        .Select(
                            p =>
                                new PropertiesItemModel
                                {
                                    propertyItemId = p.propertyItemId,
                                    propertyId = p.propertyId,
                                    propertyItemName = p.name,
                                    isDefault = p.isDefault,
                                    createDate = p.createDate,
                                    createUserId = p.createUserId,
                                    updateDate = p.updateDate,
                                    updateUserId = p.updateUserId,
                                    isActive = p.isActive,
                                }
                        )
                        .ToList();

                    if (propertiesList.Count > 0)
                    {
                        for (int i = 0; i < propertiesList.Count; i++)
                        {
                            canDelete = false;
                            if (propertiesList[i].isActive == 1)
                            {
                                long propertyItemId = (long)propertiesList[i].propertyItemId;
                                var Itemsprop = entity.itemsProp
                                    .Where(x => x.propertyItemId == propertyItemId)
                                    .Select(b => new { b.itemPropId })
                                    .FirstOrDefault();

                                if (Itemsprop is null)
                                    canDelete = true;
                            }
                            propertiesList[i].canDelete = canDelete;
                        }
                    }

                    return TokenManager.GenerateToken(propertiesList);
                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetPropItemByID")]
        public string GetPropItemByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            long propItemId = 0;
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var propertyItems = entity.propertiesItems
                        .Where(p => p.propertyItemId == propItemId)
                        .Select(
                            p =>
                                new
                                {
                                    p.propertyItemId,
                                    p.propertyId,
                                    p.name,
                                    p.isDefault,
                                    p.createDate,
                                    p.createUserId,
                                    p.updateDate,
                                    p.updateUserId,
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(propertyItems);
                }
            }
        }

        // add or update property
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
                string propItemObjects = "";
                propertiesItems propertyItemObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        propItemObjects = c.Value.Replace("\\", string.Empty);
                        propItemObjects = propItemObjects.Trim('"');
                        propertyItemObject = JsonConvert.DeserializeObject<propertiesItems>(
                            propItemObjects,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }

                if (propertyItemObject.updateUserId == 0 || propertyItemObject.updateUserId == null)
                {
                    Nullable<long> id = null;
                    propertyItemObject.updateUserId = id;
                }
                if (propertyItemObject.createUserId == 0 || propertyItemObject.createUserId == null)
                {
                    Nullable<long> id = null;
                    propertyItemObject.createUserId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        propertiesItems tmpPropertyItem = new propertiesItems();
                        var propItemEntity = entity.Set<propertiesItems>();
                        if (propertyItemObject.propertyItemId == 0)
                        {
                            propertyItemObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            propertyItemObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            propertyItemObject.updateUserId = propertyItemObject.createUserId;

                            tmpPropertyItem = propItemEntity.Add(propertyItemObject);
                            entity.SaveChanges();
                            message = tmpPropertyItem.propertyItemId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            tmpPropertyItem = entity.propertiesItems
                                .Where(p => p.propertyItemId == propertyItemObject.propertyItemId)
                                .FirstOrDefault();
                            tmpPropertyItem.name = propertyItemObject.name;
                            tmpPropertyItem.propertyId = propertyItemObject.propertyId;
                            tmpPropertyItem.isDefault = propertyItemObject.isDefault;
                            tmpPropertyItem.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpPropertyItem.updateUserId = propertyItemObject.updateUserId;
                            tmpPropertyItem.isActive = propertyItemObject.isActive;
                            entity.SaveChanges();
                            message = tmpPropertyItem.propertyItemId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
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
                long propertyItemId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        propertyItemId = long.Parse(c.Value);
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
                            propertiesItems PropertDelete = entity.propertiesItems.Find(
                                propertyItemId
                            );
                            entity.propertiesItems.Remove(PropertDelete);
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
                            propertiesItems PropertDelete = entity.propertiesItems.Find(
                                propertyItemId
                            );
                            PropertDelete.isActive = 0;
                            PropertDelete.updateUserId = userId;
                            PropertDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
