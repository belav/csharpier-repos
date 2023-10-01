using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/messagesPos")]
    public class messagesPosController : ApiController
    {
        CountriesController cc = new CountriesController();

        // GET api/<controller> get all messagesPos
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
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
                    var itemList = entity
                        .messagesPos
                        .Select(
                            S =>
                                new messagesPosModel()
                                {
                                    msgPosId = S.msgPosId,
                                    msgId = S.msgId,
                                    posId = S.posId,
                                    isReaded = S.isReaded,
                                    notes = S.notes,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    canDelete = true,
                                    userReadId = S.userReadId,
                                    toUserId = S.toUserId,
                                }
                        )
                        .ToList();

                    // can delet or not

                    return TokenManager.GenerateToken(itemList);
                }
            }
        }

        // GET api/<controller>  Get card By ID
        [HttpPost]
        [Route("GetById")]
        public string GetById(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                int Id = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        Id = int.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var item = entity
                        .messagesPos
                        .Where(S => S.msgPosId == Id)
                        .Select(
                            S =>
                                new
                                {
                                    S.msgPosId,
                                    S.msgId,
                                    S.posId,
                                    S.isReaded,
                                    S.notes,
                                    S.createUserId,
                                    S.updateUserId,
                                    S.createDate,
                                    S.updateDate,
                                    S.userReadId,
                                    S.toUserId,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(item);
                }
            }
        }

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
                messagesPos newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<messagesPos>(
                            itemObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        messagesPos tmpObject = new messagesPos();
                        var sEntity = entity.Set<messagesPos>();
                        DateTime datenow = cc.AddOffsetTodate(DateTime.Now);
                        if (newObject.msgPosId == 0)
                        {
                            newObject.createDate = datenow;
                            newObject.updateDate = datenow;
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = sEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.msgPosId.ToString();
                        }
                        else
                        {
                            tmpObject = entity
                                .messagesPos
                                .Where(p => p.msgPosId == newObject.msgPosId)
                                .FirstOrDefault();
                            tmpObject.msgPosId = newObject.msgPosId;
                            tmpObject.msgId = newObject.msgId;
                            tmpObject.posId = newObject.posId;
                            tmpObject.isReaded = newObject.isReaded;
                            tmpObject.notes = newObject.notes;

                            tmpObject.updateUserId = newObject.updateUserId;

                            tmpObject.updateDate = datenow;
                            tmpObject.updateDate = datenow; // server current date;
                            tmpObject.userReadId = newObject.userReadId;
                            tmpObject.toUserId = newObject.toUserId;
                            entity.SaveChanges();
                            message = tmpObject.msgPosId.ToString();
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
                long msgPosId = 0;
                long userId = 0;
                bool final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        msgPosId = long.Parse(c.Value);
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
                            messagesPos Obj = entity.messagesPos.Find(msgPosId);
                            entity.messagesPos.Remove(Obj);
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
                return TokenManager.GenerateToken(message);
                //else
                //{
                //    try
                //    {
                //        using (incposdbEntities entity = new incposdbEntities())
                //        {
                //            messagesPos  Obj = entity.messagesPos.Find(msgPosId);

                //            Obj.isActive = false;
                //            Obj.updateUserId = userId;
                //            Obj.updateDate = cc.AddOffsetTodate(DateTime.Now);
                //            message = entity.SaveChanges().ToString();
                //            return TokenManager.GenerateToken(message);
                //        }
                //    }
                //    catch
                //    {
                //        message = "0";
                //        return TokenManager.GenerateToken(message);
                //    }
                //}
            }
        }

        [HttpPost]
        [Route("GetBymsgId")]
        public string GetBymsgId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long msgId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        msgId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemList = (
                        from mp in entity.messagesPos
                        join pp in entity.pos on mp.posId equals pp.posId into jp
                        join uu in entity.users on mp.userReadId equals uu.userId into ju
                        join tu in entity.users on mp.toUserId equals tu.userId into jtu
                        from p in jp.DefaultIfEmpty()
                        from u in ju.DefaultIfEmpty()
                        from tou in jtu.DefaultIfEmpty()
                        where mp.msgId == msgId
                        select new messagesPosModel
                        {
                            msgPosId = mp.msgPosId,
                            msgId = mp.msgId,
                            posId = mp.posId,
                            isReaded = mp.isReaded,
                            notes = mp.notes,
                            createUserId = mp.createUserId,
                            updateUserId = mp.updateUserId,
                            createDate = mp.createDate,
                            updateDate = mp.updateDate,
                            posName = p.name,
                            branchName = p.branches.name,
                            branchId = p.branchId,
                            userReadId = mp.userReadId,
                            toUserId = mp.toUserId,
                            userRead = u.name + " " + u.lastname,
                            toUserFullName = tou.name + " " + tou.lastname,
                        }
                    ).ToList();
                    return TokenManager.GenerateToken(itemList);
                }
            }
        }

        [HttpPost]
        [Route("GetByPosId")]
        public string GetByPosId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long posId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemList = (
                        from mp in entity.messagesPos
                        join p in entity.pos on mp.posId equals p.posId
                        join m in entity.adminMessages on mp.msgId equals m.msgId
                        join u in entity.users on m.createUserId equals u.userId
                        where mp.posId == posId
                        select new messagesPosModel
                        {
                            //mp
                            msgPosId = mp.msgPosId,
                            msgId = mp.msgId,
                            posId = mp.posId,
                            isReaded = mp.isReaded,
                            notes = mp.notes,
                            updateUserId = mp.updateUserId,
                            userReadId = mp.userReadId,
                            createDate = mp.createDate,
                            updateDate = mp.updateDate,
                            //pos
                            posName = p.name,
                            branchName = p.branches.name,
                            branchId = p.branchId,
                            //message
                            title = m.title,
                            msgContent = m.msgContent,
                            isActive = m.isActive,
                            createUserId = m.createUserId,
                            branchCreatorId = m.branchCreatorId,
                            branchCreatorName = m.branches.name,
                            //user
                            msgCreatorName = u.name,
                            msgCreatorLast = u.lastname,
                            toUserId = mp.toUserId,
                            mainMsgId = m.mainMsgId,
                        }
                    ).ToList();
                    return TokenManager.GenerateToken(itemList);
                }
            }
        }

        [HttpPost]
        [Route("GetByPosIdUserId")]
        public string GetByPosIdUserId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long posId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        posId = long.Parse(c.Value);
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
                        var itemList = (
                            from mp in entity.messagesPos
                            join tpos in entity.pos on mp.posId equals tpos.posId into jp
                            join m in entity.adminMessages on mp.msgId equals m.msgId
                            join u in entity.users on m.createUserId equals u.userId
                            from p in jp.DefaultIfEmpty()
                            where mp.posId == posId || mp.toUserId == userId
                            select new messagesPosModel
                            {
                                //mp
                                msgPosId = mp.msgPosId,
                                msgId = mp.msgId,
                                posId = mp.posId,
                                isReaded = mp.isReaded,
                                notes = mp.notes,
                                updateUserId = mp.updateUserId,
                                userReadId = mp.userReadId,
                                createDate = mp.createDate,
                                updateDate = mp.updateDate,
                                //pos
                                posName = p.name,
                                branchName = p.branches.name,
                                branchId = p.branchId,
                                //message
                                title = m.title,
                                msgContent = m.msgContent,
                                isActive = m.isActive,
                                createUserId = m.createUserId,
                                branchCreatorId = m.branchCreatorId,
                                branchCreatorName = m.branches.name,
                                //user
                                msgCreatorName = u.name,
                                msgCreatorLast = u.lastname,
                                toUserId = mp.toUserId,
                                mainMsgId = m.mainMsgId,
                            }
                        ).ToList();
                        return TokenManager.GenerateToken(itemList);
                    }
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("updateIsReaded")]
        public string updateIsReaded(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "0";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long userId = 0;
                string posIdListstr = "";
                List<long> msgPosIdList = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "msgPosIdList")
                    {
                        posIdListstr = c.Value.Replace("\\", string.Empty);
                        posIdListstr = posIdListstr.Trim('"');
                        msgPosIdList = JsonConvert.DeserializeObject<List<long>>(
                            posIdListstr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
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
                        var sEntity = entity.Set<messagesPos>();
                        DateTime datenow = cc.AddOffsetTodate(DateTime.Now);

                        entity.SaveChanges();
                        datenow = cc.AddOffsetTodate(DateTime.Now);
                        List<messagesPos> newlist = new List<messagesPos>();
                        if (msgPosIdList.Count() > 0)
                        {
                            newlist = entity
                                .messagesPos
                                .Where(m => msgPosIdList.Contains(m.msgPosId))
                                .ToList();

                            newlist.ForEach(m =>
                            {
                                m.isReaded = true;
                                m.updateDate = datenow;
                                m.updateUserId = userId;
                                m.userReadId = userId;
                            });
                            message = entity.SaveChanges().ToString();
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

        public int GetNotReadCountByUserId(int userId, int posId)
        {
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemListCount = (
                        from mp in entity.messagesPos

                        where (mp.posId == posId || mp.toUserId == userId) && mp.isReaded == false
                        select new messagesPosModel
                        {
                            //mp
                            msgPosId = mp.msgPosId,
                        }
                    ).ToList().Count();
                    return itemListCount;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //public int GetNotReadCountByUserId(int userId)
        //{
        //    try
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var itemListCount = (from mp in entity.messagesPos

        //                                 where mp.toUserId == userId && mp.isReaded == false
        //                                 select new messagesPosModel
        //                                 {
        //                                     //mp
        //                                     msgPosId = mp.msgPosId,

        //                                 }).ToList().Count();
        //            return itemListCount;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        public long updateIsReadedById(long msgPosId)
        {
            long message = 0;
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var sEntity = entity.Set<messagesPos>();
                    DateTime datenow = cc.AddOffsetTodate(DateTime.Now);
                    entity.SaveChanges();
                    datenow = cc.AddOffsetTodate(DateTime.Now);
                    var row = entity
                        .messagesPos
                        .Where(m => m.msgPosId == msgPosId)
                        .FirstOrDefault();
                    row.isReaded = true;
                    row.updateDate = datenow;
                    row.updateUserId = row.toUserId;
                    row.userReadId = row.toUserId;

                    message = entity.SaveChanges();
                }
                return message;
            }
            catch
            {
                message = 0;
                return message;
            }
        }
    }
}
