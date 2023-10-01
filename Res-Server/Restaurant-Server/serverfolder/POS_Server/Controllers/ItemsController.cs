using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using POS_Server.Models;
using System.Web;
using System.IO;
using LinqKit;
using Microsoft.Ajax.Utilities;
using POS_Server.Classes;
using POS_Server.Models.VM;
using System.Security.Claims;
using Newtonsoft.Json.Converters;
using System.Web;
using System.Data.Entity.Validation;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Items")]
    public class ItemsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        private Classes.Calculate Calc = new Classes.Calculate();

        List<long> categoriesId = new List<long>();
        List<string> purchaseTypes = new List<string>() { "PurchaseNormal", "PurchaseExpire" };
        List<string> salesTypes = new List<string>() { "SalesNormal", "packageItems" };

        [HttpPost]
        [Route("GetAllItems")]
        public string GetAllItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                Boolean canDelete = false;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity.items

                        join c in entity.categories on I.categoryId equals c.categoryId into lj
                        from x in lj.DefaultIfEmpty()
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            categoryName = x.name,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();

                    var itemsofferslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            price = iu.price,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                        }
                    ).ToList();
                    itemsofferslist = itemsofferslist
                        .Where(
                            IO =>
                                (
                                    IO.isActiveOffer == 1
                                    && DateTime.Compare(((DateTime)IO.startDate).Date, datenow.Date)
                                        <= 0
                                    && System
                                        .DateTime
                                        .Compare(((DateTime)IO.endDate).Date, datenow.Date) >= 0
                                    && IO.defaultSale == 1
                                )
                                && (((DateTime)IO.startDate)).TimeOfDay <= datenow.TimeOfDay
                                && ((DateTime)IO.endDate).TimeOfDay >= datenow.TimeOfDay
                        )
                        .Distinct()
                        .ToList();
                    //.Where(IO => IO.isActiveOffer == 1 && DateTime.Compare(IO.startDate,DateTime.Now)<0 && System.DateTime.Compare(IO.endDate, DateTime.Now) > 0).ToList();

                    // test

                    var unt = (
                        from unitm in entity.itemsUnits
                        join untb in entity.units on unitm.unitId equals untb.unitId
                        join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                        select new ItemSalePurModel()
                        {
                            itemId = itemtb.itemId,
                            name = itemtb.name,
                            code = itemtb.code,
                            max = itemtb.max,
                            maxUnitId = itemtb.maxUnitId,
                            minUnitId = itemtb.minUnitId,
                            min = itemtb.min,
                            parentId = itemtb.parentId,
                            isActive = itemtb.isActive,
                            isOffer = 0,
                            desPrice = 0,
                            offerName = "",
                            createDate = itemtb.createDate,
                            defaultSale = unitm.defaultSale,
                            unitName = untb.name,
                            unitId = untb.unitId,
                            price = unitm.price,
                        }
                    ).Where(a => a.defaultSale == 1).Distinct().ToList();

                    if (itemsList.Count > 0)
                    {
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            canDelete = false;
                            if (itemsList[i].isActive == 1)
                            {
                                long itemId = (long)itemsList[i].itemId;
                                var childItemL = entity
                                    .items
                                    .Where(x => x.parentId == itemId)
                                    .Select(b => new { b.itemId })
                                    .FirstOrDefault();
                                var itemUnitsL = entity
                                    .itemsUnits
                                    .Where(x => x.itemId == itemId)
                                    .Select(b => new { b.itemUnitId })
                                    .FirstOrDefault();
                                string itemType = itemsList[i].type;
                                long isInInvoice = 0;
                                if (itemUnitsL != null)
                                {
                                    isInInvoice = entity
                                        .itemsTransfer
                                        .Where(x => x.itemUnitId == itemUnitsL.itemUnitId)
                                        .Select(x => x.itemsTransId)
                                        .FirstOrDefault();
                                }

                                if (
                                    childItemL is null
                                    && (
                                        itemUnitsL is null || itemUnitsL != null && isInInvoice == 0
                                    )
                                )
                                    canDelete = true;
                            }
                            itemsList[i].canDelete = canDelete;

                            foreach (var itofflist in itemsofferslist)
                            {
                                if (itemsList[i].itemId == itofflist.itemId)
                                {
                                    // get unit name of item that has the offer
                                    using (incposdbEntities entitydb = new incposdbEntities())
                                    { // put it in item
                                        var un = entitydb
                                            .units
                                            .Where(a => a.unitId == itofflist.unitId)
                                            .Select(u => new { u.name, u.unitId })
                                            .FirstOrDefault();
                                        itemsList[i].unitName = un.name;
                                    }

                                    itemsList[i].offerName =
                                        itemsList[i].offerName + "- " + itofflist.offerName;
                                    itemsList[i].isOffer = 1;
                                    itemsList[i].startDate = itofflist.startDate;
                                    itemsList[i].endDate = itofflist.endDate;
                                    itemsList[i].itemUnitId = itofflist.itemUnitId;
                                    itemsList[i].offerId = itofflist.offerId;
                                    itemsList[i].isActiveOffer = itofflist.isActiveOffer;

                                    itemsList[i].price = itofflist.price;
                                    itemsList[i].priceTax =
                                        itemsList[i].price
                                        + (itemsList[i].price * itemsList[i].taxes / 100);

                                    itemsList[i].avgPurchasePrice = itemsList[i].avgPurchasePrice;
                                }
                            }
                            //itemsList[i].desPrice = itemsList[i].priceTax - totaldis;
                            // is new
                            int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                            if (res >= 0)
                            {
                                itemsList[i].isNew = 1;
                            }
                        }
                    }
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetPurchaseItems")]
        public string GetPurchaseItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                Boolean canDelete = false;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity
                            .items
                            .Where(x => purchaseTypes.Contains(x.type) && x.isActive == 1)
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            categoryName = I.categories.name,
                            itemUnitId = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultPurchase == 1)
                                .FirstOrDefault()
                                .itemUnitId,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();

                    var itemsofferslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            price = iu.price,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                        }
                    ).Where(IO => IO.isActiveOffer == 1 && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1).Distinct().ToList();
                    //.Where(IO => IO.isActiveOffer == 1 && DateTime.Compare(IO.startDate,DateTime.Now)<0 && System.DateTime.Compare(IO.endDate, DateTime.Now) > 0).ToList();

                    // test

                    var unt = (
                        from unitm in entity.itemsUnits
                        join untb in entity.units on unitm.unitId equals untb.unitId
                        join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                        select new ItemSalePurModel()
                        {
                            itemId = itemtb.itemId,
                            name = itemtb.name,
                            code = itemtb.code,
                            max = itemtb.max,
                            maxUnitId = itemtb.maxUnitId,
                            minUnitId = itemtb.minUnitId,
                            min = itemtb.min,
                            parentId = itemtb.parentId,
                            isActive = itemtb.isActive,
                            isOffer = 0,
                            desPrice = 0,
                            offerName = "",
                            createDate = itemtb.createDate,
                            defaultSale = unitm.defaultSale,
                            unitName = untb.name,
                            unitId = untb.unitId,
                            price = unitm.price,
                        }
                    ).Where(a => a.defaultSale == 1).Distinct().ToList();

                    if (itemsList.Count > 0)
                    {
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            canDelete = false;
                            if (itemsList[i].isActive == 1)
                            {
                                long itemId = (long)itemsList[i].itemId;
                                var childItemL = entity
                                    .items
                                    .Where(x => x.parentId == itemId)
                                    .Select(b => new { b.itemId })
                                    .FirstOrDefault();
                                var itemUnitsL = entity
                                    .itemsUnits
                                    .Where(x => x.itemId == itemId)
                                    .Select(b => new { b.itemUnitId })
                                    .FirstOrDefault();
                                string itemType = itemsList[i].type;
                                long isInInvoice = 0;
                                if (itemUnitsL != null)
                                {
                                    isInInvoice = entity
                                        .itemsTransfer
                                        .Where(x => x.itemUnitId == itemUnitsL.itemUnitId)
                                        .Select(x => x.itemsTransId)
                                        .FirstOrDefault();
                                }

                                if (
                                    childItemL is null
                                    && (
                                        itemUnitsL is null || itemUnitsL != null && isInInvoice == 0
                                    )
                                )
                                    canDelete = true;
                            }
                            itemsList[i].canDelete = canDelete;

                            foreach (var itofflist in itemsofferslist)
                            {
                                if (itemsList[i].itemId == itofflist.itemId)
                                {
                                    // get unit name of item that has the offer
                                    using (incposdbEntities entitydb = new incposdbEntities())
                                    { // put it in item
                                        var un = entitydb
                                            .units
                                            .Where(a => a.unitId == itofflist.unitId)
                                            .Select(u => new { u.name, u.unitId })
                                            .FirstOrDefault();
                                        itemsList[i].unitName = un.name;
                                    }

                                    itemsList[i].offerName =
                                        itemsList[i].offerName + "- " + itofflist.offerName;
                                    itemsList[i].isOffer = 1;
                                    itemsList[i].startDate = itofflist.startDate;
                                    itemsList[i].endDate = itofflist.endDate;
                                    itemsList[i].itemUnitId = itofflist.itemUnitId;
                                    itemsList[i].offerId = itofflist.offerId;
                                    itemsList[i].isActiveOffer = itofflist.isActiveOffer;

                                    itemsList[i].price = itofflist.price;
                                    itemsList[i].priceTax =
                                        itemsList[i].price
                                        + (itemsList[i].price * itemsList[i].taxes / 100);

                                    itemsList[i].avgPurchasePrice = itemsList[i].avgPurchasePrice;
                                }
                            }
                            // is new
                            int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                            if (res >= 0)
                            {
                                itemsList[i].isNew = 1;
                            }
                        }
                    }
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetAllSalesItems")]
        public string GetAllSalesItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity
                            .items
                            .Where(x => salesTypes.Contains(x.type) && x.isActive == 1)
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            tagId = I.tagId,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            itemUnitId = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .itemUnitId,
                            price = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .price,
                            priceWithService = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .priceWithService,
                            categoryName = I.categories.name,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();

                    if (itemsList.Count > 0)
                    {
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            // is new
                            int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                            if (res >= 0)
                            {
                                itemsList[i].isNew = 1;
                            }
                        }
                    }
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetAllSalesItemsInv")]
        public string GetAllSalesItemsInv(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string day = "";
                string invType = "";
                long branchId = 0;
                long membershipId = 0;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "day")
                    {
                        day = c.Value;
                    }
                    else if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                    else if (c.Type == "membershipId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                #endregion
                #region get day column in db
                var searchPredicate = PredicateBuilder.New<menuSettings>();
                searchPredicate = searchPredicate.And(
                    x => x.isActive == 1 && x.branchId == branchId
                );
                switch (day)
                {
                    case "saturday":
                        searchPredicate = searchPredicate.And(x => x.sat == true);
                        break;
                    case "sunday":
                        searchPredicate = searchPredicate.And(x => x.sun == true);
                        break;
                    case "monday":
                        searchPredicate = searchPredicate.And(x => x.mon == true);
                        break;
                    case "tuesday":
                        searchPredicate = searchPredicate.And(x => x.tues == true);
                        break;
                    case "wednsday":
                        searchPredicate = searchPredicate.And(x => x.wed == true);
                        break;
                    case "thursday":
                        searchPredicate = searchPredicate.And(x => x.thur == true);
                        break;
                    case "friday":
                        searchPredicate = searchPredicate.And(x => x.fri == true);
                        break;
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity
                            .items
                            .Where(x => salesTypes.Contains(x.type) && x.isActive == 1)
                        join IU in entity.itemsUnits on I.itemId equals IU.itemId
                        join ms in entity.menuSettings.Where(searchPredicate)
                            on IU.itemUnitId equals ms.itemUnitId
                        select new ItemSalePurModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            tagId = I.tagId,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            itemUnitId = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .itemUnitId,
                            price = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .price,
                            basicPrice = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .price,
                            priceWithService = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .priceWithService,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();
                    #region offers

                    var offerslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            itemCount = itof.quantity,
                            price = iu.price,
                            priceWithService = iu.priceWithService,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                            used = itof.used,
                            forAgent = off.forAgents,
                            isActive = off.isActive,
                        }
                    ).Where(IO => IO.isActive == 1 && IO.isActiveOffer == 1 && IO.forAgent == "pb" && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1 && IO.itemCount > IO.used).Distinct().ToList();

                    var membershipOffers = (
                        from off in entity.offers
                        join mo in entity
                            .membershipsOffers
                            .Where(x => x.membershipId == membershipId)
                            on off.offerId equals mo.offerId

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            itemCount = itof.quantity,
                            price = iu.price,
                            priceWithService = iu.priceWithService,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                            used = itof.used,
                            forAgent = off.forAgents,
                            isActive = off.isActive,
                        }
                    ).Where(IO => IO.isActive == 1 && IO.isActiveOffer == 1 && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1 && IO.itemCount > IO.used).Distinct().ToList();

                    // return membershipOffers.Count.ToString();
                    offerslist.AddRange(membershipOffers);

                    #endregion

                    for (int i = 0; i < itemsList.Count; i++)
                    {
                        if (invType == "diningHall")
                        {
                            itemsList[i].price = itemsList[i].priceWithService;
                        }

                        itemsList[i].priceTax =
                            itemsList[i].price + (itemsList[i].price * itemsList[i].priceTax) / 100;
                        // is new
                        int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                        if (res >= 0)
                        {
                            itemsList[i].isNew = 1;
                        }

                        decimal totaldis = 0;
                        foreach (var itofflist in offerslist)
                        {
                            if (itemsList[i].itemId == itofflist.itemId)
                            {
                                // get unit name of item that has the offer
                                using (incposdbEntities entitydb = new incposdbEntities())
                                { // put it in item
                                    var un = entitydb
                                        .units
                                        .Where(a => a.unitId == itofflist.unitId)
                                        .Select(u => new { u.name, u.unitId })
                                        .FirstOrDefault();
                                    itemsList[i].unitName = un.name;
                                }

                                itemsList[i].offerName =
                                    itemsList[i].offerName + "- " + itofflist.offerName;
                                itemsList[i].isOffer = 1;
                                itemsList[i].startDate = itofflist.startDate;
                                itemsList[i].endDate = itofflist.endDate;
                                itemsList[i].itemUnitId = itofflist.itemUnitId;
                                itemsList[i].offerId = itofflist.offerId;
                                itemsList[i].isActiveOffer = itofflist.isActiveOffer;
                                itemsList[i].forAgent = itofflist.forAgent;

                                if (invType == "diningHall")
                                {
                                    itofflist.price = itofflist.priceWithService;
                                }

                                itemsList[i].price = itofflist.price;
                                itemsList[i].priceTax =
                                    itemsList[i].price
                                    + (itemsList[i].price * itemsList[i].taxes / 100);
                                itemsList[i].avgPurchasePrice = itemsList[i].avgPurchasePrice;
                                itemsList[i].discountType = itofflist.discountType;
                                itemsList[i].discountValue = itofflist.discountValue;

                                if (itofflist.used == null)
                                    itofflist.used = 0;

                                if (
                                    itemsList[i].itemCount >= (itofflist.itemCount - itofflist.used)
                                )
                                    itemsList[i].itemCount = (itofflist.itemCount - itofflist.used);

                                if (itemsList[i].discountType == "1") // value
                                {
                                    totaldis = totaldis + (decimal)itemsList[i].discountValue;
                                }
                                else if (itemsList[i].discountType == "2") // percent
                                {
                                    totaldis =
                                        totaldis
                                        + Calc.percentValue(
                                            itemsList[i].price,
                                            itemsList[i].discountValue
                                        );
                                }
                            }
                        }
                        itemsList[i].price = (decimal)itemsList[i].price - totaldis;
                        itemsList[i].priceTax =
                            itemsList[i].price + (itemsList[i].price * itemsList[i].taxes / 100);

                        if (itemsList[i].price < 0)
                        {
                            itemsList[i].price = 0;
                            itemsList[i].priceTax = 0;
                        }
                    }

                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetItemExtras")]
        public string GetItemExtras(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string invType = "";
                long itemId = 0;
                long membershipId = 0;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                    else if (c.Type == "membershipId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                    else if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                }
                #endregion

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity.items.Where(x => x.isActive == 1)
                        join IE in entity.itemsExtra.Where(x => x.itemId == itemId)
                            on I.itemId equals IE.extraId
                        join IU in entity.itemsUnits on I.itemId equals IU.itemId
                        select new ItemSalePurModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            tagId = I.tagId,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            itemUnitId = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .itemUnitId,
                            price = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .price,
                            basicPrice = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .price,
                            priceWithService = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId && m.defaultSale == 1)
                                .FirstOrDefault()
                                .priceWithService,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();
                    #region offers

                    var offerslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            itemCount = itof.quantity,
                            price = iu.price,
                            priceWithService = iu.priceWithService,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                            used = itof.used,
                            forAgent = off.forAgents,
                            isActive = off.isActive,
                        }
                    ).Where(IO => IO.isActive == 1 && IO.isActiveOffer == 1 && IO.forAgent == "pb" && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1 && IO.itemCount > IO.used).Distinct().ToList();

                    var membershipOffers = (
                        from off in entity.offers
                        join mo in entity
                            .membershipsOffers
                            .Where(x => x.membershipId == membershipId)
                            on off.offerId equals mo.offerId

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            itemCount = itof.quantity,
                            price = iu.price,
                            priceWithService = iu.priceWithService,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                            used = itof.used,
                            forAgent = off.forAgents,
                            isActive = off.isActive,
                        }
                    ).Where(IO => IO.isActive == 1 && IO.isActiveOffer == 1 && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1 && IO.itemCount > IO.used).Distinct().ToList();

                    // return membershipOffers.Count.ToString();
                    offerslist.AddRange(membershipOffers);

                    #endregion

                    for (int i = 0; i < itemsList.Count; i++)
                    {
                        if (invType == "diningHall")
                        {
                            itemsList[i].price = itemsList[i].priceWithService;
                        }

                        itemsList[i].priceTax =
                            itemsList[i].price + (itemsList[i].price * itemsList[i].priceTax) / 100;
                        // is new
                        int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                        if (res >= 0)
                        {
                            itemsList[i].isNew = 1;
                        }

                        decimal totaldis = 0;
                        foreach (var itofflist in offerslist)
                        {
                            if (itemsList[i].itemId == itofflist.itemId)
                            {
                                // get unit name of item that has the offer
                                using (incposdbEntities entitydb = new incposdbEntities())
                                { // put it in item
                                    var un = entitydb
                                        .units
                                        .Where(a => a.unitId == itofflist.unitId)
                                        .Select(u => new { u.name, u.unitId })
                                        .FirstOrDefault();
                                    itemsList[i].unitName = un.name;
                                }

                                itemsList[i].offerName =
                                    itemsList[i].offerName + "- " + itofflist.offerName;
                                itemsList[i].isOffer = 1;
                                itemsList[i].startDate = itofflist.startDate;
                                itemsList[i].endDate = itofflist.endDate;
                                itemsList[i].itemUnitId = itofflist.itemUnitId;
                                itemsList[i].offerId = itofflist.offerId;
                                itemsList[i].isActiveOffer = itofflist.isActiveOffer;
                                itemsList[i].forAgent = itofflist.forAgent;

                                if (invType == "diningHall")
                                {
                                    itofflist.price = itofflist.priceWithService;
                                }

                                itemsList[i].price = itofflist.price;
                                itemsList[i].priceTax =
                                    itemsList[i].price
                                    + (itemsList[i].price * itemsList[i].taxes / 100);
                                itemsList[i].avgPurchasePrice = itemsList[i].avgPurchasePrice;
                                itemsList[i].discountType = itofflist.discountType;
                                itemsList[i].discountValue = itofflist.discountValue;

                                if (itofflist.used == null)
                                    itofflist.used = 0;

                                if (
                                    itemsList[i].itemCount >= (itofflist.itemCount - itofflist.used)
                                )
                                    itemsList[i].itemCount = (itofflist.itemCount - itofflist.used);

                                if (itemsList[i].discountType == "1") // value
                                {
                                    totaldis = totaldis + (decimal)itemsList[i].discountValue;
                                }
                                else if (itemsList[i].discountType == "2") // percent
                                {
                                    totaldis =
                                        totaldis
                                        + Calc.percentValue(
                                            itemsList[i].price,
                                            itemsList[i].discountValue
                                        );
                                }
                            }
                        }
                        itemsList[i].price = (decimal)itemsList[i].price - totaldis;
                        itemsList[i].priceTax =
                            itemsList[i].price + (itemsList[i].price * itemsList[i].taxes / 100);

                        if (itemsList[i].price < 0)
                        {
                            itemsList[i].price = 0;
                            itemsList[i].priceTax = 0;
                        }
                    }

                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetItemsMenuSetting")]
        public string GetItemsMenuSetting(string token)
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
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);

                using (incposdbEntities entity = new incposdbEntities())
                {
                    try
                    {
                        var itemsList = (
                            from I in entity
                                .items
                                .Where(
                                    x =>
                                        salesTypes.Contains(x.type)
                                        && x.isActive == 1
                                        && x.categories.name != "extraOrders"
                                )
                            join iu in entity.itemsUnits on I.itemId equals iu.itemId
                            join m in entity.menuSettings.Where(x => x.branchId == branchId)
                                on iu.itemUnitId equals m.itemUnitId
                                into yj
                            from ms in yj.DefaultIfEmpty()
                            select new MenuSettingModel()
                            {
                                menuSettingId = ms.menuSettingId,
                                isActive = ms.isActive == null ? (byte)0 : ms.isActive,
                                preparingTime = ms.preparingTime,
                                sat = ms.sat == null ? false : ms.sat,
                                sun = ms.sun == null ? false : ms.sun,
                                mon = ms.mon == null ? false : ms.mon,
                                tues = ms.tues == null ? false : ms.tues,
                                wed = ms.wed == null ? false : ms.wed,
                                thur = ms.thur == null ? false : ms.thur,
                                fri = ms.fri == null ? false : ms.fri,
                                branchId = ms.branchId,
                                itemId = I.itemId,
                                name = I.name,
                                code = I.code,
                                categoryId = I.categoryId,
                                tagId = I.tagId,
                                image = I.image,
                                type = I.type,
                                //details = I.details,
                                createDate = I.createDate,
                                updateDate = I.updateDate,
                                createUserId = ms.createUserId,
                                updateUserId = ms.updateUserId,
                                itemUnitId = iu.itemUnitId,
                                price = iu.price,
                                priceWithService = iu.priceWithService,
                                isExpired = I.isExpired,
                                alertDays = I.alertDays,
                            }
                        )
                        //.Distinct()
                        .ToList();

                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            // is new
                            int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                            if (res >= 0)
                            {
                                itemsList[i].isNew = 1;
                            }
                        }

                        return TokenManager.GenerateToken(itemsList);
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        return TokenManager.GenerateToken("0");
                        //string message = "";
                        //foreach (var validationErrors in dbEx.EntityValidationErrors)
                        //{
                        //    foreach (var validationError in validationErrors.ValidationErrors)
                        //    {
                        //       message += "Property: {0} Error: {1}"+ validationError.PropertyName+ validationError.ErrorMessage;
                        //    }
                        //}
                        //return message;
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetSalesItems")]
        public string GetSalesItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                Boolean canDelete = false;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<items>();
                    searchPredicate = searchPredicate.And(x => true);
                    if (type != "")
                        searchPredicate = searchPredicate.And(x => x.type == type);
                    else
                        searchPredicate = searchPredicate.And(x => salesTypes.Contains(x.type));
                    var itemsList = (
                        from I in entity.items.Where(searchPredicate)
                        join u in entity.itemsUnits on I.itemId equals u.itemId
                        join c in entity.categories on I.categoryId equals c.categoryId into lj
                        from x in lj.DefaultIfEmpty()
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            categoryName = x.name,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            barcode = u.barcode,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            tagId = I.tagId,
                            priceWithService = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId)
                                .FirstOrDefault()
                                .priceWithService,
                            price = entity
                                .itemsUnits
                                .Where(m => m.itemId == I.itemId)
                                .FirstOrDefault()
                                .price,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();

                    var itemsofferslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            price = iu.price,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                        }
                    ).Where(IO => IO.isActiveOffer == 1 && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1).Distinct().ToList();

                    //var unt = (from unitm in entity.itemsUnits
                    //           join untb in entity.units on unitm.unitId equals untb.unitId
                    //           join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                    //           select new ItemSalePurModel()
                    //           {
                    //               itemId = itemtb.itemId,
                    //               name = itemtb.name,
                    //               code = itemtb.code,


                    //               max = itemtb.max,
                    //               maxUnitId = itemtb.maxUnitId,
                    //               minUnitId = itemtb.minUnitId,
                    //               min = itemtb.min,

                    //               parentId = itemtb.parentId,
                    //               isActive = itemtb.isActive,

                    //               isOffer = 0,
                    //               desPrice = 0,

                    //               offerName = "",
                    //               createDate = itemtb.createDate,
                    //               defaultSale = unitm.defaultSale,
                    //               unitName = untb.name,
                    //               unitId = untb.unitId,
                    //               price = unitm.price,

                    //           }).Where(a => a.defaultSale == 1).Distinct().ToList();

                    if (itemsList.Count > 0)
                    {
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            canDelete = false;
                            if (itemsList[i].isActive == 1)
                            {
                                long itemId = (long)itemsList[i].itemId;
                                var childItemL = entity
                                    .items
                                    .Where(x => x.parentId == itemId)
                                    .Select(b => new { b.itemId })
                                    .FirstOrDefault();
                                var itemUnitsL = entity
                                    .itemsUnits
                                    .Where(x => x.itemId == itemId)
                                    .Select(b => new { b.itemUnitId })
                                    .FirstOrDefault();
                                string itemType = itemsList[i].type;
                                long isInInvoice = 0;
                                if (itemUnitsL != null)
                                {
                                    isInInvoice = entity
                                        .itemsTransfer
                                        .Where(x => x.itemUnitId == itemUnitsL.itemUnitId)
                                        .Select(x => x.itemsTransId)
                                        .FirstOrDefault();
                                }

                                if (
                                    childItemL is null
                                    && (
                                        itemUnitsL is null || itemUnitsL != null && isInInvoice == 0
                                    )
                                )
                                    canDelete = true;
                            }
                            itemsList[i].canDelete = canDelete;

                            foreach (var itofflist in itemsofferslist)
                            {
                                if (itemsList[i].itemId == itofflist.itemId)
                                {
                                    // get unit name of item that has the offer
                                    using (incposdbEntities entitydb = new incposdbEntities())
                                    { // put it in item
                                        var un = entitydb
                                            .units
                                            .Where(a => a.unitId == itofflist.unitId)
                                            .Select(u => new { u.name, u.unitId })
                                            .FirstOrDefault();
                                        itemsList[i].unitName = un.name;
                                    }

                                    itemsList[i].offerName =
                                        itemsList[i].offerName + "- " + itofflist.offerName;
                                    itemsList[i].isOffer = 1;
                                    itemsList[i].startDate = itofflist.startDate;
                                    itemsList[i].endDate = itofflist.endDate;
                                    itemsList[i].itemUnitId = itofflist.itemUnitId;
                                    itemsList[i].offerId = itofflist.offerId;
                                    itemsList[i].isActiveOffer = itofflist.isActiveOffer;

                                    itemsList[i].price = itofflist.price;
                                    itemsList[i].priceTax =
                                        itemsList[i].price
                                        + (itemsList[i].price * itemsList[i].taxes / 100);

                                    itemsList[i].avgPurchasePrice = itemsList[i].avgPurchasePrice;
                                }
                            }
                            //itemsList[i].desPrice = itemsList[i].priceTax - totaldis;
                            // is new
                            int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                            if (res >= 0)
                            {
                                itemsList[i].isNew = 1;
                            }
                        }
                    }
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        // for service

        [HttpPost]
        [Route("GetAllSrItems")]
        public string GetAllSrItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                Boolean canDelete = false;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity.items

                        join c in entity.categories on I.categoryId equals c.categoryId into lj
                        from x in lj.DefaultIfEmpty()
                        where I.type == "sr"
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            categoryId = I.categoryId,
                            categoryName = x.name,
                            max = I.max,
                            maxUnitId = I.maxUnitId,
                            minUnitId = I.minUnitId,
                            min = I.min,
                            parentId = I.parentId,
                            isActive = I.isActive,
                            image = I.image,
                            type = I.type,
                            details = I.details,
                            taxes = I.taxes,
                            createDate = I.createDate,
                            updateDate = I.updateDate,
                            createUserId = I.createUserId,
                            updateUserId = I.updateUserId,
                            isNew = 0,
                            parentName = entity
                                .items
                                .Where(m => m.itemId == I.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(m => m.unitId == I.minUnitId)
                                .FirstOrDefault()
                                .name,
                            avgPurchasePrice = I.avgPurchasePrice,
                            notes = I.notes,
                            categoryString = I.categoryString,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).ToList();

                    var itemsofferslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            price = iu.price,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                        }
                    ).ToList();
                    itemsofferslist = itemsofferslist
                        .Where(
                            IO =>
                                (
                                    IO.isActiveOffer == 1
                                    && DateTime.Compare(((DateTime)IO.startDate).Date, datenow.Date)
                                        <= 0
                                    && System
                                        .DateTime
                                        .Compare(((DateTime)IO.endDate).Date, datenow.Date) >= 0
                                    && IO.defaultSale == 1
                                )
                                && (((DateTime)IO.startDate)).TimeOfDay <= datenow.TimeOfDay
                                && ((DateTime)IO.endDate).TimeOfDay >= datenow.TimeOfDay
                        )
                        .Distinct()
                        .ToList();

                    var unt = (
                        from unitm in entity.itemsUnits
                        join untb in entity.units on unitm.unitId equals untb.unitId
                        join itemtb in entity.items on unitm.itemId equals itemtb.itemId
                        where itemtb.type == "sr"
                        select new ItemSalePurModel()
                        {
                            itemId = itemtb.itemId,
                            name = itemtb.name,
                            code = itemtb.code,
                            max = itemtb.max,
                            maxUnitId = itemtb.maxUnitId,
                            minUnitId = itemtb.minUnitId,
                            min = itemtb.min,
                            parentId = itemtb.parentId,
                            isActive = itemtb.isActive,
                            isOffer = 0,
                            desPrice = 0,
                            offerName = "",
                            createDate = itemtb.createDate,
                            defaultSale = unitm.defaultSale,
                            unitName = untb.name,
                            unitId = untb.unitId,
                            price = unitm.price,
                        }
                    ).Where(a => a.defaultSale == 1).Distinct().ToList();

                    if (itemsList.Count > 0)
                    {
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            canDelete = false;
                            if (itemsList[i].isActive == 1)
                            {
                                long itemId = (long)itemsList[i].itemId;
                                var childItemL = entity
                                    .items
                                    .Where(x => x.parentId == itemId)
                                    .Select(b => new { b.itemId })
                                    .FirstOrDefault();
                                var itemUnitsL = entity
                                    .itemsUnits
                                    .Where(x => x.itemId == itemId)
                                    .Select(b => new { b.itemUnitId })
                                    .FirstOrDefault();
                                string itemType = itemsList[i].type;
                                long isInInvoice = 0;
                                if (itemUnitsL != null)
                                {
                                    isInInvoice = entity
                                        .itemsTransfer
                                        .Where(x => x.itemUnitId == itemUnitsL.itemUnitId)
                                        .Select(x => x.itemsTransId)
                                        .FirstOrDefault();
                                }

                                if (
                                    childItemL is null
                                    && (
                                        itemUnitsL is null || itemUnitsL != null && isInInvoice == 0
                                    )
                                )
                                    canDelete = true;
                            }
                            itemsList[i].canDelete = canDelete;

                            foreach (var itofflist in itemsofferslist)
                            {
                                if (itemsList[i].itemId == itofflist.itemId)
                                {
                                    // get unit name of item that has the offer
                                    using (incposdbEntities entitydb = new incposdbEntities())
                                    { // put it in item
                                        var un = entitydb
                                            .units
                                            .Where(a => a.unitId == itofflist.unitId)
                                            .Select(u => new { u.name, u.unitId })
                                            .FirstOrDefault();
                                        itemsList[i].unitName = un.name;
                                    }

                                    itemsList[i].offerName =
                                        itemsList[i].offerName + "- " + itofflist.offerName;
                                    itemsList[i].isOffer = 1;
                                    itemsList[i].startDate = itofflist.startDate;
                                    itemsList[i].endDate = itofflist.endDate;
                                    itemsList[i].itemUnitId = itofflist.itemUnitId;
                                    itemsList[i].offerId = itofflist.offerId;
                                    itemsList[i].isActiveOffer = itofflist.isActiveOffer;

                                    itemsList[i].price = itofflist.price;
                                    itemsList[i].priceTax =
                                        itemsList[i].price
                                        + (itemsList[i].price * itemsList[i].taxes / 100);

                                    itemsList[i].avgPurchasePrice = itemsList[i].avgPurchasePrice;
                                }
                            }
                            //itemsList[i].desPrice = itemsList[i].priceTax - totaldis;
                            // is new
                            int res = DateTime.Compare((DateTime)itemsList[i].createDate, cmpdate);
                            if (res >= 0)
                            {
                                itemsList[i].isNew = 1;
                            }
                        }
                    }
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetSrItemsInCategoryAndSub")]
        public string GetSrItemsInCategoryAndSub(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "categoryId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    // get all sub categories of categoryId
                    List<categories> categoriesList = entity
                        .categories
                        .ToList()
                        .Select(
                            p =>
                                new categories
                                {
                                    categoryId = p.categoryId,
                                    name = p.name,
                                    //parentId = p.parentId,
                                }
                        )
                        .ToList();

                    categoriesId = new List<long>();
                    categoriesId.Add(categoryId);

                    // get items
                    var result = Recursive(categoriesList, categoryId);
                    // end sub cat

                    var items = (
                        from itm in entity.items
                        join cat in entity.categories on itm.categoryId equals cat.categoryId
                        where itm.type == "sr"
                        select new ItemSalePurModel()
                        {
                            itemId = itm.itemId,
                            name = itm.name,
                            code = itm.code,
                            image = itm.image,
                            details = itm.details,
                            type = itm.type,
                            createUserId = itm.createUserId,
                            updateUserId = itm.updateUserId,
                            createDate = itm.createDate,
                            updateDate = itm.updateDate,
                            max = itm.max,
                            min = itm.min,
                            maxUnitId = itm.maxUnitId,
                            minUnitId = itm.minUnitId,
                            categoryId = itm.categoryId,
                            categoryName = cat.name,
                            //avgPurchasePrice

                            parentId = itm.parentId,
                            isActive = itm.isActive,
                            taxes = itm.taxes,
                            parentName = entity
                                .items
                                .Where(x => x.itemId == itm.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(x => x.unitId == itm.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(x => x.unitId == itm.minUnitId)
                                .FirstOrDefault()
                                .name,
                            isNew = 0,
                            notes = itm.notes,
                            categoryString = itm.categoryString,
                            isExpired = itm.isExpired,
                            alertDays = itm.alertDays,
                        }
                    ).Where(p => categoriesId.Contains((long)p.categoryId)).ToList();

                    //.Where(t => categoriesId.Contains((int)t.categoryId))
                    // end test

                    //  set is new
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    DateTime cmpdate = datenow.AddDays(newdays);
                    bool canDelete;
                    foreach (var item in items)
                    {
                        canDelete = false;
                        if (item.isActive == 1)
                        {
                            long itemId = (long)item.itemId;
                            var childItemL = entity
                                .items
                                .Where(x => x.parentId == itemId)
                                .Select(b => new { b.itemId })
                                .FirstOrDefault();
                            var itemUnitsL = entity
                                .itemsUnits
                                .Where(x => x.itemId == itemId)
                                .Select(b => new { b.itemUnitId })
                                .FirstOrDefault();
                            string itemType = item.type;
                            long isInInvoice = 0;
                            if (itemUnitsL != null)
                            {
                                isInInvoice = entity
                                    .itemsTransfer
                                    .Where(x => x.itemUnitId == itemUnitsL.itemUnitId)
                                    .Select(x => x.itemsTransId)
                                    .FirstOrDefault();
                            }

                            if (
                                childItemL is null
                                && (itemUnitsL is null || itemUnitsL != null && isInInvoice == 0)
                            )
                                canDelete = true;
                        }
                        item.canDelete = canDelete;
                        int res = DateTime.Compare((DateTime)item.createDate, cmpdate);
                        if (res >= 0)
                        {
                            item.isNew = 1;
                        }
                    }
                    return TokenManager.GenerateToken(items);
                }
            }
        }

        //



        [HttpPost]
        [Route("GetItemsWichHasUnits")]
        public string GetItemsWichHasUnits(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "";
                List<string> typeLst = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                        typeLst = JsonConvert.DeserializeObject<List<string>>(
                            type,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = (
                        from I in entity.items
                        join u in entity.itemsUnits on I.itemId equals u.itemId
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            type = I.type,
                            isActive = I.isActive,
                            avgPurchasePrice = I.avgPurchasePrice,
                        }
                    ).Where(x => x.isActive == 1).Distinct().ToList();
                    if (typeLst.Count == 0)
                        return TokenManager.GenerateToken(itemsList);
                    else
                    {
                        var itemsWithType = itemsList.Where(x => typeLst.Contains(x.type)).ToList();
                        return TokenManager.GenerateToken(itemsWithType);
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetKitchenItems")]
        public string GetKitchenItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                ItemsLocationsController ilc = new ItemsLocationsController();
                long branchId = 0;
                long categoryId = 0;
                List<string> typeLst = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "categoryId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<items>();
                    searchPredicate = searchPredicate.And(x => true);
                    if (categoryId != 0)
                        searchPredicate = searchPredicate.And(x => x.categoryId == categoryId);
                    var itemsList = (
                        from I in entity.items.Where(searchPredicate)
                        join u in entity.itemsUnits on I.itemId equals u.itemId
                        join l in entity
                            .itemsLocations
                            .Where(
                                x =>
                                    x.locations.branchId == branchId
                                    && x.locations.isKitchen == 1
                                    && x.quantity > 0
                            )
                            on u.itemUnitId equals l.itemUnitId
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            type = I.type,
                            isActive = I.isActive,
                            updateDate = I.updateDate,
                            categoryId = I.categoryId,
                            itemUnitId = I.itemsUnits
                                .Where(iu => iu.itemId == I.itemId && iu.defaultPurchase == 1)
                                .Select(iu => iu.itemUnitId)
                                .FirstOrDefault(),
                            unitName = entity
                                .units
                                .Where(
                                    u =>
                                        u.unitId
                                        == I.itemsUnits
                                            .Where(
                                                iu =>
                                                    iu.itemId == I.itemId && iu.defaultPurchase == 1
                                            )
                                            .Select(iu => iu.unitId)
                                            .FirstOrDefault()
                                )
                                .Select(u => u.name)
                                .FirstOrDefault(),
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).Where(x => x.isActive == 1).Distinct().ToList();

                    foreach (ItemModel item in itemsList)
                    {
                        var itemO = entity.items.Find(item.itemId);
                        if (item.itemUnitId != null && item.itemUnitId != 0)
                        {
                            long itemUnitId = (long)item.itemUnitId;
                            int count = ilc.getBranchAmount(itemUnitId, branchId, 1);
                            item.itemCount = count;
                        }
                        item.details = itemO.details;
                        item.image = itemO.image;
                    }
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetKitchenItemsWithUnits")]
        public string GetKitchenItemsWithUnits(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                ItemsLocationsController ilc = new ItemsLocationsController();
                long branchId = 0;
                long categoryId = 0;
                List<string> typeLst = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "categoryId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<items>();
                    searchPredicate.And(x => true);
                    if (categoryId != 0)
                        searchPredicate = searchPredicate.And(x => x.categoryId == categoryId);
                    var itemsList = (
                        from I in entity.items.Where(searchPredicate)
                        join u in entity.itemsUnits on I.itemId equals u.itemId
                        join l in entity
                            .itemsLocations
                            .Where(
                                x =>
                                    x.locations.branchId == branchId
                                    && x.locations.isKitchen == 1
                                    && x.quantity > 0
                            )
                            on u.itemUnitId equals l.itemUnitId
                        select new ItemModel()
                        {
                            itemId = I.itemId,
                            name = I.name,
                            code = I.code,
                            type = I.type,
                            isActive = I.isActive,
                            updateDate = I.updateDate,
                            unitName = u.units.name,
                            itemCount = (int)l.quantity,
                            endDate = l.endDate,
                            details = I.details,
                            isExpired = I.isExpired,
                            alertDays = I.alertDays,
                        }
                    ).Where(x => x.isActive == 1).ToList();
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetItemsInCategory")]
        public string GetItemsInCategory(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "categoryId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = entity
                        .items
                        .Where(I => I.categoryId == categoryId && I.isActive == 1)
                        .Select(
                            I =>
                                new
                                {
                                    I.itemId,
                                    I.name,
                                    I.code,
                                    I.max,
                                    I.maxUnitId,
                                    I.minUnitId,
                                    I.min,
                                    I.parentId,
                                    I.image,
                                    I.type,
                                    I.details,
                                    I.taxes,
                                    I.createDate,
                                    I.updateDate,
                                    I.createUserId,
                                    I.updateUserId,
                                    I.avgPurchasePrice,
                                    I.notes,
                                    I.categoryString,
                                    I.isExpired,
                                    I.alertDays,
                                }
                        )
                        .ToList();

                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        // get item in category and all of her sub gategories
        [HttpPost]
        [Route("GetItemsInCategoryAndSub")]
        public string GetItemsInCategoryAndSub(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "categoryId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    // get all sub categories of categoryId
                    List<categories> categoriesList = entity
                        .categories
                        .ToList()
                        .Select(
                            p =>
                                new categories
                                {
                                    categoryId = p.categoryId,
                                    name = p.name,
                                    //parentId = p.parentId,
                                }
                        )
                        .ToList();

                    categoriesId = new List<long>();
                    categoriesId.Add(categoryId);

                    // get items
                    var result = Recursive(categoriesList, categoryId);
                    // end sub cat

                    var items = (
                        from itm in entity.items
                        join cat in entity.categories on itm.categoryId equals cat.categoryId

                        select new ItemSalePurModel()
                        {
                            itemId = itm.itemId,
                            name = itm.name,
                            code = itm.code,
                            image = itm.image,
                            details = itm.details,
                            type = itm.type,
                            createUserId = itm.createUserId,
                            updateUserId = itm.updateUserId,
                            createDate = itm.createDate,
                            updateDate = itm.updateDate,
                            max = itm.max,
                            min = itm.min,
                            maxUnitId = itm.maxUnitId,
                            minUnitId = itm.minUnitId,
                            categoryId = itm.categoryId,
                            categoryName = cat.name,
                            //avgPurchasePrice

                            parentId = itm.parentId,
                            isActive = itm.isActive,
                            taxes = itm.taxes,
                            parentName = entity
                                .items
                                .Where(x => x.itemId == itm.parentId)
                                .FirstOrDefault()
                                .name,
                            minUnitName = entity
                                .units
                                .Where(x => x.unitId == itm.minUnitId)
                                .FirstOrDefault()
                                .name,
                            maxUnitName = entity
                                .units
                                .Where(x => x.unitId == itm.minUnitId)
                                .FirstOrDefault()
                                .name,
                            isNew = 0,
                            notes = itm.notes,
                            categoryString = itm.categoryString,
                            isExpired = itm.isExpired,
                            alertDays = itm.alertDays,
                        }
                    ).Where(p => categoriesId.Contains((long)p.categoryId)).ToList();

                    //.Where(t => categoriesId.Contains((int)t.categoryId))
                    // end test

                    //  set is new
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    DateTime cmpdate = datenow.AddDays(newdays);
                    bool canDelete;
                    foreach (var item in items)
                    {
                        canDelete = false;
                        if (item.isActive == 1)
                        {
                            long itemId = (long)item.itemId;
                            var childItemL = entity
                                .items
                                .Where(x => x.parentId == itemId)
                                .Select(b => new { b.itemId })
                                .FirstOrDefault();
                            var itemUnitsL = entity
                                .itemsUnits
                                .Where(x => x.itemId == itemId)
                                .Select(b => new { b.itemUnitId })
                                .FirstOrDefault();
                            string itemType = item.type;
                            long isInInvoice = 0;
                            if (itemUnitsL != null)
                            {
                                isInInvoice = entity
                                    .itemsTransfer
                                    .Where(x => x.itemUnitId == itemUnitsL.itemUnitId)
                                    .Select(x => x.itemsTransId)
                                    .FirstOrDefault();
                            }

                            if (
                                childItemL is null
                                && (itemUnitsL is null || itemUnitsL != null && isInInvoice == 0)
                            )
                                canDelete = true;
                        }
                        item.canDelete = canDelete;
                        int res = DateTime.Compare((DateTime)item.createDate, cmpdate);
                        if (res >= 0)
                        {
                            item.isNew = 1;
                        }
                    }
                    return TokenManager.GenerateToken(items);
                }
            }
        }

        // GET api/agent/5
        [HttpPost]
        [Route("GetItemsByType")]
        public string GetItemsByType(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value.ToString();
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var items = entity
                        .items
                        .Where(I => I.type == type)
                        .Select(
                            I =>
                                new
                                {
                                    I.itemId,
                                    I.name,
                                    I.code,
                                    I.min,
                                    I.type,
                                    I.details,
                                    I.isActive,
                                    I.avgPurchasePrice,
                                    I.notes,
                                    I.categoryString,
                                    I.isExpired,
                                    I.alertDays,
                                }
                        )
                        .ToList();
                    return TokenManager.GenerateToken(items);
                }
            }
        }

        [HttpPost]
        [Route("GetItemByID")]
        public string GetItemByID(string token)
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
                    var item = entity
                        .items
                        .Where(I => I.itemId == itemId)
                        .Select(
                            I =>
                                new
                                {
                                    I.itemId,
                                    I.name,
                                    I.code,
                                    I.categoryId,
                                    I.max,
                                    I.maxUnitId,
                                    I.minUnitId,
                                    I.min,
                                    I.parentId,
                                    I.image,
                                    I.type,
                                    I.details,
                                    I.taxes,
                                    I.createDate,
                                    I.updateDate,
                                    I.createUserId,
                                    I.updateUserId,
                                    I.avgPurchasePrice,
                                    I.notes,
                                    I.categoryString,
                                    I.isExpired,
                                    I.alertDays,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(item);
                }
            }
        }

        [HttpPost]
        [Route("GetItemsCodes")]
        public string GetItemsCodes(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsList = entity.items.Select(I => I.code).ToList();
                    return TokenManager.GenerateToken(itemsList);
                }
            }
        }

        [HttpPost]
        [Route("GetSaleOrPurItems")]
        public string GetSaleOrPurItems(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                short defaultSale = 0;
                short defaultPurchase = 0;
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                #region reding params
                foreach (Claim c in claims)
                {
                    if (c.Type == "categoryId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                    else if (c.Type == "defaultSale")
                    {
                        defaultSale = short.Parse(c.Value);
                    }
                    else if (c.Type == "defaultPurchase")
                    {
                        defaultPurchase = short.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = short.Parse(c.Value);
                    }
                }
                #endregion
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime cmpdate = datenow.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var searchPredicate = PredicateBuilder.New<items>();
                        var unitPredicate = PredicateBuilder.New<itemsUnits>();

                        if (categoryId != 0)
                        {
                            List<categories> categoriesList = entity
                                .categories
                                .ToList()
                                .Select(
                                    p =>
                                        new categories
                                        {
                                            categoryId = p.categoryId,
                                            name = p.name,
                                            //parentId = p.parentId,
                                        }
                                )
                                .ToList();

                            categoriesId = new List<long>();
                            categoriesId.Add(categoryId);

                            // get items
                            var result = Recursive(categoriesList, categoryId);
                            searchPredicate = searchPredicate.Or(
                                item =>
                                    categoriesId.Contains((long)item.categoryId)
                                    && item.isActive == 1
                            );
                        }
                        else
                            searchPredicate = searchPredicate.Or(item => item.isActive == 1);
                        #region items for movements
                        if (defaultSale == -1 && defaultPurchase == -1)
                        {
                            unitPredicate = unitPredicate.Or(unit => unit.defaultPurchase == 1);
                            var itemsList = (
                                from I in entity.items.Where(searchPredicate)
                                join u in entity.itemsUnits on I.itemId equals u.itemId
                                where I.type != "sr"
                                select new ItemModel()
                                {
                                    itemId = I.itemId,
                                    name = I.name,
                                    code = I.code,
                                    categoryId = I.categoryId,
                                    categoryName = I.categories.name,
                                    max = I.max,
                                    maxUnitId = I.maxUnitId,
                                    minUnitId = I.minUnitId,
                                    min = I.min,
                                    parentId = I.parentId,
                                    isActive = I.isActive,
                                    image = I.image,
                                    type = I.type,
                                    details = I.details,
                                    taxes = I.taxes,
                                    createDate = I.createDate,
                                    updateDate = I.updateDate,
                                    createUserId = I.createUserId,
                                    updateUserId = I.updateUserId,
                                    isNew = 0,
                                    //price = u.price,

                                    avgPurchasePrice = I.avgPurchasePrice,
                                    isExpired = I.isExpired,
                                    alertDays = I.alertDays,
                                }
                            ).DistinctBy(x => x.itemId).ToList();

                            foreach (ItemModel itemL in itemsList)
                            {
                                int res = DateTime.Compare((DateTime)itemL.createDate, cmpdate);
                                if (res >= 0)
                                {
                                    itemL.isNew = 1;
                                }
                            }
                            return TokenManager.GenerateToken(itemsList);
                        }
                        #endregion
                        #region items for order
                        else if (defaultSale == 0 && defaultPurchase == 0)
                        {
                            var itemsList = (
                                from I in entity.items.Where(searchPredicate)
                                join u in entity.itemsUnits on I.itemId equals u.itemId
                                select new ItemSalePurModel()
                                {
                                    itemId = I.itemId,
                                    name = I.name,
                                    code = I.code,
                                    categoryId = I.categoryId,
                                    categoryName = I.categories.name,
                                    max = I.max,
                                    maxUnitId = I.maxUnitId,
                                    minUnitId = I.minUnitId,
                                    min = I.min,
                                    parentId = I.parentId,
                                    image = I.image,
                                    type = I.type,
                                    details = I.details,
                                    taxes = I.taxes,
                                    createDate = I.createDate,
                                    updateDate = I.updateDate,
                                    createUserId = I.createUserId,
                                    updateUserId = I.updateUserId,
                                    isNew = 0,
                                    //price = u.price,

                                    isExpired = I.isExpired,
                                    alertDays = I.alertDays,
                                }
                            ).DistinctBy(x => x.itemId).ToList();
                            var itemsofferslist = (
                                from off in entity.offers

                                join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                                //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                                join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                                //from un in entity.units
                                select new ItemSalePurModel()
                                {
                                    itemId = iu.itemId,
                                    itemUnitId = itof.iuId,
                                    offerName = off.name,
                                    offerId = off.offerId,
                                    discountValue = off.discountValue,
                                    isNew = 0,
                                    isOffer = 1,
                                    isActiveOffer = off.isActive,
                                    startDate = off.startDate,
                                    endDate = off.endDate,
                                    unitId = iu.unitId,
                                    price = iu.price,
                                    discountType = off.discountType,
                                    desPrice = iu.price,
                                    defaultSale = iu.defaultSale,
                                }
                            ).ToList();
                            itemsofferslist = itemsofferslist
                                .Where(
                                    IO =>
                                        (
                                            IO.isActiveOffer == 1
                                            && DateTime.Compare(
                                                ((DateTime)IO.startDate).Date,
                                                datenow.Date
                                            ) <= 0
                                            && System
                                                .DateTime
                                                .Compare(((DateTime)IO.endDate).Date, datenow.Date)
                                                >= 0
                                            && IO.defaultSale == 1
                                        )
                                        && (((DateTime)IO.startDate)).TimeOfDay <= datenow.TimeOfDay
                                        && ((DateTime)IO.endDate).TimeOfDay >= datenow.TimeOfDay
                                )
                                .Distinct()
                                .ToList();

                            var unt = (
                                from unitm in entity.itemsUnits
                                join untb in entity.units on unitm.unitId equals untb.unitId
                                join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                                select new ItemSalePurModel()
                                {
                                    itemId = itemtb.itemId,
                                    name = itemtb.name,
                                    code = itemtb.code,
                                    max = itemtb.max,
                                    maxUnitId = itemtb.maxUnitId,
                                    minUnitId = itemtb.minUnitId,
                                    min = itemtb.min,
                                    parentId = itemtb.parentId,
                                    isActive = itemtb.isActive,
                                    isOffer = 0,
                                    desPrice = 0,
                                    offerName = "",
                                    createDate = itemtb.createDate,
                                    defaultSale = unitm.defaultSale,
                                    unitName = untb.name,
                                    unitId = untb.unitId,
                                    price = unitm.price,
                                }
                            ).Where(a => a.defaultSale == 1).Distinct().ToList();

                            // end test

                            foreach (var iunlist in itemsList)
                            {
                                if (iunlist.itemUnitId != null && iunlist.itemUnitId != 0)
                                {
                                    long itemUnitId = (long)iunlist.itemUnitId.Value;
                                    int count = getItemUnitAmount(itemUnitId, branchId);
                                    iunlist.itemCount = count;
                                }
                                foreach (var row in unt)
                                {
                                    if (row.itemId == iunlist.itemId)
                                    {
                                        iunlist.unitName = row.unitName;
                                        iunlist.unitId = row.unitId;
                                        iunlist.price = row.price;
                                        iunlist.priceTax =
                                            iunlist.price
                                            + Calc.percentValue(row.price, iunlist.taxes);
                                    }
                                }

                                // get set is new
                                // DateTime cmpdate = DateTime.Now.AddDays(newdays);

                                int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                                if (res >= 0)
                                {
                                    iunlist.isNew = 1;
                                }
                                // end is new
                                decimal? totaldis = 0;

                                foreach (var itofflist in itemsofferslist)
                                {
                                    if (iunlist.itemId == itofflist.itemId)
                                    {
                                        // get unit name of item that has the offer
                                        using (incposdbEntities entitydb = new incposdbEntities())
                                        { // put it in item
                                            var un = entitydb
                                                .units
                                                .Where(a => a.unitId == itofflist.unitId)
                                                .Select(u => new { u.name, u.unitId })
                                                .FirstOrDefault();
                                            iunlist.unitName = un.name;
                                        }

                                        iunlist.offerName =
                                            iunlist.offerName + "- " + itofflist.offerName;
                                        iunlist.isOffer = 1;
                                        iunlist.startDate = itofflist.startDate;
                                        iunlist.endDate = itofflist.endDate;
                                        iunlist.itemUnitId = itofflist.itemUnitId;
                                        iunlist.offerId = itofflist.offerId;
                                        iunlist.isActiveOffer = itofflist.isActiveOffer;

                                        iunlist.price = itofflist.price;
                                        iunlist.priceTax =
                                            iunlist.price + (iunlist.price * iunlist.taxes / 100);
                                        iunlist.discountType = itofflist.discountType;
                                        iunlist.discountValue = itofflist.discountValue;

                                        if (iunlist.discountType == "1") // value
                                        {
                                            totaldis = totaldis + iunlist.discountValue;
                                        }
                                        else if (iunlist.discountType == "2") // percent
                                        {
                                            totaldis =
                                                totaldis
                                                + Calc.percentValue(
                                                    iunlist.price,
                                                    iunlist.discountValue
                                                );
                                        }
                                    }
                                }
                                iunlist.desPrice = iunlist.priceTax - totaldis;
                            }
                            return TokenManager.GenerateToken(itemsList);
                        }
                        #endregion
                        #region items for sale
                        else if (defaultSale != 0)
                        {
                            unitPredicate = unitPredicate.Or(unit => unit.defaultSale == 1);

                            var itemsList = (
                                from I in entity.items.Where(searchPredicate)
                                join u in entity.itemsUnits on I.itemId equals u.itemId
                                join il in entity.itemsLocations
                                    on u.itemUnitId equals il.itemUnitId
                                join l in entity.locations on il.locationId equals l.locationId
                                join s in entity.sections.Where(x => x.branchId == branchId)
                                    on l.sectionId equals s.sectionId
                                where il.quantity > 0
                                select new ItemSalePurModel()
                                {
                                    itemId = I.itemId,
                                    name = I.name,
                                    code = I.code,
                                    categoryId = I.categoryId,
                                    tagId = I.tagId,
                                    categoryName = I.categories.name,
                                    max = I.max,
                                    maxUnitId = I.maxUnitId,
                                    minUnitId = I.minUnitId,
                                    min = I.min,
                                    parentId = I.parentId,
                                    isActive = I.isActive,
                                    image = I.image,
                                    type = I.type,
                                    details = I.details,
                                    taxes = I.taxes,
                                    createDate = I.createDate,
                                    updateDate = I.updateDate,
                                    createUserId = I.createUserId,
                                    updateUserId = I.updateUserId,
                                    isNew = 0,
                                    price = I.itemsUnits
                                        .Where(iu => iu.itemId == I.itemId && iu.defaultSale == 1)
                                        .Select(iu => iu.price)
                                        .FirstOrDefault(),
                                    itemUnitId = I.itemsUnits
                                        .Where(iu => iu.itemId == I.itemId && iu.defaultSale == 1)
                                        .Select(iu => iu.itemUnitId)
                                        .FirstOrDefault(),
                                    notes = I.notes,
                                    categoryString = I.categoryString,
                                    isExpired = I.isExpired,
                                    alertDays = I.alertDays,
                                }
                            ).DistinctBy(x => x.itemId).ToList();

                            var itemsofferslist = (
                                from off in entity.offers

                                join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                                //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                                join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                                //from un in entity.units
                                select new ItemSalePurModel()
                                {
                                    itemId = iu.itemId,
                                    itemUnitId = itof.iuId,
                                    offerName = off.name,
                                    offerId = off.offerId,
                                    discountValue = off.discountValue,
                                    isNew = 0,
                                    isOffer = 1,
                                    isActiveOffer = off.isActive,
                                    startDate = off.startDate,
                                    endDate = off.endDate,
                                    unitId = iu.unitId,
                                    itemCount = itof.quantity,
                                    price = iu.price,
                                    discountType = off.discountType,
                                    desPrice = iu.price,
                                    defaultSale = iu.defaultSale,
                                    used = itof.used,
                                }
                            ).ToList();
                            itemsofferslist = itemsofferslist
                                .Where(
                                    IO =>
                                        (
                                            IO.isActiveOffer == 1
                                            && DateTime.Compare(
                                                ((DateTime)IO.startDate).Date,
                                                datenow.Date
                                            ) <= 0
                                            && System
                                                .DateTime
                                                .Compare(((DateTime)IO.endDate).Date, datenow.Date)
                                                >= 0
                                            && IO.defaultSale == 1
                                            && IO.itemCount > IO.used
                                        )
                                        && (((DateTime)IO.startDate)).TimeOfDay <= datenow.TimeOfDay
                                        && ((DateTime)IO.endDate).TimeOfDay >= datenow.TimeOfDay
                                )
                                .Distinct()
                                .ToList();

                            var unt = (
                                from unitm in entity.itemsUnits
                                join untb in entity.units on unitm.unitId equals untb.unitId
                                join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                                select new ItemSalePurModel()
                                {
                                    itemId = itemtb.itemId,
                                    name = itemtb.name,
                                    code = itemtb.code,
                                    max = itemtb.max,
                                    maxUnitId = itemtb.maxUnitId,
                                    minUnitId = itemtb.minUnitId,
                                    min = itemtb.min,
                                    parentId = itemtb.parentId,
                                    isActive = itemtb.isActive,
                                    isOffer = 0,
                                    desPrice = 0,
                                    offerName = "",
                                    createDate = itemtb.createDate,
                                    defaultSale = unitm.defaultSale,
                                    unitName = untb.name,
                                    unitId = untb.unitId,
                                    price = unitm.price,
                                }
                            ).Where(a => a.defaultSale == 1).Distinct().ToList();

                            // end test

                            foreach (var iunlist in itemsList)
                            {
                                if (iunlist.itemUnitId != null && iunlist.itemUnitId != 0)
                                {
                                    long itemUnitId = (long)iunlist.itemUnitId.Value;
                                    int count = getItemUnitAmount(itemUnitId, branchId);
                                    iunlist.itemCount = count;
                                }
                                foreach (var row in unt)
                                {
                                    if (row.itemId == iunlist.itemId)
                                    {
                                        iunlist.unitName = row.unitName;
                                        iunlist.unitId = row.unitId;
                                        iunlist.price = row.price;
                                        iunlist.priceTax =
                                            iunlist.price
                                            + Calc.percentValue(row.price, iunlist.taxes);
                                    }
                                }

                                // get set is new
                                // DateTime cmpdate = DateTime.Now.AddDays(newdays);

                                int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                                if (res >= 0)
                                {
                                    iunlist.isNew = 1;
                                }
                                // end is new
                                decimal totaldis = 0;

                                foreach (var itofflist in itemsofferslist)
                                {
                                    if (iunlist.itemId == itofflist.itemId)
                                    {
                                        // get unit name of item that has the offer
                                        using (incposdbEntities entitydb = new incposdbEntities())
                                        { // put it in item
                                            var un = entitydb
                                                .units
                                                .Where(a => a.unitId == itofflist.unitId)
                                                .Select(u => new { u.name, u.unitId })
                                                .FirstOrDefault();
                                            iunlist.unitName = un.name;
                                        }

                                        iunlist.offerName =
                                            iunlist.offerName + "- " + itofflist.offerName;
                                        iunlist.isOffer = 1;
                                        iunlist.startDate = itofflist.startDate;
                                        iunlist.endDate = itofflist.endDate;
                                        iunlist.itemUnitId = itofflist.itemUnitId;
                                        iunlist.offerId = itofflist.offerId;
                                        iunlist.isActiveOffer = itofflist.isActiveOffer;

                                        iunlist.price = itofflist.price;
                                        iunlist.priceTax =
                                            iunlist.price + (iunlist.price * iunlist.taxes / 100);
                                        iunlist.discountType = itofflist.discountType;
                                        iunlist.discountValue = itofflist.discountValue;
                                        if (itofflist.used == null)
                                            itofflist.used = 0;

                                        if (
                                            iunlist.itemCount
                                            >= (itofflist.itemCount - itofflist.used)
                                        )
                                            iunlist.itemCount = (
                                                itofflist.itemCount - itofflist.used
                                            );

                                        if (iunlist.discountType == "1") // value
                                        {
                                            totaldis = totaldis + (decimal)iunlist.discountValue;
                                        }
                                        else if (iunlist.discountType == "2") // percent
                                        {
                                            totaldis =
                                                totaldis
                                                + Calc.percentValue(
                                                    iunlist.price,
                                                    iunlist.discountValue
                                                );
                                        }
                                    }
                                }
                                iunlist.priceTax = iunlist.priceTax - totaldis;
                            }
                            searchPredicate = searchPredicate.And(x => x.type == "sr");
                            var serviceItems = (
                                from I in entity.items.Where(searchPredicate)
                                join u in entity.itemsUnits on I.itemId equals u.itemId
                                select new ItemSalePurModel()
                                {
                                    itemId = I.itemId,
                                    name = I.name,
                                    code = I.code,
                                    categoryId = I.categoryId,
                                    categoryName = I.categories.name,
                                    max = I.max,
                                    maxUnitId = I.maxUnitId,
                                    minUnitId = I.minUnitId,
                                    min = I.min,
                                    parentId = I.parentId,
                                    isActive = I.isActive,
                                    image = I.image,
                                    type = I.type,
                                    details = I.details,
                                    taxes = I.taxes,
                                    createDate = I.createDate,
                                    updateDate = I.updateDate,
                                    createUserId = I.createUserId,
                                    updateUserId = I.updateUserId,
                                    isNew = 0,
                                    price = I.itemsUnits
                                        .Where(iu => iu.itemId == I.itemId && iu.defaultSale == 1)
                                        .Select(iu => iu.price)
                                        .FirstOrDefault(),
                                    itemUnitId = I.itemsUnits
                                        .Where(iu => iu.itemId == I.itemId && iu.defaultSale == 1)
                                        .Select(iu => iu.itemUnitId)
                                        .FirstOrDefault(),
                                    notes = I.notes,
                                    categoryString = I.categoryString,
                                    isExpired = I.isExpired,
                                    alertDays = I.alertDays,
                                }
                            ).DistinctBy(x => x.itemId).ToList();
                            foreach (var iunlist in serviceItems)
                            {
                                iunlist.itemCount = 0;
                                iunlist.priceTax =
                                    iunlist.price + Calc.percentValue(iunlist.price, iunlist.taxes);

                                // get set is new
                                // DateTime cmpdate = DateTime.Now.AddDays(newdays);

                                int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                                if (res >= 0)
                                {
                                    iunlist.isNew = 1;
                                }
                                // end is new
                                decimal totaldis = 0;

                                foreach (var itofflist in itemsofferslist)
                                {
                                    if (iunlist.itemId == itofflist.itemId)
                                    {
                                        // get unit name of item that has the offer
                                        using (incposdbEntities entitydb = new incposdbEntities())
                                        { // put it in item
                                            var un = entitydb
                                                .units
                                                .Where(a => a.unitId == itofflist.unitId)
                                                .Select(u => new { u.name, u.unitId })
                                                .FirstOrDefault();
                                            iunlist.unitName = un.name;
                                        }

                                        iunlist.offerName =
                                            iunlist.offerName + "- " + itofflist.offerName;
                                        iunlist.isOffer = 1;
                                        iunlist.startDate = itofflist.startDate;
                                        iunlist.endDate = itofflist.endDate;
                                        iunlist.itemUnitId = itofflist.itemUnitId;
                                        iunlist.offerId = itofflist.offerId;
                                        iunlist.isActiveOffer = itofflist.isActiveOffer;

                                        iunlist.price = itofflist.price;
                                        iunlist.priceTax =
                                            iunlist.price + (iunlist.price * iunlist.taxes / 100);
                                        iunlist.discountType = itofflist.discountType;
                                        iunlist.discountValue = itofflist.discountValue;
                                        if (itofflist.used == null)
                                            itofflist.used = 0;

                                        if (
                                            iunlist.itemCount
                                            >= (itofflist.itemCount - itofflist.used)
                                        )
                                            iunlist.itemCount = (
                                                itofflist.itemCount - itofflist.used
                                            );

                                        if (iunlist.discountType == "1") // value
                                        {
                                            totaldis = totaldis + (decimal)iunlist.discountValue;
                                        }
                                        else if (iunlist.discountType == "2") // percent
                                        {
                                            totaldis =
                                                totaldis
                                                + Calc.percentValue(
                                                    iunlist.price,
                                                    iunlist.discountValue
                                                );
                                        }
                                    }
                                }
                                iunlist.priceTax = iunlist.priceTax - totaldis;
                            }
                            itemsList.AddRange(serviceItems.ToArray());
                            return TokenManager.GenerateToken(itemsList);
                        }
                        #endregion
                        #region items for purchase
                        else if (defaultPurchase != 0)
                        {
                            unitPredicate = unitPredicate.Or(unit => unit.defaultPurchase == 1);
                            var itemsList = (
                                from I in entity.items.Where(searchPredicate)
                                join u in entity.itemsUnits on I.itemId equals u.itemId
                                where I.type != "p" && I.type != "sr"
                                select new ItemModel()
                                {
                                    itemId = I.itemId,
                                    name = I.name,
                                    code = I.code,
                                    categoryId = I.categoryId,
                                    categoryName = I.categories.name,
                                    max = I.max,
                                    maxUnitId = I.maxUnitId,
                                    minUnitId = I.minUnitId,
                                    min = I.min,
                                    parentId = I.parentId,
                                    isActive = I.isActive,
                                    image = I.image,
                                    type = I.type,
                                    details = I.details,
                                    taxes = I.taxes,
                                    createDate = I.createDate,
                                    updateDate = I.updateDate,
                                    createUserId = I.createUserId,
                                    updateUserId = I.updateUserId,
                                    isNew = 0,
                                    //price = u.price,

                                    avgPurchasePrice = I.avgPurchasePrice,
                                    isExpired = I.isExpired,
                                    alertDays = I.alertDays,
                                }
                            ).DistinctBy(x => x.itemId).ToList();

                            foreach (ItemModel itemL in itemsList)
                            {
                                int res = DateTime.Compare((DateTime)itemL.createDate, cmpdate);
                                if (res >= 0)
                                {
                                    itemL.isNew = 1;
                                }
                            }
                            return TokenManager.GenerateToken(itemsList);
                        }
                        #endregion
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
                return TokenManager.GenerateToken("0");
            }
        }

        private int getItemUnitAmount(long itemUnitId, long branchId, int isKitchen = 0)
        {
            int amount = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemInLocs = (
                    from b in entity.branches
                    where b.branchId == branchId
                    join s in entity.sections.Where(x => x.isKitchen == isKitchen)
                        on b.branchId equals s.branchId
                    join l in entity.locations on s.sectionId equals l.sectionId
                    join il in entity.itemsLocations on l.locationId equals il.locationId
                    where il.itemUnitId == itemUnitId && il.quantity > 0
                    select new
                    {
                        il.itemsLocId,
                        il.quantity,
                        il.itemUnitId,
                        il.locationId,
                        s.sectionId,
                    }
                ).ToList();
                for (int i = 0; i < itemInLocs.Count; i++)
                {
                    amount += (int)itemInLocs[i].quantity;
                }

                var unit = entity
                    .itemsUnits
                    .Where(x => x.itemUnitId == itemUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity
                    .itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId && x.itemId == unit.itemId && x.isActive == 1
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit != null && itemUnitId == upperUnit.itemUnitId)
                    return amount;
                if (upperUnit != null)
                    amount +=
                        (int)upperUnit.unitValue
                        * getItemUnitAmount(upperUnit.itemUnitId, branchId, isKitchen);

                return amount;
            }
        }

        [HttpPost]
        [Route("GetItemByBarcode")]
        public string GetItemByBarcode(string barcode)
        {
            long itemId = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                // itemId = (int)entity.barcodes
                //    .Where(I => I.barcode == barcode)
                //    .Select(I => I.itemId).SingleOrDefault();
            }
            string token = TokenManager.GenerateToken(itemId);
            //return GetItemByID(itemId);
            return GetItemByID(token);
        }

        public IEnumerable<categories> Recursive(List<categories> categoriesList, long toplevelid)
        {
            List<categories> inner = new List<categories>();

            foreach (var t in categoriesList)
            {
                categoriesId.Add(t.categoryId);
                inner.Add(t);
                inner = inner.Union(Recursive(categoriesList, t.categoryId)).ToList();
            }

            return inner;
        }

        // add or update item
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
                items itemObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        itemObj = JsonConvert.DeserializeObject<items>(
                            itemObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (itemObj != null)
                {
                    message = saveItem(itemObj);
                }
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("saveItemsCosting")]
        public string saveItemsCosting(string token)
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
                List<ItemModel> itemObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        itemObj = JsonConvert.DeserializeObject<List<ItemModel>>(
                            itemObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (itemObj != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (ItemModel item in itemObj)
                            {
                                var it = entity.items.Find(item.itemId);
                                it.avgPurchasePrice = item.avgPurchasePrice;

                                long itemUnitId = (long)item.itemUnitId;
                                var itemUnit = entity.itemsUnits.Find(itemUnitId);
                                itemUnit.price = item.price;
                                itemUnit.priceWithService = item.priceWithService;

                                entity.SaveChanges();
                            }
                            message = "1";
                        }
                    }
                    catch
                    {
                        message = "0";
                    }
                }
                return TokenManager.GenerateToken(message);
            }
        }

        private string saveItem(items itemObj)
        {
            string message = "";
            if (itemObj.updateUserId == 0 || itemObj.updateUserId == null)
            {
                Nullable<long> id = null;
                itemObj.updateUserId = id;
            }
            if (itemObj.createUserId == 0 || itemObj.createUserId == null)
            {
                Nullable<long> id = null;
                itemObj.createUserId = id;
            }
            if (itemObj.categoryId == 0 || itemObj.categoryId == null)
            {
                Nullable<long> id = null;
                itemObj.categoryId = id;
            }
            if (itemObj.minUnitId == 0 || itemObj.minUnitId == null)
            {
                Nullable<long> id = null;
                itemObj.minUnitId = id;
            }
            if (itemObj.maxUnitId == 0 || itemObj.maxUnitId == null)
            {
                Nullable<long> id = null;
                itemObj.maxUnitId = id;
            }
            try
            {
                items itemModel;
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var ItemEntity = entity.Set<items>();
                    if (itemObj.itemId == 0)
                    {
                        ProgramInfo programInfo = new ProgramInfo();
                        int itemMaxCount = programInfo.getItemCount();
                        int itemsCount = entity.items.Count();
                        if (itemsCount >= itemMaxCount)
                        {
                            message = "-1";
                        }
                        else
                        {
                            itemObj.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            itemObj.updateDate = itemObj.createDate;
                            itemObj.updateUserId = itemObj.createUserId;

                            itemModel = ItemEntity.Add(itemObj);
                            entity.SaveChanges();
                            message = itemObj.itemId.ToString();
                        }
                    }
                    else
                    {
                        itemModel = entity.items.Where(p => p.itemId == itemObj.itemId).First();
                        itemModel.code = itemObj.code;
                        itemModel.categoryId = itemObj.categoryId;
                        itemModel.parentId = itemObj.parentId;
                        itemModel.details = itemObj.details;
                        itemModel.image = itemObj.image;
                        itemModel.max = itemObj.max;
                        itemModel.maxUnitId = itemObj.maxUnitId;
                        itemModel.min = itemObj.min;
                        itemModel.minUnitId = itemObj.minUnitId;
                        itemModel.name = itemObj.name;
                        itemModel.tagId = itemObj.tagId;
                        itemModel.taxes = itemObj.taxes;
                        itemModel.type = itemObj.type;
                        itemModel.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        itemModel.updateUserId = itemObj.updateUserId;
                        itemModel.isActive = itemObj.isActive;
                        itemModel.avgPurchasePrice = itemObj.avgPurchasePrice;
                        itemModel.notes = itemObj.notes;
                        itemModel.categoryString = itemObj.categoryString;
                        itemModel.isExpired = itemObj.isExpired;
                        itemModel.alertDays = itemObj.alertDays;

                        entity.SaveChanges();
                        message = itemModel.itemId.ToString();
                    }
                }
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    foreach (var validationErrors in dbEx.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            message ="Property: {0} Error: {1}"+ validationError.PropertyName+ validationError.ErrorMessage;
            //        }
            //    }
            //}
            catch
            {
                message = "0";
            }
            return message;
        }

        [HttpPost]
        [Route("SaveSaleItem")]
        public string SaveSaleItem(string token)
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
                items itemObj = null;
                itemsUnits itemUnitObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        itemObj = JsonConvert.DeserializeObject<items>(
                            itemObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "itemUnit")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        itemUnitObj = JsonConvert.DeserializeObject<itemsUnits>(
                            itemObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                if (itemObj != null)
                {
                    message = saveItem(itemObj);
                    long itemId = long.Parse(message);
                    long itemUnitId = 0;
                    if (long.Parse(message) > 0)
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            itemUnitId = entity
                                .itemsUnits
                                .Where(x => x.itemId == itemId)
                                .Select(x => x.itemUnitId)
                                .FirstOrDefault();
                        }
                        ItemsUnitsController ic = new ItemsUnitsController();
                        itemUnitObj.itemId = long.Parse(message);
                        itemUnitObj.itemUnitId = itemUnitId;
                        ic.saveItemUnit(itemUnitObj);
                    }
                }
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("UpdateImage")]
        public string UpdateImage(string token)
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
                string imageName = "";
                long itemId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                    else if (c.Type == "imageName")
                    {
                        imageName = c.Value;
                    }
                }
                try
                {
                    items item;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemEntity = entity.Set<items>();
                        item = entity.items.Where(p => p.itemId == itemId).First();
                        item.image = imageName;
                        entity.SaveChanges();
                    }
                    message = item.itemId.ToString();
                    return TokenManager.GenerateToken(message);
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
                long itemId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
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
                            var tmpItem = entity.items.Where(I => I.itemId == itemId).First();

                            var iuitems = entity
                                .itemsUnits
                                .Where(x => x.itemId == tmpItem.itemId)
                                .ToList();
                            // remove from itemunit table
                            entity.itemsUnits.RemoveRange(iuitems);
                            entity.SaveChanges();

                            entity.items.Remove(tmpItem);
                            message = entity.SaveChanges().ToString();
                        }
                        return TokenManager.GenerateToken(message);
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var tmpItem = entity.items.Where(I => I.itemId == itemId).First();
                            tmpItem.isActive = 0;
                            tmpItem.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpItem.updateUserId = userId;

                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetSubItems")]
        public string GetSubItems(string token)
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
                    if (itemId != 0)
                    {
                        var itemsList = entity
                            .items
                            .Where(c => c.parentId == itemId && c.isActive == 1)
                            .Select(
                                I =>
                                    new
                                    {
                                        I.itemId,
                                        I.name,
                                        I.code,
                                        I.max,
                                        I.maxUnitId,
                                        I.minUnitId,
                                        I.min,
                                        I.parentId,
                                        I.image,
                                        I.type,
                                        I.details,
                                        I.isActive,
                                        I.taxes,
                                        I.createDate,
                                        I.updateDate,
                                        I.createUserId,
                                        I.updateUserId,
                                        I.notes,
                                        I.categoryString,
                                        I.isExpired,
                                        I.alertDays,
                                    }
                            )
                            .ToList();
                        return TokenManager.GenerateToken(itemsList);
                    }
                    else
                    {
                        var itemsList = entity
                            .items
                            .Where(c => c.parentId == 0 && c.isActive == 1)
                            .Select(
                                I =>
                                    new
                                    {
                                        I.itemId,
                                        I.name,
                                        I.code,
                                        I.max,
                                        I.maxUnitId,
                                        I.minUnitId,
                                        I.min,
                                        I.parentId,
                                        I.image,
                                        I.type,
                                        I.details,
                                        I.isActive,
                                        I.taxes,
                                        I.createDate,
                                        I.updateDate,
                                        I.createUserId,
                                        I.updateUserId,
                                        I.notes,
                                        I.categoryString,
                                        I.isExpired,
                                        I.alertDays,
                                    }
                            )
                            .ToList();
                        return TokenManager.GenerateToken(itemsList);
                    }
                }
            }
        }

        [Route("PostItemImage")]
        public IHttpActionResult PostItemImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    string imageName = postedFile.FileName;
                    string imageWithNoExt = Path.GetFileNameWithoutExtension(postedFile.FileName);

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB

                        IList<string> AllowedFileExtensions = new List<string>
                        {
                            ".jpg",
                            ".gif",
                            ".png",
                            ".bmp",
                            ".jpeg",
                            ".tiff",
                            ".jfif"
                        };
                        var ext = postedFile
                            .FileName
                            .Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();

                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format(
                                "Please Upload image of type .jpg,.gif,.png, .jfif, .bmp , .jpeg ,.tiff"
                            );
                            return Ok(message);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1 mb.");

                            return Ok(message);
                        }
                        else
                        {
                            //  check if image exist
                            var pathCheck = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\item"),
                                imageWithNoExt
                            );
                            var files = Directory.GetFiles(
                                System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\item"),
                                imageWithNoExt + ".*"
                            );
                            if (files.Length > 0)
                            {
                                File.Delete(files[0]);
                            }

                            //Userimage myfolder name where i want to save my image
                            var filePath = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\item"),
                                imageName
                            );
                            postedFile.SaveAs(filePath);
                        }
                    }
                    var message1 = string.Format("Image Updated Successfully.");
                    return Ok(message1);
                }
                var res = string.Format("Please Upload a image.");

                return Ok(res);
            }
            catch (Exception ex)
            {
                var res = string.Format("some Message");

                return Ok(res);
            }
        }

        [HttpPost]
        [Route("GetImage")]
        public string GetImage(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string imageName = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "imageName")
                    {
                        imageName = c.Value;
                    }
                }
                if (String.IsNullOrEmpty(imageName))
                    return TokenManager.GenerateToken("0");

                string localFilePath;

                try
                {
                    localFilePath = Path.Combine(
                        System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\item"),
                        imageName
                    );

                    byte[] b = System.IO.File.ReadAllBytes(localFilePath);
                    return TokenManager.GenerateToken(Convert.ToBase64String(b));
                }
                catch
                {
                    return TokenManager.GenerateToken(null);
                }
            }
        }

        // get all items where defaultSale is 1 and set isNew=1 if new item  and set isOffer=1 if Has Active Offer
        int newdays = -15;

        #region
        [HttpPost]
        [Route("GetAllSaleItems")]
        public IHttpActionResult GetAllSaleItems()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }

            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsunit = (
                        from itm in entity.items
                        join cat in entity.categories on itm.categoryId equals cat.categoryId
                        //  join itun in entity.itemsUnits on itm.itemId equals itun.itemId
                        //   join untb in entity.units on itun.unitId equals untb.unitId

                        select new ItemSalePurModel()
                        {
                            itemId = itm.itemId,
                            name = itm.name,
                            code = itm.code,
                            image = itm.image,
                            details = itm.details,
                            type = itm.type,
                            createUserId = itm.createUserId,
                            updateUserId = itm.updateUserId,
                            updateDate = itm.updateDate,
                            categoryId = itm.categoryId,
                            categoryName = cat.name,
                            max = itm.max,
                            maxUnitId = itm.maxUnitId,
                            minUnitId = itm.minUnitId,
                            min = itm.min,
                            parentId = itm.parentId,
                            isActive = itm.isActive,
                            taxes = itm.taxes,
                            isOffer = 0,
                            desPrice = 0,
                            isNew = 0,
                            offerName = "",
                            createDate = itm.createDate,
                            notes = itm.notes,
                            categoryString = itm.categoryString,
                            isExpired = itm.isExpired,
                            alertDays = itm.alertDays,
                        }
                    ).ToList();

                    var itemsofferslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            price = iu.price,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                        }
                    ).ToList();
                    itemsofferslist = itemsofferslist
                        .Where(
                            IO =>
                                (
                                    IO.isActiveOffer == 1
                                    && DateTime.Compare(((DateTime)IO.startDate).Date, datenow.Date)
                                        <= 0
                                    && System
                                        .DateTime
                                        .Compare(((DateTime)IO.endDate).Date, datenow.Date) >= 0
                                    && IO.defaultSale == 1
                                )
                                && (((DateTime)IO.startDate)).TimeOfDay <= datenow.TimeOfDay
                                && ((DateTime)IO.endDate).TimeOfDay >= datenow.TimeOfDay
                        )
                        .Distinct()
                        .ToList();

                    var unt = (
                        from unitm in entity.itemsUnits
                        join untb in entity.units on unitm.unitId equals untb.unitId
                        join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                        select new ItemSalePurModel()
                        {
                            itemId = itemtb.itemId,
                            name = itemtb.name,
                            code = itemtb.code,
                            max = itemtb.max,
                            maxUnitId = itemtb.maxUnitId,
                            minUnitId = itemtb.minUnitId,
                            min = itemtb.min,
                            parentId = itemtb.parentId,
                            isActive = itemtb.isActive,
                            isOffer = 0,
                            desPrice = 0,
                            offerName = "",
                            createDate = itemtb.createDate,
                            defaultSale = unitm.defaultSale,
                            unitName = untb.name,
                            unitId = untb.unitId,
                            price = unitm.price,
                        }
                    ).Where(a => a.defaultSale == 1).Distinct().ToList();

                    // end test

                    foreach (var iunlist in itemsunit)
                    {
                        foreach (var row in unt)
                        {
                            if (row.itemId == iunlist.itemId)
                            {
                                iunlist.unitName = row.unitName;
                                iunlist.unitId = row.unitId;
                                iunlist.price = row.price;
                                iunlist.priceTax =
                                    iunlist.price + Calc.percentValue(row.price, iunlist.taxes);
                            }
                        }

                        // get set is new

                        DateTime cmpdate = datenow.AddDays(newdays);

                        int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                        if (res >= 0)
                        {
                            iunlist.isNew = 1;
                        }
                        // end is new
                        decimal? totaldis = 0;

                        foreach (var itofflist in itemsofferslist)
                        {
                            if (iunlist.itemId == itofflist.itemId)
                            {
                                // get unit name of item that has the offer
                                using (incposdbEntities entitydb = new incposdbEntities())
                                { // put it in item
                                    var un = entitydb
                                        .units
                                        .Where(a => a.unitId == itofflist.unitId)
                                        .Select(u => new { u.name, u.unitId })
                                        .FirstOrDefault();
                                    iunlist.unitName = un.name;
                                }

                                iunlist.offerName = iunlist.offerName + "- " + itofflist.offerName;
                                iunlist.isOffer = 1;
                                iunlist.startDate = itofflist.startDate;
                                iunlist.endDate = itofflist.endDate;
                                iunlist.itemUnitId = itofflist.itemUnitId;
                                iunlist.offerId = itofflist.offerId;
                                iunlist.isActiveOffer = itofflist.isActiveOffer;

                                iunlist.price = itofflist.price;
                                iunlist.priceTax =
                                    iunlist.price + (iunlist.price * iunlist.taxes / 100);
                                iunlist.discountType = itofflist.discountType;
                                iunlist.discountValue = itofflist.discountValue;

                                if (iunlist.discountType == "1") // value
                                {
                                    totaldis = totaldis + iunlist.discountValue;
                                }
                                else if (iunlist.discountType == "2") // percent
                                {
                                    totaldis =
                                        totaldis
                                        + Calc.percentValue(iunlist.price, iunlist.discountValue);
                                }
                            }
                        }
                        iunlist.desPrice = iunlist.priceTax - totaldis;
                    }

                    //  if (itemsunit == null)
                    //if (itemsofferslist == null)
                    if (itemsunit == null)
                        return NotFound();
                    else
                        return Ok(itemsunit);
                }
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
        // get all items where defaultPurchase is 1 and set isNew=1 if new item
        #region
        [HttpPost]
        [Route("GetAllPurItems")]
        public IHttpActionResult GetAllPurItems()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }

            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemsunit = (
                        from itm in entity.items
                        join cat in entity.categories on itm.categoryId equals cat.categoryId
                        join itun in entity.itemsUnits on itm.itemId equals itun.itemId
                        join untb in entity.units on itun.unitId equals untb.unitId
                        select new ItemSalePurModel()
                        {
                            itemId = itm.itemId,
                            name = itm.name,
                            code = itm.code,
                            image = itm.image,
                            details = itm.details,
                            type = itm.type,
                            createUserId = itm.createUserId,
                            updateUserId = itm.updateUserId,
                            createDate = itm.createDate,
                            updateDate = itm.updateDate,
                            max = itm.max,
                            min = itm.min,
                            maxUnitId = itm.maxUnitId,
                            minUnitId = itm.minUnitId,
                            categoryId = itm.categoryId,
                            categoryName = cat.name,
                            parentId = itm.parentId,
                            isActive = itm.isActive,
                            taxes = itm.taxes,
                            isOffer = 0,
                            desPrice = 0,
                            isNew = 0,
                            offerName = "",
                            defaultPurchase = itun.defaultPurchase,
                            unitId = itun.unitId,
                            price = itun.price,
                            unitName = untb.name,
                            itemUnitId = itun.itemUnitId,
                            notes = itm.notes,
                            categoryString = itm.categoryString,
                            isExpired = itm.isExpired,
                            alertDays = itm.alertDays,
                        }
                    ).Where(p => p.defaultPurchase == 1).OrderBy(a => a.itemId).ToList();

                    // end test

                    //  set is new
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    DateTime cmpdate = datenow.AddDays(newdays);
                    foreach (var iunlist in itemsunit)
                    {
                        int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                        if (res >= 0)
                        {
                            iunlist.isNew = 1;
                        }
                    }

                    //  if (itemsunit == null)
                    //if (itemsofferslist == null)
                    if (itemsunit == null)
                        return NotFound();
                    else
                        return Ok(itemsunit);
                }
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        // get all items where defaultSale is 1 and set isNew=1 if new item  and set isOffer=1 if Has Active Offer
        //by category and its sub categories
        #region
        [HttpPost]
        [Route("GetSaleItemsByCategory")]
        public IHttpActionResult GetSaleItemsByCategory(long categoryId)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }

            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    // get all sub categories of categoryId
                    List<categories> categoriesList = entity
                        .categories
                        .Where(c => c.isActive == 1)
                        .ToList()
                        .Select(
                            p =>
                                new categories
                                {
                                    categoryId = p.categoryId,
                                    name = p.name,
                                    //parentId = p.parentId,
                                }
                        )
                        .ToList();

                    categoriesId = new List<long>();
                    categoriesId.Add(categoryId);

                    // get items
                    var result = Recursive(categoriesList, categoryId);
                    // end sub cat




                    var itemsunit = (
                        from itm in entity.items
                        join cat in entity.categories on itm.categoryId equals cat.categoryId
                        //  join itun in entity.itemsUnits on itm.itemId equals itun.itemId
                        //   join untb in entity.units on itun.unitId equals untb.unitId

                        select new ItemSalePurModel()
                        {
                            itemId = itm.itemId,
                            name = itm.name,
                            code = itm.code,
                            image = itm.image,
                            details = itm.details,
                            type = itm.type,
                            createUserId = itm.createUserId,
                            updateUserId = itm.updateUserId,
                            updateDate = itm.updateDate,
                            categoryId = itm.categoryId,
                            categoryName = cat.name,
                            max = itm.max,
                            maxUnitId = itm.maxUnitId,
                            minUnitId = itm.minUnitId,
                            min = itm.min,
                            parentId = itm.parentId,
                            isActive = itm.isActive,
                            taxes = itm.taxes,
                            isOffer = 0,
                            desPrice = 0,
                            isNew = 0,
                            offerName = "",
                            createDate = itm.createDate,
                            isExpired = itm.isExpired,
                            alertDays = itm.alertDays,
                        }
                    ).Where(t => categoriesId.Contains((long)t.categoryId)).ToList();

                    var itemsofferslist = (
                        from off in entity.offers

                        join itof in entity.itemsOffers on off.offerId equals itof.offerId // itemsOffers and offers

                        //  join iu in entity.itemsUnits on itof.iuId  equals  iu.itemUnitId //itemsUnits and itemsOffers
                        join iu in entity.itemsUnits on itof.iuId equals iu.itemUnitId
                        //from un in entity.units
                        select new ItemSalePurModel()
                        {
                            itemId = iu.itemId,
                            itemUnitId = itof.iuId,
                            offerName = off.name,
                            offerId = off.offerId,
                            discountValue = off.discountValue,
                            isNew = 0,
                            isOffer = 1,
                            isActiveOffer = off.isActive,
                            startDate = off.startDate,
                            endDate = off.endDate,
                            unitId = iu.unitId,
                            price = iu.price,
                            discountType = off.discountType,
                            desPrice = iu.price,
                            defaultSale = iu.defaultSale,
                        }
                    ).Where(IO => IO.isActiveOffer == 1 && DateTime.Compare((DateTime)IO.startDate, datenow) <= 0 && System.DateTime.Compare((DateTime)IO.endDate, datenow) >= 0 && IO.defaultSale == 1).Distinct().ToList();
                    //.Where(IO => IO.isActiveOffer == 1 && DateTime.Compare(IO.startDate,DateTime.Now)<0 && System.DateTime.Compare(IO.endDate, DateTime.Now) > 0).ToList();

                    // test

                    var unt = (
                        from unitm in entity.itemsUnits
                        join untb in entity.units on unitm.unitId equals untb.unitId
                        join itemtb in entity.items on unitm.itemId equals itemtb.itemId

                        select new ItemSalePurModel()
                        {
                            itemId = itemtb.itemId,
                            name = itemtb.name,
                            code = itemtb.code,
                            max = itemtb.max,
                            maxUnitId = itemtb.maxUnitId,
                            minUnitId = itemtb.minUnitId,
                            min = itemtb.min,
                            parentId = itemtb.parentId,
                            isActive = itemtb.isActive,
                            isOffer = 0,
                            desPrice = 0,
                            offerName = "",
                            createDate = itemtb.createDate,
                            defaultSale = unitm.defaultSale,
                            unitName = untb.name,
                            unitId = untb.unitId,
                            price = unitm.price,
                        }
                    ).Where(a => a.defaultSale == 1).Distinct().ToList();

                    // end test

                    foreach (var iunlist in itemsunit)
                    {
                        foreach (var row in unt)
                        {
                            if (row.itemId == iunlist.itemId)
                            {
                                iunlist.unitName = row.unitName;
                                iunlist.unitId = row.unitId;
                                iunlist.price = row.price;
                                iunlist.priceTax =
                                    iunlist.price + Calc.percentValue(row.price, iunlist.taxes);
                            }
                        }

                        // get set is new

                        DateTime cmpdate = datenow.AddDays(newdays);

                        int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                        if (res >= 0)
                        {
                            iunlist.isNew = 1;
                        }
                        // end is new
                        decimal? totaldis = 0;

                        foreach (var itofflist in itemsofferslist)
                        {
                            if (iunlist.itemId == itofflist.itemId)
                            {
                                // get unit name of item that has the offer
                                using (incposdbEntities entitydb = new incposdbEntities())
                                { // put it in item
                                    var un = entitydb
                                        .units
                                        .Where(a => a.unitId == itofflist.unitId)
                                        .Select(u => new { u.name, u.unitId })
                                        .FirstOrDefault();
                                    iunlist.unitName = un.name;
                                }

                                iunlist.offerName = iunlist.offerName + "- " + itofflist.offerName;
                                iunlist.isOffer = 1;
                                iunlist.startDate = itofflist.startDate;
                                iunlist.endDate = itofflist.endDate;
                                iunlist.itemUnitId = itofflist.itemUnitId;
                                iunlist.offerId = itofflist.offerId;
                                iunlist.isActiveOffer = itofflist.isActiveOffer;

                                iunlist.price = itofflist.price;
                                iunlist.priceTax =
                                    iunlist.price + (iunlist.price * iunlist.taxes / 100);
                                iunlist.discountType = itofflist.discountType;
                                iunlist.discountValue = itofflist.discountValue;

                                if (iunlist.discountType == "1") // value
                                {
                                    totaldis = totaldis + iunlist.discountValue;
                                }
                                else if (iunlist.discountType == "2") // percent
                                {
                                    totaldis =
                                        totaldis
                                        + Calc.percentValue(iunlist.price, iunlist.discountValue);
                                }
                            }
                        }
                        iunlist.desPrice = iunlist.priceTax - totaldis;
                    }

                    //  if (itemsunit == null)
                    //if (itemsofferslist == null)
                    if (itemsunit == null)
                        return NotFound();
                    else
                        return Ok(itemsunit);
                }
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        // get all items where defaultPurchase is 1 and set isNew=1 if new item
        //by category and its sub categories
        #region
        [HttpPost]
        [Route("GetPurItemsByCategory")]
        public IHttpActionResult GetPurItemsByCategory(long categoryId)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }

            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    // get all sub categories of categoryId
                    List<categories> categoriesList = entity
                        .categories
                        .Where(c => c.isActive == 1)
                        .ToList()
                        .Select(
                            p =>
                                new categories
                                {
                                    categoryId = p.categoryId,
                                    name = p.name,
                                    //parentId = p.parentId,
                                }
                        )
                        .ToList();

                    categoriesId = new List<long>();
                    categoriesId.Add(categoryId);

                    // get items
                    var result = Recursive(categoriesList, categoryId);
                    // end sub cat

                    var itemsunit = (
                        from itm in entity.items
                        join cat in entity.categories on itm.categoryId equals cat.categoryId
                        join itun in entity.itemsUnits on itm.itemId equals itun.itemId
                        join untb in entity.units on itun.unitId equals untb.unitId
                        select new ItemSalePurModel()
                        {
                            itemId = itm.itemId,
                            name = itm.name,
                            code = itm.code,
                            image = itm.image,
                            details = itm.details,
                            type = itm.type,
                            createUserId = itm.createUserId,
                            updateUserId = itm.updateUserId,
                            createDate = itm.createDate,
                            updateDate = itm.updateDate,
                            max = itm.max,
                            min = itm.min,
                            maxUnitId = itm.maxUnitId,
                            minUnitId = itm.minUnitId,
                            categoryId = itm.categoryId,
                            categoryName = cat.name,
                            parentId = itm.parentId,
                            isActive = itm.isActive,
                            taxes = itm.taxes,
                            isOffer = 0,
                            desPrice = 0,
                            isNew = 0,
                            offerName = "",
                            defaultPurchase = itun.defaultPurchase,
                            unitId = itun.unitId,
                            price = itun.price,
                            unitName = untb.name,
                            itemUnitId = itun.itemUnitId,
                            notes = itm.notes,
                            categoryString = itm.categoryString,
                            isExpired = itm.isExpired,
                            alertDays = itm.alertDays,
                        }
                    ).Where(p => p.defaultPurchase == 1 && categoriesId.Contains((long)p.categoryId)).ToList();

                    //.Where(t => categoriesId.Contains((int)t.categoryId))
                    // end test

                    //  set is new
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    DateTime cmpdate = datenow.AddDays(newdays);
                    foreach (var iunlist in itemsunit)
                    {
                        int res = DateTime.Compare((DateTime)iunlist.createDate, cmpdate);
                        if (res >= 0)
                        {
                            iunlist.isNew = 1;
                        }
                    }

                    //  if (itemsunit == null)
                    //if (itemsofferslist == null)
                    if (itemsunit == null)
                        return NotFound();
                    else
                        return Ok(itemsunit);
                }
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
