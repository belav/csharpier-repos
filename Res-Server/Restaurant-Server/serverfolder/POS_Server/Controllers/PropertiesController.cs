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
    [RoutePrefix("api/Properties")]
    public class PropertiesController : ApiController
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
                    var propertiesList = entity.properties
                        .Select(
                            p =>
                                new PropertyModel
                                {
                                    propertyId = p.propertyId,
                                    name = p.name,
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
                            string values = "";
                            canDelete = false;

                            long propertyId = (long)propertiesList[i].propertyId;
                            var propItems = entity.propertiesItems
                                .Where(x => x.propertyId == propertyId)
                                .Select(b => new { b.name, b.propertyId })
                                .ToList();

                            if (propItems is null || propItems.Count == 0)
                            {
                                canDelete = true;
                            }
                            else
                            {
                                foreach (var s in propItems)
                                {
                                    if (values == "")
                                        values += s.name;
                                    else
                                        values += ", " + s.name;
                                }
                            }
                            propertiesList[i].canDelete = canDelete;
                            propertiesList[i].propertyValues = values;
                        }
                    }

                    return TokenManager.GenerateToken(propertiesList);
                }
            }
        }

        [HttpPost]
        [Route("GetPropertyValues")]
        public string GetPropertyValues(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
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
                    var propertiesList = (
                        from PI in entity.propertiesItems.Where(x => x.propertyId == propertyId)
                        join P in entity.properties on PI.propertyId equals P.propertyId
                        select new PropertiesItemModel()
                        {
                            propertyItemId = PI.propertyItemId,
                            propertyId = PI.propertyId,
                            propertyItemName = PI.name,
                            createDate = PI.createDate,
                            createUserId = PI.createUserId,
                            updateDate = PI.updateDate,
                            updateUserId = PI.updateUserId,
                            propertyName = P.name,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(propertiesList);
                }
            }
        }

        [HttpPost]
        [Route("GetAllPropertiesValues")]
        public string GetAllPropertiesValues(string token)
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
                    var propertiesList = (
                        from PI in entity.propertiesItems
                        join P in entity.properties on PI.propertyId equals P.propertyId
                        select new PropertiesItemModel()
                        {
                            propertyId = PI.propertyId,
                            propertyItemName = PI.name,
                            createDate = PI.createDate,
                            createUserId = PI.createUserId,
                            updateDate = PI.updateDate,
                            updateUserId = PI.updateUserId,
                            propertyName = P.name,
                        }
                    ).ToList();

                    if (propertiesList.Count > 0)
                    {
                        for (int i = 0; i < propertiesList.Count; i++)
                        {
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
        [Route("GetPropertyByID")]
        public string GetPropertyByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
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
                    var property = entity.properties
                        .Where(p => p.propertyId == propertyId)
                        .Select(
                            p =>
                                new
                                {
                                    p.propertyId,
                                    p.name,
                                    p.createDate,
                                    p.createUserId,
                                    p.updateDate,
                                    p.updateUserId,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(property);
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
                string propertiesObjects = "";
                properties propertiesObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        propertiesObjects = c.Value.Replace("\\", string.Empty);
                        propertiesObjects = propertiesObjects.Trim('"');
                        propertiesObject = JsonConvert.DeserializeObject<properties>(
                            propertiesObjects,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }

                if (propertiesObject.updateUserId == 0 || propertiesObject.updateUserId == null)
                {
                    Nullable<long> id = null;
                    propertiesObject.updateUserId = id;
                }
                if (propertiesObject.createUserId == 0 || propertiesObject.createUserId == null)
                {
                    Nullable<long> id = null;
                    propertiesObject.createUserId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        properties tmpProperty = new properties();
                        var propEntity = entity.Set<properties>();
                        if (propertiesObject.propertyId == 0)
                        {
                            propertiesObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            propertiesObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            propertiesObject.updateUserId = propertiesObject.createUserId;

                            tmpProperty = propEntity.Add(propertiesObject);
                            entity.SaveChanges();
                            message = tmpProperty.propertyId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            tmpProperty = entity.properties
                                .Where(p => p.propertyId == propertiesObject.propertyId)
                                .FirstOrDefault();
                            tmpProperty.name = propertiesObject.name;
                            tmpProperty.isActive = propertiesObject.isActive;
                            tmpProperty.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpProperty.updateUserId = propertiesObject.updateUserId;
                            entity.SaveChanges();
                            message = tmpProperty.propertyId.ToString();
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
                long propertyId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        propertyId = long.Parse(c.Value);
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
                            properties PropertDelete = entity.properties.Find(propertyId);
                            entity.properties.Remove(PropertDelete);
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
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        properties PropertDelete = entity.properties.Find(propertyId);
                        PropertDelete.isActive = 0;
                        PropertDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        PropertDelete.updateUserId = userId;
                        message = entity.SaveChanges().ToString();
                        return TokenManager.GenerateToken(message);
                    }
                }
            }
        }
    }
}
