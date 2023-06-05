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
    [RoutePrefix("api/notificationUser")]
    public class notificationUserController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("GetByUserId")]
        public string GetByUserId(string token)
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
                string type = "";
                long posId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (
                        from S in entity.notificationUser
                        where
                            (S.userId == userId || S.posId == posId)
                            && S.notification.msgType.Contains(type)
                        select new NotificationUserModel()
                        {
                            notUserId = S.notUserId,
                            notId = S.notId,
                            userId = S.userId,
                            posId = S.posId,
                            isRead = S.isRead,
                            createUserId = S.createUserId,
                            updateUserId = S.updateUserId,
                            createDate = S.createDate,
                            updateDate = S.updateDate,
                            title = S.notification.title,
                            ncontent = S.notification.ncontent,
                            path = S.notification.path,
                            msgType = S.notification.msgType,
                            side = S.notification.side,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("GetNotUserCount")]
        public string GetNotUserCount(string token)
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
                string type = "";
                long posId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }

                int notCount = GetNotUserCount(posId, userId, type);
                return TokenManager.GenerateToken(notCount);
                //using (incposdbEntities entity = new incposdbEntities())
                //{
                //    var notificationCount = entity.notificationUser
                //        .Where(x => (x.userId == userId || x.posId == posId) && x.isRead == false && x.notification.msgType.Contains(type))
                //        .ToList().Count;
                //    return TokenManager.GenerateToken(notificationCount);

                //}
            }
        }

        public int GetNotUserCount(long posId, long userId, string type)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var notificationCount = entity.notificationUser
                    .Where(
                        x =>
                            (x.userId == userId || x.posId == posId)
                            && x.isRead == false
                            && x.notification.msgType.Contains(type)
                    )
                    .ToList()
                    .Count;
                return notificationCount;
            }
        }

        // add or update notification
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
                string obj = "";
                notificationUser Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        obj = c.Value.Replace("\\", string.Empty);
                        obj = obj.Trim('"');
                        Object = JsonConvert.DeserializeObject<notificationUser>(
                            obj,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var notEntity = entity.Set<notificationUser>();

                        Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        Object.updateUserId = Object.createUserId;

                        notificationUser not = notEntity.Add(Object);
                        entity.SaveChanges();
                        message = not.notUserId.ToString();
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
        [Route("setAsRead")]
        public string setAsRead(string token)
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
                try
                {
                    long notUserId = 0;
                    long posId = 0;
                    string type = "";
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "notUserId")
                        {
                            notUserId = long.Parse(c.Value);
                        }
                        else if (c.Type == "posId")
                        {
                            posId = long.Parse(c.Value);
                        }
                        else if (c.Type == "type")
                        {
                            type = c.Value;
                        }
                    }
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // var notEntity = entity.Set<notificationUser>();

                        var List = (
                            from S in entity.notificationUser
                            where
                                (
                                    (S.userId == notUserId || S.posId == posId)
                                    && S.isRead == false
                                    && S.notification.msgType.Contains(type)
                                )
                            select new NotificationUserModel()
                            {
                                notUserId = S.notUserId,
                                notId = S.notId,
                                userId = S.userId,
                                posId = S.posId,
                                isRead = S.isRead,
                                createUserId = S.createUserId,
                                updateUserId = S.updateUserId,
                                createDate = S.createDate,
                                updateDate = S.updateDate,
                                title = S.notification.title,
                                ncontent = S.notification.ncontent,
                                path = S.notification.path,
                                msgType = S.notification.msgType,
                                side = S.notification.side,
                            }
                        ).ToList();

                        //List.ForEach(a => a.isRead = true  );
                        //List.ForEach(a => a.updateDate = DateTime.Now);
                        foreach (NotificationUserModel m in List)
                        {
                            var notEntity = entity.notificationUser.Find(m.notUserId);
                            notEntity.isRead = true;
                            notEntity.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            entity.SaveChanges();
                        }

                        message = "1";
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
