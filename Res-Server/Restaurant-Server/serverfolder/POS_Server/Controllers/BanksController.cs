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
    [RoutePrefix("api/Banks")]
    public class BanksController : ApiController
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
                    var banksList = entity.banks
                        .Select(
                            b =>
                                new BankModel
                                {
                                    accNumber = b.accNumber,
                                    address = b.address,
                                    bankId = b.bankId,
                                    mobile = b.mobile,
                                    name = b.name,
                                    notes = b.notes,
                                    phone = b.phone,
                                    createDate = b.createDate,
                                    updateDate = b.updateDate,
                                    createUserId = b.createUserId,
                                    updateUserId = b.updateUserId,
                                    isActive = b.isActive,
                                }
                        )
                        .ToList();

                    if (banksList.Count > 0)
                    {
                        for (int i = 0; i < banksList.Count; i++)
                        {
                            canDelete = false;
                            if (banksList[i].isActive == 1)
                            {
                                long bankId = (long)banksList[i].bankId;
                                var operationsL = entity.cashTransfer
                                    .Where(x => x.bankId == bankId)
                                    .Select(b => new { b.cashTransId })
                                    .FirstOrDefault();

                                if (operationsL is null)
                                    canDelete = true;
                            }
                            banksList[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(banksList);
                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetBankByID")]
        public string GetBankByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long bankId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        bankId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.banks
                        .Where(b => b.bankId == bankId)
                        .Select(
                            b =>
                                new
                                {
                                    b.accNumber,
                                    b.address,
                                    b.bankId,
                                    b.mobile,
                                    b.name,
                                    b.notes,
                                    b.phone,
                                    b.createDate,
                                    b.updateDate,
                                    b.createUserId,
                                    b.updateUserId
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(bank);
                }
            }
        }

        // add or update bank
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
                string bankId = "";
                banks newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        bankId = c.Value.Replace("\\", string.Empty);
                        bankId = bankId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<banks>(
                            bankId,
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
                        banks tmpBank = new banks();
                        var bankEntity = entity.Set<banks>();
                        if (newObject.bankId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpBank = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpBank.bankId.ToString();
                            ;
                        }
                        else
                        {
                            tmpBank = entity.banks
                                .Where(p => p.bankId == newObject.bankId)
                                .FirstOrDefault();
                            tmpBank.name = newObject.name;
                            tmpBank.accNumber = newObject.accNumber;
                            tmpBank.address = newObject.address;
                            tmpBank.mobile = newObject.mobile;
                            tmpBank.notes = newObject.notes;
                            tmpBank.phone = newObject.phone;
                            tmpBank.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpBank.updateUserId = newObject.updateUserId;
                            tmpBank.isActive = newObject.isActive;
                            entity.SaveChanges();
                            message = tmpBank.bankId.ToString();
                        }
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
                long bankId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        bankId = long.Parse(c.Value);
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
                            banks bankDelete = entity.banks.Find(bankId);
                            entity.banks.Remove(bankDelete);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
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
                            banks bankObj = entity.banks.Find(bankId);
                            bankObj.isActive = 0;
                            bankObj.updateUserId = userId;
                            bankObj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
    }
}
