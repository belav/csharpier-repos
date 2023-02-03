using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;

using Newtonsoft.Json.Converters;
using LinqKit;
using System.Threading.Tasks;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/ItemsLocations")]
    public class ItemsLocationsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        ItemsUnitsController itemsUnitsController = new ItemsUnitsController();
        GroupObjectController group = new GroupObjectController();
        NotificationController notificationController = new NotificationController();
        notificationUserController notUserController = new notificationUserController();

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
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageList = (from b in entity.itemsLocations
                                            where b.quantity > 0 && b.invoiceId == null
                                            join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                            join i in entity.items on u.itemId equals i.itemId
                                            join l in entity.locations on b.locationId equals l.locationId
                                            join s in entity.sections on l.sectionId equals s.sectionId
                                            where s.branchId == branchId && s.isFreeZone != 1 && s.isKitchen != 1

                                            select new ItemLocationModel
                                            {
                                                createDate = b.createDate,
                                                createUserId = b.createUserId,
                                                endDate = b.endDate,
                                                itemsLocId = b.itemsLocId,
                                                itemUnitId = b.itemUnitId,
                                                locationId = b.locationId,
                                                notes = b.notes,
                                                quantity = b.quantity,
                                                startDate = b.startDate,

                                                updateDate = b.updateDate,
                                                updateUserId = b.updateUserId,
                                                itemName = i.name,
                                                location = l.x + l.y + l.z,
                                                section = s.name,
                                                sectionId = s.sectionId,
                                                itemType = i.type,
                                                unitName = u.units.name,
                                                invoiceId = b.invoiceId,
                                            }).ToList().OrderBy(x => x.location).ToList();


                        return TokenManager.GenerateToken(docImageList);

                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }
        [HttpPost]
        [Route("GetItemsHasQuantity")]
        public string GetItemsHasQuantity(string token)
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
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemsList = (from b in entity.itemsLocations
                                            where b.quantity > 0 && b.invoiceId == null 
                                            join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                            join I in entity.items on u.itemId equals I.itemId
                                            join l in entity.locations on b.locationId equals l.locationId
                                            join s in entity.sections on l.sectionId equals s.sectionId
                                            where s.branchId == branchId && s.isKitchen != 1

                                            select new ItemModel()
                                            {
                                                itemId = I.itemId,
                                                name = I.name,
                                                unitName = u.units.name,
                                                type = I.type,
                                                isActive = I.isActive,
                                                avgPurchasePrice = I.avgPurchasePrice,
                                            }).Where(x => x.isActive == 1).Distinct().ToList();


                        return TokenManager.GenerateToken(itemsList);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public string checkLocationAmounts(long itemLocId, long quantity)
        {
            string res = "";
            using (incposdbEntities entity = new incposdbEntities())
            {
                var locationQuantity = entity.itemsLocations.Find(itemLocId).quantity;
                if (locationQuantity < quantity)
                    return "-3";
            }
            return res;
        }

        [HttpPost]
        [Route("GetLockedItems")]
        public string GetLockedItems(string token)
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
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageList = (from b in entity.itemsLocations
                                            where b.quantity > 0 && b.invoiceId != null
                                            join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                            join i in entity.items on u.itemId equals i.itemId
                                            join l in entity.locations on b.locationId equals l.locationId
                                            where l.sections.branchId == branchId

                                            select new ItemLocationModel
                                            {
                                                createDate = b.createDate,
                                                createUserId = b.createUserId,
                                                endDate = b.endDate,
                                                itemsLocId = b.itemsLocId,
                                                itemUnitId = b.itemUnitId,
                                                locationId = b.locationId,
                                                notes = b.notes,
                                                quantity = b.quantity,
                                                startDate = b.startDate,

                                                updateDate = b.updateDate,
                                                updateUserId = b.updateUserId,
                                                itemName = i.name,
                                                location = l.x + l.y + l.z,
                                                section = l.sections.name,
                                                sectionId = l.sectionId,
                                                itemType = i.type,
                                                unitName = u.units.name,
                                                invoiceId = b.invoiceId,
                                                invNumber = b.invoices.invNumber,
                                            }).ToList().OrderBy(x => x.location).ToList();


                        return TokenManager.GenerateToken(docImageList);

                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
        {
            //long branchId string token
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
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageList = (from b in entity.itemsLocations
                                            where b.quantity > 0 && b.invoiceId == null
                                            join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                            join i in entity.items on u.itemId equals i.itemId
                                            join l in entity.locations on b.locationId equals l.locationId
                                            join s in entity.sections on l.sectionId equals s.sectionId
                                            where s.branchId == branchId && s.isKitchen != 1

                                            select new ItemLocationModel
                                            {
                                                createDate = b.createDate,
                                                createUserId = b.createUserId,
                                                endDate = b.endDate,
                                                itemsLocId = b.itemsLocId,
                                                itemUnitId = b.itemUnitId,
                                                locationId = b.locationId,
                                                notes = b.notes,
                                                quantity = b.quantity,
                                                startDate = b.startDate,

                                                updateDate = b.updateDate,
                                                updateUserId = b.updateUserId,
                                                itemName = i.name,
                                                location = l.x + l.y + l.z,
                                                section = s.name,
                                                sectionId = s.sectionId,
                                                itemType = i.type,
                                                unitName = u.units.name,
                                                invoiceId = b.invoiceId,
                                            }).ToList().OrderBy(x => x.location).ToList();

                        return TokenManager.GenerateToken(docImageList);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }
        [HttpPost]
        [Route("getAmountByItemLocId")]
        public string getAmountByItemLocId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemLocId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemLocId")
                    {
                        itemLocId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemLoc = entity.itemsLocations.Find(itemLocId);

                        if (itemLoc == null)
                            //  return NotFound();
                            return TokenManager.GenerateToken("0");
                        else
                            //   return Ok(itemLoc.quantity);

                            return TokenManager.GenerateToken(itemLoc.quantity.ToString());
                    }



                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }      
        }
        [HttpPost]
        [Route("getWithSequence")]
        public string getWithSequence(string token)
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
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }


                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemLocList = (from b in entity.itemsLocations
                                           where b.quantity > 0 && b.invoiceId == null
                                           join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                           join un in entity.units on u.unitId equals un.unitId
                                           join i in entity.items on u.itemId equals i.itemId
                                           join l in entity.locations on b.locationId equals l.locationId
                                           join s in entity.sections on l.sectionId equals s.sectionId
                                           where s.branchId == branchId && s.isFreeZone != 1 && s.isKitchen != 1

                                           select new ItemLocationModel
                                           {
                                               createDate = b.createDate,
                                               createUserId = b.createUserId,
                                               endDate = b.endDate,
                                               itemsLocId = b.itemsLocId,
                                               itemUnitId = b.itemUnitId,
                                               locationId = b.locationId,
                                               notes = b.notes,
                                               quantity = b.quantity,
                                               startDate = b.startDate,

                                               updateDate = b.updateDate,
                                               updateUserId = b.updateUserId,
                                               itemName = i.name,
                                               location = l.x + l.y + l.z,
                                               section = s.name,
                                               unitName = un.name,
                                               sectionId = s.sectionId,
                                               itemType = i.type,
                                           }).ToList();
                        int sequence = 1;
                        foreach (ItemLocationModel i in itemLocList)
                        {
                            i.sequence = sequence;
                            sequence++;
                        }
                        return TokenManager.GenerateToken(itemLocList);

                    }

                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            
        }
        [HttpPost]
        [Route("GetFreeZoneItems")]
        public string GetFreeZoneItems(string token)
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
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }


                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageList = (from b in entity.itemsLocations
                                            where b.quantity > 0 && b.invoiceId == null
                                            join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                            join i in entity.items on u.itemId equals i.itemId
                                            join l in entity.locations on b.locationId equals l.locationId
                                            join s in entity.sections on l.sectionId equals s.sectionId
                                            where s.branchId == branchId && s.isFreeZone == 1 && s.isKitchen != 1

                                            select new ItemLocationModel
                                            {
                                                createDate = b.createDate,
                                                createUserId = b.createUserId,
                                                endDate = b.endDate,
                                                itemsLocId = b.itemsLocId,
                                                itemUnitId = b.itemUnitId,
                                                locationId = b.locationId,
                                                notes = b.notes,
                                                quantity = b.quantity,
                                                startDate = b.startDate,

                                                updateDate = b.updateDate,
                                                updateUserId = b.updateUserId,
                                                itemName = i.name,
                                                sectionId = s.sectionId,
                                                isFreeZone = s.isFreeZone,
                                                itemType = i.type,
                                                location = l.x + l.y + l.z,
                                                section = s.name,
                                                unitName = u.units.name,
                                            })
                                        .ToList();


                        return TokenManager.GenerateToken(docImageList);

                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }          
        }
        [HttpPost]
        [Route("GetByItemUnitId")]
        public string GetByItemUnitId(string token)
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
                long locationId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                    else if (c.Type == "locationId")
                    {
                        locationId = long.Parse(c.Value);
                    }



                }
                try
                {

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageList = entity.itemsLocations
                            .Where(b => b.itemUnitId == itemUnitId && b.locationId == locationId && b.invoiceId == null && b.locations.isKitchen != 1)
                            .Select(b => new
                            {
                                b.createDate,
                                b.createUserId,
                                b.endDate,
                                b.itemsLocId,
                                b.itemUnitId,
                                b.locationId,
                                b.notes,
                                b.quantity,
                                b.startDate,

                                b.updateDate,
                                b.updateUserId,
                            })
                        .ToList();

                        return TokenManager.GenerateToken(docImageList);

                    }

                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }


        [HttpPost]
        [Route("save")]
        public string save(string token)
        {
            string message = "";



            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                List<itemsLocations> newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsLocations>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }
                if (newObject != null)
                {



                    try
                    {
                        itemsLocations item;
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (itemsLocations itemLoc in newObject)
                            {
                                if (itemLoc.updateUserId == 0 || itemLoc.updateUserId == null)
                                {
                                    Nullable<long> id = null;
                                    itemLoc.updateUserId = id;
                                }
                                if (itemLoc.createUserId == 0 || itemLoc.createUserId == null)
                                {
                                    Nullable<long> id = null;
                                    itemLoc.createUserId = id;
                                }
                                var itemEntity = entity.Set<itemsLocations>();
                                item = itemEntity.Find(itemLoc.itemUnitId, itemLoc.locationId);
                                if (item == null)
                                {
                                    itemLoc.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemLoc.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemLoc.updateUserId = itemLoc.createUserId;

                                    item = itemEntity.Add(itemLoc);
                                }
                                else
                                {
                                    item.quantity = itemLoc.quantity;
                                    item.startDate = itemLoc.startDate;
                                    item.endDate = itemLoc.endDate;
                                    item.notes = itemLoc.notes;
                                    item.invoiceId = itemLoc.invoiceId;
                                    item.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    item.updateUserId = itemLoc.updateUserId;
                                }
                            }

                            message = entity.SaveChanges().ToString();
                            //  return true;
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
                    return TokenManager.GenerateToken("0");
                }


            }        
        }
        [HttpPost]
        [Route("receiptInvoice")]
        public async Task<string> receiptInvoice(string token)
        {
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long branchId = 0;
                long userId = 0;
                string objectName = "";
                string notificationObj = "";

                List<itemsTransfer> newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "branchId")
                        branchId = long.Parse(c.Value);
                    else if (c.Type == "userId")
                        userId = long.Parse(c.Value);
                    else if (c.Type == "objectName")
                        objectName = c.Value;
                    else if (c.Type == "notificationObj")
                        notificationObj = c.Value;
                }

                if (newObject != null)
                {
                try
                {
                       await receiptInvoice(branchId,newObject,userId,objectName,notificationObj);
                    //using (incposdbEntities entity = new incposdbEntities())
                    //    {
                    //        var freeZoneLocation = (from s in entity.sections.Where(x => x.branchId == branchId && x.isFreeZone == 1 && x.isKitchen != 1)
                    //                                join l in entity.locations on s.sectionId equals l.sectionId
                    //                                select l.locationId).SingleOrDefault();
                    //        foreach (itemsTransfer item in newObject)
                    //        {
                    //            var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();
                    //            var itemV = entity.items.Find(itemId);
                           
                    //            if (item.invoiceId == 0 || item.invoiceId == null)
                    //                increaseItemQuantity(item.itemUnitId.Value, freeZoneLocation, (int)item.quantity, userId);
                    //            else//for order
                    //                increaseLockedItem(item.itemUnitId.Value, freeZoneLocation, (int)item.quantity, (long)item.invoiceId, userId);
                    //            if(item.offerId != 0 && item.offerId != null)
                    //            {
                    //                long offerId = (long)item.offerId;
                    //                long itemUnitId = (long)item.itemUnitId;
                    //                var offer = entity.itemsOffers.Where(x => x.iuId == itemUnitId && x.offerId == offerId).FirstOrDefault();
                    //                offer.used -= (int)item.quantity;
                    //                entity.SaveChanges();
                    //            }
                    //            bool isExcedded = isExceddMaxQuantity((long)item.itemUnitId, branchId, userId);
                    //            if (isExcedded == true) //add notification
                    //            {
                    //                notificationController.addNotifications(objectName, notificationObj, branchId, itemV.name);
                    //            }
                    //        }

                    //    }
                        return TokenManager.GenerateToken("1");
            }
                catch
            {
                message = "0";
                return TokenManager.GenerateToken(message);
            }
        }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
        }
    }

        public async Task receiptInvoice(long branchId, List<itemsTransfer> newObject, long userId, string objectName, string notificationObj)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var freeZoneLocation = (from s in entity.sections.Where(x => x.branchId == branchId && x.isFreeZone == 1 && x.isKitchen != 1)
                                        join l in entity.locations on s.sectionId equals l.sectionId
                                        select l.locationId).SingleOrDefault();
                foreach (itemsTransfer item in newObject)
                {
                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();
                    var itemV = entity.items.Find(itemId);

                   // if (item.invoiceId == 0 || item.invoiceId == null)
                        increaseItemQuantity(item.itemUnitId.Value, freeZoneLocation, (int)item.quantity, userId);
                    //else//for order
                    //    increaseLockedItem(item.itemUnitId.Value, freeZoneLocation, (int)item.quantity, (long)item.invoiceId, userId);
                    if (item.offerId != 0 && item.offerId != null)
                    {
                        long offerId = (long)item.offerId;
                        long itemUnitId = (long)item.itemUnitId;
                        var offer = entity.itemsOffers.Where(x => x.iuId == itemUnitId && x.offerId == offerId).FirstOrDefault();
                        offer.used -= (int)item.quantity;
                        entity.SaveChanges();
                    }
                    bool isExcedded = isExceddMaxQuantity((long)item.itemUnitId, branchId, userId);
                    if (isExcedded == true) //add notification
                    {
                        notificationController.addNotifications(objectName, notificationObj, branchId, itemV.name);
                    }
                }

            }
        }
        public bool isExceddMaxQuantity(long itemUnitId, long branchId, long userId)
        {
            bool isExcedded = false;
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => x.itemId).Single();
                    var item = entity.items.Find(itemId);
                    long maxUnitId = (long)item.maxUnitId;
                    int maxQuantity = (int)item.max;
                    if (maxQuantity == 0)
                        return false;
                    var maxUnit = entity.itemsUnits.Where(x => x.itemId == itemId && x.unitId == maxUnitId).FirstOrDefault();
                    if (maxUnit == null)
                        isExcedded = false;
                    else
                    {
                        int itemUnitQuantity = getItemAmount(maxUnit.itemUnitId, branchId);
                        if (itemUnitQuantity >= maxQuantity)
                        {
                            isExcedded = true;
                        }
                        if (isExcedded == false)
                        {
                            long smallestItemUnit = entity.itemsUnits.Where(x => x.itemId == itemId && x.subUnitId == x.unitId).Select(x => x.itemUnitId).Single();
                            int smallUnitQuantity = getLevelItemUnitAmount(smallestItemUnit, maxUnit.itemUnitId, branchId);
                            int unitValue = itemsUnitsController.getLargeUnitConversionQuan(smallestItemUnit, maxUnit.itemUnitId);
                            int quantity = 0;
                            if (unitValue != 0)
                                quantity = smallUnitQuantity / unitValue;

                            quantity += itemUnitQuantity;
                            if (quantity >= maxQuantity)
                            {
                                isExcedded = true;
                            }
                        }

                    }
                }
            }
            catch
            {
            }
            return isExcedded;
        }

        [HttpPost]
        [Route("generatePackage")]
        public string generatePackage(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                long packageParentId = 0;
                int quantity = 0;
                long locationId = 0;
                long branchId = 0;
                long userId = 0;

            IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
            foreach (Claim c in claims)
            {
                if (c.Type == "branchId")
                {
                    branchId = long.Parse(c.Value);

                }
                else if (c.Type == "userId")
                {
                    userId = long.Parse(c.Value);

                }
                else if (c.Type == "locationId")
                {
                    locationId = long.Parse(c.Value);

                }
                else if (c.Type == "quantity")
                {
                    quantity = int.Parse(c.Value);

                }
                else if (c.Type == "packageParentId")
                {
                    packageParentId = long.Parse(c.Value);

                }
            }
            try
            {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var packageIems = (from s in entity.packages.Where(x => x.parentIUId == packageParentId)
                                           select new PackageModel
                                           {
                                               childIUId = s.childIUId,
                                               quantity = s.quantity
                                           }).ToList();
                        foreach (PackageModel item in packageIems)
                        {
                            int itemQuantity = item.quantity * quantity;
                            long itemUnitId = (long)item.childIUId;
                            convertToPackage(itemUnitId,  branchId, itemQuantity, userId);

                            var itemInLocs = (from b in entity.branches
                                              where b.branchId == branchId
                                              join s in entity.sections on b.branchId equals s.branchId
                                              join l in entity.locations on s.sectionId equals l.sectionId
                                              join il in entity.itemsLocations on l.locationId equals il.locationId
                                              where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && s.isKitchen != 1
                                              select new
                                              {
                                                  il.itemsLocId,
                                                  il.quantity,
                                                  il.itemUnitId,
                                                  il.locationId,
                                                  s.sectionId,
                                              }).ToList();

                            for (int i = 0; i < itemInLocs.Count; i++)
                            {
                                int availableAmount = (int)itemInLocs[i].quantity;
                                var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                                itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                if (availableAmount >= itemQuantity)
                                {
                                    itemL.quantity = availableAmount - itemQuantity;
                                    itemQuantity = 0;
                                    entity.SaveChanges();
                                }
                                else if (availableAmount > 0)
                                {
                                    itemL.quantity = 0;
                                    itemQuantity = itemQuantity - availableAmount;
                                    entity.SaveChanges();
                                }
                                if (itemQuantity == 0)
                                    break;
                            }                          
                        }
                        increaseItemQuantity(packageParentId, locationId, quantity, userId);
                    }
                    return TokenManager.GenerateToken("1");
            }
            catch
            {
                message = "0";
                return TokenManager.GenerateToken(message);
            }
        }
    }
        public void convertToPackage(long itemUnitId,  long branchId, int requiredAmount, long userId)
        {
            long locationId = 0;
            Dictionary<string, long> dic = new Dictionary<string, long>();
            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemInLocs = (from s in entity.sections
                                  where s.branchId == branchId
                                  join l in entity.locations on s.sectionId equals l.sectionId
                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                  where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && s.isKitchen != 1
                                  select new
                                  {
                                      il.itemsLocId,
                                      il.quantity,
                                      il.itemUnitId,
                                      il.locationId,
                                      il.updateDate,
                                      s.sectionId,
                                  }).ToList().OrderBy(x => x.updateDate).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    int availableAmount = (int)itemInLocs[i].quantity;
                    int lockedAmount = 0;
                    var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    if (availableAmount >= requiredAmount)
                    {
                        lockedAmount = requiredAmount;
                        requiredAmount = 0;
                    }
                    else if (availableAmount > 0)
                    {
                        requiredAmount = requiredAmount - availableAmount;
                        lockedAmount = availableAmount;
                        entity.SaveChanges();
                    }
                    if (requiredAmount == 0)
                        break;
                }

                if (requiredAmount != 0)
                {
                    dic = lockUpperUnit(itemUnitId, branchId, requiredAmount, userId);

                     if ((dic["remainQuantity"] + dic["lockedQuantity"])> 0)
                    {
                        var item = (from il in entity.itemsLocations
                                    where il.itemUnitId == itemUnitId && il.invoiceId == null
                                    join l in entity.locations on il.locationId equals l.locationId
                                    join s in entity.sections on l.sectionId equals s.sectionId
                                    where s.branchId == branchId
                                    select new
                                    {
                                        il.itemsLocId,
                                    }).FirstOrDefault();
                        if (item != null)
                        {
                            var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                            itemloc.quantity += dic["remainQuantity"] + dic["lockedQuantity"];
                            entity.SaveChanges();
                        }
                        else
                        {
                            var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                            locationId = dic["locationId"];
                            if ((locationId == 0 && locationId == null) && locations.Count > 1)
                                locationId = locations[0].locationId; // free zoon
                            itemsLocations itemL = new itemsLocations();
                            itemL.itemUnitId = itemUnitId;
                            itemL.locationId = locationId;
                            itemL.quantity = dic["remainQuantity"] + dic["lockedQuantity"];
                            itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.createUserId = userId;
                            itemL.updateUserId = userId;
                            itemL.invoiceId = null;

                            entity.itemsLocations.Add(itemL);
                            entity.SaveChanges();
                        }
                    }

                    if (dic["requiredQuantity"] > 0)
                    {
                        dic = lockLowerUnit(itemUnitId, branchId, dic["requiredQuantity"], userId);
                        if (dic["lockedQuantity"] > 0)
                        {
                            var item = (from il in entity.itemsLocations
                                        where il.itemUnitId == itemUnitId && il.invoiceId == null
                                        join l in entity.locations on il.locationId equals l.locationId
                                        join s in entity.sections on l.sectionId equals s.sectionId
                                        where s.branchId == branchId
                                        select new
                                        {
                                            il.itemsLocId,
                                        }).FirstOrDefault();
                            if (item != null)
                            {
                                var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                                itemloc.quantity += dic["remainQuantity"];
                                entity.SaveChanges();
                            }
                            else
                            {
                                var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                                locationId = dic["locationId"];
                                if ((locationId == 0 && locationId == null) && locations.Count > 1)
                                    locationId = locations[0].locationId; // free zoon
                                itemsLocations itemL = new itemsLocations();
                                itemL.itemUnitId = itemUnitId;
                                itemL.locationId = locationId;
                                itemL.quantity = dic["remainQuantity"];
                                itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                itemL.createUserId = userId;
                                itemL.updateUserId = userId;
                                itemL.invoiceId = null;

                                entity.itemsLocations.Add(itemL);
                                entity.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
        [HttpPost]
        [Route("receiptOrder")]
        public string receiptOrder(string token)
        {
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                string orderList = "";

                long toBranch = 0;
                long userId = 0;
                string objectName = "";
                string notificationObj = "";

                List<itemsLocations> newObject = new List<itemsLocations>();
                List<itemsTransfer> items = new List<itemsTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsLocations>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "orderList")
                    {
                        orderList = c.Value.Replace("\\", string.Empty);
                        orderList = orderList.Trim('"');
                        items = JsonConvert.DeserializeObject<List<itemsTransfer>>(orderList, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "toBranch")
                    {
                        toBranch = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "objectName")
                    {
                        objectName = c.Value;
                    }
                    else if (c.Type == "notificationObj")
                    {
                        notificationObj = c.Value;
                    }

                }

                if (newObject != null)
                {
                    try
                    {
                        receiptOrder(newObject, items, toBranch, userId, objectName, notificationObj);
                        return TokenManager.GenerateToken("1");
                        //using (incposdbEntities entity = new incposdbEntities())
                        //{
                        //    var freeZoneLocation = (from s in entity.sections.Where(x => x.branchId == toBranch && x.isFreeZone == 1 && x.isKitchen != 1)
                        //                            join l in entity.locations on s.sectionId equals l.sectionId
                        //                            select l.locationId).SingleOrDefault();
                        //    foreach (itemsLocations item in newObject)
                        //    {
                        //        itemsLocations itemL = new itemsLocations();

                        //        itemL = entity.itemsLocations.Find(item.itemsLocId);
                        //        itemL.quantity -= item.quantity;
                        //        itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        //        itemL.updateUserId = userId;
                        //        entity.SaveChanges();

                        //        var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();

                        //        var itemV = entity.items.Find(itemId);
                        //        int quantity = (int)item.quantity;
                        //        foreach (itemsTransfer it in items)
                        //        {
                        //            if (it.itemUnitId == item.itemUnitId && it.invoiceId != 0 && it.invoiceId != null)//for order
                        //            {
                        //                int itemQuantity = 0;
                        //                if (quantity >= item.quantity)
                        //                {
                        //                    itemQuantity = (int)item.quantity;
                        //                    quantity -= (int)item.quantity;
                        //                    item.quantity = quantity;
                        //                    it.quantity = 0;
                        //                }
                        //                else
                        //                {
                        //                    itemQuantity = quantity;
                        //                    quantity = 0;
                        //                    it.quantity -= quantity;
                        //                }
                        //                increaseLockedItem(item.itemUnitId.Value, freeZoneLocation, itemQuantity, (long)it.invoiceId, userId);
                        //            }
                        //        }
                        //        if (quantity != 0)
                        //            increaseItemQuantity(item.itemUnitId.Value, freeZoneLocation, quantity, userId);

                        //        bool isExcedded = isExceddMaxQuantity((long)item.itemUnitId, toBranch, userId);
                        //        if (isExcedded == true) //add notification
                        //        {
                        //            notificationController.addNotifications(objectName, notificationObj, toBranch, itemV.name);
                        //        }
                        //    }
                        //    return TokenManager.GenerateToken("1");
                        //}
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
            }          
        }

        public void receiptOrder(List<itemsLocations> newObject, List<itemsTransfer> items, long toBranch, long userId, string objectName, string notificationObj)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var freeZoneLocation = (from s in entity.sections.Where(x => x.branchId == toBranch && x.isFreeZone == 1 && x.isKitchen != 1)
                                        join l in entity.locations on s.sectionId equals l.sectionId
                                        select l.locationId).SingleOrDefault();
                foreach (itemsLocations item in newObject)
                {
                    itemsLocations itemL = new itemsLocations();

                    itemL = entity.itemsLocations.Find(item.itemsLocId);
                    itemL.quantity -= item.quantity;
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.updateUserId = userId;
                    entity.SaveChanges();

                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();

                    var itemV = entity.items.Find(itemId);
                    int quantity = (int)item.quantity;
                    foreach (itemsTransfer it in items)
                    {
                        if (it.itemUnitId == item.itemUnitId && it.invoiceId != 0 && it.invoiceId != null)//for order
                        {
                            int itemQuantity = 0;
                            if (quantity >= item.quantity)
                            {
                                itemQuantity = (int)item.quantity;
                                quantity -= (int)item.quantity;
                                item.quantity = quantity;
                                it.quantity = 0;
                            }
                            else
                            {
                                itemQuantity = quantity;
                                quantity = 0;
                                it.quantity -= quantity;
                            }
                            increaseLockedItem(item.itemUnitId.Value, freeZoneLocation, itemQuantity, (long)it.invoiceId, userId);
                        }
                    }
                    if (quantity != 0)
                        increaseItemQuantity(item.itemUnitId.Value, freeZoneLocation, quantity, userId);

                    bool isExcedded = isExceddMaxQuantity((long)item.itemUnitId, toBranch, userId);
                    if (isExcedded == true) //add notification
                    {
                        notificationController.addNotifications(objectName, notificationObj, toBranch, itemV.name);
                    }
                }
                
            }
        }

        public string checkItemsAmounts(List<ItemTransferModel> billDetails, long branchId,int isKitchen)
        {
            string res = "";
            ItemsOffersController ioc = new ItemsOffersController();
            foreach (var item in billDetails)
            {
                int availableAmount = getAmountInBranch((long)item.itemUnitId, branchId,isKitchen);
                if (item.offerId != 0 && item.offerId != null)
                {
                    int remainAmount = ioc.getRemain((long)item.offerId, (long)item.itemUnitId);
                    if ((availableAmount < item.quantity || remainAmount < item.quantity))
                    {
                        res = item.itemName == null? "": item.itemName;
                        return res;
                    }
                }
                else if (availableAmount < item.quantity && item.itemType != "sr")
                {
                    res = item.itemName;
                    return res;
                }
            }
            return res;
        }
       
        [HttpPost]
        [Route("transferToKitchen")]
        public string transferToKitchen(string token)
        {
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                string orderList = "";

                long branchId = 0;
                long userId = 0;

                List<itemsLocations> newObject = new List<itemsLocations>();
                List<itemsTransfer> items = new List<itemsTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsLocations>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "orderList")
                    {
                        orderList = c.Value.Replace("\\", string.Empty);
                        orderList = orderList.Trim('"');
                        items = JsonConvert.DeserializeObject<List<itemsTransfer>>(orderList, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }

                }
                #endregion
                if (newObject != null)
                {
                    try
                    {
                        transferToKitchen(newObject, branchId, userId);
                        return TokenManager.GenerateToken("1");

                        //using (incposdbEntities entity = new incposdbEntities())
                        //{
                        //    var kitchenLocation = (from s in entity.sections.Where(x => x.branchId == branchId &&  x.isKitchen == 1)
                        //                            join l in entity.locations on s.sectionId equals l.sectionId
                        //                            select l.locationId).SingleOrDefault();
                        //    foreach (itemsLocations item in newObject)
                        //    {
                        //        itemsLocations itemL = new itemsLocations();

                        //        itemL = entity.itemsLocations.Find(item.itemsLocId);
                        //        itemL.quantity -= item.quantity;
                        //        itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        //        itemL.updateUserId = userId;
                        //        entity.SaveChanges();

                        //        var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();

                        //        var itemV = entity.items.Find(itemId);
                        //        int quantity = (int)item.quantity;
                               
                        //        if (quantity != 0)
                        //            increaseItemQuantity(item.itemUnitId.Value, kitchenLocation, quantity, userId);
                        //    }
                        //    return TokenManager.GenerateToken("1");
                        //}
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public void transferToKitchen(List<itemsLocations> newObject,long branchId,long userId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var kitchenLocation = (from s in entity.sections.Where(x => x.branchId == branchId && x.isKitchen == 1)
                                       join l in entity.locations on s.sectionId equals l.sectionId
                                       select l.locationId).SingleOrDefault();
                foreach (itemsLocations item in newObject)
                {
                    itemsLocations itemL = new itemsLocations();

                    itemL = entity.itemsLocations.Find(item.itemsLocId);
                    itemL.quantity -= item.quantity;
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.updateUserId = userId;
                    entity.SaveChanges();

                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();

                    var itemV = entity.items.Find(itemId);
                    int quantity = (int)item.quantity;

                    if (quantity != 0)
                        increaseItemQuantity(item.itemUnitId.Value, kitchenLocation, quantity, userId);
                }
            }
        }
     
        [HttpPost]
        [Route("transferAmountbetweenUnits")]
        public string transferAmountbetweenUnits(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long locationId = 0;
                long itemLocId = 0;
                long toItemUnitId = 0;
                int fromQuantity = 0;
                int toQuantity = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "locationId")
                    {
                        locationId = long.Parse(c.Value);

                    }
                    else if (c.Type == "itemLocId")
                    {
                        itemLocId = long.Parse(c.Value);

                    }
                    else if (c.Type == "toItemUnitId")
                    {
                        toItemUnitId = long.Parse(c.Value);

                    }
                    else if (c.Type == "fromQuantity")
                    {
                        fromQuantity = int.Parse(c.Value);

                    }
                    else if (c.Type == "toQuantity")
                    {
                        toQuantity = int.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        decreaseItemLocationQuantity(itemLocId, fromQuantity, userId, "", "");
                        increaseItemQuantity(toItemUnitId, locationId, toQuantity, userId);
                    }
                    //  return Ok(1);
                    return TokenManager.GenerateToken("1");
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }

            }
        }
        private void increaseItemQuantity(long itemUnitId, long locationId, int quantity, long userId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemUnit = (from il in entity.itemsLocations
                                where il.itemUnitId == itemUnitId && il.locationId == locationId && il.invoiceId == null
                                select new { il.itemsLocId }
                                ).FirstOrDefault();
                itemsLocations itemL = new itemsLocations();
                if (itemUnit == null)//add item in new location
                {
                    itemL.itemUnitId = itemUnitId;
                    itemL.locationId = locationId;
                    itemL.quantity = quantity;
                    itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.createUserId = userId;
                    itemL.updateUserId = userId;

                    entity.itemsLocations.Add(itemL);
                }
                else
                {
                    itemL = entity.itemsLocations.Find(itemUnit.itemsLocId);
                    itemL.quantity += quantity;
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.updateUserId = userId;
                }
                entity.SaveChanges();
            }
        }


        [HttpPost]
        [Route("trasnferItem")]
        public string trasnferItem(string token)
        {
            string message = "";



            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                string Object = "";
                long itemLocId = 0;

                ItemLocationModel newObject = new ItemLocationModel();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemLocId")
                    {
                        itemLocId = long.Parse(c.Value);

                    }

                    else if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<ItemLocationModel>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                }

                if (newObject != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var oldItemL = entity.itemsLocations.Find(itemLocId);
                            long userId = (long)newObject.updateUserId;
                            long newQuantity = (long)oldItemL.quantity - (long)newObject.quantity;
                            oldItemL.quantity = (long)newQuantity;
                            oldItemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            oldItemL.updateUserId = userId;


                            var newtemLocation = (from il in entity.itemsLocations
                                                  where il.itemUnitId == newObject.itemUnitId && il.locationId == newObject.locationId
                                                  && il.startDate == newObject.startDate && il.endDate == newObject.endDate && il.invoiceId == newObject.invoiceId && il.locations.isKitchen != 1
                                                  select new { il.itemsLocId }
                                           ).FirstOrDefault();

                            itemsLocations newItemL;
                            if (newtemLocation == null)//add item in new location
                            {
                                newItemL = new itemsLocations();
                                newItemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                newItemL.createUserId = (long)newObject.createUserId;
                                if (newObject.endDate != null)
                                    newItemL.endDate = newObject.endDate;
                                if (newObject.startDate != null)
                                    newItemL.startDate = newObject.startDate;
                                newItemL.updateDate = newItemL.createDate;
                                newItemL.updateUserId = (long)newObject.createUserId;
                                newItemL.itemUnitId = (long)newObject.itemUnitId;
                                newItemL.locationId = (long)newObject.locationId;
                                newItemL.notes = newObject.notes;
                                newItemL.quantity = (long)newObject.quantity;
                                newItemL.invoiceId = newObject.invoiceId;
                                entity.itemsLocations.Add(newItemL);
                            }
                            else
                            {
                                newItemL = new itemsLocations();
                                newItemL = entity.itemsLocations.Find(newtemLocation.itemsLocId);
                                newQuantity = (long)newItemL.quantity + (long)newObject.quantity;
                                newItemL.quantity = (long)newQuantity;
                                newItemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                newItemL.updateUserId = (long)newObject.updateUserId;

                            }
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
                    return TokenManager.GenerateToken("0");
                }

            }

            
        }

        [HttpPost]
        [Route("updateItemQuantity")]
        public string updateItemQuantity(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemUnitId = 0;
                long branchId = 0;
                int requiredAmount = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "requiredAmount")
                    {
                        requiredAmount = int.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                }
                try
                {
                    Dictionary<string, long> dic = new Dictionary<string, long>();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemInLocs = (from b in entity.branches
                                          where b.branchId == branchId
                                          join s in entity.sections on b.branchId equals s.branchId
                                          join l in entity.locations on s.sectionId equals l.sectionId
                                          join il in entity.itemsLocations on l.locationId equals il.locationId
                                          where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && s.isKitchen != 1
                                          select new
                                          {
                                              il.itemsLocId,
                                              il.quantity,
                                              il.itemUnitId,
                                              il.locationId,
                                              il.updateDate,
                                              s.sectionId,
                                          }).ToList().OrderBy(x => x.updateDate).ToList();
                        for (int i = 0; i < itemInLocs.Count; i++)
                        {
                            int availableAmount = (int)itemInLocs[i].quantity;
                            var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            if (availableAmount >= requiredAmount)
                            {
                                itemL.quantity = availableAmount - requiredAmount;
                                requiredAmount = 0;
                                entity.SaveChanges();
                            }
                            else if (availableAmount > 0)
                            {
                                itemL.quantity = 0;
                                requiredAmount = requiredAmount - availableAmount;
                                entity.SaveChanges();
                            }

                            if (requiredAmount == 0)

                                //  return Ok(3);
                                return TokenManager.GenerateToken("3");
                        }
                        if (requiredAmount != 0)
                        {
                            dic = checkUpperUnit(itemUnitId, branchId, requiredAmount, userId);

                            var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                            var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId && x.isActive == 1).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();


                            if (dic["remainQuantity"] > 0)
                            {
                                var item = (from il in entity.itemsLocations
                                            where il.itemUnitId == itemUnitId && il.invoiceId == null && il.locations.isKitchen != 1
                                            join l in entity.locations on il.locationId equals l.locationId
                                            join s in entity.sections on l.sectionId equals s.sectionId
                                            where s.branchId == branchId
                                            select new
                                            {
                                                il.itemsLocId,
                                            }).FirstOrDefault();
                                if (item != null)
                                {
                                    var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                                    itemloc.quantity = dic["remainQuantity"];
                                    entity.SaveChanges();
                                }
                                else
                                {
                                    var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();

                                    long locationId = dic["locationId"];
                                    if (locationId == 0 && locations.Count > 1)
                                        locationId = locations[0].locationId; // free zoon
                                    itemsLocations itemL = new itemsLocations();
                                    itemL.itemUnitId = itemUnitId;
                                    itemL.locationId = locationId;
                                    itemL.quantity = dic["remainQuantity"];
                                    itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemL.createUserId = userId;
                                    itemL.updateUserId = userId;

                                    entity.itemsLocations.Add(itemL);
                                    entity.SaveChanges();
                                }
                            }
                            if (dic["requiredQuantity"] > 0)
                            {
                                checkLowerUnit(itemUnitId, branchId, dic["requiredQuantity"], userId);
                            }

                        }
                    }
                    return TokenManager.GenerateToken("2");

                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }        
        }

        public int updateItemQuantity(long itemUnitId, long branchId, int requiredAmount, long userId, int isKitchen = 0)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            using (incposdbEntities entity = new incposdbEntities())
            {
                var searchPredicate = PredicateBuilder.New<sections>();
                searchPredicate = searchPredicate.And(x => x.isKitchen == isKitchen);
                var itemInLocs = (from b in entity.branches
                                  where b.branchId == branchId
                                  join s in entity.sections.Where(searchPredicate) on b.branchId equals s.branchId
                                  join l in entity.locations on s.sectionId equals l.sectionId
                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                  where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null 
                                  select new
                                  {
                                      il.itemsLocId,
                                      il.quantity,
                                      il.itemUnitId,
                                      il.locationId,
                                      il.updateDate,
                                      s.sectionId,
                                  }).ToList().OrderBy(x => x.updateDate).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    int availableAmount = (int)itemInLocs[i].quantity;
                    var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    if (availableAmount >= requiredAmount)
                    {
                        itemL.quantity = availableAmount - requiredAmount;
                        requiredAmount = 0;
                        entity.SaveChanges();
                    }
                    else if (availableAmount > 0)
                    {
                        itemL.quantity = 0;
                        requiredAmount = requiredAmount - availableAmount;
                        entity.SaveChanges();
                    }

                    if (requiredAmount == 0)
                        return (3);
                }
                if (requiredAmount != 0)
                {
                    dic = checkUpperUnit(itemUnitId, branchId, requiredAmount, userId,isKitchen);

                    var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                    var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId && x.isActive == 1).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();


                    if (dic["remainQuantity"] > 0)
                    {
                        var item = (from il in entity.itemsLocations
                                    where il.itemUnitId == itemUnitId && il.invoiceId == null 
                                    join l in entity.locations on il.locationId equals l.locationId
                                    join s in entity.sections.Where(searchPredicate) on l.sectionId equals s.sectionId
                                    where s.branchId == branchId
                                    select new
                                    {
                                        il.itemsLocId,
                                    }).FirstOrDefault();
                        if (item != null)
                        {
                            var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                            itemloc.quantity = dic["remainQuantity"];
                            entity.SaveChanges();
                        }
                        else
                        {
                            var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1 && x.isKitchen == isKitchen).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                            // if (locations.Count > 0)
                            // {
                            long locationId = dic["locationId"];
                            if (locationId == 0 && locations.Count > 1)
                                locationId = locations[0].locationId; // free zoon
                            itemsLocations itemL = new itemsLocations();
                            itemL.itemUnitId = itemUnitId;
                            itemL.locationId = locationId;
                            itemL.quantity = dic["remainQuantity"];
                            itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.createUserId = userId;
                            itemL.updateUserId = userId;

                            entity.itemsLocations.Add(itemL);
                            entity.SaveChanges();
                        }
                    }
                    if (dic["requiredQuantity"] > 0)
                    {
                        checkLowerUnit(itemUnitId, branchId, dic["requiredQuantity"], userId, isKitchen);
                    }

                }
            }
            return (2);

        }

        private Dictionary<string, long> checkUpperUnit(long itemUnitId, long branchId, long requiredAmount, long userId, int isKitchen = 0)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            dic.Add("remainQuantity", 0);
            dic.Add("locationId", 0);
            dic.Add("requiredQuantity", 0);
            dic.Add("isConsumed", 0);
            long remainQuantity = 0;
            long firstRequir = requiredAmount;
            decimal newQuant = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var searchPredicate = PredicateBuilder.New<sections>();
                searchPredicate = searchPredicate.And(x => x.isKitchen == isKitchen);

                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId && x.isActive == 1).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if (upperUnit != null)
                {
                    decimal unitValue = (decimal)upperUnit.unitValue;
                    int breakNum = (int)Math.Ceiling(requiredAmount / unitValue);
                    newQuant = (decimal)(breakNum * upperUnit.unitValue);
                    var itemInLocs = (from b in entity.branches
                                      where b.branchId == branchId
                                      join s in entity.sections.Where(searchPredicate) on b.branchId equals s.branchId
                                      join l in entity.locations on s.sectionId equals l.sectionId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == upperUnit.itemUnitId && il.quantity > 0 && il.invoiceId == null
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                          il.itemUnitId,
                                          il.locationId,
                                          il.updateDate,
                                          s.sectionId,
                                      }).ToList().OrderBy(x => x.updateDate).ToList();

                    for (int i = 0; i < itemInLocs.Count; i++)
                    {
                        dic["isConsumed"] = 1;
                        var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                        var smallUnitLocId = entity.itemsLocations.Where(x => x.itemUnitId == itemUnitId && x.invoiceId == null && x.locations.isKitchen == isKitchen).
                            Select(x => x.itemsLocId).FirstOrDefault();

                        if (breakNum <= itemInLocs[i].quantity)
                        {
                            itemL.quantity = itemInLocs[i].quantity - breakNum;
                            entity.SaveChanges();
                            remainQuantity = (int)newQuant - firstRequir;
                            requiredAmount = 0;
                            dic["remainQuantity"] = remainQuantity;
                            dic["locationId"] = (long)itemInLocs[i].locationId;
                            dic["requiredQuantity"] = 0;

                            return dic;
                        }
                        else
                        {
                            itemL.quantity = 0;
                            breakNum = (int)(breakNum - itemInLocs[i].quantity);
                            requiredAmount = requiredAmount - ((int)itemInLocs[i].quantity * (int)upperUnit.unitValue);
                            entity.SaveChanges();
                        }
                        if (breakNum == 0)
                            break;
                    }
                    if (breakNum != 0)
                    {
                        dic = new Dictionary<string, long>();
                        dic = checkUpperUnit(upperUnit.itemUnitId, branchId, breakNum, userId,isKitchen);
                        var item = (from s in entity.sections
                                    where s.branchId == branchId
                                    join l in entity.locations on s.sectionId equals l.sectionId
                                    join il in entity.itemsLocations on l.locationId equals il.locationId
                                    where il.itemUnitId == upperUnit.itemUnitId && il.invoiceId == null && il.locations.isKitchen == isKitchen
                                    select new
                                    {
                                        il.itemsLocId,
                                    }).FirstOrDefault();
                        if (item != null)
                        {
                            var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                            itemloc.quantity = dic["remainQuantity"];
                            entity.SaveChanges();
                        }
                        else
                        {
                            var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1 && x.isKitchen == isKitchen).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();

                            long locationId = dic["locationId"];
                            if (locationId == 0 && locations.Count > 1)
                                locationId = locations[0].locationId; // free zoon

                            itemsLocations itemL = new itemsLocations();
                            //itemL.itemUnitId = itemUnitId;
                            itemL.itemUnitId = upperUnit.itemUnitId;
                            itemL.locationId = locationId;
                            itemL.quantity = dic["remainQuantity"];
                            itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.createUserId = userId;
                            itemL.updateUserId = userId;

                            entity.itemsLocations.Add(itemL);
                            entity.SaveChanges();

                        }

                        ///////////////////
                        if (dic["isConsumed"] == 0)
                        {
                            dic["requiredQuantity"] = requiredAmount;
                            dic["remainQuantity"] = 0;
                        }
                        else
                        {
                            dic["remainQuantity"] = (int)newQuant - firstRequir;
                            dic["requiredQuantity"] = breakNum * (int)upperUnit.unitValue;
                        }
                        return dic;
                    }
                }
                else
                {
                    dic["remainQuantity"] = 0;
                    dic["requiredQuantity"] = requiredAmount;
                    dic["locationId"] = 0;

                    return dic;
                }
            }
            return dic;
        }
        private Dictionary<string, long> checkLowerUnit(long itemUnitId, long branchId, long requiredAmount, long userId, int isKitchen = 0)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            long remainQuantity = 0;
            long firstRequir = requiredAmount;
            decimal newQuant = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId, x.subUnitId, x.unitValue }).FirstOrDefault();
                var lowerUnit = entity.itemsUnits.Where(x => x.unitId == unit.subUnitId && x.itemId == unit.itemId && x.isActive == 1).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if (lowerUnit != null)
                {
                    decimal unitValue = (decimal)unit.unitValue;
                    int breakNum = (int)requiredAmount * (int)unitValue;
                    newQuant = (decimal)Math.Ceiling(breakNum / (decimal)lowerUnit.unitValue);
                    var itemInLocs = (from b in entity.branches
                                      where b.branchId == branchId
                                      join s in entity.sections.Where(x => x.isKitchen == isKitchen) on b.branchId equals s.branchId
                                      join l in entity.locations on s.sectionId equals l.sectionId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == lowerUnit.itemUnitId && il.quantity > 0 && il.invoiceId == null 
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                          il.itemUnitId,
                                          il.locationId,
                                          il.updateDate,
                                          s.sectionId,
                                      }).ToList().OrderBy(x => x.updateDate).ToList();

                    for (int i = 0; i < itemInLocs.Count; i++)
                    {

                        var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                        var smallUnitLocId = entity.itemsLocations.Where(x => x.itemUnitId == itemUnitId && x.invoiceId == null && x.locations.isKitchen == isKitchen).
                            Select(x => x.itemsLocId).FirstOrDefault();

                        if (breakNum <= itemInLocs[i].quantity)
                        {
                            itemL.quantity = itemInLocs[i].quantity - breakNum;
                            entity.SaveChanges();
                            remainQuantity = (int)newQuant - firstRequir;
                            requiredAmount = 0;
                            // return remainQuantity;
                            dic.Add("remainQuantity", remainQuantity);
                            dic.Add("locationId", (long)itemInLocs[i].locationId);
                            return dic;
                        }
                        else
                        {
                            itemL.quantity = 0;
                            breakNum = (int)(breakNum - itemInLocs[i].quantity);
                            requiredAmount = requiredAmount - ((int)itemInLocs[i].quantity / (int)unit.unitValue);
                            entity.SaveChanges();
                        }
                        if (breakNum == 0)
                            break;
                    }
                    if (itemUnitId == lowerUnit.itemUnitId)
                        return dic;
                    if (breakNum != 0)
                    {
                        dic = new Dictionary<string, long>();
                        dic = checkLowerUnit(lowerUnit.itemUnitId, branchId, breakNum, userId,isKitchen);
                       
                        dic["remainQuantity"] = (int)newQuant - firstRequir;
                        dic["requiredQuantity"] = breakNum;
                        return dic;
                    }
                }
            }
            return dic;
        }

        [HttpPost]
        [Route("getAmountInBranch")]
        public string getAmountInBranch(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                long itemUnitId = 0;
                long branchId = 0;
                int isKitchen = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "isKitchen")
                    {
                        isKitchen = int.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    int amount = getAmountInBranch(itemUnitId,branchId,isKitchen);
                    //int amount = 0;
                    //amount += getItemUnitAmount(itemUnitId, branchId,isKitchen); // from bigger unit
                    //amount += getSmallItemUnitAmount(itemUnitId, branchId,isKitchen);
                    return TokenManager.GenerateToken(amount.ToString());
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }           
        }

        public int getAmountInBranch(long itemUnitId, long branchId,int isKitchen)
        {
            int amount = 0;
            amount += getItemUnitAmount(itemUnitId, branchId, isKitchen); // from bigger unit
            amount += getSmallItemUnitAmount(itemUnitId, branchId, isKitchen);
            return amount;
        }
        public int getBranchAmount(long itemUnitId, long branchId,int isKitchen = 0)
        {

            int amount = 0;
            amount += getItemUnitAmount(itemUnitId, branchId,isKitchen); // from bigger unit
                amount += getSmallItemUnitAmount(itemUnitId, branchId, isKitchen);
            
            return amount;
        }



        private int getItemUnitAmount(long itemUnitId, long branchId, int isKitchen = 0)
        {
            int amount = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemInLocs = (from b in entity.branches
                                  where b.branchId == branchId
                                  join s in entity.sections on b.branchId equals s.branchId
                                  join l in entity.locations on s.sectionId equals l.sectionId
                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                  where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen == isKitchen
                                  select new
                                  {
                                      il.itemsLocId,
                                      il.quantity,
                                      il.itemUnitId,
                                      il.locationId,
                                      s.sectionId,
                                  }).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    amount += (int)itemInLocs[i].quantity;
                }

                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId && x.isActive == 1).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if ((upperUnit != null && itemUnitId == upperUnit.itemUnitId))
                    return amount;
                if (upperUnit != null)
                    amount += (int)upperUnit.unitValue * getItemUnitAmount(upperUnit.itemUnitId, branchId,isKitchen);

                return amount;
            }
        }
        private int getSmallItemUnitAmount(long itemUnitId, long branchId, int isKitchen = 0)
        {
            int amount = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.subUnitId, x.unitId, x.unitValue, x.itemId }).FirstOrDefault();

                var smallUnit = entity.itemsUnits.Where(x => x.unitId == unit.subUnitId && x.itemId == unit.itemId && x.isActive == 1).Select(x => new { x.itemUnitId }).FirstOrDefault();
                if (smallUnit == null || smallUnit.itemUnitId == itemUnitId)
                {
                    return 0;
                }
                else
                {
                    var itemInLocs = (from b in entity.branches
                                      where b.branchId == branchId
                                      join s in entity.sections on b.branchId equals s.branchId
                                      join l in entity.locations on s.sectionId equals l.sectionId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == smallUnit.itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen == isKitchen
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                          il.itemUnitId,
                                          il.locationId,
                                          s.sectionId,
                                      }).ToList();
                    for (int i = 0; i < itemInLocs.Count; i++)
                    {
                        amount += (int)itemInLocs[i].quantity;
                    }
                    if (unit.unitValue != 0)
                        amount = amount / (int)unit.unitValue;
                    else
                        amount += getSmallItemUnitAmount(smallUnit.itemUnitId, branchId) / (int)unit.unitValue;

                    return amount;
                }
            }
        }
        private int getItemAmount(long itemUnitId, long branchId)
        {
            int amount = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemInLocs = (from b in entity.branches
                                  where b.branchId == branchId
                                  join s in entity.sections on b.branchId equals s.branchId
                                  join l in entity.locations on s.sectionId equals l.sectionId
                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                  where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                  select new
                                  {
                                      il.itemsLocId,
                                      il.quantity,
                                      il.itemUnitId,
                                      il.locationId,
                                      s.sectionId,
                                  }).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    amount += (int)itemInLocs[i].quantity;
                }

                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if ((upperUnit != null && itemUnitId == upperUnit.itemUnitId) || upperUnit == null)
                    return amount;
                if (upperUnit != null)
                    amount += (int)upperUnit.unitValue * getItemUnitAmount(upperUnit.itemUnitId, branchId);

                return amount;
            }
        }
        private int getLevelItemUnitAmount(long itemUnitId, long topLevelUnit, long branchId)
        {
            int amount = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemInLocs = (from b in entity.branches
                                  where b.branchId == branchId
                                  join s in entity.sections on b.branchId equals s.branchId
                                  join l in entity.locations on s.sectionId equals l.sectionId
                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                  where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                  select new
                                  {
                                      il.itemsLocId,
                                      il.quantity,
                                      il.itemUnitId,
                                      il.locationId,
                                      s.sectionId,
                                  }).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    amount += (int)itemInLocs[i].quantity;
                }

                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if ((upperUnit != null && itemUnitId == upperUnit.itemUnitId) || upperUnit == null)
                    return amount;
                if (upperUnit != null && upperUnit.itemUnitId != topLevelUnit)
                    amount += (int)upperUnit.unitValue * getLevelItemUnitAmount(upperUnit.itemUnitId, topLevelUnit, branchId);

                return amount;
            }
        }


        [HttpPost]
        [Route("getUnitAmount")]
        public string getUnitAmount(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                long itemUnitId = 0;
                long branchId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }

                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var amount = (from b in entity.branches
                                      where b.branchId == branchId
                                      join s in entity.sections on b.branchId equals s.branchId
                                      join l in entity.locations on s.sectionId equals l.sectionId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                          il.itemUnitId,
                                          il.locationId,
                                          s.sectionId,
                                      }).ToList().Sum(x => x.quantity);
                        return TokenManager.GenerateToken(amount.ToString());
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
        [Route("getAmountInLocation")]
        public string getAmountInLocation(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                long itemUnitId = 0;
                long locationId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);

                    }
                    else if (c.Type == "locationId")
                    {
                        locationId = long.Parse(c.Value);

                    }

                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var amount = (from l in entity.locations
                                      where l.locationId == locationId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                      }).ToList().Sum(x => x.quantity);
                        return TokenManager.GenerateToken(amount.ToString());
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
        [Route("returnInvoice")]
        public string returnInvoice(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";

                long branchId = 0;
                long userId = 0;

                List<itemsTransfer> newObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                }

                if (newObject != null)
                {
                    try
                    {

                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var freeZoneLocation = (from s in entity.sections.Where(x => x.branchId == branchId && x.isFreeZone == 1 && x.isKitchen != 1)
                                                    join l in entity.locations on s.sectionId equals l.sectionId
                                                    select l.locationId).SingleOrDefault();
                            foreach (itemsTransfer item in newObject)
                            {
                                decreaseItemQuantity(item.itemUnitId.Value, freeZoneLocation, (int)item.quantity, userId);
                            }
                        }
                        return TokenManager.GenerateToken("1");
                    }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }
        private void decreaseItemQuantity(long itemUnitId, long locationId, int quantity, long userId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemUnit = (from il in entity.itemsLocations
                                where il.itemUnitId == itemUnitId && il.locationId == locationId && il.invoiceId == null && il.locations.isKitchen != 1
                                select new { il.itemsLocId }
                                ).FirstOrDefault();
                itemsLocations itemL = new itemsLocations();

                itemL = entity.itemsLocations.Find(itemUnit.itemsLocId);
                itemL.quantity -= quantity;
                itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                itemL.updateUserId = userId;
                entity.SaveChanges();
            }
        }
        [HttpPost]
        [Route("returnSpendingOrder")]
        public string returnSpendingOrder(string token)
        {
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long branchId = 0;
                long userId = 0;
                List<itemsTransfer> newObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }

                }

                if (newObject != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var freeZoneLocation = (from s in entity.sections.Where(x => x.branchId == branchId && x.isFreeZone == 1 && x.isKitchen != 1)
                                                    join l in entity.locations on s.sectionId equals l.sectionId
                                                    select l.locationId).SingleOrDefault();
                            foreach (itemsTransfer item in newObject)
                            {
                                var itemL = entity.itemsLocations.Where(x => x.itemUnitId == item.itemUnitId && x.locations.isKitchen == 1).FirstOrDefault();
                                if (item.quantity > 0)
                                {
                                    itemL.quantity -= item.quantity;
                                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemL.updateUserId = userId;
                                    entity.SaveChanges();

                                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();

                                    var itemV = entity.items.Find(itemId);
                                    int quantity = (int)item.quantity;

                                    if (quantity != 0)
                                        increaseItemQuantity(item.itemUnitId.Value, freeZoneLocation, quantity, userId);
                                }
                            }
                            return TokenManager.GenerateToken("1");
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
                    return TokenManager.GenerateToken("0");
                }
            }
        }
       
        [HttpPost]
        [Route("destroyItem")]
        public string destroyItem(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";

                long userId = 0;

                List<itemsLocations> newObject = new List<itemsLocations>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsLocations>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }


                }

                if (newObject != null)
                {
                    try
                    {

                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (itemsLocations item in newObject)
                            {
                                itemsLocations itemL = new itemsLocations();

                                itemL = entity.itemsLocations.Find(item.itemsLocId);
                                itemL.quantity -= item.quantity;
                                itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                itemL.updateUserId = userId;
                            }
                            entity.SaveChanges();
                        }
                        return TokenManager.GenerateToken("1");
                    }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }      



        [HttpPost]
        [Route("decreaseItemLocationQuantity")]
        public string decreaseItemLocationQuantity(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemLocId = 0;
                int quantity = 0;
                long userId = 0;

                string objectName = "";
                string notificationObj = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemLocId")
                    {
                        itemLocId = long.Parse(c.Value);

                    }
                    else if (c.Type == "quantity")
                    {
                        quantity = int.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                    else if (c.Type == "objectName")
                    {
                        objectName = c.Value;

                    }
                    else if (c.Type == "notificationObj")
                    {
                        notificationObj = c.Value;

                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        itemsLocations itemL = new itemsLocations();

                        itemL = entity.itemsLocations.Find(itemLocId);
                        itemL.quantity -= quantity;
                        itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        itemL.updateUserId = userId;
                        entity.SaveChanges();
                        if (objectName != "")
                        {
                            var branchId = (from l in entity.itemsLocations
                                            where l.itemsLocId == itemLocId
                                            select l.locations.branchId).Single();
                            bool isExcedded = isExceddMinQuantity((long)itemL.itemUnitId, (long)branchId, userId);
                            if (isExcedded == true) //add notification
                            {
                                var itemId = entity.itemsUnits.Where(x => x.itemUnitId == itemL.itemUnitId).Select(x => x.itemId).Single();
                                var itemV = entity.items.Find(itemId);
                                notificationController.addNotifications(objectName, notificationObj, (long)branchId, itemV.name);
                            }
                        }
                        return TokenManager.GenerateToken("1");
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }

            }
        }

        public void decreaseItemLocationQuantity(long itemLocId, int quantity, long userId, string objectName, string notificationObj)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                itemsLocations itemL = new itemsLocations();

                itemL = entity.itemsLocations.Find(itemLocId);
                itemL.quantity -= quantity;
                itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                itemL.updateUserId = userId;
                entity.SaveChanges();
                if (objectName != "")
                {
                    var branchId = (from l in entity.itemsLocations
                                    where l.itemsLocId == itemLocId
                                    select l.locations.branchId).Single();
                    bool isExcedded = isExceddMinQuantity((long)itemL.itemUnitId, (long)branchId, userId);
                    if (isExcedded == true) //add notification
                    {
                        var itemId = entity.itemsUnits.Where(x => x.itemUnitId == itemL.itemUnitId).Select(x => x.itemId).Single();
                        var itemV = entity.items.Find(itemId);
                        notificationController.addNotifications(objectName, notificationObj, (long)branchId, itemV.name);
                    }
                }
            }
        }

        public bool isExceddMinQuantity(long itemUnitId, long branchId, long userId)
        {
            bool isExcedded = false;
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => x.itemId).Single();
                    var item = entity.items.Find(itemId);
                    long minUnitId = (long)item.minUnitId;
                    int minQuantity = (int)item.min;
                    if (minQuantity == 0)
                        return false;
                    var minUnit = entity.itemsUnits.Where(x => x.itemId == itemId && x.unitId == minUnitId).FirstOrDefault();
                    if (minUnit == null)
                        isExcedded = false;
                    else
                    {
                        int itemUnitQuantity = getItemAmount(minUnit.itemUnitId, branchId);
                        if (itemUnitQuantity <= minQuantity)
                        {
                            isExcedded = true;
                        }
                        if (isExcedded == false)
                        {
                            long smallestItemUnit = entity.itemsUnits.Where(x => x.itemId == itemId && x.subUnitId == x.unitId).Select(x => x.itemUnitId).Single();
                            int smallUnitQuantity = getLevelItemUnitAmount(smallestItemUnit, minUnit.itemUnitId, branchId);
                            int unitValue = itemsUnitsController.getLargeUnitConversionQuan(smallestItemUnit, minUnit.itemUnitId);
                            int quantity = 0;
                            if (unitValue != 0)
                                quantity = smallUnitQuantity / unitValue;

                            quantity += itemUnitQuantity;
                            if (quantity <= minQuantity)
                                isExcedded = true;
                        }
                    }
                }
            }
            catch
            {
            }
            return isExcedded;
        }



        [HttpPost]
        [Route("decraseAmounts")]
        public string decraseAmounts(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";

                long branchId = 0;
                long userId = 0;
                string objectName = "";
                string notificationObj = "";

                List<itemsTransfer> newObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "objectName")
                    {
                        objectName = c.Value;

                    }
                    else if (c.Type == "notificationObj")
                    {
                        notificationObj = c.Value;

                    }

                }

                if (newObject != null)
                {
                    try
                    {


                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (itemsTransfer item in newObject)
                            {
                                updateItemQuantity(item.itemUnitId.Value, branchId, (int)item.quantity, userId);

                                bool isExcedded = isExceddMinQuantity((long)item.itemUnitId, (long)branchId, userId);
                                if (isExcedded == true) //add notification
                                {
                                    var itemId = entity.itemsUnits.Where(x => x.itemUnitId == item.itemUnitId).Select(x => x.itemId).Single();
                                    var itemV = entity.items.Find(itemId);
                                    notificationController.addNotifications(objectName, notificationObj, (long)branchId, itemV.name);
                                }
                            }
                        }
                        //  return true;

                        return TokenManager.GenerateToken("1");
                    }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }
        [HttpPost]
        [Route("decreaseAmountsInKitchen")]
        public string decreaseAmountsInKitchen(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long branchId = 0;
                long userId = 0;
                List<itemsTransfer> newObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }                  
                }

                if (newObject != null)
                {
                    try
                    {
                        decreaseAmountsInKitchen(newObject,branchId,userId);
                        //using (incposdbEntities entity = new incposdbEntities())
                        //{
                        //    foreach (itemsTransfer item in newObject)
                        //    {
                        //        updateItemQuantity(item.itemUnitId.Value, branchId, (int)item.quantity, userId,1);

                        //    }
                        //}

                        return TokenManager.GenerateToken("1");
                    }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }

        public void decreaseAmountsInKitchen(List<itemsTransfer> newObject,long branchId,long userId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                foreach (itemsTransfer item in newObject)
                {
                    updateItemQuantity(item.itemUnitId.Value, branchId, (int)item.quantity, userId, 1);

                }
            }
        }
        [HttpPost]
        [Route("reserveItems")]
        public string reserveItems(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long invoiceId = 0;
                long branchId = 0;
                long userId = 0;
                List<itemsTransfer> newObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }


                }

                if (newObject != null)
                {
                    try
                    {

                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (itemsTransfer item in newObject)
                            {
                                lockItem(item.itemUnitId.Value, invoiceId, branchId, (int)item.quantity, userId);
                            }
                        }
                        return TokenManager.GenerateToken("1");
                    }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }
        [HttpPost]
        [Route("reReserveItems")]
        public string reReserveItems(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long invoiceId = 0;
                long branchId = 0;
                long userId = 0;

                List<ItemTransferModel> newObject = new List<ItemTransferModel>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<ItemTransferModel>>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                }

                if (newObject != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (ItemTransferModel item in newObject)
                            {
                                if (item.newLocked > item.lockedQuantity)
                                {
                                    int lockedQuantity = (int)(item.newLocked - item.lockedQuantity);
                                    lockItem(item.itemUnitId.Value, invoiceId, branchId, lockedQuantity, userId);
                                }
                                else if(item.newLocked < item.lockedQuantity)
                                {
                                    int unreserveQnt = (int)(item.lockedQuantity - item.newLocked);
                                    unlockQuantity(invoiceId, (long)item.itemUnitId, unreserveQnt);
                                }
                            }
                        }
                        return TokenManager.GenerateToken("1");
                    }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }
        [HttpPost]
        [Route("lockItem")]
        public string lockItem(string token)
        {

            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
             var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemUnitId = 0;
                long invoiceId = 0;
                long branchId = 0;
                long userId = 0;

                int requiredAmount = 0;
                long locationId = 0;


                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                    else if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);

                    }
                    else if (c.Type == "requiredAmount")
                    {
                        requiredAmount = int.Parse(c.Value);

                    }


                }
                try
                {


                    Dictionary<string, long> dic = new Dictionary<string, long>();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemInLocs = (from s in entity.sections
                                          where s.branchId == branchId
                                          join l in entity.locations on s.sectionId equals l.sectionId
                                          join il in entity.itemsLocations on l.locationId equals il.locationId
                                          where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                          select new
                                          {
                                              il.itemsLocId,
                                              il.quantity,
                                              il.itemUnitId,
                                              il.locationId,
                                              il.updateDate,
                                              s.sectionId,
                                          }).ToList().OrderBy(x => x.updateDate).ToList();
                        for (int i = 0; i < itemInLocs.Count; i++)
                        {
                            int availableAmount = (int)itemInLocs[i].quantity;
                            int lockedAmount = 0;
                            var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            if (availableAmount >= requiredAmount)
                            {
                                itemL.quantity = availableAmount - requiredAmount;
                                lockedAmount = requiredAmount;
                                requiredAmount = 0;
                                entity.SaveChanges();
                            }
                            else if (availableAmount > 0)
                            {
                                itemL.quantity = 0;
                                requiredAmount = requiredAmount - availableAmount;
                                lockedAmount = availableAmount;
                                entity.SaveChanges();
                            }
                            if (lockedAmount > 0)
                                increaseLockedItem((long)itemInLocs[i].itemUnitId, (long)itemInLocs[i].locationId, lockedAmount, invoiceId, userId);

                            if (requiredAmount == 0)
                                // return Ok(3);
                                return TokenManager.GenerateToken("3");
                        }

                        if (requiredAmount != 0)
                        {
                            dic = lockUpperUnit(itemUnitId, branchId, requiredAmount, userId);

                            //var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId }).FirstOrDefault();
                            //var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();
                            if (dic["remainQuantity"] > 0)
                            {
                                var item = (from il in entity.itemsLocations
                                            where il.itemUnitId == itemUnitId && il.invoiceId == null && il.locations.isKitchen != 1
                                            join l in entity.locations on il.locationId equals l.locationId
                                            join s in entity.sections on l.sectionId equals s.sectionId
                                            where s.branchId == branchId
                                            select new
                                            {
                                                il.itemsLocId,
                                            }).FirstOrDefault();
                                if (item != null)
                                {
                                    var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                                    itemloc.quantity += dic["remainQuantity"];
                                    entity.SaveChanges();
                                }
                                else
                                {
                                    var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                                    locationId = dic["locationId"];
                                    if ((locationId == 0 && locationId == null) && locations.Count > 1)
                                        locationId = locations[0].locationId; // free zoon
                                    itemsLocations itemL = new itemsLocations();
                                    itemL.itemUnitId = itemUnitId;
                                    itemL.locationId = locationId;
                                    itemL.quantity = dic["remainQuantity"];
                                    itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    itemL.createUserId = userId;
                                    itemL.updateUserId = userId;
                                    itemL.invoiceId = null;

                                    entity.itemsLocations.Add(itemL);
                                    entity.SaveChanges();
                                }
                            }
                            // return Ok(dic["lockedQuantity"] +":"+ dic["remainQuantity"]+ ":"+ dic["requiredQuantity"]);
                            // reserve items
                            if (dic["lockedQuantity"] > 0)
                            {
                                long lockedQuantity = dic["lockedQuantity"];
                                if (lockedQuantity > requiredAmount)
                                    lockedQuantity = requiredAmount;
                                var item = (from il in entity.itemsLocations
                                            where il.itemUnitId == itemUnitId && il.invoiceId == invoiceId && il.locations.isKitchen != 1
                                            select new
                                            {
                                                il.itemsLocId,
                                            }).FirstOrDefault();
                                if (item != null)
                                {
                                    var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                                    locationId = (long)itemloc.locationId;
                                }
                                else
                                {
                                    var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                                    locationId = dic["locationId"];
                                    if (locationId == 0 && locations.Count > 1)
                                        locationId = locations[0].locationId; // free zoon
                                }

                                increaseLockedItem(itemUnitId, locationId, lockedQuantity, invoiceId, userId);
                            }
                            if (dic["requiredQuantity"] > 0)
                            {
                                dic = lockLowerUnit(itemUnitId, branchId, dic["requiredQuantity"], userId);
                                if (dic["lockedQuantity"] > 0)
                                {
                                    var item = (from il in entity.itemsLocations
                                                where il.itemUnitId == itemUnitId && il.invoiceId == invoiceId && il.locations.isKitchen != 1
                                                join l in entity.locations on il.locationId equals l.locationId
                                                join s in entity.sections on l.sectionId equals s.sectionId
                                                where s.branchId == branchId
                                                select new
                                                {
                                                    il.itemsLocId,
                                                }).FirstOrDefault();
                                    if (item != null)
                                    {
                                        var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                                        locationId = (long)itemloc.locationId;
                                    }
                                    else
                                    {
                                        var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                                        locationId = dic["locationId"];
                                        if (locationId == 0 && locations.Count > 1)
                                            locationId = locations[0].locationId; // free zoon
                                    }
                                    increaseLockedItem(itemUnitId, locationId, dic["lockedQuantity"], invoiceId, userId);
                                }
                            }

                        }
                    }
                    return TokenManager.GenerateToken("2");
                }

                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        public int lockItem(long itemUnitId, long invoiceId, long branchId, int requiredAmount, long userId)
        {

            long locationId = 0;
            Dictionary<string, long> dic = new Dictionary<string, long>();
            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemInLocs = (from s in entity.sections
                                  where s.branchId == branchId
                                  join l in entity.locations on s.sectionId equals l.sectionId
                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                  where il.itemUnitId == itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                  select new
                                  {
                                      il.itemsLocId,
                                      il.quantity,
                                      il.itemUnitId,
                                      il.locationId,
                                      il.updateDate,
                                      s.sectionId,
                                  }).ToList().OrderBy(x => x.updateDate).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    int availableAmount = (int)itemInLocs[i].quantity;
                    int lockedAmount = 0;
                    var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    if (availableAmount >= requiredAmount)
                    {
                        itemL.quantity = availableAmount - requiredAmount;
                        lockedAmount = requiredAmount;
                        requiredAmount = 0;
                        entity.SaveChanges();
                    }
                    else if (availableAmount > 0)
                    {
                        itemL.quantity = 0;
                        requiredAmount = requiredAmount - availableAmount;
                        lockedAmount = availableAmount;
                        entity.SaveChanges();
                    }
                    if (lockedAmount > 0)
                        increaseLockedItem((int)itemInLocs[i].itemUnitId, (int)itemInLocs[i].locationId, lockedAmount, invoiceId, userId);

                    if (requiredAmount == 0)
                        return (3);
                }

                if (requiredAmount != 0)
                {
                    dic = lockUpperUnit(itemUnitId, branchId, requiredAmount, userId);

                    if (dic["remainQuantity"] > 0)
                    {
                        var item = (from il in entity.itemsLocations
                                    where il.itemUnitId == itemUnitId && il.invoiceId == null && il.locations.isKitchen != 1
                                    join l in entity.locations on il.locationId equals l.locationId
                                    join s in entity.sections on l.sectionId equals s.sectionId
                                    where s.branchId == branchId
                                    select new
                                    {
                                        il.itemsLocId,
                                    }).FirstOrDefault();
                        if (item != null)
                        {
                            var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                            itemloc.quantity += dic["remainQuantity"];
                            entity.SaveChanges();
                        }
                        else
                        {
                            var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                            locationId = dic["locationId"];
                            if ((locationId == 0 && locationId == null) && locations.Count > 1)
                                locationId = locations[0].locationId; // free zoon
                            itemsLocations itemL = new itemsLocations();
                            itemL.itemUnitId = itemUnitId;
                            itemL.locationId = locationId;
                            itemL.quantity = dic["remainQuantity"];
                            itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.createUserId = userId;
                            itemL.updateUserId = userId;
                            itemL.invoiceId = null;

                            entity.itemsLocations.Add(itemL);
                            entity.SaveChanges();
                        }
                    }

                    if (dic["lockedQuantity"] > 0)
                    {
                        long lockedQuantity = dic["lockedQuantity"];
                        if (lockedQuantity > requiredAmount)
                            lockedQuantity = requiredAmount;
                        var item = (from il in entity.itemsLocations
                                    where il.itemUnitId == itemUnitId && il.invoiceId == invoiceId && il.locations.isKitchen != 1
                                    select new
                                    {
                                        il.itemsLocId,
                                    }).FirstOrDefault();
                        if (item != null)
                        {
                            var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                            locationId = (int)itemloc.locationId;
                        }
                        else
                        {
                            var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                            locationId = dic["locationId"];
                            if (locationId == 0 && locations.Count > 1)
                                locationId = locations[0].locationId; // free zoon
                        }

                        increaseLockedItem(itemUnitId, locationId, lockedQuantity, invoiceId, userId);
                    }
                    if (dic["requiredQuantity"] > 0)
                    {
                        dic = lockLowerUnit(itemUnitId, branchId, dic["requiredQuantity"], userId);
                        if (dic["lockedQuantity"] > 0)
                        {
                            var item = (from il in entity.itemsLocations
                                        where il.itemUnitId == itemUnitId && il.invoiceId == invoiceId && il.locations.isKitchen != 1
                                        join l in entity.locations on il.locationId equals l.locationId
                                        join s in entity.sections on l.sectionId equals s.sectionId
                                        where s.branchId == branchId
                                        select new
                                        {
                                            il.itemsLocId,
                                        }).FirstOrDefault();
                            if (item != null)
                            {
                                var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                                locationId = (long)itemloc.locationId;
                            }
                            else
                            {
                                var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();
                                locationId = dic["locationId"];
                                if (locationId == 0 && locations.Count > 1)
                                    locationId = locations[0].locationId; // free zoon
                            }
                            increaseLockedItem(itemUnitId, locationId, dic["lockedQuantity"], invoiceId, userId);
                        }
                    }

                }
            }
            return (2);
        }


        private Dictionary<string, long> lockLowerUnit(long itemUnitId, long branchId, long requiredAmount, long userId)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            long remainQuantity = 0;
            long firstRequir = requiredAmount;
            int lockedQuantity = 0;
            decimal newQuant = 0;
            dic.Add("lockedQuantity", 0);
            dic.Add("remainQuantity", 0);
            dic.Add("locationId", 0);

            using (incposdbEntities entity = new incposdbEntities())
            {
                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId, x.subUnitId, x.unitValue }).FirstOrDefault();
                var lowerUnit = entity.itemsUnits.Where(x => x.unitId == unit.subUnitId && x.itemId == unit.itemId).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if (lowerUnit != null && lowerUnit.itemUnitId != itemUnitId)
                {
                    decimal unitValue = (decimal)unit.unitValue;
                    int breakNum = (int)requiredAmount * (int)unitValue;
                    newQuant = (decimal)Math.Ceiling(breakNum / (decimal)unit.unitValue);
                    var itemInLocs = (from b in entity.branches
                                      where b.branchId == branchId
                                      join s in entity.sections on b.branchId equals s.branchId
                                      join l in entity.locations on s.sectionId equals l.sectionId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == lowerUnit.itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                          il.itemUnitId,
                                          il.locationId,
                                          il.updateDate,
                                          s.sectionId,
                                      }).ToList().OrderBy(x => x.updateDate).ToList();

                    for (int i = 0; i < itemInLocs.Count; i++)
                    {

                        var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);

                        if (breakNum <= (int)itemInLocs[i].quantity)
                        {
                            itemL.quantity = itemInLocs[i].quantity - breakNum;
                            entity.SaveChanges();
                            remainQuantity = (int)newQuant - firstRequir;
                            requiredAmount = 0;
                            lockedQuantity = breakNum;

                            dic["remainQuantity"] = remainQuantity;
                            dic["locationId"] = (long)itemInLocs[i].locationId;
                            dic["lockedQuantity"] += lockedQuantity / (int)unit.unitValue;
                            return dic;
                        }
                        else
                        {
                            itemL.quantity = 0;
                            breakNum = (int)(breakNum - itemInLocs[i].quantity);
                            requiredAmount = requiredAmount - ((int)itemInLocs[i].quantity / (int)unit.unitValue);
                            lockedQuantity += (int)itemInLocs[i].quantity / (int)unit.unitValue;
                            entity.SaveChanges();
                            dic["lockedQuantity"] += lockedQuantity;
                        }
                        if (breakNum == 0)
                            break;
                    }
                    if (itemUnitId == lowerUnit.itemUnitId)
                        return dic;
                    if (breakNum != 0)
                    {
                        dic = new Dictionary<string, long>();
                        dic = lockLowerUnit(lowerUnit.itemUnitId, branchId, breakNum, userId);

                        dic["remainQuantity"] = (int)newQuant - firstRequir;
                        dic["requiredQuantity"] = breakNum;
                        dic["lockedQuantity"] += ((int)newQuant - firstRequir) / (int)unit.unitValue;
                        return dic;
                    }
                }
            }
            return dic;
        }
        private Dictionary<string, long> lockUpperUnit(long itemUnitId, long branchId, long requiredAmount, long userId)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            dic.Add("remainQuantity", 0);
            dic.Add("locationId", 0);
            dic.Add("requiredQuantity", 0);
            dic.Add("lockedQuantity", 0);
            dic.Add("isConsumed", 0);

            long remainQuantity = 0;
            long firstRequir = requiredAmount;
            decimal newQuant = 0;
            long lockedAmount = 0;
            int isConsumed = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemUnitId).Select(x => new { x.unitId, x.itemId, x.unitValue }).FirstOrDefault();
                var upperUnit = entity.itemsUnits.Where(x => x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.subUnitId != x.unitId).Select(x => new { x.unitValue, x.itemUnitId }).FirstOrDefault();

                if (upperUnit != null && upperUnit.itemUnitId != itemUnitId)
                {
                    decimal unitValue = (decimal)upperUnit.unitValue;
                    int breakNum = (int)Math.Ceiling(requiredAmount / unitValue);
                    newQuant = (decimal)(breakNum * upperUnit.unitValue);
                    var itemInLocs = (from b in entity.branches
                                      where b.branchId == branchId
                                      join s in entity.sections on b.branchId equals s.branchId
                                      join l in entity.locations on s.sectionId equals l.sectionId
                                      join il in entity.itemsLocations on l.locationId equals il.locationId
                                      where il.itemUnitId == upperUnit.itemUnitId && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                      select new
                                      {
                                          il.itemsLocId,
                                          il.quantity,
                                          il.itemUnitId,
                                          il.locationId,
                                          il.updateDate,
                                          s.sectionId,
                                      }).ToList().OrderBy(x => x.updateDate).ToList();

                    for (int i = 0; i < itemInLocs.Count; i++)
                    {
                        dic["isConsumed"] = 1;
                        isConsumed = 1;
                        var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);

                        if (breakNum <= itemInLocs[i].quantity)
                        {
                            itemL.quantity = itemInLocs[i].quantity - breakNum;
                            entity.SaveChanges();
                            remainQuantity = (long)newQuant - firstRequir;

                            lockedAmount = firstRequir;
                            requiredAmount = 0;
                            // return remainQuantity;
                            dic["remainQuantity"] = remainQuantity;
                            dic["locationId"] = (long)itemInLocs[i].locationId;
                            dic["requiredQuantity"] = 0;
                            dic["lockedQuantity"] += lockedAmount;

                            return dic;
                        }
                        else
                        {
                            itemL.quantity = 0;
                            breakNum = (int)(breakNum - itemInLocs[i].quantity);
                            lockedAmount += (int)itemInLocs[i].quantity;
                            requiredAmount = requiredAmount - ((int)itemInLocs[i].quantity * (int)upperUnit.unitValue);
                            entity.SaveChanges();
                            dic["locationId"] = (long)itemInLocs[i].locationId;
                            dic["requiredQuantity"] = requiredAmount;
                        }
                        if (breakNum == 0)
                            break;
                    }
                    if (breakNum != 0)
                    {
                        dic = new Dictionary<string, long>();
                        dic = lockUpperUnit(upperUnit.itemUnitId, branchId, breakNum, userId);


                        long locationId = dic["locationId"];
                        if (locationId == 0)
                        {
                            var locations = entity.locations.Where(x => x.branchId == branchId && x.isActive == 1).Select(x => new { x.locationId }).OrderBy(x => x.locationId).ToList();

                            if (locationId == 0 && locations.Count >= 1)
                                locationId = locations[0].locationId; // free zoon
                        }
                        var item = (from s in entity.sections
                                    where s.branchId == branchId
                                    join l in entity.locations on s.sectionId equals l.sectionId
                                    join il in entity.itemsLocations on l.locationId equals il.locationId
                                    where il.itemUnitId == upperUnit.itemUnitId && il.invoiceId == null && il.locations.isKitchen != 1
                                    && il.locationId == locationId
                                    select new
                                    {
                                        il.itemsLocId,
                                    }).FirstOrDefault();
                        if (item != null)
                        {
                            var itemloc = entity.itemsLocations.Find(item.itemsLocId);
                            itemloc.quantity += dic["remainQuantity"];
                            entity.SaveChanges();
                        }
                        else
                        {

                            itemsLocations itemL = new itemsLocations();
                            itemL.itemUnitId = upperUnit.itemUnitId;
                            itemL.locationId = locationId;
                            itemL.quantity = dic["remainQuantity"];
                            itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            itemL.createUserId = userId;
                            itemL.updateUserId = userId;

                            entity.itemsLocations.Add(itemL);
                            entity.SaveChanges();

                        }

                        dic["locationId"] = locationId;
                        if (dic["lockedQuantity"] > 0)
                        {
                            isConsumed = 1;

                            lockedAmount += dic["lockedQuantity"] * (int)upperUnit.unitValue;
                            dic["lockedQuantity"] = lockedAmount;
                        }
                        if (isConsumed == 0)
                        {
                            dic["requiredQuantity"] = requiredAmount;
                            dic["remainQuantity"] = 0;
                        }
                        else
                        {
                            dic["remainQuantity"] = (int)newQuant - firstRequir;
                            dic["requiredQuantity"] = dic["requiredQuantity"] * (int)upperUnit.unitValue;
                        }
                        return dic;
                    }
                }
                else
                {
                    dic["remainQuantity"] = 0;
                    dic["requiredQuantity"] = requiredAmount;
                    dic["locationId"] = 0;
                    dic["lockedQuantity"] = 0;
                    return dic;
                }
            }
            return dic;
        }
        private void increaseLockedItem(long itemUnitId, long locationId, long quantity, long invoiceId, long userId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemUnit = (from il in entity.itemsLocations
                                where il.itemUnitId == itemUnitId && il.locationId == locationId && il.invoiceId == invoiceId 
                                select new { il.itemsLocId }
                                ).FirstOrDefault();
                itemsLocations itemL = new itemsLocations();
                if (itemUnit == null)//add item in new location
                {
                    itemL.itemUnitId = itemUnitId;
                    itemL.locationId = locationId;
                    itemL.quantity = quantity;
                    itemL.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.createUserId = userId;
                    itemL.updateUserId = userId;
                    itemL.invoiceId = invoiceId;

                    entity.itemsLocations.Add(itemL);
                }
                else
                {
                    itemL = entity.itemsLocations.Find(itemUnit.itemsLocId);
                    itemL.quantity += quantity;
                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                    itemL.updateUserId = userId;
                }
                entity.SaveChanges();
            }
        }

        [HttpPost]
        [Route("unitsConversion")]
        public string unitsConversion(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request); 
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long branchId = 0;
                long fromItemUnit = 0;
                long toItemUnit = 0;
                int fromQuantity = 0;
                int toQuantity = 0;
                long userId = 0;

                itemsUnits newObject = new itemsUnits();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<itemsUnits>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }
                    else if (c.Type == "fromItemUnit")
                    {
                        fromItemUnit = long.Parse(c.Value);

                    }
                    else if (c.Type == "toItemUnit")
                    {
                        toItemUnit = long.Parse(c.Value);

                    }
                    else if (c.Type == "fromQuantity")
                    {
                        fromQuantity = int.Parse(c.Value);

                    }
                    else if (c.Type == "toQuantity")
                    {
                        toQuantity = int.Parse(c.Value);

                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);

                    }
                }

                if (newObject != null)
                {
                    try
                    {
                        #region covert from unit (fromItemUnit) is bigger than the last (toItemUnit)
                        if (newObject.itemUnitId != 0)// covert from unit (fromItemUnit) is bigger than the last (toItemUnit)
                        {
                            using (incposdbEntities entity = new incposdbEntities())
                            {
                                var itemInLocs = (from b in entity.branches
                                                  where b.branchId == branchId
                                                  join s in entity.sections on b.branchId equals s.branchId
                                                  join l in entity.locations on s.sectionId equals l.sectionId
                                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                                  where il.itemUnitId == fromItemUnit && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                                  select new
                                                  {
                                                      il.itemsLocId,
                                                      il.quantity,
                                                      il.itemUnitId,
                                                      il.locationId,
                                                      s.sectionId,
                                                  }).ToList();
                                int unitValue = getUnitValue(fromItemUnit, toItemUnit);

                                for (int i = 0; i < itemInLocs.Count; i++)
                                {
                                    int toQuant = 0;
                                    int availableAmount = (int)itemInLocs[i].quantity;
                                    var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    if (availableAmount >= fromQuantity)
                                    {
                                        itemL.quantity = availableAmount - fromQuantity;
                                        toQuant = fromQuantity * unitValue;
                                        fromQuantity = 0;
                                        entity.SaveChanges();
                                    }
                                    else if (availableAmount > 0)
                                    {
                                        itemL.quantity = 0;
                                        fromQuantity = fromQuantity - availableAmount;
                                        toQuant = availableAmount * unitValue;
                                        entity.SaveChanges();
                                    }

                                    increaseItemQuantity(toItemUnit, (int)itemInLocs[i].locationId, toQuant, userId);

                                    if (fromQuantity == 0)
                                        //  return true;
                                        return TokenManager.GenerateToken("1");
                                }
                            }
                            // return true;
                            return TokenManager.GenerateToken("1");
                        }
                        #endregion
                        #region from small to large
                        else
                        {
                            using (incposdbEntities entity = new incposdbEntities())
                            {
                                var itemInLocs = (from b in entity.branches
                                                  where b.branchId == branchId
                                                  join s in entity.sections on b.branchId equals s.branchId
                                                  join l in entity.locations on s.sectionId equals l.sectionId
                                                  join il in entity.itemsLocations on l.locationId equals il.locationId
                                                  where il.itemUnitId == fromItemUnit && il.quantity > 0 && il.invoiceId == null && il.locations.isKitchen != 1
                                                  select new
                                                  {
                                                      il.itemsLocId,
                                                      il.quantity,
                                                      il.itemUnitId,
                                                      il.locationId,
                                                      s.sectionId,
                                                  }).ToList();

                                int unitValue = getUnitValue(toItemUnit, fromItemUnit);
                                int i = 0;
                                for (i = 0; i < itemInLocs.Count; i++)
                                {
                                    int availableAmount = (int)itemInLocs[i].quantity;
                                    var itemL = entity.itemsLocations.Find(itemInLocs[i].itemsLocId);
                                    itemL.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                    if (availableAmount >= fromQuantity)
                                    {
                                        itemL.quantity = availableAmount - fromQuantity;
                                        fromQuantity = 0;
                                        entity.SaveChanges();
                                    }
                                    else if (availableAmount > 0)
                                    {
                                        itemL.quantity = 0;
                                        fromQuantity = fromQuantity - availableAmount;
                                        entity.SaveChanges();
                                    }



                                    if (fromQuantity == 0)
                                        //  return true;
                                        return TokenManager.GenerateToken("1");
                                }
                                increaseItemQuantity(toItemUnit, (int)itemInLocs[i].locationId, toQuantity, userId);
                                //  return true;
                                return TokenManager.GenerateToken("1");
                            }
                            #endregion
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
                    return TokenManager.GenerateToken("0");
                }


            }
        }



        private int getUnitValue(long itemunitId, long smallestItemUnitId)
        {
            int unitValue = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var unit = entity.itemsUnits.Where(x => x.itemUnitId == itemunitId).Select(x => new { x.subUnitId, x.unitId, x.unitValue, x.itemId }).FirstOrDefault();
                long smallUnitId = entity.itemsUnits.Where(x => x.unitId == unit.subUnitId && x.itemId == unit.itemId).Select(x => x.itemUnitId).Single();
                unitValue = (int)unit.unitValue;
                if (itemunitId == smallestItemUnitId)
                    return unitValue;
                else
                {
                    unitValue = unitValue * getUnitValue(smallUnitId, smallestItemUnitId);
                }
            }
            return unitValue;
        }


        [HttpPost]
        [Route("getSpecificItemLocation")]
        public string getSpecificItemLocation(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long branchId = 0;

                string newObject = "";
                List<long> ids = new List<long>();
                List<string> strIds = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitsIds")
                    {
                        newObject = c.Value;
                        strIds = newObject.Split(',').ToList();
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                if (strIds != null)
                {
                    try
                    {
                        for (int i = 0; i < strIds.Count; i++)
                        {
                            if (!strIds[i].Equals(""))
                                ids.Add(long.Parse(strIds[i]));
                        }

                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var locList = (from b in entity.itemsLocations
                                           where b.quantity > 0 && b.invoiceId == null && b.locations.isKitchen != 1 && ids.Contains((int)b.itemUnitId)
                                           join u in entity.itemsUnits on b.itemUnitId equals u.itemUnitId
                                           join i in entity.items on u.itemId equals i.itemId
                                           join l in entity.locations on b.locationId equals l.locationId
                                           join s in entity.sections on l.sectionId equals s.sectionId
                                           where s.branchId == branchId

                                           select new ItemLocationModel
                                           {
                                               createDate = b.createDate,
                                               createUserId = b.createUserId,
                                               endDate = b.endDate,
                                               itemsLocId = b.itemsLocId,
                                               itemUnitId = b.itemUnitId,
                                               locationId = b.locationId,
                                               notes = b.notes,
                                               quantity = b.quantity,
                                               startDate = b.startDate,

                                               updateDate = b.updateDate,
                                               updateUserId = b.updateUserId,
                                               itemName = i.name,
                                               unitName = u.units.name,
                                               sectionId = s.sectionId,
                                               isFreeZone = s.isFreeZone,
                                               itemType = i.type,
                                               location = l.x + l.y + l.z,
                                           }).OrderBy(a => a.endDate)
                                            .ToList();

                            return TokenManager.GenerateToken(locList);
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
                    return TokenManager.GenerateToken("0");
                }
            }

        }



        [HttpPost]
        [Route("getShortageItems")]
        public string getShortageItems(string token)
        {
            string message = "";

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
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);

                    }

                }
                try
                {

                    InvoicesController c = new InvoicesController();
                    var orders = c.getUnhandeledOrdersList("or", 0, branchId);

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<ItemTransferModel> requiredTransfers = new List<ItemTransferModel>();
                        foreach (InvoiceModel invoice in orders)
                        {
                            var itemsTransfer = entity.itemsTransfer.Where(x => x.invoiceId == invoice.invoiceId).ToList();
                            foreach (itemsTransfer tr in itemsTransfer)
                            {
                                var lockedQuantity = entity.itemsLocations
                                    .Where(x => x.invoiceId == invoice.invoiceId && x.itemUnitId == tr.itemUnitId)
                                    .Select(x => x.quantity).Sum();
                                var availableAmount = getBranchAmount((long)tr.itemUnitId, branchId);
                                var item = (from i in entity.items
                                            join u in entity.itemsUnits on i.itemId equals u.itemId
                                            where u.itemUnitId == tr.itemUnitId
                                            select new ItemModel()
                                            {
                                                itemId = i.itemId,
                                                name = i.name,
                                                unitName = u.units.name,
                                            }).FirstOrDefault();
                                if (lockedQuantity == null)
                                    lockedQuantity = 0;
                                if ((lockedQuantity + availableAmount) < tr.quantity) // there is a shortage in order amount
                                {
                                    long requiredQuantity = (long)tr.quantity - ((long)lockedQuantity + (long)availableAmount);
                                    ItemTransferModel transfer = new ItemTransferModel()
                                    {
                                        invNumber = invoice.invNumber,
                                        invoiceId = invoice.invoiceId,
                                        price = 0,
                                        quantity = requiredQuantity,
                                        itemUnitId = tr.itemUnitId,
                                        itemId = item.itemId,
                                        itemName = item.name,
                                        unitName = item.unitName,
                                    };
                                    requiredTransfers.Add(transfer);
                                }

                            }
                        }
                        return TokenManager.GenerateToken(requiredTransfers);
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
        [Route("unlockItem")]
        public string unlockItem(string token)
        {
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                long branchId = 0;
                itemsLocations newObject = new itemsLocations();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {

                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<itemsLocations>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "branchId")
                        branchId = long.Parse(c.Value);
                }

                if (newObject != null)
                {
                    try
                    {
                        DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var itemLoc = (from b in entity.itemsLocations
                                           where b.invoiceId == null && b.locations.isKitchen != 1 && b.itemUnitId == newObject.itemUnitId && b.locationId == newObject.locationId
                                           && b.startDate == newObject.startDate && b.endDate == newObject.endDate
                                           && b.locations.sections.branchId == branchId
                                           select new ItemLocationModel
                                           {
                                               itemsLocId = b.itemsLocId,
                                           }).FirstOrDefault();
                            var orderItem = entity.itemsLocations.Find(newObject.itemsLocId);
                            if (orderItem.quantity == newObject.quantity)
                                entity.itemsLocations.Remove(orderItem);
                            else
                                orderItem.quantity -= newObject.quantity;

                            if (itemLoc == null)
                            {
                                var loc = new itemsLocations()
                                {
                                    locationId = newObject.locationId,
                                    quantity = newObject.quantity,
                                    createDate = datenow,
                                    updateDate = datenow,
                                    createUserId = newObject.createUserId,
                                    updateUserId = newObject.createUserId,
                                    startDate = newObject.startDate,
                                    endDate = newObject.endDate,
                                    itemUnitId = newObject.itemUnitId,
                                    notes = newObject.notes,
                                };
                                entity.itemsLocations.Add(loc);
                            }
                            else
                            {
                                var loc = entity.itemsLocations.Find(itemLoc.itemsLocId);
                                loc.quantity += newObject.quantity;
                                loc.updateDate = datenow;
                                loc.updateUserId = newObject.updateUserId;

                            }
                            entity.SaveChanges();
                        }
                        return TokenManager.GenerateToken("1");
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }

                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }


            }
        }
        public void unlockQuantity(long invoiceId, long itemUnitId, int quantity)
        {
            string Object = "";  
            try
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemLoc = entity.itemsLocations.Where(b => b.invoiceId == invoiceId && b.itemUnitId == itemUnitId).FirstOrDefault();
                    itemLoc.quantity -= quantity;
                    if (itemLoc.quantity == 0)
                        entity.itemsLocations.Remove(itemLoc);
                    var location = entity.itemsLocations.Where(x => x.invoiceId == null && x.locationId == itemLoc.locationId && x.locations.isKitchen != 1 && 
                                    x.itemUnitId == itemLoc.itemUnitId && x.startDate == itemLoc.startDate && x.endDate == itemLoc.endDate).FirstOrDefault();


                    if (location == null)
                    {
                        var loc = new itemsLocations()
                        {
                            locationId = itemLoc.locationId,
                            quantity = quantity,
                            createDate = datenow,
                            updateDate = datenow,
                            createUserId = itemLoc.createUserId,
                            updateUserId = itemLoc.createUserId,
                            startDate = itemLoc.startDate,
                            endDate = itemLoc.endDate,
                            itemUnitId = itemLoc.itemUnitId,
                            notes = itemLoc.notes,
                        };
                        entity.itemsLocations.Add(loc);
                    }
                    else
                    {
                        location.quantity += quantity;
                        location.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        location.updateUserId = itemLoc.updateUserId;
                    }
                    entity.SaveChanges();
                }
            }
            catch
            {
               
            }   
        }

        
    }
}
