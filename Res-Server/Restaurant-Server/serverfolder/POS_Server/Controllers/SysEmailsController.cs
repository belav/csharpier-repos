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
    [RoutePrefix("api/SysEmails")]
    public class SysEmailsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                //long mainBranchId = 0;
                //long userId = 0;

                //IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                //foreach (Claim c in claims)
                //{
                //    if (c.Type == "mainBranchId")
                //    {
                //        mainBranchId = long.Parse(c.Value);
                //    }
                //    else if (c.Type == "userId")
                //    {
                //        userId = long.Parse(c.Value);
                //    }

                //}

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = (
                            from S in entity.sysEmails
                            join B in entity.branches on S.branchId equals B.branchId
                            select new SysEmailsModel()
                            {
                                emailId = S.emailId,
                                name = S.name,
                                email = S.email,
                                password = S.password,
                                port = S.port,
                                isSSL = S.isSSL,
                                smtpClient = S.smtpClient,
                                side = S.side,
                                notes = S.notes,
                                branchId = S.branchId,
                                isMajor = S.isMajor,
                                isActive = S.isActive,
                                createDate = S.createDate,
                                updateDate = S.updateDate,
                                createUserId = S.createUserId,
                                updateUserId = S.updateUserId,
                                canDelete = true,
                                branchName = B.name,
                            }
                        ).ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";
            //bool canDelete = false;

            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var List = (from S in  entity.sysEmails
            //                    join B in entity.branches  on S.branchId equals B.branchId
            //                             select new  SysEmailsModel()
            //                             {
            //                                emailId=S.emailId,
            //                                name=S.name,
            //                                email=S.email,
            //                                password=S.password,
            //                                port=S.port,
            //                                isSSL=S.isSSL,
            //                                smtpClient=S.smtpClient,
            //                                side=S.side,
            //                                notes=S.notes,
            //                                branchId=S.branchId,
            //                                 isMajor= S.isMajor,
            //                                 isActive =S.isActive,
            //                                createDate = S.createDate,
            //                                updateDate = S.updateDate,
            //                                createUserId = S.createUserId,
            //                                updateUserId=S.updateUserId,
            //                                canDelete=true,
            //                                branchName=B.name,

            //                             }).ToList();



            //        if (List == null)
            //            return NotFound();
            //        else
            //            return Ok(List);
            //    }
            //}
            ////else
            //return NotFound();
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            // public ResponseVM GetPurinv(string token)long emailId




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long emailId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "emailId")
                    {
                        emailId = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var item = entity.sysEmails
                            .Where(u => u.emailId == emailId)
                            .Select(
                                S =>
                                    new
                                    {
                                        S.emailId,
                                        S.name,
                                        S.email,
                                        S.password,
                                        S.port,
                                        S.isSSL,
                                        S.smtpClient,
                                        S.side,
                                        S.notes,
                                        S.branchId,
                                        S.isMajor,
                                        S.isActive,
                                        S.createDate,
                                        S.updateDate,
                                        S.createUserId,
                                        S.updateUserId,
                                    }
                            )
                            .FirstOrDefault();
                        return TokenManager.GenerateToken(item);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //
            //
            //        string token = "";
            //        if (headers.Contains("APIKey"))
            //        {
            //            token = headers.GetValues("APIKey").First();
            //        }
            //        Validation validation = new Validation();
            //        bool valid = validation.CheckApiKey(token);

            //        if (valid)
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                var row = entity.sysEmails
            //               .Where(u => u.emailId == emailId)
            //               .Select(S => new
            //               {
            //                    S.emailId,
            //                      S.name,
            //                    S.email,
            //                       S.password,
            //                     S.port,
            //                    S.isSSL,
            //                    S.smtpClient,
            //                    S.side,
            //                     S.notes,
            //                    S.branchId,
            //                   S.isMajor,
            //                    S.isActive,
            //                     S.createDate,
            //                     S.updateDate,
            //                     S.createUserId,
            //                  S.updateUserId,




            //})
            //               .FirstOrDefault();

            //                if (row == null)
            //                    return NotFound();
            //                else
            //                    return Ok(row);
            //            }
            //        }
            //        else
            //            return NotFound();
        }

        //
        // get
        [HttpPost]
        [Route("GetByBranchIdandSide")]
        public string GetByBranchIdandSide(string token)
        {
            // public ResponseVM GetPurinv(string token)long branchId,string side

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string side = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                }
                sysEmails emptyrow = null;
                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //  return email with same branch and side or
                        // return email with same side and isMajor
                        var row = entity.sysEmails
                            .Where(u => u.branchId == branchId && u.side == side)
                            .Select(
                                S =>
                                    new
                                    {
                                        S.emailId,
                                        S.name,
                                        S.email,
                                        S.password,
                                        S.port,
                                        S.isSSL,
                                        S.smtpClient,
                                        S.side,
                                        S.notes,
                                        S.branchId,
                                        S.isMajor,
                                        S.isActive,
                                        S.createDate,
                                        S.updateDate,
                                        S.createUserId,
                                        S.updateUserId,
                                    }
                            )
                            .FirstOrDefault();

                        if (row == null)
                        {
                            var row2 = entity.sysEmails
                                .Where(u => u.side == side && u.isMajor == true)
                                .Select(
                                    S =>
                                        new
                                        {
                                            S.emailId,
                                            S.name,
                                            S.email,
                                            S.password,
                                            S.port,
                                            S.isSSL,
                                            S.smtpClient,
                                            S.side,
                                            S.notes,
                                            S.branchId,
                                            S.isMajor,
                                            S.isActive,
                                            S.createDate,
                                            S.updateDate,
                                            S.createUserId,
                                            S.updateUserId,
                                        }
                                )
                                .FirstOrDefault();
                            if (row2 == null)
                            {
                                return TokenManager.GenerateToken(emptyrow);
                            }
                            else
                            {
                                return TokenManager.GenerateToken(row2);
                            }
                        }
                        else
                            return TokenManager.GenerateToken(row);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken(emptyrow);
                }
            }
        }

        // add or update location
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            //string Object
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
                sysEmails newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<sysEmails>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    sysEmails tmpObject = null;

                    try
                    {
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
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            // check if there is other same side in same branch
                            var sidebranch = entity.sysEmails
                                .Where(
                                    e =>
                                        e.branchId == newObject.branchId
                                        && e.side == newObject.side
                                        && e.emailId != newObject.emailId
                                )
                                .ToList();
                            //if not exist continue save
                            if (sidebranch == null || sidebranch.Count() == 0)
                            {
                                var locationEntity = entity.Set<sysEmails>();
                                if (newObject.emailId == 0)
                                {
                                    newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    newObject.updateUserId = newObject.createUserId;
                                    //  string encodedStr = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("inputStr"));
                                    // string inputStr = Encoding.UTF8.GetString(Convert.FromBase64String(encodedStr));
                                    locationEntity.Add(newObject);
                                    entity.SaveChanges();
                                    message = newObject.emailId.ToString();
                                }
                                else
                                {
                                    tmpObject = entity.sysEmails
                                        .Where(p => p.emailId == newObject.emailId)
                                        .FirstOrDefault();

                                    tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    tmpObject.updateUserId = newObject.updateUserId;

                                    tmpObject.name = newObject.name;
                                    tmpObject.emailId = newObject.emailId;

                                    tmpObject.email = newObject.email;
                                    tmpObject.password = newObject.password;
                                    tmpObject.port = newObject.port;
                                    tmpObject.isSSL = newObject.isSSL;
                                    tmpObject.smtpClient = newObject.smtpClient;
                                    tmpObject.side = newObject.side;
                                    tmpObject.notes = newObject.notes;
                                    tmpObject.branchId = newObject.branchId;

                                    tmpObject.isActive = newObject.isActive;
                                    tmpObject.isMajor = newObject.isMajor;

                                    entity.SaveChanges();

                                    message = tmpObject.emailId.ToString();
                                }
                                //  entity.SaveChanges();

                                return TokenManager.GenerateToken(message);
                            }
                            else
                            {
                                message = "-4";
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

                // return new ResponseVM { Status = "Fail", Message = TokenManager.GenerateToken(message) };
                return TokenManager.GenerateToken(message);
            }
            //var re = Request;
            //
            //string token = "";
            //string message = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    Object = Object.Replace("\\", string.Empty);
            //    Object = Object.Trim('"');
            //    sysEmails newObject = JsonConvert.DeserializeObject<sysEmails>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    if (newObject.updateUserId == 0 || newObject.updateUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.updateUserId = id;
            //    }
            //    if (newObject.createUserId == 0 || newObject.createUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.createUserId = id;
            //    }

            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {

            //            // check if there is other same side in same branch
            //            var sidebranch = entity.sysEmails
            //              .Where(e => e.branchId == newObject.branchId && e.side == newObject.side && e.emailId != newObject.emailId).ToList();
            //            //if not exist continue save
            //            if (sidebranch == null || sidebranch.Count()==0)
            //            {
            //                var locationEntity = entity.Set<sysEmails>();
            //            if (newObject.emailId == 0)
            //            {

            //                newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newObject.updateUserId = newObject.createUserId;
            //                //  string encodedStr = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("inputStr"));
            //                // string inputStr = Encoding.UTF8.GetString(Convert.FromBase64String(encodedStr));
            //                locationEntity.Add(newObject);
            //                entity.SaveChanges();
            //                message = newObject.emailId.ToString();
            //            }
            //            else
            //            {
            //                var tmpObject = entity.sysEmails.Where(p => p.emailId == newObject.emailId).FirstOrDefault();

            //                tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                tmpObject.updateUserId = newObject.updateUserId;

            //                tmpObject.name = newObject.name;
            //                tmpObject.emailId = newObject.emailId;

            //                tmpObject.email = newObject.email;
            //                tmpObject.password = newObject.password;
            //                tmpObject.port = newObject.port;
            //                tmpObject.isSSL = newObject.isSSL;
            //                tmpObject.smtpClient = newObject.smtpClient;
            //                tmpObject.side = newObject.side;
            //                tmpObject.notes = newObject.notes;
            //                tmpObject.branchId = newObject.branchId;

            //                tmpObject.isActive = newObject.isActive;
            //                tmpObject.isMajor = newObject.isMajor;


            //                entity.SaveChanges();

            //                message = tmpObject.emailId.ToString();
            //            }
            //            //  entity.SaveChanges();
            //        }
            //            else
            //            {
            //                message = "-4";
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        message = "-1";
            //    }
            //}
            //return message;
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            // public ResponseVM Delete(string token)long emailId, long userId, bool final
            //long Id, long userId
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long emailId = 0;
                long userId = 0;
                bool final = false;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "emailId")
                    {
                        emailId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
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
                        sysEmails objectDelete = entity.sysEmails.Find(emailId);

                        entity.sysEmails.Remove(objectDelete);
                        message = entity.SaveChanges().ToString();

                        return TokenManager.GenerateToken(message);
                    }
                    //  return TokenManager.GenerateToken(message);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";
            //int message = 0;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}

            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);
            //if (valid)
            //{
            //    if (final)
            //    {
            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                sysEmails objectDelete = entity.sysEmails.Find(emailId);

            //                entity.sysEmails.Remove(objectDelete);
            //                message = entity.SaveChanges();

            //                return message.ToString();
            //            }
            //        }
            //        catch
            //        {
            //            return "-1";
            //        }
            //    }
            //    else
            //    {
            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                sysEmails objectDelete = entity.sysEmails.Find(emailId);

            //                objectDelete.isActive = 0;
            //                objectDelete.updateUserId = userId;
            //                objectDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                message = entity.SaveChanges();

            //                return message.ToString(); ;
            //            }
            //        }
            //        catch
            //        {
            //            return "-2";
            //        }
            //    }
            //}
            //else
            //    return "-3";
        }
    }
}
