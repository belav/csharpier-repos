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
    [RoutePrefix("api/ShippingCompanies")]
    public class ShippingCompaniesController : ApiController
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
                    var List = (from S in entity.shippingCompanies
                                select new ShippingCompaniesModel()
                                {
                                    shippingCompanyId = S.shippingCompanyId,
                                    name = S.name,
                                    realDeliveryCost = S.realDeliveryCost,
                                    deliveryCost = S.deliveryCost,
                                    deliveryType = S.deliveryType,
                                    notes = S.notes,
                                    isActive = S.isActive,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    balance = S.balance,
                                    balanceType = S.balanceType,

                                    email = S.email,
                                    phone = S.phone,
                                    mobile = S.mobile,
                                    fax = S.fax,
                                    address = S.address,


                                }).ToList();
                    if (List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            if (List[i].isActive == 1)
                            {
                                long shippingCompanyId = (long)List[i].shippingCompanyId;
                                var itemsI = entity.invoices.Where(x => x.shippingCompanyId == shippingCompanyId).Select(b => new { b.invoiceId }).FirstOrDefault();

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
        [Route("GetForAccount")]
        public string GetForAccount(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);

            string payType = "";

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "payType")
                    {
                        payType = c.Value;
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var usersList = entity.shippingCompanies.Where(s => s.isActive == 1 ||
                                                 (s.isActive == 0 && payType == "p" && s.balanceType == 0) ||
                                                 (s.isActive == 0 && payType == "d" && s.balanceType == 1))
                    .Select(S => new ShippingCompaniesModel
                    {
                        shippingCompanyId = S.shippingCompanyId,
                        name = S.name,
                        realDeliveryCost = S.realDeliveryCost,
                        deliveryCost = S.deliveryCost,
                        deliveryType = S.deliveryType,
                        notes = S.notes,
                        isActive = S.isActive,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,
                        balance = S.balance,
                        balanceType = S.balanceType,

                        email = S.email,
                        phone = S.phone,
                        mobile = S.mobile,
                        fax = S.fax,
                        address = S.address
                    })
                    .ToList();

                    return TokenManager.GenerateToken(usersList);
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
                long shippingCompanyId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var row = entity.shippingCompanies
                   .Where(u => u.shippingCompanyId == shippingCompanyId)
                   .Select(S => new
                   {
                       S.shippingCompanyId,
                       S.name,
                       S.realDeliveryCost,
                       S.deliveryCost,
                       S.deliveryType,
                       S.notes,
                       S.createDate,
                       S.updateDate,
                       S.createUserId,
                       S.updateUserId,
                       S.isActive,
                       S.balance,
                       S.balanceType,
                       S.email,
                       S.phone,
                       S.mobile,
                       S.fax,
                       S.address,

                   })
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
                string shippingCompaniesObject = "";
                shippingCompanies newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        shippingCompaniesObject = c.Value.Replace("\\", string.Empty);
                        shippingCompaniesObject = shippingCompaniesObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<shippingCompanies>(shippingCompaniesObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
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
                        var locationEntity = entity.Set<shippingCompanies>();
                        if (newObject.shippingCompanyId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;


                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.shippingCompanyId.ToString();
                        }
                        else
                        {
                            var tmpObject = entity.shippingCompanies.Where(p => p.shippingCompanyId == newObject.shippingCompanyId).FirstOrDefault();

                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.updateUserId = newObject.updateUserId;

                            tmpObject.name = newObject.name;
                            tmpObject.realDeliveryCost = newObject.realDeliveryCost;
                            tmpObject.deliveryCost = newObject.deliveryCost;
                            tmpObject.deliveryType = newObject.deliveryType;
                            tmpObject.notes = newObject.notes;
                            tmpObject.isActive = newObject.isActive;
                            tmpObject.balance = newObject.balance;
                            tmpObject.balanceType = newObject.balanceType;
                            tmpObject.email = newObject.email;
                            tmpObject.phone = newObject.phone;
                            tmpObject.mobile = newObject.mobile;
                            tmpObject.fax = newObject.fax;
                            tmpObject.address = newObject.address;

                            entity.SaveChanges();

                            message = tmpObject.shippingCompanyId.ToString();
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
        [Route("editShippingComBalance")]
        public string editShippingComBalance(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "1";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long shippingComId = 0;
                decimal amount = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        shippingComId = long.Parse(c.Value);
                    }
                    else if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var shCom = entity.shippingCompanies.Find(shippingComId);

                        if (shCom.balanceType == 0)
                        {
                            if (amount > shCom.balance)
                            {
                                amount -= shCom.balance;
                                shCom.balance = amount;
                                shCom.balanceType = 1;
                            }
                            else
                                shCom.balance -= amount;
                        }
                        else
                        {
                            shCom.balance += amount;
                        }

                        entity.SaveChanges();
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
                long shippingCompanyId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
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
                            shippingCompanies objectDelete = entity.shippingCompanies.Find(shippingCompanyId);

                            entity.shippingCompanies.Remove(objectDelete);
                            message = entity.SaveChanges().ToString();

                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        message =  "-1";
                        return TokenManager.GenerateToken(message);

                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            shippingCompanies objectDelete = entity.shippingCompanies.Find(shippingCompanyId);

                            objectDelete.isActive = 0;
                            objectDelete.updateUserId = userId;
                            objectDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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