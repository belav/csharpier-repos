using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;
using Newtonsoft.Json.Converters;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/ItemsTransfer")]
    public class ItemsTransferController : ApiController
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
                long invoiceId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                try
                {
                    var transferList = Get(invoiceId);
                    return TokenManager.GenerateToken(transferList);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public List<ItemTransferModel> Get(long invoiceId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var transferList = (
                    from t in entity.itemsTransfer.Where(
                        x => x.invoiceId == invoiceId && x.mainCourseId == null
                    )
                    join u in entity.itemsUnits on t.itemUnitId equals u.itemUnitId
                    join i in entity.items on u.itemId equals i.itemId
                    join un in entity.units on u.unitId equals un.unitId
                    join inv in entity.invoices on t.invoiceId equals inv.invoiceId
                    select new ItemTransferModel()
                    {
                        itemsTransId = t.itemsTransId,
                        itemId = i.itemId,
                        itemName = i.name,
                        quantity = t.quantity,
                        invoiceId = entity.invoiceOrder
                            .Where(x => x.itemsTransferId == t.itemsTransId)
                            .Select(x => x.orderId)
                            .FirstOrDefault(),
                        invNumber = inv.invNumber,
                        createUserId = t.createUserId,
                        updateUserId = t.updateUserId,
                        notes = t.notes,
                        createDate = t.createDate,
                        updateDate = t.updateDate,
                        itemUnitId = u.itemUnitId,
                        price = t.price,
                        unitName = un.name,
                        unitId = un.unitId,
                        barcode = u.barcode,
                        itemSerial = t.itemSerial,
                        itemType = i.type,
                        offerId = t.offerId,
                        forAgents = t.forAgents,
                    }
                ).ToList();

                foreach (var item in transferList)
                {
                    item.itemsIngredients = entity.itemsTransferIngredients
                        .Where(x => x.itemsTransId == item.itemsTransId)
                        .Select(
                            x =>
                                new itemsTransferIngredientsModel()
                                {
                                    dishIngredId = x.dishIngredId,
                                    isActive = x.isActive,
                                    DishIngredientName = x.dishIngredients.name,
                                    itemUnitId = x.itemsTransfer.itemUnitId,
                                    itemsTransId = x.itemsTransId,
                                    isBasic = x.dishIngredients.isBasic,
                                    itemsTransIngredId = x.itemsTransIngredId
                                }
                        )
                        .ToList();

                    item.itemExtras = (
                        from t in entity.itemsTransfer.Where(
                            x => x.mainCourseId == item.itemsTransId
                        )
                        join u in entity.itemsUnits on t.itemUnitId equals u.itemUnitId
                        join i in entity.items on u.itemId equals i.itemId
                        join un in entity.units on u.unitId equals un.unitId
                        join inv in entity.invoices on t.invoiceId equals inv.invoiceId
                        select new ItemTransferModel()
                        {
                            itemsTransId = t.itemsTransId,
                            itemId = i.itemId,
                            itemName = i.name,
                            quantity = t.quantity,
                            invoiceId = entity.invoiceOrder
                                .Where(x => x.itemsTransferId == t.itemsTransId)
                                .Select(x => x.orderId)
                                .FirstOrDefault(),
                            invNumber = inv.invNumber,
                            createUserId = t.createUserId,
                            updateUserId = t.updateUserId,
                            notes = t.notes,
                            createDate = t.createDate,
                            updateDate = t.updateDate,
                            itemUnitId = u.itemUnitId,
                            price = t.price,
                            unitName = un.name,
                            unitId = un.unitId,
                            barcode = u.barcode,
                            itemSerial = t.itemSerial,
                            itemType = i.type,
                            offerId = t.offerId,
                            forAgents = t.forAgents,
                        }
                    ).ToList();
                }
                return transferList;
            }
        }

        // add or update item transfer
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
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
                long invoiceId = 0;
                string Object = "";
                List<itemsTransfer> newObject = new List<itemsTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                if (newObject != null)
                {
                    try
                    {
                        string res = saveInvoiceItems(newObject, invoiceId);
                        if (res == "0")
                            message = "0";
                        else
                            message = "1";
                        return TokenManager.GenerateToken(message);
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

        public string saveInvoiceItems(List<itemsTransfer> newObject, long invoiceId)
        {
            string message = "";
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<invoiceOrder> iol = entity.invoiceOrder
                        .Where(x => x.invoiceId == invoiceId)
                        .ToList();
                    entity.invoiceOrder.RemoveRange(iol);
                    entity.SaveChanges();

                    List<itemsTransfer> items = entity.itemsTransfer
                        .Where(x => x.invoiceId == invoiceId)
                        .ToList();
                    entity.itemsTransfer.RemoveRange(items);
                    entity.SaveChanges();

                    var invoice = entity.invoices.Find(invoiceId);
                    for (int i = 0; i < newObject.Count; i++)
                    {
                        itemsTransfer t;
                        if (newObject[i].createUserId == 0 || newObject[i].createUserId == null)
                        {
                            Nullable<long> id = null;
                            newObject[i].createUserId = id;
                        }
                        if (newObject[i].offerId == 0)
                        {
                            Nullable<long> id = null;
                            newObject[i].offerId = id;
                        }
                        if (newObject[i].itemSerial == null)
                            newObject[i].itemSerial = "";

                        var transferEntity = entity.Set<itemsTransfer>();
                        long orderId = 0;
                        try
                        {
                            orderId = (int)newObject[i].invoiceId;
                        }
                        catch { }

                        newObject[i].invoiceId = invoiceId;
                        newObject[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject[i].updateUserId = newObject[i].createUserId;

                        t = entity.itemsTransfer.Add(newObject[i]);
                        entity.SaveChanges();

                        if (orderId != 0)
                        {
                            invoiceOrder invoiceOrder = new invoiceOrder()
                            {
                                invoiceId = invoiceId,
                                orderId = orderId,
                                quantity = (int)newObject[i].quantity,
                                itemsTransferId = t.itemsTransId,
                            };
                            entity.invoiceOrder.Add(invoiceOrder);
                        }
                        if (newObject[i].offerId != null && invoice.invType == "s")
                        {
                            long offerId = (int)newObject[i].offerId;
                            long itemUnitId = (int)newObject[i].itemUnitId;
                            var offer = entity.itemsOffers
                                .Where(x => x.iuId == itemUnitId && x.offerId == offerId)
                                .FirstOrDefault();

                            offer.used += (int)newObject[i].quantity;
                        }
                    }
                    entity.SaveChanges();
                    message = "1";
                }
            }
            catch
            {
                message = "0";
            }
            return message;
        }

        public string saveSalesInvoiceItems(
            List<itemsTransfer> newObject,
            List<ItemTransferModel> itemsTransfer,
            long invoiceId
        )
        {
            string message = "";
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<itemsTransfer> items = entity.itemsTransfer
                        .Where(x => x.invoiceId == invoiceId)
                        .ToList();

                    foreach (var item in items)
                    {
                        var orderItem = entity.itemOrderPreparing
                            .Where(x => x.itemsTransId == item.itemsTransId)
                            .FirstOrDefault();
                        if (orderItem == null)
                        {
                            // remove transfer ingredients
                            var itemIngredients = entity.itemsTransferIngredients
                                .Where(x => x.itemsTransId == item.itemsTransId)
                                .ToList();
                            entity.itemsTransferIngredients.RemoveRange(itemIngredients);

                            //remove item transfer extra
                            var extras = entity.itemsTransfer
                                .Where(x => x.mainCourseId == item.itemsTransId)
                                .ToList();
                            entity.itemsTransferIngredients.RemoveRange(itemIngredients);
                            entity.SaveChanges();

                            var it = entity.itemsTransfer.Find(item.itemsTransId);
                            entity.itemsTransfer.Remove(it);
                        }
                    }
                    //entity.SaveChanges();

                    //entity.itemsTransfer.RemoveRange(items);
                    entity.SaveChanges();

                    var invoice = entity.invoices.Find(invoiceId);
                    for (int i = 0; i < newObject.Count; i++)
                    {
                        long itemUnitId = (long)newObject[i].itemUnitId;

                        #region get avg price for item
                        var avgPrice = entity.items
                            .Where(
                                m =>
                                    m.itemId
                                    == entity.itemsUnits
                                        .Where(x => x.itemUnitId == itemUnitId)
                                        .Select(x => x.itemId)
                                        .FirstOrDefault()
                            )
                            .Select(m => m.avgPurchasePrice)
                            .Single();
                        #endregion

                        itemOrderPreparing orderItem = null;
                        if (newObject[i].itemsTransId != 0)
                        {
                            long id = newObject[i].itemsTransId;
                            orderItem = entity.itemOrderPreparing
                                .Where(x => x.itemsTransId == id)
                                .FirstOrDefault();
                        }

                        if (orderItem == null)
                        {
                            itemsTransfer t;
                            if (newObject[i].createUserId == 0 || newObject[i].createUserId == null)
                            {
                                Nullable<long> id = null;
                                newObject[i].createUserId = id;
                            }
                            if (newObject[i].offerId == 0)
                            {
                                Nullable<long> id = null;
                                newObject[i].offerId = id;
                            }
                            if (newObject[i].itemSerial == null)
                                newObject[i].itemSerial = "";

                            var transferEntity = entity.Set<itemsTransfer>();

                            newObject[i].invoiceId = invoiceId;
                            newObject[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject[i].updateUserId = newObject[i].createUserId;
                            newObject[i].purchasePrice = avgPrice;

                            t = entity.itemsTransfer.Add(newObject[i]);
                            entity.SaveChanges();

                            if (newObject[i].mainCourseId == null)
                            {
                                #region add ingrediants

                                foreach (var ing in itemsTransfer[i].itemsIngredients)
                                {
                                    var itemIngredient = new itemsTransferIngredients()
                                    {
                                        dishIngredId = ing.dishIngredId,
                                        isActive = ing.isActive,
                                        itemsTransId = t.itemsTransId,
                                        notes = t.notes,
                                    };
                                    entity.itemsTransferIngredients.Add(itemIngredient);
                                    entity.SaveChanges();
                                }

                                #endregion
                                #region add extras
                                foreach (var itm in itemsTransfer[i].itemExtras)
                                {
                                    var str = JsonConvert.SerializeObject(itm);
                                    itemsTransfer it = new itemsTransfer();
                                    it = JsonConvert.DeserializeObject<itemsTransfer>(
                                        str,
                                        new JsonSerializerSettings
                                        {
                                            DateParseHandling = DateParseHandling.None
                                        }
                                    );

                                    if (it.createUserId == 0 || it.createUserId == null)
                                    {
                                        Nullable<long> id = null;
                                        it.createUserId = id;
                                    }
                                    if (it.offerId == 0)
                                    {
                                        Nullable<long> id = null;
                                        it.offerId = id;
                                    }
                                    if (it.itemSerial == null)
                                        it.itemSerial = "";

                                    it.mainCourseId = t.itemsTransId;
                                    it.invoiceId = invoiceId;
                                    it.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    it.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    it.updateUserId = it.createUserId;

                                    entity.itemsTransfer.Add(it);
                                    entity.SaveChanges();
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            var itemT = entity.itemsTransfer.Find(newObject[i].itemsTransId);
                            itemT.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            itemT.updateUserId = newObject[i].createUserId;
                            itemT.quantity = newObject[i].quantity;
                            itemT.price = newObject[i].price;
                            itemT.itemUnitId = newObject[i].itemUnitId;
                            itemT.offerId = newObject[i].offerId;
                            itemT.offerType = newObject[i].offerType;
                            itemT.offerValue = newObject[i].offerValue;
                            itemT.itemTax = newObject[i].itemTax;
                            itemT.itemUnitPrice = newObject[i].itemUnitPrice;
                            itemT.forAgents = newObject[i].forAgents;
                            itemT.purchasePrice = avgPrice;
                        }
                        entity.SaveChanges();
                        if (newObject[i].offerId != null && invoice.invType == "s")
                        {
                            long offerId = (int)newObject[i].offerId;

                            var offer = entity.itemsOffers
                                .Where(x => x.iuId == itemUnitId && x.offerId == offerId)
                                .FirstOrDefault();

                            offer.used += (int)newObject[i].quantity;
                        }
                    }
                    entity.SaveChanges();
                    message = "1";
                }
            }
            catch
            {
                message = "0";
            }
            return message;
        }

        public string saveImExItems(List<itemsTransfer> newObject, long invoiceId, long exportInvId)
        {
            string message = "";

            using (incposdbEntities entity2 = new incposdbEntities())
            {
                List<itemsTransfer> items = entity2.itemsTransfer
                    .Where(x => x.invoiceId == invoiceId)
                    .ToList();
                List<itemsTransfer> expItems = entity2.itemsTransfer
                    .Where(x => x.invoiceId == exportInvId)
                    .ToList();
                entity2.itemsTransfer.RemoveRange(items);
                entity2.itemsTransfer.RemoveRange(expItems);
                entity2.SaveChanges();

                for (int i = 0; i < newObject.Count; i++)
                {
                    itemsTransfer importItem;
                    itemsTransfer exportItem;

                    if (newObject[i].createUserId == 0 || newObject[i].createUserId == null)
                    {
                        Nullable<int> id = null;
                        newObject[i].createUserId = id;
                    }
                    if (newObject[i].offerId == 0)
                    {
                        Nullable<int> id = null;
                        newObject[i].offerId = id;
                    }
                    if (newObject[i].itemSerial == null)
                        newObject[i].itemSerial = "";

                    newObject[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    newObject[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    newObject[i].updateUserId = newObject[i].createUserId;

                    importItem = newObject[i];
                    importItem.invoiceId = (int)invoiceId;

                    entity2.itemsTransfer.Add(importItem);
                    entity2.SaveChanges();

                    exportItem = newObject[i];
                    exportItem.invoiceId = (int)exportInvId;
                    entity2.itemsTransfer.Add(exportItem);

                    entity2.SaveChanges();
                }

                message = "1";
            }
            return message;
        }

        [HttpPost]
        [Route("getOrderItems")]
        public string getOrderItems(string token)
        {
            string message = "";
            ItemsLocationsController ilc = new ItemsLocationsController();
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                long invoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                        branchId = long.Parse(c.Value);
                    else if (c.Type == "invoiceId")
                        invoiceId = long.Parse(c.Value);
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<ItemTransferModel> requiredTransfers = new List<ItemTransferModel>();
                        var itemsTransfer = entity.itemsTransfer
                            .Where(x => x.invoiceId == invoiceId)
                            .ToList();
                        foreach (itemsTransfer tr in itemsTransfer)
                        {
                            var lockedQuantity = entity.itemsLocations
                                .Where(
                                    x => x.invoiceId == invoiceId && x.itemUnitId == tr.itemUnitId
                                )
                                .Select(x => x.quantity)
                                .Sum();
                            var availableAmount = ilc.getBranchAmount(
                                (long)tr.itemUnitId,
                                branchId
                            );
                            var item = (
                                from i in entity.items
                                join u in entity.itemsUnits on i.itemId equals u.itemId
                                where u.itemUnitId == tr.itemUnitId
                                select new ItemModel()
                                {
                                    itemId = i.itemId,
                                    name = i.name,
                                    unitName = u.units.name,
                                }
                            ).FirstOrDefault();
                            if (lockedQuantity == null)
                                lockedQuantity = 0;

                            long requiredQuantity =
                                (long)tr.quantity - ((long)lockedQuantity + (long)availableAmount);
                            ItemTransferModel transfer = new ItemTransferModel()
                            {
                                invoiceId = invoiceId,
                                price = 0,
                                quantity = tr.quantity,
                                lockedQuantity = lockedQuantity,
                                newLocked = lockedQuantity,
                                availableQuantity = availableAmount,
                                itemUnitId = tr.itemUnitId,
                                itemId = item.itemId,
                                itemName = item.name,
                                unitName = item.unitName,
                            };
                            requiredTransfers.Add(transfer);
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
        [Route("GetItemIngredients")]
        public string GetItemIngredients(string token)
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
                long itemTransferId = 0;
                long itemUnitId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemTransferId")
                    {
                        itemTransferId = long.Parse(c.Value);
                    }
                    else if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<itemsTransferIngredientsModel> List =
                        new List<itemsTransferIngredientsModel>();
                    if (itemTransferId != 0)
                    {
                        List = entity.itemsTransferIngredients
                            .Where(S => S.itemsTransId == itemTransferId)
                            .Select(
                                S =>
                                    new itemsTransferIngredientsModel
                                    {
                                        itemsTransIngredId = S.itemsTransIngredId,
                                        dishIngredId = S.dishIngredId,
                                        itemName = S.dishIngredients.itemsUnits.items.name,
                                        DishIngredientName = S.dishIngredients.name,
                                        itemUnitId = S.dishIngredients.itemUnitId,
                                        notes = S.notes,
                                        isActive = S.isActive,
                                        isBasic = S.dishIngredients.isBasic,
                                    }
                            )
                            .ToList();
                    }
                    else
                    {
                        List = entity.dishIngredients
                            .Where(x => x.itemUnitId == itemUnitId && x.isActive == 1)
                            .Select(
                                S =>
                                    new itemsTransferIngredientsModel
                                    {
                                        dishIngredId = S.dishIngredId,
                                        itemName = S.itemsUnits.items.name,
                                        DishIngredientName = S.name,
                                        itemUnitId = S.itemUnitId,
                                        notes = S.notes,
                                        isActive = S.isActive,
                                        isBasic = S.isBasic,
                                    }
                            )
                            .ToList();
                    }

                    return TokenManager.GenerateToken(List);
                }
            }
        }
    }
}
