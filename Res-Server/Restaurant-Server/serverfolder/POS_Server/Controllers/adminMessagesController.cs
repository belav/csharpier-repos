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
    [RoutePrefix("api/adminMessages")]
    public class adminMessagesController : ApiController
    {
        CountriesController cc = new CountriesController();

        // GET api/<controller> get all adminMessages
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            bool canDelete = false;
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
                        .adminMessages
                        .Select(
                            S =>
                                new adminMessagesModel()
                                {
                                    msgId = S.msgId,
                                    title = S.title,
                                    msgContent = S.msgContent,
                                    isActive = S.isActive,
                                    notes = S.notes,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    branchCreatorId = S.branchCreatorId,
                                    mainMsgId = S.mainMsgId,
                                }
                        )
                        .ToList();

                    // can delet or not
                    if (itemList.Count > 0)
                    {
                        foreach (adminMessagesModel item in itemList)
                        {
                            canDelete = false;

                            int Id = (int)item.msgId;
                            var rowitem = entity
                                .messagesPos
                                .Where(x => x.msgId == Id && x.isReaded == true)
                                .Select(x => new { x.msgId })
                                .FirstOrDefault();

                            if ((rowitem is null))
                                canDelete = true;

                            item.canDelete = canDelete;
                        }
                    }
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
                        .adminMessages
                        .Where(S => S.msgId == Id)
                        .Select(
                            S =>
                                new
                                {
                                    S.msgId,
                                    S.title,
                                    S.msgContent,
                                    S.isActive,
                                    S.notes,
                                    S.createUserId,
                                    S.updateUserId,
                                    S.createDate,
                                    S.updateDate,
                                    S.branchCreatorId,
                                    S.mainMsgId,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(item);
                }
            }
        }

        public adminMessagesModel GetById(long msgId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                adminMessagesModel item = entity
                    .adminMessages
                    .Where(S => S.msgId == msgId)
                    .Select(
                        S =>
                            new adminMessagesModel
                            {
                                msgId = S.msgId,
                                title = S.title,
                                msgContent = S.msgContent,
                                isActive = S.isActive,
                                notes = S.notes,
                                createUserId = S.createUserId,
                                updateUserId = S.updateUserId,
                                createDate = S.createDate,
                                updateDate = S.updateDate,
                                branchCreatorId = S.branchCreatorId,
                                mainMsgId = S.mainMsgId,
                            }
                    )
                    .FirstOrDefault();
                return item;
            }
        }

        public adminMessagesModel GetLastMessageByUserId(long userId, long posId)
        {
            long res = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemLast = (
                    from mp in entity.messagesPos

                    join m in entity.adminMessages on mp.msgId equals m.msgId
                    join u in entity.users on m.createUserId equals u.userId

                    where (mp.posId == posId || mp.toUserId == userId) && mp.isReaded == false

                    select new messagesPosModel
                    {
                        //mp
                        msgPosId = mp.msgPosId,
                        msgId = mp.msgId,
                        toUserId = mp.toUserId,
                        createDate = mp.createDate,
                    }
                ).ToList().OrderByDescending(x => x.createDate).FirstOrDefault();
                adminMessagesModel lastMsg = new adminMessagesModel();
                messagesPosController mpctrlr = new messagesPosController();
                if (itemLast != null)
                {
                    res = mpctrlr.updateIsReadedById(itemLast.msgPosId);
                    // itemLast.isReaded = true;
                    if (res > 0)
                    {
                        lastMsg = GetById((long)itemLast.msgId);
                        return lastMsg;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        [Route("GetLastMessageByUserId")]
        public string GetLastMessageByUserId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long res = 0;
                long userId = 0;
                long posId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }

                try
                {
                    adminMessagesModel lastmsg = new adminMessagesModel();
                    lastmsg = GetLastMessageByUserId(userId, posId);
                    return TokenManager.GenerateToken(lastmsg);
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken("0");
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
                adminMessages newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<adminMessages>(
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
                        adminMessages tmpObject = new adminMessages();
                        var sEntity = entity.Set<adminMessages>();
                        DateTime datenow = cc.AddOffsetTodate(DateTime.Now);
                        if (newObject.msgId == 0)
                        {
                            newObject.createDate = datenow;
                            newObject.updateDate = datenow;
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = sEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.msgId.ToString();
                        }
                        else
                        {
                            tmpObject = entity
                                .adminMessages
                                .Where(p => p.msgId == newObject.msgId)
                                .FirstOrDefault();
                            tmpObject.msgId = newObject.msgId;
                            tmpObject.title = newObject.title;
                            tmpObject.msgContent = newObject.msgContent;
                            tmpObject.isActive = newObject.isActive;
                            tmpObject.notes = newObject.notes;
                            tmpObject.updateDate = datenow;
                            //  tmpObject.branchCreatorId = newObject.branchCreatorId;
                            //tmpObject.createDate = newObject.createDate;

                            //   tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;

                            tmpObject.updateDate = datenow; // server current date;

                            tmpObject.mainMsgId = newObject.mainMsgId;

                            entity.SaveChanges();
                            message = tmpObject.msgId.ToString();
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
                long msgId = 0;
                long userId = 0;
                bool final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        msgId = long.Parse(c.Value);
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
                            adminMessages Obj = entity.adminMessages.Find(msgId);
                            //check
                            var rowitem = entity
                                .messagesPos
                                .Where(x => x.msgId == msgId && x.isReaded == true)
                                .Select(x => new { x.msgId })
                                .FirstOrDefault();
                            if ((rowitem is null))
                            {
                                //delete related rows
                                var Listitem = entity
                                    .messagesPos
                                    .Where(x => x.msgId == msgId)
                                    .ToList();
                                entity.messagesPos.RemoveRange(Listitem);
                                entity.adminMessages.Remove(Obj);
                                message = entity.SaveChanges().ToString();
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
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            adminMessages Obj = entity.adminMessages.Find(msgId);

                            Obj.isActive = false;
                            Obj.updateUserId = userId;
                            Obj.updateDate = cc.AddOffsetTodate(DateTime.Now);

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

        //
        [HttpPost]
        [Route("SendMessage")]
        public string SendMessage(string token)
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
                /*
                 * message
posIdList
all
branchId

                 * */
                string messagestr = "";
                bool all = false;
                long branchId = 0;
                string posIdListstr = "";

                adminMessages newMessage = null;
                List<long> posIdList = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "message")
                    {
                        messagestr = c.Value.Replace("\\", string.Empty);
                        messagestr = messagestr.Trim('"');
                        newMessage = JsonConvert.DeserializeObject<adminMessages>(
                            messagestr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "posIdList")
                    {
                        posIdListstr = c.Value.Replace("\\", string.Empty);
                        posIdListstr = posIdListstr.Trim('"');
                        posIdList = JsonConvert.DeserializeObject<List<long>>(
                            posIdListstr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "all")
                    {
                        all = bool.Parse(c.Value);
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
                        adminMessages tmpObject = new adminMessages();
                        var sEntity = entity.Set<adminMessages>();
                        DateTime datenow = cc.AddOffsetTodate(DateTime.Now);
                        long msgId = 0;
                        if (newMessage.msgId == 0)
                        {
                            newMessage.createDate = datenow;
                            newMessage.updateDate = datenow;
                            newMessage.updateUserId = newMessage.createUserId;
                            tmpObject = sEntity.Add(newMessage);
                            entity.SaveChanges();

                            msgId = tmpObject.msgId;
                            message = msgId.ToString();
                            datenow = cc.AddOffsetTodate(DateTime.Now);
                            List<messagesPos> newlist = new List<messagesPos>();
                            List<long> posListid = new List<long>();
                            //case 1 all
                            if (all)
                            {
                                var posList = (
                                    from p in entity.pos
                                    where p.isActive == 1
                                    select new PosModel()
                                    {
                                        posId = p.posId,
                                        //balance = p.balance != null ? p.balance : 0,
                                        //branchId = p.branchId,
                                        //code = p.code,
                                        //name = p.name,
                                        //branchName = x.name,
                                        //createDate = p.createDate,
                                        //updateDate = p.updateDate,
                                        //createUserId = p.createUserId,
                                        //updateUserId = p.updateUserId,
                                        isActive = p.isActive,
                                        //balanceAll = p.balanceAll,
                                        //note = p.note,
                                        //branchCode = x.code,
                                        //boxState = p.boxState,
                                        //isAdminClose = p.isAdminClose,
                                    }
                                ).ToList();
                                posListid = posList.Select(x => x.posId).ToList();

                                //foreach(PosModel posrow in posList)
                                //{
                                //    messagesPos newobj = new messagesPos();
                                //    newobj.posId = posrow.posId;
                                //    newobj.msgId = msgId;
                                //    newobj.isReaded = false;
                                //    newobj.createDate = datenow;
                                //    newobj.updateDate = datenow;
                                //    newobj.createUserId = tmpObject.createUserId;
                                //    newobj.updateUserId = tmpObject.createUserId;
                                //    newobj.notes = "";
                                //    newlist.Add(newobj);

                                //}
                                //entity.messagesPos.AddRange(newlist);
                                //entity.SaveChanges();
                            }
                            else if (branchId > 0)
                            {
                                var posList = (
                                    from p in entity.pos
                                    where p.isActive == 1 && p.branchId == branchId
                                    select new PosModel()
                                    {
                                        posId = p.posId,
                                        //balance = p.balance != null ? p.balance : 0,
                                        //branchId = p.branchId,
                                        //code = p.code,
                                        //name = p.name,
                                        //branchName = x.name,
                                        //createDate = p.createDate,
                                        //updateDate = p.updateDate,
                                        //createUserId = p.createUserId,
                                        //updateUserId = p.updateUserId,
                                        isActive = p.isActive,
                                        //balanceAll = p.balanceAll,
                                        //note = p.note,
                                        //branchCode = x.code,
                                        //boxState = p.boxState,
                                        //isAdminClose = p.isAdminClose,
                                    }
                                ).ToList();
                                posListid = posList.Select(x => x.posId).ToList();
                            }
                            else //case3
                            {
                                posListid = posIdList;
                                //foreach (int posId in posIdList)
                                //{
                                //    messagesPos newobj = new messagesPos();
                                //    newobj.posId = posId;
                                //    newobj.msgId = msgId;
                                //    newobj.isReaded = false;
                                //    newobj.createDate = datenow;
                                //    newobj.updateDate = datenow;
                                //    newobj.createUserId = tmpObject.createUserId;
                                //    newobj.updateUserId = tmpObject.createUserId;
                                //    newobj.notes = "";
                                //    newlist.Add(newobj);

                                //}
                                //entity.messagesPos.AddRange(newlist);
                                //entity.SaveChanges();
                            }
                            foreach (int posIdrow in posListid)
                            {
                                messagesPos newobj = new messagesPos();
                                newobj.posId = posIdrow;
                                newobj.msgId = msgId;
                                newobj.isReaded = false;
                                newobj.createDate = datenow;
                                newobj.updateDate = datenow;
                                newobj.createUserId = tmpObject.createUserId;
                                newobj.updateUserId = tmpObject.createUserId;

                                newobj.notes = "";
                                newlist.Add(newobj);
                            }
                            if (newlist.Count() > 0)
                            {
                                entity.messagesPos.AddRange(newlist);
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            //tmpObject = entity.adminMessages.Where(p => p.msgId == newObject.msgId).FirstOrDefault();
                            //tmpObject.msgId = newObject.msgId;
                            //tmpObject.title = newObject.title;
                            //tmpObject.msgContent = newObject.msgContent;
                            //tmpObject.isActive = newObject.isActive;
                            //tmpObject.notes = newObject.notes;
                            //tmpObject.updateDate = newObject.updateDate;

                            ////tmpObject.createDate = newObject.createDate;

                            ////   tmpObject.createUserId = newObject.createUserId;
                            //tmpObject.updateUserId = newObject.updateUserId;
                            //tmpObject.updateDate = datenow;// server current date;

                            //entity.SaveChanges();
                            //message = tmpObject.msgId.ToString();
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
        [Route("GetByCreatUserId")]
        public string GetByCreatUserId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long userId = 0;
                bool canDelete = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemList = (
                        from S in entity.adminMessages

                        where S.createUserId == userId
                        select new adminMessagesModel
                        {
                            msgId = S.msgId,
                            title = S.title,
                            msgContent = S.msgContent,
                            isActive = S.isActive,
                            notes = S.notes,
                            createUserId = S.createUserId,
                            updateUserId = S.updateUserId,
                            createDate = S.createDate,
                            updateDate = S.updateDate,
                            branchCreatorId = S.branchCreatorId,
                            branchCreatorName = S.branches.name,
                            mainMsgId = S.mainMsgId,
                        }
                    ).ToList();
                    // can delet or not
                    if (itemList.Count > 0)
                    {
                        foreach (adminMessagesModel item in itemList)
                        {
                            canDelete = false;

                            int Id = (int)item.msgId;
                            var rowitem = entity
                                .messagesPos
                                .Where(x => x.msgId == Id && x.isReaded == true)
                                .Select(x => new { x.msgId })
                                .FirstOrDefault();

                            if ((rowitem is null))
                                canDelete = true;

                            item.canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(itemList);
                }
            }
        }

        [HttpPost]
        [Route("SendMessageToUsers")]
        public string SendMessageToUsers(string token)
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
                /*
                 all
                 userid list
                 * */
                string messagestr = "";
                bool all = false;

                string userIdListstr = "";

                adminMessages newMessage = null;
                List<long> userIdList = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "message")
                    {
                        messagestr = c.Value.Replace("\\", string.Empty);
                        messagestr = messagestr.Trim('"');
                        newMessage = JsonConvert.DeserializeObject<adminMessages>(
                            messagestr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userIdList") //userIdList
                    {
                        userIdListstr = c.Value.Replace("\\", string.Empty);
                        userIdListstr = userIdListstr.Trim('"');
                        userIdList = JsonConvert.DeserializeObject<List<long>>(
                            userIdListstr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "all")
                    {
                        all = bool.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        adminMessages tmpObject = new adminMessages();
                        var sEntity = entity.Set<adminMessages>();
                        DateTime datenow = cc.AddOffsetTodate(DateTime.Now);
                        long msgId = 0;
                        if (newMessage.msgId == 0)
                        {
                            newMessage.createDate = datenow;
                            newMessage.updateDate = datenow;
                            newMessage.updateUserId = newMessage.createUserId;
                            tmpObject = sEntity.Add(newMessage);
                            entity.SaveChanges();

                            msgId = tmpObject.msgId;
                            message = msgId.ToString();
                            datenow = cc.AddOffsetTodate(DateTime.Now);
                            List<messagesPos> newlist = new List<messagesPos>();
                            List<long> userListid = new List<long>();
                            //case 1 all
                            if (all)
                            {
                                var userList = (
                                    from u in entity.users
                                    where
                                        u.isActive == 1
                                        && !(u.isAdmin == true && u.username == "Support@Increase")
                                        && u.userId != (int)newMessage.createUserId
                                    select new UserModel()
                                    {
                                        userId = u.userId,
                                        isActive = u.isActive,
                                    }
                                ).ToList();
                                userListid = userList.Select(x => x.userId).ToList();
                            }
                            else //case2
                            {
                                userListid = userIdList;
                            }
                            foreach (int userIdrow in userListid)
                            {
                                messagesPos newobj = new messagesPos();
                                newobj.toUserId = userIdrow;
                                // newobj.posId = posIdrow;
                                newobj.msgId = msgId;
                                newobj.isReaded = false;
                                newobj.createDate = datenow;
                                newobj.updateDate = datenow;
                                newobj.createUserId = tmpObject.createUserId;
                                newobj.updateUserId = tmpObject.createUserId;

                                newobj.notes = "";
                                newlist.Add(newobj);
                            }
                            if (newlist.Count() > 0)
                            {
                                entity.messagesPos.AddRange(newlist);
                                entity.SaveChanges();
                            }
                        }
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch (Exception ex)
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }
    }
}
