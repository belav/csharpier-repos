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
    [RoutePrefix("api/UsersLogs")]
    public class UsersLogsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        private static System.Timers.Timer logoutTimer;
        private static System.Timers.Timer oneminuteTimer;
        public static double oneMtime = 60000;

        // public DispatcherTimer logoutTimer;
        public static double Repeattime = 600000; //milliSecond//600000=10 minute
        public int maxIdleperiod = 15; //minute

        // GET api/<controller>
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            //public string GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = (
                            from S in entity.usersLogs
                            select new UsersLogsModel()
                            {
                                logId = S.logId,
                                sInDate = S.sInDate,
                                sOutDate = S.sOutDate,
                                posId = S.posId,
                                userId = S.userId,
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

            //
            //
            //            string token = "";
            //            bool canDelete = false;

            //            if (headers.Contains("APIKey"))
            //            {
            //                token = headers.GetValues("APIKey").First();
            //            }
            //            Validation validation = new Validation();
            //            bool valid = validation.CheckApiKey(token);

            //            if (valid) // APIKey is valid
            //            {
            //                using (incposdbEntities entity = new incposdbEntities())
            //                {
            //                    var List = (from S in  entity.usersLogs
            //                                         select new UsersLogsModel()
            //                                         {
            //                                            logId=S.logId,

            //                                             sInDate=S.sInDate,
            //                                             sOutDate=S.sOutDate,
            //                                             posId=S.posId,
            //                                             userId=S.userId,

            //                                         }).ToList();
            //                    /*
            //                     *


            //    public long logId { get; set; }
            //            public Nullable<System.DateTime> sInDate { get; set; }
            //            public Nullable<System.DateTime> sOutDate { get; set; }
            //            public Nullable<long> posId { get; set; }
            //            public Nullable<long> userId { get; set; }
            //            public bool canDelete { get; set; }

            //logId
            //sInDate
            //sOutDate
            //posId
            //userId
            //canDelete


            //                    */



            //                    if (List == null)
            //                        return NotFound();
            //                    else
            //                        return Ok(List);
            //                }
            //            }
            //            //else
            //            return NotFound();
        }

        // get by userId
        [HttpPost]
        [Route("GetByUserId")]
        public string GetByUserId(string token)
        {
            //public string GetPurinv(string token)long userId




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var item = (
                            from S in entity.usersLogs
                            where S.userId == userId
                            select new UsersLogsModel()
                            {
                                logId = S.logId,
                                sInDate = S.sInDate,
                                sOutDate = S.sOutDate,
                                posId = S.posId,
                                userId = S.userId,
                            }
                        ).ToList();

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
            //            string token = "";


            //            if (headers.Contains("APIKey"))
            //            {
            //                token = headers.GetValues("APIKey").First();
            //            }
            //            Validation validation = new Validation();
            //            bool valid = validation.CheckApiKey(token);

            //            if (valid) // APIKey is valid
            //            {
            //                using (incposdbEntities entity = new incposdbEntities())
            //                {
            //                    var List = (from S in entity.usersLogs
            //                                where S.userId== userId
            //                                select new UsersLogsModel()
            //                                {
            //                                    logId = S.logId,

            //                                    sInDate = S.sInDate,
            //                                    sOutDate = S.sOutDate,
            //                                    posId = S.posId,
            //                                    userId = S.userId,

            //                                }).ToList();
            //                    /*
            //                     *


            //    public long logId { get; set; }
            //            public Nullable<System.DateTime> sInDate { get; set; }
            //            public Nullable<System.DateTime> sOutDate { get; set; }
            //            public Nullable<long> posId { get; set; }
            //            public Nullable<long> userId { get; set; }
            //            public bool canDelete { get; set; }

            //logId
            //sInDate
            //sOutDate
            //posId
            //userId
            //canDelete


            //                    */



            //                    if (List == null)
            //                        return NotFound();
            //                    else
            //                        return Ok(List);
            //                }
            //            }
            //            //else
            //            return NotFound();
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            //public string GetPurinv(string token)long logId




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long logId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "logId")
                    {
                        logId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var item = entity.usersLogs
                            .Where(u => u.logId == logId)
                            .Select(
                                S =>
                                    new
                                    {
                                        S.logId,
                                        S.sInDate,
                                        S.sOutDate,
                                        S.posId,
                                        S.userId,
                                    }
                            )
                            .FirstOrDefault();
                        return TokenManager.GenerateToken(item);
                        // return TokenManager.GenerateToken(item);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                    //    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var row = entity.usersLogs
            //       .Where(u => u.logId == logId)
            //       .Select(S => new
            //       {

            //              S.logId,
            //              S.sInDate,
            //              S.sOutDate,
            //              S.posId,
            //              S.userId,


            //       })
            //       .FirstOrDefault();

            //        if (row == null)
            //            return NotFound();
            //        else
            //            return Ok(row);
            //    }
            //}
            //else
            //    return NotFound();
        }

        //checkOtherUser
        [HttpPost]
        [Route("checkOtherUser")]
        public string checkOtherUser(string token)
        {
            //public string GetPurinv(string token)long userId
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<usersLogs> List = entity.usersLogs
                            .Where(S => S.userId == userId && S.sOutDate == null)
                            .ToList();
                        if (List != null)
                        {
                            foreach (usersLogs row in List)
                            {
                                row.sOutDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                message = entity.SaveChanges().ToString();
                            }

                            //  message = List.LastOrDefault().sOutDate.ToString();


                            //  return Ok(msg);
                        }
                        else
                        {
                            message = "-1";
                            // return Ok("none");
                        }

                        return TokenManager.GenerateToken(message);
                    }
                    //  return TokenManager.GenerateToken(item);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //string msg = "";

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
            //        List<usersLogs> List = entity.usersLogs.Where(S => S.userId == userId && S.sOutDate == null).ToList();
            //        if(List !=null)
            //        {
            //            foreach(usersLogs row in List)
            //           {
            //            row.sOutDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //            entity.SaveChanges();
            //           }

            //                msg = List.LastOrDefault().sOutDate.ToString();

            //            return Ok(msg);
            //        }
            //        else { return Ok("none"); }


            //    }
            //}
            ////else
            //return Ok("error");
        }

        // add or update location
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            //public string Save(string token)
            //string Object string newObject
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
                usersLogs newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<usersLogs>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    usersLogs tmpObject = null;

                    try
                    {
                        if (newObject.posId == 0 || newObject.posId == null)
                        {
                            Nullable<long> id = null;
                            newObject.posId = id;
                        }
                        if (newObject.userId == 0 || newObject.userId == null)
                        {
                            Nullable<long> id = null;
                            newObject.userId = id;
                        }
                        DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var locationEntity = entity.Set<usersLogs>();
                            if (newObject.logId == 0 || newObject.logId == null)
                            {
                                // signIn

                                newObject.sInDate = datenow;

                                locationEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.logId.ToString();
                                //sign out old user

                                using (incposdbEntities entity2 = new incposdbEntities())
                                {
                                    List<usersLogs> ul = new List<usersLogs>();
                                    List<usersLogs> locationE = entity2.usersLogs.ToList();
                                    ul = locationE
                                        .Where(
                                            s =>
                                                s.sOutDate == null
                                                && ((datenow - (DateTime)s.sInDate).TotalHours >= 8)
                                        )
                                        .ToList();
                                    if (ul != null)
                                    {
                                        foreach (usersLogs row in ul)
                                        {
                                            row.sOutDate = datenow;
                                            entity2.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            { //signOut
                                tmpObject = entity.usersLogs
                                    .Where(p => p.logId == newObject.logId)
                                    .FirstOrDefault();

                                tmpObject.logId = newObject.logId;
                                //  tmpObject.sInDate=newObject.sInDate;
                                tmpObject.sOutDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                //    tmpObject.posId=newObject.posId;
                                //  tmpObject.userId = newObject.userId;


                                entity.SaveChanges();

                                message = tmpObject.logId.ToString();
                            }
                            //  entity.SaveChanges();
                        }

                        return TokenManager.GenerateToken(message);
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }

                return TokenManager.GenerateToken(message);
            }

            //var re = Request;
            //var headers = re.Headers;
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
            //    usersLogs newObject = JsonConvert.DeserializeObject<usersLogs>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    if (newObject.posId == 0 || newObject.posId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.posId = id;
            //    }
            //    if (newObject.userId == 0 || newObject.userId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.userId = id;
            //    }



            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var locationEntity = entity.Set<usersLogs>();
            //            if (newObject.logId == 0 || newObject.logId == null)
            //            {
            //                // signIn

            //                newObject.sInDate =  coctrlr.AddOffsetTodate(DateTime.Now);


            //                locationEntity.Add(newObject);
            //                entity.SaveChanges();
            //                message = newObject.logId.ToString();
            //                //sign out old user

            //                using (incposdbEntities entity2 = new incposdbEntities())
            //                {
            //                    List<usersLogs> ul = new List<usersLogs>();
            //                    List<usersLogs>  locationE = entity2.usersLogs.ToList();
            //                    ul = locationE.Where(s => s.sOutDate == null &&
            //                   ( (DateTime.Now-(DateTime)s.sInDate).TotalHours>=24)).ToList();
            //                    if (ul != null)
            //                    {
            //                        foreach(usersLogs row in ul)
            //                        {
            //                            row.sOutDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                            entity2.SaveChanges();
            //                        }
            //                    }
            //                }

            //                }



            //            else
            //            {//signOut
            //                var tmpObject = entity.usersLogs.Where(p => p.logId == newObject.logId).FirstOrDefault();



            //                tmpObject.logId = newObject.logId;
            //               //  tmpObject.sInDate=newObject.sInDate;
            //                 tmpObject.sOutDate=  coctrlr.AddOffsetTodate(DateTime.Now);
            //             //    tmpObject.posId=newObject.posId;
            //              //  tmpObject.userId = newObject.userId;


            //                entity.SaveChanges();

            //                message = tmpObject.logId.ToString();
            //            }
            //          //  entity.SaveChanges();
            //        }
            //    }
            //    catch
            //    {
            //        message = "-1";
            //    }
            //}
            //return message;
        }

        public bool checkLogByID(long logId)
        {
            try
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var item = entity.usersLogs.Where(u => u.logId == logId).FirstOrDefault();
                    UpdateLastRequest(item);
                    //check if user change server date
                    if (item.sInDate > datenow)
                        return true;
                    if (item.sOutDate != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            //public string Delete(string token)long logId, long userId,bool final
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
                long logId = 0;
                long userId = 0;
                bool final = false;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "logId")
                    {
                        logId = long.Parse(c.Value);
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

                try
                {
                    if (final)
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            usersLogs objectDelete = entity.usersLogs.Find(logId);

                            entity.usersLogs.Remove(objectDelete);
                            message = entity.SaveChanges().ToString();

                            //   return TokenManager.GenerateToken(message);
                        }

                        return TokenManager.GenerateToken(message);
                    }
                    else
                    {
                        return TokenManager.GenerateToken("-2");
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            //var re = Request;
            //var headers = re.Headers;
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
            //                usersLogs objectDelete = entity.usersLogs.Find(logId);

            //                entity.usersLogs.Remove(objectDelete);
            //            message=    entity.SaveChanges();

            //                return message.ToString();
            //            }
            //        }
            //        catch
            //        {
            //            return "-1";
            //        }
            //    }
            //    return "-2";

            //}
            //else
            //    return "-3";
        }

        //////////////////////
        ///

        public string Save(usersLogs newObject)
        {
            //public string Save(string token)
            //string Object string newObject
            string message = "";

            if (newObject != null)
            {
                usersLogs tmpObject = null;

                try
                {
                    if (newObject.posId == 0 || newObject.posId == null)
                    {
                        Nullable<long> id = null;
                        newObject.posId = id;
                    }
                    if (newObject.userId == 0 || newObject.userId == null)
                    {
                        Nullable<long> id = null;
                        newObject.userId = id;
                    }
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<usersLogs>();
                        if (newObject.logId == 0 || newObject.logId == null)
                        {
                            // signIn
                            // sign out old
                            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                            using (incposdbEntities entity2 = new incposdbEntities())
                            {
                                List<usersLogs> ul = new List<usersLogs>();
                                List<usersLogs> locationE = entity2.usersLogs.ToList();
                                ul = locationE
                                    .Where(
                                        s =>
                                            s.sOutDate == null
                                                && ((datenow - (DateTime)s.sInDate).TotalHours >= 8)
                                            || (s.userId == newObject.userId && s.sOutDate == null)
                                    )
                                    .ToList();
                                if (ul != null)
                                {
                                    foreach (usersLogs row in ul)
                                    {
                                        row.sOutDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                        entity2.SaveChanges();
                                    }
                                }
                            }
                            newObject.sInDate = coctrlr.AddOffsetTodate(DateTime.Now);

                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.logId.ToString();
                            //sign out old user
                        }
                        else
                        { //signOut
                            tmpObject = entity.usersLogs
                                .Where(p => p.logId == newObject.logId)
                                .FirstOrDefault();

                            tmpObject.logId = newObject.logId;
                            //  tmpObject.sInDate=newObject.sInDate;
                            tmpObject.sOutDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            //    tmpObject.posId=newObject.posId;
                            //  tmpObject.userId = newObject.userId;


                            entity.SaveChanges();

                            message = tmpObject.logId.ToString();
                        }
                        //  entity.SaveChanges();
                    }

                    return message;
                }
                catch
                {
                    message = "0";
                    return message;
                }
            }
            else
            {
                return "0";
            }
        }

        public usersLogs GetByID(long logId)
        {
            usersLogs item = new usersLogs();

            if (logId > 0)
            {
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemlist = entity.usersLogs.Where(u => u.logId == logId).ToList();

                        item = itemlist
                            .Where(u => u.logId == logId)
                            .Select(
                                S =>
                                    new usersLogs
                                    {
                                        logId = S.logId,
                                        sInDate = S.sInDate,
                                        sOutDate = S.sOutDate,
                                        posId = S.posId,
                                        userId = S.userId,
                                    }
                            )
                            .FirstOrDefault();
                        return item;
                    }
                }
                catch (Exception ex)
                {
                    return item;
                }
            }
            else
            {
                return item;
            }
        }

        void timerFunction(object sender, EventArgs e)
        {
            try
            {
                SignOutErrorExit();
                TokenManager.deleteDirectoryFiles();
                BackupController backcntrlr = new BackupController();
                backcntrlr.autoBackup();
                // notificationTimer();
            }
            catch (Exception ex) { }
        }

        public void startapp()
        {
            int res = 0;
            try
            {
                logoutTimer = new System.Timers.Timer();
                logoutTimer.Interval = Repeattime;
                // logoutTimer.Interval = Repeattime;
                logoutTimer.Elapsed += timerFunction;

                logoutTimer.AutoReset = true; //to repeat timer
                logoutTimer.Enabled = true; // to start timer

                oneminuteTimer = new System.Timers.Timer();
                oneminuteTimer.Interval = oneMtime;
                // logoutTimer.Interval = Repeattime;
                oneminuteTimer.Elapsed += oneminuteTimerFunction;
                oneminuteTimer.AutoReset = true; //to repeat timer
                oneminuteTimer.Enabled = true; // to start timer
            }
            catch
            {
                // return 0;
            }
        }

        private void SignOutErrorExit()
        {
            int res = 0;
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<UsersRequest> soutlist = entity.UsersRequest.ToList();
                    soutlist = soutlist
                        .Where(
                            r =>
                                (
                                    coctrlr.AddOffsetTodate(DateTime.Now)
                                    - (DateTime)r.lastRequestDate.Value
                                ).Minutes >= maxIdleperiod
                                && r.sOutDate == null
                        )
                        .ToList();
                    foreach (UsersRequest urout in soutlist)
                    {
                        usersLogs userlogobj = entity.usersLogs
                            .Where(u => u.userId == urout.userId && u.sOutDate == null)
                            .ToList()
                            .LastOrDefault();

                        if (userlogobj.logId > 0)
                        {
                            userlogobj.sOutDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            urout.sOutDate = userlogobj.sOutDate;
                            //  Saverequest(urout);
                            entity.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public string Saverequest(UsersRequest newObject)
        {
            string res = "0";
            if (newObject != null)
            {
                try
                {
                    UsersRequest tmpObject = null;
                    if (newObject.userId == 0 || newObject.userId == null)
                    {
                        Nullable<int> id = null;
                        newObject.userId = id;
                    }

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<UsersRequest>();
                        if (newObject.UserRequestId == 0 || newObject.UserRequestId == null)
                        {
                            locationEntity.Add(newObject);
                            res = entity.SaveChanges().ToString();
                        }
                        else
                        { //signOut
                            tmpObject = entity.UsersRequest
                                .Where(p => p.userId == newObject.userId)
                                .FirstOrDefault();
                            tmpObject.sInDate = newObject.sInDate;
                            tmpObject.sOutDate = newObject.sOutDate;

                            tmpObject.lastRequestDate = newObject.lastRequestDate;

                            res = entity.SaveChanges().ToString();
                        }
                    }

                    return res;
                }
                catch (Exception ex)
                {
                    // return ex.ToString();
                    return "0";
                }
            }
            else
            {
                return "0";
            }
        }

        public string UpdateLastRequest(usersLogs item)
        {
            UsersRequest urModel = new UsersRequest();
            string res = "0";
            List<UsersRequest> reqlist = new List<UsersRequest>();
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    reqlist = entity.UsersRequest.Where(r => r.userId == item.userId).ToList();
                }

                if (reqlist.Count() == 0)
                {
                    //new user in list

                    urModel.userId = item.userId;
                    urModel.sInDate = item.sInDate;
                    urModel.sOutDate = item.sOutDate;
                    urModel.lastRequestDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    res = Saverequest(urModel);
                    //entity.UsersRequest.Add(urModel);
                    //entity.SaveChanges();
                }
                else
                {
                    //update
                    urModel = reqlist.FirstOrDefault();
                    urModel.sInDate = item.sInDate;
                    urModel.sOutDate = item.sOutDate;
                    urModel.lastRequestDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    res = Saverequest(urModel);
                }
                return res;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        public void notificationTimer()
        {
            setValues setValuesModel = new setValues();
            NotificationController notctrlr = new NotificationController();
            setValuesController svalctrlr = new setValuesController();
            DateTime datetoday = coctrlr.AddOffsetTodate(DateTime.Now);
            setValuesModel = svalctrlr.GetRowBySettingName("isAlertDone");
            try
            {
                DateTime Lastdate = Convert.ToDateTime(setValuesModel.value);
                //    DateTime Lastdate= DateTime.Parse();
                if (datetoday.Date > Lastdate.Date)
                {
                    // notctrlr.addExpiredAlert();
                    setValuesModel.value = datetoday.Date.ToString("yyyy-MM-dd");
                    svalctrlr.Save(setValuesModel);
                }
            }
            catch (Exception ex)
            {
                setValuesModel.value = datetoday.Date.ToString("yyyy-MM-dd");
                svalctrlr.Save(setValuesModel);
            }
        }

        public async void oneminuteTimerFunction(object sender, EventArgs e)
        {
            try
            {
                StatisticsController sts = new StatisticsController();
                await sts.MakeStatement();
            }
            catch { }
        }
    }
}
