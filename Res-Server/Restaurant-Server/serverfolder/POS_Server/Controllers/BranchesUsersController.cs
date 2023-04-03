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
    [RoutePrefix("api/BranchesUsers")]
    public class BranchesUsersController : ApiController
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
                        from S in entity.branchesUsers
                        join B in entity.branches on S.branchId equals B.branchId into JB
                        join U in entity.users on S.userId equals U.userId into JU
                        from JBB in JB.DefaultIfEmpty()
                        from JUU in JU.DefaultIfEmpty()
                        select new BranchesUsersModel()
                        {
                            branchsUsersId = S.branchsUsersId,
                            branchId = S.branchId,
                            userId = S.userId,
                            createDate = S.createDate,
                            updateDate = S.updateDate,
                            createUserId = S.createUserId,
                            updateUserId = S.updateUserId,
                            // branch
                            bbranchId = JBB.branchId,
                            bcode = JBB.code,
                            bname = JBB.name,
                            baddress = JBB.address,
                            bemail = JBB.email,
                            bphone = JBB.phone,
                            bmobile = JBB.mobile,
                            bcreateDate = JBB.createDate,
                            bupdateDate = JBB.updateDate,
                            bcreateUserId = JBB.createUserId,
                            bupdateUserId = JBB.updateUserId,
                            bnotes = JBB.notes,
                            bparentId = JBB.parentId,
                            bisActive = JBB.isActive,
                            btype = JBB.type,
                            // user
                            uuserId = JUU.userId,
                            uusername = JUU.username,
                            upassword = JUU.password,
                            uname = JUU.name,
                            ulastname = JUU.lastname,
                            ujob = JUU.job,
                            uworkHours = JUU.workHours,
                            ucreateDate = JUU.createDate,
                            uupdateDate = JUU.updateDate,
                            ucreateUserId = JUU.createUserId,
                            uupdateUserId = JUU.updateUserId,
                            uphone = JUU.phone,
                            umobile = JUU.mobile,
                            uemail = JUU.email,
                            unotes = JUU.notes,
                            uaddress = JUU.address,
                            uisActive = JUU.isActive,
                            uisOnline = JUU.isOnline,
                            uimage = JUU.image,
                        }
                    ).ToList();
                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("GetBranchesByUserId")]
        public string GetBranchesByUserId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                int userId = 0;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        userId = int.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (
                        from S in entity.branchesUsers
                        join B in entity.branches on S.branchId equals B.branchId into JB
                        join U in entity.users on S.userId equals U.userId into JU
                        from JBB in JB.DefaultIfEmpty()
                        from JUU in JU.DefaultIfEmpty()
                        where S.userId == userId && JBB.type == type
                        select new BranchesUsersModel()
                        {
                            branchsUsersId = S.branchsUsersId,
                            branchId = S.branchId,
                            userId = S.userId,
                            createDate = S.createDate,
                            updateDate = S.updateDate,
                            createUserId = S.createUserId,
                            updateUserId = S.updateUserId,
                            // branch
                            bbranchId = JBB.branchId,
                            bcode = JBB.code,
                            bname = JBB.name,
                            baddress = JBB.address,
                            bemail = JBB.email,
                            bphone = JBB.phone,
                            bmobile = JBB.mobile,
                            bcreateDate = JBB.createDate,
                            bupdateDate = JBB.updateDate,
                            bcreateUserId = JBB.createUserId,
                            bupdateUserId = JBB.updateUserId,
                            bnotes = JBB.notes,
                            bparentId = JBB.parentId,
                            bisActive = JBB.isActive,
                            btype = JBB.type,
                            // user
                            uuserId = JUU.userId,
                            uusername = JUU.username,
                            upassword = JUU.password,
                            uname = JUU.name,
                            ulastname = JUU.lastname,
                            ujob = JUU.job,
                            uworkHours = JUU.workHours,
                            ucreateDate = JUU.createDate,
                            uupdateDate = JUU.updateDate,
                            ucreateUserId = JUU.createUserId,
                            uupdateUserId = JUU.updateUserId,
                            uphone = JUU.phone,
                            umobile = JUU.mobile,
                            uemail = JUU.email,
                            unotes = JUU.notes,
                            uaddress = JUU.address,
                            uisActive = JUU.isActive,
                            uisOnline = JUU.isOnline,
                            uimage = JUU.image,
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
                long branchsUsersId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        branchsUsersId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var row = entity.branchesUsers
                        .Where(u => u.branchsUsersId == branchsUsersId)
                        .Select(
                            S =>
                                new
                                {
                                    S.branchsUsersId,
                                    S.branchId,
                                    S.userId,
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

        // add or update location//BranchesUsers/UpdateBranchByUserId"
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
                string Objects = "";
                branchesUsers newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Objects = c.Value.Replace("\\", string.Empty);
                        Objects = Objects.Trim('"');
                        newObject = JsonConvert.DeserializeObject<branchesUsers>(
                            Objects,
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

                if (newObject.branchId == 0 || newObject.branchId == null)
                {
                    Nullable<long> id = null;
                    newObject.branchId = id;
                }
                if (newObject.userId == 0 || newObject.userId == null)
                {
                    Nullable<long> id = null;
                    newObject.userId = id;
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<branchesUsers>();
                        if (newObject.branchsUsersId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;

                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.branchsUsersId.ToString();
                        }
                        else
                        {
                            var tmpObject = entity.branchesUsers
                                .Where(p => p.branchsUsersId == newObject.branchsUsersId)
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.updateUserId = newObject.updateUserId;
                            tmpObject.branchsUsersId = newObject.branchsUsersId;

                            tmpObject.branchId = newObject.branchId;
                            tmpObject.userId = newObject.userId;

                            entity.SaveChanges();

                            message = tmpObject.branchsUsersId.ToString();
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

        //update branches list by userId
        [HttpPost]
        [Route("UpdateBranchByUserId")]
        public string UpdateBranchByUserId(string token)
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
                string branchesUsersObject = "";
                List<branchesUsers> newListObj = null;
                long userId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        branchesUsersObject = c.Value.Replace("\\", string.Empty);
                        branchesUsersObject = branchesUsersObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<branchesUsers>>(
                            branchesUsersObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "updateUserId")
                    {
                        updateUserId = long.Parse(c.Value);
                    }
                }
                List<branchesUsers> items = null;
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity.branchesUsers.Where(x => x.userId == userId).ToList();
                    if (items != null)
                    {
                        entity.branchesUsers.RemoveRange(items);
                        try
                        {
                            entity.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            message = "-2";
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    for (int i = 0; i < newListObj.Count; i++)
                    {
                        if (newListObj[i].updateUserId == 0 || newListObj[i].updateUserId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].updateUserId = id;
                        }
                        if (newListObj[i].createUserId == 0 || newListObj[i].createUserId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].createUserId = id;
                        }
                        if (newListObj[i].branchId == 0 || newListObj[i].branchId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].branchId = id;
                        }
                        if (newListObj[i].userId == 0 || newListObj[i].userId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].userId = id;
                        }
                        var branchEntity = entity.Set<branchesUsers>();

                        newListObj[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newListObj[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newListObj[i].updateUserId = updateUserId;
                        newListObj[i].userId = userId;
                        branchEntity.Add(newListObj[i]);
                        entity.SaveChanges();
                    }
                    try
                    {
                        entity.SaveChanges().ToString();
                        message = "1";
                        return TokenManager.GenerateToken(message);
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
                long branchsUsersId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchsUsersId")
                    {
                        branchsUsersId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        branchesUsers objectDelete = entity.branchesUsers.Find(branchsUsersId);

                        entity.branchesUsers.Remove(objectDelete);
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
