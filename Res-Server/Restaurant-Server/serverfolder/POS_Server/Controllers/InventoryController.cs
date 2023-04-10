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
using System.Threading.Tasks;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Inventory")]
    public class InventoryController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all Inventory
        //[HttpPost]
        //[Route("Get")]
        //public string Get(string token string type)
        //{
        //    bool canDelete = false;
        //    if (TokenManager.GetPrincipal(token) == null)//invalid authorization
        //    {
        //        return TokenManager.GenerateToken("-7");
        //    }
        //    else
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var List = entity.Inventory
        //          .Where(c => c.inventoryType == type)
        //           .Select(c => new InventoryModel {
        //               inventoryId = c.inventoryId,
        //               num = c.num,
        //               notes = c.notes,
        //               createDate = c.createDate,
        //               updateDate = c.updateDate,
        //               createUserId = c.createUserId,
        //               updateUserId = c.updateUserId,
        //               isActive = c.isActive,
        //               inventoryType = c.inventoryType,

        //           })
        //           .ToList();
        //            if (List.Count > 0)
        //            {
        //                for (int i = 0; i < List.Count; i++)
        //                {
        //                    canDelete = false;
        //                    if (List[i].isActive == 1)
        //                    {
        //                        long inventoryId = (int)List[i].inventoryId;
        //                        var operationsL = entity.inventoryItemLocation.Where(x => x.inventoryId == inventoryId).Select(b => new { b.id }).FirstOrDefault();

        //                        if (operationsL is null)
        //                            canDelete = true;
        //                    }
        //                    List[i].canDelete = canDelete;
        //                }
        //            }

        //            return TokenManager.GenerateToken(List);
        //        }
        //    }
        //}
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
                long cId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        cId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var list = entity.Inventory
                        .Where(c => c.inventoryId == cId)
                        .Select(
                            c =>
                                new
                                {
                                    c.inventoryId,
                                    c.num,
                                    c.notes,
                                    c.createDate,
                                    c.updateDate,
                                    c.createUserId,
                                    c.updateUserId,
                                    c.isActive,
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(list);
                }
            }
        }

        [HttpPost]
        [Route("GetLastNumOfInv")]
        public string GetLastNumOfInv(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invCode = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invCode")
                    {
                        invCode = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                List<string> numberList;
                int lastNum = 0;
                using (incposdbEntities entity = new incposdbEntities())
                {
                    numberList = entity.Inventory
                        .Where(b => b.num.Contains(invCode + "-") && b.branchId == branchId)
                        .Select(b => b.num)
                        .ToList();

                    for (int i = 0; i < numberList.Count; i++)
                    {
                        string code = numberList[i];
                        string s = code.Substring(code.LastIndexOf("-") + 1);
                        numberList[i] = s;
                    }
                    if (numberList.Count > 0)
                    {
                        numberList.Sort();
                        lastNum = int.Parse(numberList[numberList.Count - 1]);
                    }
                }
                return TokenManager.GenerateToken(lastNum);
            }
        }

        [HttpPost]
        [Route("GetByCreator")]
        public string GetByCreator(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string inventoryType = "";
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "inventoryType")
                    {
                        inventoryType = c.Value;
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = entity.Inventory
                        .Where(
                            c =>
                                c.inventoryType.Contains(inventoryType)
                                && c.createUserId == userId
                                && c.isActive == 1
                        )
                        .Select(
                            c =>
                                new InventoryModel
                                {
                                    inventoryId = c.inventoryId,
                                    num = c.num,
                                    notes = c.notes,
                                    createDate = c.createDate,
                                    updateDate = c.updateDate,
                                    createUserId = c.createUserId,
                                    updateUserId = c.updateUserId,
                                    isActive = c.isActive,
                                    inventoryType = c.inventoryType,
                                }
                        )
                        .ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("getByBranch")]
        public string getByBranch(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string inventoryType = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "inventoryType")
                    {
                        inventoryType = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = entity.Inventory
                        .Where(
                            c =>
                                c.inventoryType.Contains(inventoryType)
                                && c.branchId == branchId
                                && c.isActive == 1
                        )
                        .Select(
                            c =>
                                new InventoryModel
                                {
                                    inventoryId = c.inventoryId,
                                    num = c.num,
                                    notes = c.notes,
                                    createDate = c.createDate,
                                    updateDate = c.updateDate,
                                    createUserId = c.createUserId,
                                    updateUserId = c.updateUserId,
                                    isActive = c.isActive,
                                    inventoryType = c.inventoryType,
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        // GET api/<controller>  Get medal By ID

        [HttpPost]
        [Route("shortageIsManipulated")]
        public string shortageIsManipulated(string token)
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
                        from c in entity.inventoryItemLocation.Where(
                            c =>
                                c.realAmount - c.amount > 0
                                && c.inventoryId == inventoryId
                                && c.isFalls == false
                        )
                        select new InventoryItemLocationModel() { id = c.id, }
                    ).ToList();
                    bool result = false;
                    if (List.Count == 0)
                        result = true;
                    else
                        result = false;
                    return TokenManager.GenerateToken(result);
                }
            }
        }

        // add or update
        [HttpPost]
        [Route("Save")]
        public async Task<string> Save(string token)
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
                string newObject = "";
                Inventory Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        newObject = c.Value.Replace("\\", string.Empty);
                        newObject = newObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<Inventory>(
                            newObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    if (Object.updateUserId == 0 || Object.updateUserId == null)
                    {
                        Nullable<long> id = null;
                        Object.updateUserId = id;
                    }
                    if (Object.createUserId == 0 || Object.createUserId == null)
                    {
                        Nullable<long> id = null;
                        Object.createUserId = id;
                    }

                    #region generate InvNumber
                    if (Object.num != null)
                    {
                        int branchId = (int)Object.branchId;
                        string invNumber = await generateInvNumber(Object.num, branchId);
                        Object.num = invNumber;
                    }
                    #endregion

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var sEntity = entity.Set<Inventory>();
                        if (Object.inventoryId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            Object.isActive = 1;

                            entity.Inventory.Add(Object);
                            entity.SaveChanges();
                            message = Object.inventoryId.ToString();
                        }
                        else
                        {
                            var tmps = entity.Inventory
                                .Where(p => p.inventoryId == Object.inventoryId)
                                .FirstOrDefault();
                            tmps.inventoryId = Object.inventoryId;
                            tmps.num = Object.num;
                            tmps.notes = Object.notes;
                            tmps.inventoryType = Object.inventoryType;
                            tmps.isActive = Object.isActive;
                            tmps.createDate = Object.createDate;
                            tmps.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date
                            tmps.updateUserId = Object.updateUserId;
                            entity.SaveChanges();
                            message = tmps.inventoryId.ToString();
                        }
                    }

                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        public async Task<string> generateInvNumber(string invoiceCode, long branchId)
        {
            #region check if last of code is num
            var num = invoiceCode.Substring(invoiceCode.LastIndexOf("-") + 1);

            if (!num.Equals(invoiceCode))
                return invoiceCode;

            #endregion
            int sequence = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var numberList = entity.Inventory
                    .Where(b => b.num.Contains(invoiceCode + "-") && b.branchId == branchId)
                    .Select(b => b.num)
                    .ToList();
                for (int i = 0; i < numberList.Count; i++)
                {
                    string code = numberList[i];
                    string s = code.Substring(code.LastIndexOf("-") + 1);

                    numberList[i] = s;
                }
                if (numberList.Count > 0)
                {
                    numberList.Sort();
                    try
                    {
                        sequence = int.Parse(numberList[numberList.Count - 1]);
                    }
                    catch
                    {
                        sequence = 0;
                    }
                }
            }
            sequence++;

            string strSeq = sequence.ToString();
            if (sequence <= 999999)
                strSeq = sequence.ToString().PadLeft(6, '0');
            string invoiceNum = invoiceCode + "-" + strSeq;
            return invoiceNum;
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
                long inventoryId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        inventoryId = long.Parse(c.Value);
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
                            Inventory Deleterow = entity.Inventory.Find(inventoryId);
                            entity.Inventory.Remove(Deleterow);
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
                            Inventory Obj = entity.Inventory.Find(inventoryId);
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

        [HttpPost]
        [Route("getNotifications")]
        public async Task<string> getNotifications(string token)
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

                long branchId = 0;
                string result = "{";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    string str = "";

                    #region check if there is draft inventory

                    var List = entity.Inventory
                        .Where(
                            c => c.inventoryType == "d" && c.branchId == branchId && c.isActive == 1
                        )
                        .FirstOrDefault();

                    if (List is null)
                        str = "no";
                    else
                        str = "yes";
                    result += "isThereInventoryDraft:'" + str + "'";
                    #endregion

                    #region check if there is draft inventory

                    var List2 = entity.Inventory
                        .Where(
                            c => c.inventoryType == "n" && c.branchId == branchId && c.isActive == 1
                        )
                        .FirstOrDefault();
                    if (List2 is null)
                        str = "no";
                    else
                        str = "yes";
                    result += ",isThereSavedInventory:'" + str + "'";

                    #endregion


                    result += "}";
                }
                return TokenManager.GenerateToken(result);
            }
        }
    }
}
