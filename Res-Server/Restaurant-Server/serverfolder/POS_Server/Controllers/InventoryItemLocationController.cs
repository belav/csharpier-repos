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
    [RoutePrefix("api/InventoryItemLocation")]
    public class InventoryItemLocationController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all InventoryItemLocation
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
                long inventoryId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        inventoryId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (
                        from c in entity
                            .inventoryItemLocation
                            .Where(c => c.inventoryId == inventoryId)
                        join l in entity.itemsLocations on c.itemLocationId equals l.itemsLocId
                        join u in entity.itemsUnits on l.itemUnitId equals u.itemUnitId
                        join un in entity.units on u.unitId equals un.unitId
                        join lo in entity.locations on l.locationId equals lo.locationId
                        select new InventoryItemLocationModel()
                        {
                            id = c.id,
                            isDestroyed = c.isDestroyed,
                            amount = c.amount,
                            amountDestroyed = c.amountDestroyed,
                            quantity = c.realAmount,
                            itemLocationId = c.itemLocationId,
                            inventoryId = c.inventoryId,
                            isActive = c.isActive,
                            notes = c.notes,
                            createDate = c.createDate,
                            updateDate = c.updateDate,
                            createUserId = c.createUserId,
                            updateUserId = c.updateUserId,
                            canDelete = true,
                            itemName = u.items.name,
                            section = lo.sections.name,
                            location = lo.x + lo.y + lo.z,
                            unitName = un.name,
                        }
                    ).ToList().OrderBy(x => x.location).ToList();
                    int sequence = 0;
                    for (int i = 0; i < List.Count(); i++)
                    {
                        sequence++;
                        List[i].sequence = sequence;
                    }

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("GetItemToDestroy")]
        public string GetItemToDestroy(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (
                        from c in entity
                            .inventoryItemLocation
                            .Where(
                                c =>
                                    c.amountDestroyed > 0
                                    && c.isDestroyed == false
                                    && c.Inventory.branchId == branchId
                                    && c.Inventory.inventoryType == "n"
                                    && c.Inventory.isActive == 1
                            )
                        join l in entity.itemsLocations on c.itemLocationId equals l.itemsLocId
                        join u in entity.itemsUnits on l.itemUnitId equals u.itemUnitId
                        join un in entity.units on u.unitId equals un.unitId
                        join lo in entity.locations on l.locationId equals lo.locationId
                        select new InventoryItemLocationModel()
                        {
                            id = c.id,
                            isDestroyed = c.isDestroyed,
                            amount = c.amount,
                            amountDestroyed = c.amountDestroyed,
                            quantity = c.realAmount,
                            itemLocationId = c.itemLocationId,
                            inventoryId = c.inventoryId,
                            isActive = c.isActive,
                            notes = c.notes,
                            createDate = c.createDate,
                            updateDate = c.updateDate,
                            createUserId = c.createUserId,
                            updateUserId = c.updateUserId,
                            canDelete = true,
                            itemId = u.items.itemId,
                            itemName = u.items.name,
                            unitId = un.unitId,
                            itemUnitId = u.itemUnitId,
                            unitName = un.name,
                            section = lo.sections.name,
                            location = lo.x + lo.y + lo.z,
                            itemType = u.items.type,
                            inventoryDate = c.Inventory.createDate,
                            inventoryNum = c.Inventory.num,
                            avgPurchasePrice = u.items.avgPurchasePrice,
                        }
                    ).ToList().OrderBy(x => x.location).ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("GetShortageItem")]
        public string GetShortageItem(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (
                        from c in entity
                            .inventoryItemLocation
                            .Where(
                                c =>
                                    c.realAmount - c.amount > 0
                                    && c.Inventory.branchId == branchId
                                    && c.isFalls == false
                                    && c.Inventory.inventoryType == "n"
                                    && c.Inventory.isActive == 1
                            )
                        join l in entity.itemsLocations on c.itemLocationId equals l.itemsLocId
                        join u in entity.itemsUnits on l.itemUnitId equals u.itemUnitId
                        join un in entity.units on u.unitId equals un.unitId
                        join lo in entity.locations on l.locationId equals lo.locationId
                        select new InventoryItemLocationModel()
                        {
                            id = c.id,
                            isDestroyed = c.isDestroyed,
                            amount = c.realAmount - c.amount,
                            amountDestroyed = c.amountDestroyed,
                            quantity = c.realAmount,
                            itemLocationId = c.itemLocationId,
                            inventoryId = c.inventoryId,
                            isActive = c.isActive,
                            notes = c.notes,
                            createDate = c.createDate,
                            updateDate = c.updateDate,
                            createUserId = c.createUserId,
                            updateUserId = c.updateUserId,
                            canDelete = true,
                            itemId = u.items.itemId,
                            itemName = u.items.name,
                            unitId = un.unitId,
                            itemUnitId = u.itemUnitId,
                            unitName = un.name,
                            section = lo.sections.name,
                            location = lo.x + lo.y + lo.z,
                            itemType = u.items.type,
                            inventoryDate = c.Inventory.createDate,
                            inventoryNum = c.Inventory.num,
                            avgPurchasePrice = u.items.avgPurchasePrice,
                        }
                    ).ToList().OrderBy(x => x.location).ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        // GET api/<controller>  Get medal By ID
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            long cId = 0;
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var list = entity
                        .inventoryItemLocation
                        .Where(c => c.id == cId)
                        .Select(
                            c =>
                                new
                                {
                                    c.id,
                                    c.isDestroyed,
                                    c.amount,
                                    c.amountDestroyed,
                                    c.realAmount,
                                    c.itemLocationId,
                                    c.inventoryId,
                                    c.isActive,
                                    c.notes,
                                    c.createDate,
                                    c.updateDate,
                                    c.createUserId,
                                    c.updateUserId,
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(list);
                }
            }
        }

        // add or update
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string message = "";
                string newObject = "";
                long inventoryId = 0;
                List<InventoryItemLocationModel> Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        newObject = c.Value.Replace("\\", string.Empty);
                        newObject = newObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<List<InventoryItemLocationModel>>(
                            newObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "inventoryId")
                    {
                        inventoryId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<inventoryItemLocation> items = entity
                        .inventoryItemLocation
                        .Where(x => x.inventoryId == inventoryId)
                        .ToList();
                    if (items == null || items.Count == 0) // add first time
                    {
                        foreach (InventoryItemLocationModel il in Object)
                        {
                            inventoryItemLocation tmp = new inventoryItemLocation();
                            if (il.createUserId == 0 || il.createUserId == null)
                            {
                                Nullable<long> id = null;
                                tmp.createUserId = id;
                            }

                            tmp.inventoryId = inventoryId;
                            tmp.amount = il.amount;
                            tmp.amountDestroyed = il.amountDestroyed;
                            tmp.isDestroyed = il.isDestroyed;
                            tmp.isFalls = il.isFalls;
                            tmp.realAmount = il.quantity;
                            tmp.cause = il.cause;
                            tmp.notes = il.notes;
                            tmp.isActive = 1;
                            tmp.itemLocationId = il.itemLocationId;
                            tmp.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmp.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmp.updateUserId = il.createUserId;
                            entity.inventoryItemLocation.Add(tmp);
                            message = tmp.id.ToString();
                        }
                        entity.SaveChanges();
                        return TokenManager.GenerateToken(message);
                    }
                    else // edit saved inventory details
                    {
                        foreach (InventoryItemLocationModel il in Object)
                        {
                            inventoryItemLocation invItem = entity
                                .inventoryItemLocation
                                .Find(il.id);
                            invItem.amount = il.amount;
                            invItem.isDestroyed = il.isDestroyed;
                            invItem.isFalls = il.isFalls;
                            invItem.amountDestroyed = il.amountDestroyed;
                            invItem.cause = il.cause;
                            invItem.notes = il.notes;
                            invItem.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            invItem.updateUserId = il.updateUserId;
                            message = invItem.id.ToString();
                        }
                        entity.SaveChanges();
                        return TokenManager.GenerateToken(message);
                    }
                }
                message = "0";
                return TokenManager.GenerateToken(message);
            }
        }

        public string destroyItem(inventoryItemLocation Object)
        {
            string message = "";
            using (incposdbEntities entity = new incposdbEntities())
            {
                var unitEntity = entity.Set<pos>();

                var tmpItem = entity
                    .inventoryItemLocation
                    .Where(p => p.id == Object.id)
                    .FirstOrDefault();
                tmpItem.notes = Object.notes;
                tmpItem.isDestroyed = true;
                tmpItem.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                tmpItem.updateUserId = Object.updateUserId;
                tmpItem.cause = Object.cause;
                entity.SaveChanges();
                message = tmpItem.id.ToString();
                return message;
            }
        }

        [HttpPost]
        [Route("fallItem")]
        public string fallItem(string token)
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
                    string newObject = "";
                    inventoryItemLocation Object = null;
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "itemObject")
                        {
                            newObject = c.Value.Replace("\\", string.Empty);
                            newObject = newObject.Trim('"');
                            Object = JsonConvert.DeserializeObject<inventoryItemLocation>(
                                newObject,
                                new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                            );
                            break;
                        }
                    }
                    message = fallItem(Object);
                    //using (incposdbEntities entity = new incposdbEntities())
                    //{
                    //    var unitEntity = entity.Set<pos>();

                    //    var tmpItem = entity.inventoryItemLocation.Where(p => p.id == Object.id).FirstOrDefault();
                    //    tmpItem.notes = Object.notes;
                    //    tmpItem.isFalls = true;
                    //    tmpItem.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    //    tmpItem.updateUserId = Object.updateUserId;
                    //    tmpItem.fallCause = Object.fallCause;
                    //    message = tmpItem.id.ToString();

                    //    entity.SaveChanges();
                    //}
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
            return TokenManager.GenerateToken(message);
        }

        public string fallItem(inventoryItemLocation Object)
        {
            string message = "";
            using (incposdbEntities entity = new incposdbEntities())
            {
                var unitEntity = entity.Set<pos>();

                var tmpItem = entity
                    .inventoryItemLocation
                    .Where(p => p.id == Object.id)
                    .FirstOrDefault();
                tmpItem.notes = Object.notes;
                tmpItem.isFalls = true;
                tmpItem.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                tmpItem.updateUserId = Object.updateUserId;
                tmpItem.fallCause = Object.fallCause;
                message = tmpItem.id.ToString();

                entity.SaveChanges();
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
                            inventoryItemLocation Deleterow = entity.inventoryItemLocation.Find(id);
                            entity.inventoryItemLocation.Remove(Deleterow);
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
                            inventoryItemLocation Obj = entity.inventoryItemLocation.Find(id);
                            Obj.isActive = 0;
                            Obj.updateUserId = userId;
                            Obj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
