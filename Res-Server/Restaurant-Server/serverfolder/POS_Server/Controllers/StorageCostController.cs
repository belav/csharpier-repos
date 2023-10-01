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
    [RoutePrefix("api/StorageCost")]
    public class StorageCostController : ApiController
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
                    var List = (
                        from S in entity.storageCost
                        select new StorageCostModel()
                        {
                            storageCostId = S.storageCostId,
                            name = S.name,
                            cost = S.cost,
                            notes = S.notes,
                            isActive = S.isActive,
                            createDate = S.createDate,
                            updateDate = S.updateDate,
                            createUserId = S.createUserId,
                            updateUserId = S.updateUserId,
                        }
                    ).ToList();

                    if (List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            if (List[i].isActive == 1)
                            {
                                long storageCostId = (long)List[i].storageCostId;
                                var itemsI = entity
                                    .itemsUnits
                                    .Where(x => x.storageCostId == storageCostId)
                                    .Select(b => new { b.itemUnitId })
                                    .FirstOrDefault();

                                if ((itemsI is null))
                                    canDelete = true;
                            }
                            List[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("GetStorageCostUnits")]
        public string GetStorageCostUnits(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long storageCostId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "storageCostId")
                    {
                        storageCostId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (
                        from IU in entity.itemsUnits.Where(x => x.storageCostId == storageCostId)
                        join u in entity.units on IU.unitId equals u.unitId into lj
                        from v in lj.DefaultIfEmpty()
                        join I in entity.items.Where(x => x.isActive == 1)
                            on IU.itemId equals I.itemId
                        select new ItemUnitModel()
                        {
                            itemUnitId = IU.itemUnitId,
                            unitId = IU.unitId,
                            mainUnit = v.name,
                            itemId = IU.itemId,
                            itemName = I.name + "-" + v.name,
                            unitValue = IU.unitValue,
                            createDate = IU.createDate,
                            createUserId = IU.createUserId,
                            defaultPurchase = IU.defaultPurchase,
                            defaultSale = IU.defaultSale,
                            price = IU.price,
                            priceWithService = IU.priceWithService,
                            subUnitId = IU.subUnitId,
                            barcode = IU.barcode,
                            updateDate = IU.updateDate,
                            updateUserId = IU.updateUserId,
                            storageCostId = IU.storageCostId,
                            purchasePrice = IU.purchasePrice,
                            isActive = IU.isActive,
                            type = I.type,
                            categoryId = I.categoryId,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long storageCostId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        storageCostId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var row = entity
                        .storageCost
                        .Where(u => u.storageCostId == storageCostId)
                        .Select(
                            S =>
                                new
                                {
                                    S.storageCostId,
                                    S.name,
                                    S.cost,
                                    S.notes,
                                    S.isActive,
                                    S.createDate,
                                    S.updateDate,
                                    S.createUserId,
                                    S.updateUserId,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(row);
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
                string storageCostObject = "";
                storageCost newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        storageCostObject = c.Value.Replace("\\", string.Empty);
                        storageCostObject = storageCostObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<storageCost>(
                            storageCostObject,
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

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<storageCost>();
                        if (newObject.storageCostId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;

                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.storageCostId.ToString();
                        }
                        else
                        {
                            var tmpObject = entity
                                .storageCost
                                .Where(p => p.storageCostId == newObject.storageCostId)
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.updateUserId = newObject.updateUserId;

                            tmpObject.name = newObject.name;
                            tmpObject.cost = newObject.cost;
                            tmpObject.notes = newObject.notes;

                            tmpObject.isActive = newObject.isActive;
                            entity.SaveChanges();
                            message = tmpObject.storageCostId.ToString();
                        }
                    }
                }
                catch
                {
                    message = "-1";
                }
            }
            return TokenManager.GenerateToken(message);
        }

        [HttpPost]
        [Route("setCostToUnits")]
        public string setCostToUnits(string token)
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
                string itemsUnits = "";
                long storageCostId = 0;
                long userId = 0;
                List<long> itemsUnitsIds = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitsIds")
                    {
                        itemsUnits = c.Value.Replace("\\", string.Empty);
                        itemsUnits = itemsUnits.Trim('"');
                        itemsUnitsIds = JsonConvert.DeserializeObject<List<long>>(
                            itemsUnits,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "storageCostId")
                        storageCostId = long.Parse(c.Value);
                    else if (c.Type == "userId")
                        userId = long.Parse(c.Value);
                }
                try
                {
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var sotrageCostUnits = entity
                            .itemsUnits
                            .Where(x => x.storageCostId == storageCostId)
                            .ToList();
                        foreach (itemsUnits iu in sotrageCostUnits)
                        {
                            iu.storageCostId = null;
                            iu.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            iu.updateUserId = userId;
                        }
                        entity.SaveChanges();
                        var itemsUnitsList = entity
                            .itemsUnits
                            .Where(x => itemsUnitsIds.Contains(x.itemUnitId))
                            .ToList();
                        itemsUnitsList.ForEach(x => x.storageCostId = storageCostId);
                        itemsUnitsList.ForEach(x => x.updateUserId = userId);
                        itemsUnitsList.ForEach(x => x.updateDate = datenow);
                        entity.SaveChanges();
                        message = "1";
                    }
                }
                catch
                {
                    message = "-1";
                }
            }
            return TokenManager.GenerateToken(message);
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
                long storageCostId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        storageCostId = long.Parse(c.Value);
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
                            storageCost objectDelete = entity.storageCost.Find(storageCostId);

                            entity.storageCost.Remove(objectDelete);
                            message = entity.SaveChanges().ToString();

                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        message = "-1";
                        return TokenManager.GenerateToken(message);
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            storageCost objectDelete = entity.storageCost.Find(storageCostId);

                            objectDelete.isActive = 0;
                            objectDelete.updateUserId = userId;
                            objectDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            message = entity.SaveChanges().ToString();

                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        message = "-2";
                        return TokenManager.GenerateToken(message);
                    }
                }
            }
        }
    }
}
