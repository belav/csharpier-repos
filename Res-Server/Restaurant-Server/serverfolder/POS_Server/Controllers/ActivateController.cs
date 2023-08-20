using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Activate")]
    public class ActivateController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        public string ServerID()
        {
            string deviceCode = "";
            // deviceCode = GetMotherBoardID() + "-" + GetHDDSerialNo();
            deviceCode = GetHDDSerialNo();
            return deviceCode;
        }

        public static string GetMotherBoardID()
        {
            string mbInfo = String.Empty;
            ManagementScope scope = new ManagementScope(
                "\\\\" + Environment.MachineName + "\\root\\cimv2"
            );
            scope.Connect();
            ManagementObject wmiClass = new ManagementObject(
                scope,
                new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""),
                new ObjectGetOptions()
            );

            foreach (PropertyData propData in wmiClass.Properties)
            {
                if (propData.Name == "SerialNumber")
                    mbInfo = Convert.ToString(propData.Value);
            }

            return mbInfo;
        }

        public static String GetHDDSerialNo()
        {
            string systemLogicalDiskDeviceId = Environment
                .GetFolderPath(Environment.SpecialFolder.System)
                .Substring(0, 2);

            // Start by enumerating the logical disks
            using (
                var searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_LogicalDisk WHERE DeviceID='"
                        + systemLogicalDiskDeviceId
                        + "'"
                )
            )
            {
                foreach (ManagementObject logicalDisk in searcher.Get())
                foreach (
                    ManagementObject partition in logicalDisk.GetRelated("Win32_DiskPartition")
                )
                foreach (ManagementObject diskDrive in partition.GetRelated("Win32_DiskDrive"))
                    return diskDrive["SerialNumber"].ToString();
            }

            return null;
        }

        //public async Task<SendDetail> GetSerialsAndDetails(string packageSaleCode, string customerServerCode, string packState)
        //{
        //    SendDetail item = new SendDetail();
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();

        //    parameters.Add("activeState", packState);
        //    parameters.Add("packageSaleCode", packageSaleCode);
        //    parameters.Add("customerServerCode", customerServerCode);

        //    //#################
        //    IEnumerable<Claim> claims = await APIResult.getList("packageUser/ActivateServer", parameters);

        //    foreach (Claim c in claims)
        //    {
        //        if (c.Type == "scopes")
        //        {
        //            item = JsonConvert.DeserializeObject<SendDetail>(c.Value, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

        //        }
        //    }
        //    return item;

        //}

        public async Task<SendDetail> GetSerialsAndDetails(
            string packageSaleCode,
            string customerServerCode,
            packagesSend packState
        )
        {
            SendDetail item = new SendDetail();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var myContent = JsonConvert.SerializeObject(packState);
            parameters.Add("packState", myContent);
            parameters.Add("packageSaleCode", packageSaleCode);
            parameters.Add("customerServerCode", customerServerCode);

            //#################
            IEnumerable<Claim> claims = await APIResult.getList(
                "packageUser/ActivateServerState",
                parameters
            );

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<SendDetail>(
                        c.Value,
                        new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }
                    );
                }
            }
            return item;
        }

        public async Task<string> GetSerialsAndDetails2(
            string packageSaleCode,
            string customerServerCode,
            packagesSend packState
        )
        {
            string item = "0";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var myContent = JsonConvert.SerializeObject(packState);
            parameters.Add("packState", myContent);
            parameters.Add("packageSaleCode", packageSaleCode);
            parameters.Add("customerServerCode", customerServerCode);

            //#################
            IEnumerable<Claim> claims = await APIResult.getList(
                "packageUser/ActivateServerState",
                parameters
            );

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = c.Value;
                }
            }
            return item;
        }

        public List<PosSerialSend> getserialsinfo()
        {
            SendDetail sd = new SendDetail();
            packagesSend packs = new packagesSend();
            List<PosSerialSend> serialsendList = new List<PosSerialSend>();

            using (incposdbEntities entity = new incposdbEntities())
            {
                serialsendList = (
                    from PS in entity.posSetting
                    join S in entity.posSerials on PS.posSerialId equals S.id
                    //  join p in entity.posSetting on S.id equals p.posSerialId
                    where PS.posSerialId != null
                    select new PosSerialSend
                    {
                        serial = S.posSerial,
                        isActive = (S.isActive == true) ? 1 : 0,
                        posName = PS.pos.name,
                        branchName = PS.pos.branches.name,
                        posId = PS.posId,
                        posSettingId = PS.posSettingId,
                        //  isBooked = S.posSetting.Where(x => x.posSerialId == S.id).ToList().Count > 0 ? true : false,
                        isBooked = (PS.posSerialId == 0 || PS.posSerialId == null) ? false : true,
                        posDeviceCode = PS.posDeviceCode,
                    }
                ).ToList();
            }

            return serialsendList;
        }

        public packagesSend getpackinfo()
        {
            packagesSend packs = new packagesSend();

            using (incposdbEntities entity = new incposdbEntities())
            {
                packs = (
                    from p in entity.ProgramDetails
                    //  join p in entity.posSetting on S.id equals p.posSerialId
                    select new packagesSend
                    {
                        programName = p.programName,
                        branchCount = p.branchCount,
                        posCount = p.posCount,
                        userCount = p.userCount,
                        vendorCount = p.vendorCount,
                        customerCount = p.customerCount,
                        itemCount = p.itemCount,
                        salesInvCount = p.saleinvCount,
                        storeCount = p.storeCount,
                        packageSaleCode = p.packageSaleCode,
                        customerServerCode = p.customerServerCode,
                        expireDate = p.expireDate,
                        isOnlineServer = p.isOnlineServer,
                        // updateDate = p.updateDate,
                        islimitDate = (p.isLimitDate == true) ? true : false,
                        //  isLimitCount = (bool)p.isLimitCount,
                        isActive = (p.isActive == true) ? 1 : 0,
                        canRenew = false,
                        isPayed = true,
                        isServerActivated = p.isServerActivated,
                        activatedate = p.activatedate,
                        pId = p.pId,
                        pcdId = p.pcdId,
                        bookDate = p.bookDate,
                        customerName = p.customerName,
                        customerLastName = p.customerLastName,
                        agentName = p.agentName,
                        agentLastName = p.agentLastName,
                        agentAccountName = p.agentAccountName,
                        pocrDate = p.pocrDate,
                        poId = p.poId,
                        upnum = p.upnum,
                        notes = p.notes,
                        verName = p.versionName,
                        packageNumber = p.packageNumber,
                        packageName = p.packageName,
                        isDemo = p.isDemo,
                    }
                ).FirstOrDefault();
            }

            return packs;
        }

        public SendDetail getinfo()
        {
            SendDetail sd = new SendDetail();
            packagesSend packs = new packagesSend();
            List<PosSerialSend> serialsendList = new List<PosSerialSend>();

            using (incposdbEntities entity = new incposdbEntities())
            {
                //serialsendList = (from S in entity.posSerials
                //                      //  join p in entity.posSetting on S.id equals p.posSerialId
                //                  select new PosSerialSend
                //                  {
                //                      serial = S.posSerial,
                //                      isActive = (S.isActive == true) ? 1 : 0,
                //                      // isBooked=true,

                //                      isBooked = S.posSetting.Where(x => x.posSerialId == S.id).ToList().Count > 0 ? true : false,

                //                      posDeviceCode = S.posSetting.Where(x => x.posSerialId == S.id).FirstOrDefault().posDeviceCode,
                //                  }).ToList();
                serialsendList = getserialsinfo();
                packs = (
                    from p in entity.ProgramDetails
                    //  join p in entity.posSetting on S.id equals p.posSerialId
                    select new packagesSend
                    {
                        programName = p.programName,
                        branchCount = p.branchCount,
                        posCount = p.posCount,
                        userCount = p.userCount,
                        vendorCount = p.vendorCount,
                        customerCount = p.customerCount,
                        itemCount = p.itemCount,
                        salesInvCount = p.saleinvCount,
                        storeCount = p.storeCount,
                        packageSaleCode = p.packageSaleCode,
                        customerServerCode = p.customerServerCode,
                        expireDate = p.expireDate,
                        isOnlineServer = p.isOnlineServer,
                        //  updateDate = p.updateDate,
                        islimitDate = (p.isLimitDate == true) ? true : false,
                        //  isLimitCount = (bool)p.isLimitCount,
                        isActive = (p.isActive == true) ? 1 : 0,
                        canRenew = false,
                        isPayed = true,
                        isServerActivated = p.isServerActivated,
                        activatedate = p.activatedate,
                        pId = p.pId,
                        pcdId = p.pcdId,
                        bookDate = p.bookDate,
                        pocrDate = p.pocrDate,
                        poId = p.poId,
                        customerName = p.customerName,
                        customerLastName = p.customerLastName,
                        agentName = p.agentName,
                        agentLastName = p.agentLastName,
                        agentAccountName = p.agentAccountName,
                        packageName = p.packageName,
                        isDemo = p.isDemo,
                    }
                ).FirstOrDefault();
            }
            sd.packageSend = packs;

            sd.PosSerialSendList = serialsendList;
            return sd;
        }

        public async Task<string> SendCustDetail(SendDetail sdd)
        {
            string message = "";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //var myContent = JsonConvert.SerializeObject(sd);
            //parameters.Add("pks", myContent);
            //var myContent2 = JsonConvert.SerializeObject(sr);
            //parameters.Add("psr", myContent2);
            var myContent3 = JsonConvert.SerializeObject(sdd);
            parameters.Add("object", myContent3);

            //#################
            IEnumerable<Claim> claims = await APIResult.getList(
                "packageUser/SendCustDetail",
                parameters
            );
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    //    user = JsonConvert.DeserializeObject<User>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    message = c.Value;

                    break;
                }
            }
            return message;
        }

        private int SaveProgDetails(packagesSend newObject)
        {
            int message = 0;
            if (newObject != null)
            {
                ProgramDetails tmpObject;

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<ProgramDetails>();

                        // List<ProgramDetails> Objectlist = entity.ProgramDetails.ToList();
                        tmpObject = entity.ProgramDetails.FirstOrDefault();

                        if (tmpObject != null)
                        {
                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.programName = newObject.programName;
                            tmpObject.branchCount = newObject.branchCount;
                            tmpObject.posCount = newObject.posCount;
                            tmpObject.userCount = newObject.userCount;
                            tmpObject.vendorCount = newObject.vendorCount;
                            tmpObject.customerCount = newObject.customerCount;
                            tmpObject.itemCount = newObject.itemCount;

                            //customer
                            tmpObject.saleinvCount = newObject.salesInvCount;

                            //if (newObject.salesInvCount == -1)
                            //{
                            //    //new is unlimited
                            //    tmpObject.saleinvCount = newObject.salesInvCount;
                            //}
                            //else
                            //{
                            //    //new is limited
                            //    if (tmpObject.saleinvCount == -1)
                            //    {
                            //        //old is unlimited
                            //        tmpObject.saleinvCount = newObject.totalsalesInvCount;
                            //    }
                            //    else
                            //    {
                            //        //old is limited
                            //        tmpObject.saleinvCount += newObject.totalsalesInvCount;
                            //    }



                            //}

                            tmpObject.versionName = newObject.verName;
                            tmpObject.storeCount = newObject.storeCount;

                            tmpObject.packageSaleCode = newObject.packageSaleCode;

                            tmpObject.customerServerCode = newObject.customerServerCode; // from function

                            tmpObject.expireDate = newObject.expireDate;
                            tmpObject.isOnlineServer = newObject.isOnlineServer;
                            tmpObject.isLimitDate = newObject.islimitDate;
                            tmpObject.isActive = (newObject.isActive == 1) ? true : false;

                            tmpObject.packageNumber = newObject.packageNumber;
                            tmpObject.customerName = newObject.customerName;
                            //  tmpObject.isLimitCount = newObject.isLimitCount;
                            tmpObject.customerLastName = newObject.customerLastName;
                            tmpObject.agentName = newObject.agentName;
                            tmpObject.agentLastName = newObject.agentLastName;
                            tmpObject.agentAccountName = newObject.agentAccountName;
                            tmpObject.pId = newObject.pId;
                            tmpObject.pcdId = newObject.pcdId;
                            tmpObject.bookDate = newObject.bookDate;
                            tmpObject.packageName = newObject.packageName;
                            tmpObject.pocrDate = newObject.pocrDate;
                            tmpObject.poId = newObject.poId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.upnum = newObject.upnum;

                            tmpObject.isServerActivated = newObject.isServerActivated;
                            tmpObject.activatedate = newObject.activatedate;
                            tmpObject.isDemo = newObject.isDemo;
                        }
                        else
                        {
                            message = -1;
                        }

                        //  tmpObject.posSerial = newObject.posSerial;      public Nullable<int> docPapersizeId { get; set; }

                        message = entity.SaveChanges();

                        //  entity.SaveChanges();
                        //   return (message);
                    }
                    return (message);
                }
                catch
                {
                    message = -1;
                    return (message);
                    //  return (ex.ToString());
                }
            }
            else
            {
                return (-1);
            }
        }

        private int updateActiveKey(packagesSend newObject)
        {
            int message = 0;
            if (newObject != null)
            {
                ProgramDetails tmpObject;
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<ProgramDetails>();

                        // List<ProgramDetails> Objectlist = entity.ProgramDetails.ToList();
                        tmpObject = entity.ProgramDetails.FirstOrDefault();

                        if (tmpObject != null)
                        {
                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.packageSaleCode = newObject.packageSaleCode;
                        }
                        else
                        {
                            message = -1;
                        }

                        message = entity.SaveChanges();
                    }
                    return (message);
                }
                catch
                {
                    message = -1;
                    return (message);
                    //  return (ex.ToString());
                }
            }
            else
            {
                return (-1);
            }
        }

        //private int SaveposSerials(List<PosSerialSend> newObjectlist)
        //{
        //    int message = 0;
        //    if (newObjectlist != null)
        //    {
        //        List<posSerials> poslist = new List<posSerials>();
        //        List<string> newserial = new List<string>();
        //        List<string> oldserial = new List<string>();
        //        foreach (PosSerialSend sendrow in newObjectlist)
        //        {

        //            newserial.Add(sendrow.serial);

        //            // poslist.Except
        //        }




        //        try
        //        {
        //            using (incposdbEntities entity = new incposdbEntities())
        //            {
        //                //  var locationEntity = entity.Set<posSerials>();
        //                List<posSerials> alllist = entity.posSerials.ToList();

        //                oldserial = alllist.Select(s => s.posSerial).ToList();
        //                List<string> finallist = new List<string>();
        //                // for no duplicate
        //                finallist = newserial.Except(oldserial).ToList();
        //                foreach (string sendrow in finallist)
        //                {
        //                    posSerials positem = new posSerials();
        //                    positem.posSerial = sendrow;

        //                    poslist.Add(positem);

        //                }


        //                entity.posSerials.AddRange(poslist);


        //                message = entity.SaveChanges();



        //            }
        //            return (message);

        //        }
        //        catch
        //        {
        //            message = -1;
        //            return (message);
        //        }

        //    }
        //    else
        //    {
        //        return (-1);
        //    }

        //}

        private int SaveposSerials(List<PosSerialSend> newObjectlist)
        {
            int message = 0;
            if (newObjectlist != null)
            {
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //  var locationEntity = entity.Set<posSerials>();
                        List<posSerials> alllist = entity.posSerials.ToList();
                        //1-dis activate serials
                        foreach (posSerials oldrow in alllist)
                        {
                            oldrow.isActive = false;

                            message += entity.SaveChanges();
                        }

                        foreach (PosSerialSend snewrow in newObjectlist)
                        {
                            bool exist = false;
                            foreach (posSerials oldrow in alllist)
                            {
                                if (oldrow.posSerial == snewrow.serial)
                                {
                                    exist = true;
                                    oldrow.isActive = true;
                                }
                            }
                            if (exist == false)
                            {
                                posSerials newsr = new posSerials();
                                newsr.isActive = true;
                                newsr.posSerial = snewrow.serial;
                                entity.posSerials.Add(newsr);
                            }
                            message += entity.SaveChanges();
                        }

                        //   message += entity.SaveChanges();
                    }
                    return (message);
                }
                catch
                {
                    message = -1;
                    return (message);
                }
            }
            else
            {
                return (-1);
            }
        }

        private int SaveunlimitedSerials(List<PosSerialSend> newObjectlist)
        {
            int message = 0;
            if (newObjectlist != null)
            {
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //  var locationEntity = entity.Set<posSerials>();
                        List<posSerials> alllist = entity.posSerials.ToList();
                        //1-dis activate serials
                        foreach (posSerials oldrow in alllist)
                        {
                            oldrow.isActive = false;

                            message += entity.SaveChanges();
                        }

                        // get unlimited serial{
                        PosSerialSend unlimitedser = new PosSerialSend();
                        unlimitedser = newObjectlist.Where(x => x.unLimited == true).First();
                        //get booked pos and serialId
                        if (unlimitedser.serial != null || unlimitedser.serial != "")
                        {
                            long unlimitedserialId = 0;
                            List<PosSerialSend> linkdpos = getserialsinfo();
                            linkdpos = linkdpos.Where(x => x.isBooked == true).ToList();
                            //add unlimited serial
                            foreach (PosSerialSend snewrow in newObjectlist)
                            {
                                bool exist = false;
                                foreach (posSerials oldrow in alllist)
                                {
                                    if (
                                        oldrow.posSerial == snewrow.serial
                                        && snewrow.unLimited == true
                                    )
                                    {
                                        exist = true;
                                        oldrow.isActive = true;
                                        oldrow.notes = "1";
                                        unlimitedserialId = oldrow.id;
                                    }
                                }
                                if (exist == false)
                                {
                                    posSerials newsr = new posSerials();
                                    newsr.isActive = true;
                                    newsr.posSerial = snewrow.serial;
                                    newsr.notes = "1";
                                    entity.posSerials.Add(newsr);
                                    unlimitedserialId = newsr.id;
                                }
                                message += entity.SaveChanges();
                            }
                            //
                            // change serialId
                            foreach (PosSerialSend newrow in linkdpos)
                            {
                                long? posId = newrow.posId == null ? 0 : newrow.posId;
                                var posdb = entity.posSetting
                                    .Where(x => x.posId == posId)
                                    .FirstOrDefault();
                                posdb.posSerialId = unlimitedserialId;
                                entity.SaveChanges();
                            }
                            //
                        }

                        //   message += entity.SaveChanges();
                    }
                    return (message);
                }
                catch
                {
                    message = -1;
                    return (message);
                }
            }
            else
            {
                return (-1);
            }
        }

        //[HttpPost]
        //[Route("saveserials")]
        //public async Task<string> saveserials(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            SendDetail sd = new SendDetail();
        //            sd = getinfo();
        //            PosSerialSend ss = new PosSerialSend();
        //            ss.serial = "dNquBuW1qzgxbvA2";
        //            ss.isActive = 1;
        //            List<PosSerialSend> lsr = new List<PosSerialSend>();
        //            lsr = sd.PosSerialSendList.Skip(5).Take(10).ToList();
        //            lsr.Add(ss);
        //            // int res = sd.packageSend.salesInvCount;
        //            // string res = sd.PosSerialSendList.FirstOrDefault().serial;
        //            int res = SaveposSerials(lsr);
        //            // int res=  await   checkIncServerConn();
        //            return TokenManager.GenerateToken(res.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            return TokenManager.GenerateToken(ex.ToString());
        //        }


        //    }
        //}

        // GET api/<controller>

        [HttpPost]
        [Route("checkconn")]
        public string checkconn(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                {
                    int id = 0;
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "id")
                        {
                            id = int.Parse(c.Value);
                        }
                    }
                    if (id == 1)
                    {
                        return TokenManager.GenerateToken(2.ToString());
                    }
                    else
                    {
                        return TokenManager.GenerateToken(0.ToString());
                    }
                }
            }
        }

        //[HttpPost]
        //[Route("Sendserverkey")]
        //public async Task<string> Sendserverkey(string token)
        //{
        //    getIncSite();
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        string res1 = "";
        //        string skey = "";
        //        string activeState = "";
        //        string serverId = "";
        //        SendDetail sendDetailItem = new SendDetail();
        //        int res = 0;
        //        int tempres = 0;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "skey")
        //            {
        //                skey = c.Value;
        //            }
        //            else if (c.Type == "activeState")
        //            {
        //                activeState = c.Value;
        //            }



        //        }
        //        try
        //        {
        //            //  return TokenManager.GenerateToken("1212".ToString());
        //            serverId = ServerID();
        //            // serverId = "server13213ascas";


        //            int conres = await checkIncServerConn();

        //            // check con to increase server
        //            if (conres > 0)
        //            {
        //                // return TokenManager.GenerateToken(conres.ToString());
        //                sendDetailItem = await GetSerialsAndDetails(skey, serverId, "");
        //                //update server detail

        //                if (sendDetailItem.packageSend.result <= 0)
        //                {

        //                    /*
        //                     *   // -2 : package not active

        //                         // -3 :serverID not match
        //                         // -4 :not payed
        //                         // -5 :serial not found
        //                         //"0" :  catch error


        //                     * */
        //                    res = sendDetailItem.packageSend.result;
        //                }
        //                else
        //                {
        //                    sendDetailItem.packageSend.customerServerCode = serverId;
        //                    sendDetailItem.packageSend.packageSaleCode = skey;
        //                    tempres = SaveProgDetails(sendDetailItem.packageSend);
        //                    //    return TokenManager.GenerateToken(res1);
        //                    //update serials
        //                    if (tempres >= 0)
        //                    {
        //                        res += 1;
        //                        tempres = 0;
        //                        //if (sendDetailItem.packageSend.posCount==-1)
        //                        //{
        //                        //    //unlimited pos

        //                        //    tempres = SaveunlimitedSerials(sendDetailItem.PosSerialSendList);
        //                        //}

        //                        tempres = SaveposSerials(sendDetailItem.PosSerialSendList);


        //                    }
        //                    if (tempres >= 0)
        //                    {
        //                        res += 1;
        //                    }
        //                    else
        //                    {
        //                        // activation error
        //                        res = 0;
        //                    }


        //                    //here send data to inc server
        //                    SendDetail sd = new SendDetail();
        //                    sd = getinfo();

        //                    string sendres = await SendCustDetail(sd);

        //                }
        //            }
        //            else
        //            {
        //                // connection error
        //                res = -1;
        //            }




        //            return TokenManager.GenerateToken(res);
        //        }
        //        catch (Exception ex)
        //        {
        //            // connection error
        //            // return TokenManager.GenerateToken(-1);
        //            return TokenManager.GenerateToken(ex.ToString());
        //        }
        //    }

        //}

        public async Task<int> checkIncServerConn()
        {
            long id = 1;
            int item = 0;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("id", id.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList(
                "packageUser/checkconn",
                parameters
            );

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = int.Parse(c.Value);
                    break;
                }
            }
            return item;
        }

        int count = 0;

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                int message = 0;

                ProgramDetails tmpObject = new ProgramDetails();
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var locationEntity = entity.Set<ProgramDetails>();

                    tmpObject = entity.ProgramDetails.FirstOrDefault();

                    if (tmpObject != null)
                    {
                        tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        count++;
                        tmpObject.itemCount = count;
                    }
                    else
                    {
                        message = -1;
                    }

                    message = entity.SaveChanges();
                }
            }
            catch { }
        }

        public int periodTimer()
        {
            try
            {
                System.Timers.Timer t = new System.Timers.Timer(10000);

                t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);

                t.Start();

                return 1;

                /*
                 *Global.asax.cs  add next code in  protected void Application_Start()
           //  ActivateController us = new ActivateController();
          //  int s = 0;
          //s=  us.periodTimer();
                 * */
            }
            catch
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("CheckPeriod")]
        public string CheckPeriod(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                int res = CheckPeriod();
                return TokenManager.GenerateToken(res.ToString());
            }
        }

        public int CheckPeriod()
        {
            ProgramDetails tmpObject;
            // 1 :  time not end-
            //  0 : time is end

            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var locationEntity = entity.Set<ProgramDetails>();
                    tmpObject = entity.ProgramDetails.FirstOrDefault();
                    if (tmpObject != null)
                    {
                        if (tmpObject.isLimitDate == false)
                        {
                            return 1;
                        }
                        else
                        { // limited
                            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                            if (tmpObject.expireDate <= datenow || tmpObject.expireDate == null)
                            {
                                return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        [HttpPost]
        [Route("senddata")]
        public async Task<string> senddata(string token)
        {
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
                    SendDetail sd = new SendDetail();
                    sd = getinfo();
                    // int res = sd.packageSend.salesInvCount;
                    // string res = sd.PosSerialSendList.FirstOrDefault().serial;
                    string res = await SendCustDetail(sd);
                    // int res=  await   checkIncServerConn();
                    return TokenManager.GenerateToken(res);
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        //[HttpPost]
        //[Route("sendserials")]
        //public async Task<string> sendserials(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            List<PosSerialSend> sd = new List<PosSerialSend>();
        //            sd = getserialsinfo();
        //            // int res = sd.packageSend.salesInvCount;
        //            // string res = sd.PosSerialSendList.FirstOrDefault().serial;
        //          //  string res = await SendCustDetail(sd);
        //            // int res=  await   checkIncServerConn();
        //            return TokenManager.GenerateToken(sd);
        //        }
        //        catch (Exception ex)
        //        {
        //            return TokenManager.GenerateToken(ex.ToString());
        //        }


        //    }
        //}
        // get increase site
        public string getIncSite()
        {
            string uri = "";
            setValuesController ctrObject = new setValuesController();
            uri = ctrObject.GetBySettingName("active_site");
            //  Global.APIUri = tb_serverUri.Text + @"/api/";
            uri = uri + @"/api/";
            APIResult.APIUri = uri;
            return uri;
        }

        // get state then activate
        [HttpPost]
        [Route("StatSendserverkey")]
        public async Task<string> StatSendserverkey(string token)
        {
            getIncSite();
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                //  APIResult.APIUri = "ssxs";

                string res1 = "";
                string skey = "";
                string activeState = "";
                string activeSite = "";
                string serverId = "";
                SendDetail sendDetailItem = new SendDetail();
                int res = 0;
                int tempres = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "skey")
                    {
                        skey = c.Value;
                    }
                    else if (c.Type == "activeState")
                    {
                        activeState = c.Value;
                    }
                }

                try
                {
                    //  return TokenManager.GenerateToken("1212".ToString());
                    serverId = ServerID();
                    // serverId = "server13213ascas";


                    int conres = await checkIncServerConn();

                    // check con to increase server
                    if (conres > 0)
                    {
                        packagesSend packState = new packagesSend();
                        packState = getpackinfo();
                        if (packState.isServerActivated == false)
                        {
                            packState.customerServerCode = serverId;
                        }
                        packState.activeState = activeState;

                        // return TokenManager.GenerateToken(conres.ToString());
                        //packState=up:upgrade - rn:renew


                        sendDetailItem = await GetSerialsAndDetails(skey, serverId, packState); //no coment
                        //  string mm =await GetSerialsAndDetails2(skey, serverId, packState);

                        //   return TokenManager.GenerateToken(sendDetailItem.packageSend.result.ToString());
                        //update server detail
                        res = sendDetailItem.packageSend.result;
                        // return TokenManager.GenerateToken(res);
                        //   return TokenManager.GenerateToken(sendDetailItem.packageSend.result);
                        if (sendDetailItem.packageSend.result <= 0)
                        {
                            //   return TokenManager.GenerateToken(sendDetailItem.packageSend.result.ToString());
                            /*
                             *   // -2 : package not active

                                 // -3 :serverID not match
                                 // -4 :not payed
                                 // -5 :serial not found
                             // -6 : package changed but not payed ==noch
                                 //"0" :  catch error
                                 // -7  method not match // online or offline
//-8->18 the current updat is newr than the offline update
//-9 the client command is different from activate file


                             * */
                            res = sendDetailItem.packageSend.result;
                        }
                        else
                        {
                            if (sendDetailItem.packageSend.result == 1)
                            {
                                sendDetailItem.packageSend.customerServerCode = serverId;
                                sendDetailItem.packageSend.packageSaleCode = skey;

                                tempres = SaveProgDetails(sendDetailItem.packageSend);

                                //    return TokenManager.GenerateToken(res1);
                                //update serials
                                if (tempres >= 0)
                                {
                                    res = 1;
                                    tempres = 0;
                                    //if (sendDetailItem.packageSend.posCount==-1)
                                    //{
                                    //    //unlimited pos

                                    //    tempres = SaveunlimitedSerials(sendDetailItem.PosSerialSendList);
                                    //}

                                    tempres = SaveposSerials(sendDetailItem.PosSerialSendList);
                                }
                                if (tempres >= 0)
                                {
                                    res = 1;
                                }
                                else
                                {
                                    // activation error
                                    res = 0;
                                }
                                //

                                //here send data to inc server
                                SendDetail sd = new SendDetail();
                                sd = getinfo();

                                string sendres = await SendCustDetail(sd);
                            }
                            else
                            {
                                // no change // dont save any thing
                            }
                        }
                    }
                    else
                    {
                        // connection error
                        res = -1;
                    }

                    //   return TokenManager.GenerateToken("44");
                    //
                    if (
                        (sendDetailItem.packageSend.activeres == "noch" && res > 0)
                        || sendDetailItem.packageSend.result == -6
                    )
                    {
                        //nochange
                        res = 2;
                    }
                    else if (
                        sendDetailItem.packageSend.activeres == "ch"
                        && sendDetailItem.packageSend.result > 0
                    )
                    {
                        //change
                        res = 3;
                    }
                    if (
                        sendDetailItem.packageSend.activeState == "all"
                        && sendDetailItem.packageSend.result > 0
                    )
                    {
                        //change
                        res = 3;
                    }

                    return TokenManager.GenerateToken(res);
                    //   return TokenManager.GenerateToken(res);
                }
                catch (Exception ex)
                {
                    // connection error
                    // return TokenManager.GenerateToken(-1);
                    return TokenManager.GenerateToken(ex.ToString());
                }
                //return TokenManager.GenerateToken(res);
            }
            // return TokenManager.GenerateToken("-11");

            //    return TokenManager.GenerateToken("-10");
        }

        //[HttpPost]
        //[Route("activesite")]
        //public async Task<string> activesite(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            string site = getIncSite();
        //            return TokenManager.GenerateToken(site);
        //        }
        //        catch (Exception ex)
        //        {
        //            return TokenManager.GenerateToken(ex.ToString());
        //        }


        //    }
        //}


        //
        [HttpPost]
        [Route("getactivesite")]
        public async Task<string> getactivesite(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                setValues sv = new setValues();
                try
                {
                    string uri = "";

                    setValuesController ctrObject = new setValuesController();
                    sv = ctrObject.GetRowBySettingName("active_site");
                    return TokenManager.GenerateToken(sv);
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("updatesalecode")]
        public async Task<string> updatesalecode(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string skey = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "skey")
                    {
                        skey = c.Value;
                    }
                }
                packagesSend ps = new packagesSend();
                int res = 0;
                try
                {
                    ps.packageSaleCode = skey;
                    res = updateActiveKey(ps);

                    return TokenManager.GenerateToken(res.ToString());
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("OfflineActivate")]
        public async Task<string> OfflineActivate(string token)
        {
            //  getIncSite();
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string activeState = "";

                SendDetail sendDetailItem = new SendDetail();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "object")
                    {
                        sendDetailItem = JsonConvert.DeserializeObject<SendDetail>(
                            c.Value,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "activeState")
                    {
                        activeState = c.Value;
                    }
                }
                try
                {
                    SendDetail returnsend = new SendDetail();
                    returnsend = ActivateOfflineState(sendDetailItem, activeState);
                    //  return TokenManager.GenerateToken(sendDetailItem.packageSend.activeres);
                    return TokenManager.GenerateToken(returnsend);
                }
                catch (Exception ex)
                {
                    // connection error
                    SendDetail senditem = new SendDetail();

                    packagesSend ps = new packagesSend();
                    ps.result = 0;
                    senditem.packageSend = ps;
                    return TokenManager.GenerateToken(senditem);
                }
            }
        }

        // same as inc server but run in customer server
        public SendDetail ActivateOfflineState(SendDetail SendDetailFile, string activeState)
        {
            string message = "";
            int res = 0;

            string packageSaleCode = "";
            string customerServerCode = "";
            //    activeState command from client ;

            packagesSend packState = new packagesSend();
            packagesSend packuserfile = new packagesSend();

            packageSaleCode = SendDetailFile.packageSend.packageSaleCode;
            //get server Id
            string serverId = "";
            serverId = ServerID();
            customerServerCode = serverId;

            packState = getpackinfo();
            packuserfile = SendDetailFile.packageSend;

            // SaveProgDetails(packuserfile);
            try
            {
                //  payOpModel lastpayrow = new payOpModel();
                List<PosSerialSend> serialList = new List<PosSerialSend>();
                serialList = SendDetailFile.PosSerialSendList;
                SendDetail senditem = new SendDetail();
                packagesSend package = new packagesSend();
                string activeres = "noch";

                using (incposdbEntities entity = new incposdbEntities())
                {
                    //get packageuser row
                    // List<packageUser> list = entity.packageUser.Where(u => u.packageSaleCode == packageSaleCode).ToList();
                    //  packuserrow = getPUbycode(packageSaleCode);
                    if (packuserfile.packageUserId > 0)
                    {
                        // get last payed row

                        if (packuserfile.poId > 0)
                        {
                            // check if same method metho is
                            if (
                                packuserfile.isOnlineServer == packState.isOnlineServer
                                || (
                                    packState.isServerActivated == false
                                    && packState.activatedate == null
                                )
                            )
                            {
                                // same method or first time

                                // check if current activate(upgrade or extend) is newr than the exist or first time
                                if (
                                    (
                                        (
                                            packuserfile.pocrDate > packState.pocrDate
                                            && packuserfile.poId != packState.poId
                                        )
                                        || (
                                            packState.isServerActivated == false
                                            && packState.activatedate == null
                                        )
                                    )
                                    || (
                                        packuserfile.pocrDate == packState.pocrDate
                                        && packuserfile.poId == packState.poId
                                        && packState.isServerActivated == false
                                        && packuserfile.isServerActivated == true
                                        && packState.expireDate == packuserfile.bookDate
                                    )
                                )
                                {
                                    if (
                                        packuserfile.activeState == activeState
                                        || (
                                            packState.isServerActivated == false
                                            && packState.activatedate == null
                                        )
                                        || (
                                            packuserfile.pocrDate == packState.pocrDate
                                            && packuserfile.poId == packState.poId
                                            && packState.isServerActivated == false
                                            && packuserfile.isServerActivated == true
                                            && packState.expireDate == packuserfile.bookDate
                                        )
                                    )
                                    {
                                        // check if the command is same as the activate file or first time

                                        //ssss
                                        // check if there are changes

                                        package.activeres = "noch";
                                        if (packuserfile.activeState == "up")
                                        {
                                            if (
                                                packState.pId != packuserfile.pId
                                                || (
                                                    packState.pId == packuserfile.pId
                                                    && packState.pcdId != packuserfile.pcdId
                                                )
                                            )
                                            {
                                                // changed
                                                package.activeres = "ch";
                                                activeres = "ch";
                                            }
                                            else
                                            {
                                                //no  changed
                                                package.activeres = "noch";
                                                activeres = "noch";
                                            }
                                        }
                                        else if (packuserfile.activeState == "rn")
                                        {
                                            if (
                                                packuserfile.type == "rn"
                                                && packuserfile.expireDate > packState.expireDate
                                            )
                                            {
                                                package.activeres = "ch";
                                                activeres = "ch";
                                            }
                                            else
                                            {
                                                package.activeres = "noch";
                                                activeres = "noch";
                                            }
                                        }
                                        // end check


                                        if (
                                            packuserfile.packuserType == "chpk"
                                            && packuserfile.isPayed == false
                                            && packuserfile.canRenew == false
                                        )
                                        {
                                            // chpk not payed yet
                                            // dont activate until pay
                                            // return TokenManager.GenerateToken("0");


                                            package = packState;
                                            package.activeres = activeres;

                                            package.result = -6; //  // -6 : package changed but not payed
                                            senditem.packageSend = package;

                                            return senditem;
                                        }
                                        else if (
                                            packState.isServerActivated == false
                                            || (
                                                packuserfile.isServerActivated == true
                                                && packuserfile.customerServerCode
                                                    == customerServerCode
                                            )
                                            || (
                                                packuserfile.pocrDate == packState.pocrDate
                                                && packuserfile.poId == packState.poId
                                                && packState.isServerActivated == false
                                                && packuserfile.isServerActivated == true
                                                && packState.expireDate == packuserfile.bookDate
                                            )
                                        ) //&&  row.expireDate==null
                                        {
                                            //get poserials


                                            List<string> serialposlist = new List<string>();

                                            //serialList = serialmodel.GetBypackageUserId(packuserrow.packageUserId);
                                            serialList = SendDetailFile.PosSerialSendList;
                                            // get package details

                                            //start
                                            // check if there are changes
                                            package.activeres = activeres;
                                            if (
                                                activeres == "ch"
                                                || (packuserfile.activeApp == "all")
                                                || (
                                                    packuserfile.pocrDate == packState.pocrDate
                                                    && packuserfile.poId == packState.poId
                                                    && packState.isServerActivated == false
                                                    && packuserfile.isServerActivated == true
                                                    && packState.expireDate == packuserfile.bookDate
                                                )
                                            )
                                            {
                                                //make changes
                                                // if(pack.isActive==1 && prog.isActive==1 && ver.isActive==1){
                                                package = packuserfile;
                                                //package.programName = packuserfile.programName;
                                                //package.verName = packuserfile.verName;
                                                //package.packageSaleCode = packuserfile.packageSaleCode;
                                                //package.expireDate = packuserfile.expireDate;
                                                //package.isOnlineServer = packuserfile.isOnlineServer;
                                                //package.packageNumber = packuserfile.packageNumber;
                                                //package.totalsalesInvCount = packuserfile.totalsalesInvCount;
                                                //package.packageName = packuserfile.packageName;
                                                ////packuserrow.countryPackageId
                                                //package.islimitDate = packuserfile.islimitDate;
                                                //                         //   SendDetail senditem = new SendDetail();
                                                //package.isServerActivated = packuserfile.isServerActivated;
                                                //package.customerName = packuserfile.customerName;
                                                //package.customerLastName = packuserfile.customerLastName;
                                                //package.agentAccountName = packuserfile.agentAccountName;
                                                //package.agentName = packuserfile.agentName;
                                                //package.agentLastName = packuserfile.agentLastName;
                                                //package.pId = packuserfile.pId;
                                                //package.pcdId = packuserfile.pcdId;
                                                //package.bookDate = packuserfile.bookDate;
                                                //package.type = packuserfile.type;


                                                //package.packageUserId = packuserfile.packageUserId;
                                                //package.packageName = packuserfile.notes;
                                                //package.pocrDate = packuserfile.pocrDate;
                                                //package.poId = packuserfile.poId;
                                                //package.upnum = "";

                                                package.packuserType = packuserfile.type;
                                                package.isActive = (int)packuserfile.isActive;

                                                package.result = 1;
                                                if (packState.isServerActivated == false)
                                                {
                                                    package.customerServerCode = serverId;
                                                    package.activatedate = coctrlr.AddOffsetTodate(
                                                        DateTime.Now
                                                    ); // save on client if null
                                                }

                                                // senditem.packageSend = package;
                                                //    senditem.PosSerialSendList = serialList;


                                                int tempres = SaveProgDetails(package);

                                                if (tempres >= 0)
                                                {
                                                    res = 1;
                                                    tempres = 0;
                                                    //if (sendDetailItem.packageSend.posCount==-1)
                                                    //{
                                                    //    //unlimited pos

                                                    //    tempres = SaveunlimitedSerials(sendDetailItem.PosSerialSendList);
                                                    //}

                                                    tempres = SaveposSerials(serialList);
                                                }
                                                if (tempres >= 0)
                                                {
                                                    res = 1;
                                                }
                                                else
                                                {
                                                    // activation error
                                                    res = 0;
                                                }
                                                //    return TokenManager.GenerateToken(senditem);
                                                // date tosend to inc program

                                                //  package.isServerActivated = true;

                                                //if (packState.activatedate == null)
                                                //{
                                                //    package.activatedate;
                                                //}

                                                // package.customerServerCode = customerServerCode;
                                                package.totalsalesInvCount = 0;

                                                //   package.canRenew = false;
                                                package.activeState = packuserfile.activeState;

                                                serialList = getserialsinfo();

                                                senditem.packageSend = package;
                                                senditem.PosSerialSendList = serialList;
                                                return senditem;
                                            }
                                            else
                                            {
                                                //nochange



                                                package = packState;
                                                package.activeres = activeres;

                                                package.result = 2; // no change
                                                package.pocrDate = packuserfile.pocrDate;
                                                package.poId = packuserfile.poId;
                                                package.upnum = "";

                                                senditem.packageSend = package;
                                                senditem.PosSerialSendList = serialList;
                                                return senditem;
                                            }

                                            //end
                                        }
                                        else
                                        {
                                            // serverID not match or package not active
                                            serialList = new List<PosSerialSend>();
                                            package = new packagesSend();

                                            senditem = new SendDetail();
                                            package.activeres = activeres;
                                            package.pocrDate = packuserfile.pocrDate;
                                            package.poId = packuserfile.poId;
                                            package.upnum = "";

                                            senditem.packageSend = package;
                                            senditem.PosSerialSendList = serialList;
                                            if (packuserfile.isActive != 1)
                                            {
                                                //package not active
                                                package.result = -2;
                                            }
                                            else if (
                                                !(
                                                    packuserfile.isServerActivated == false
                                                    || (
                                                        packuserfile.isServerActivated == true
                                                        && packuserfile.customerServerCode
                                                            == customerServerCode
                                                    )
                                                )
                                            )
                                            {
                                                // serverID not match
                                                package.result = -3;
                                            }

                                            senditem.packageSend = package;

                                            return senditem;

                                            //if (packuserrow.canRenew == true)
                                            //{
                                            //    // write code here
                                            //    //  return TokenManager.GenerateToken(senditem);
                                            //}
                                            //else
                                            //{

                                            //    packagesSend ps = new packagesSend();
                                            //    ps.posCount = -2;
                                            //    senditem.packageSend = ps;
                                            //    //senditem.packageSend.posCount = -2;
                                            //    //  return TokenManager.GenerateToken(senditem);

                                            //}
                                        }

                                        //   return TokenManager.GenerateToken(senditem);

                                        ///////ssssssss
                                        ///
                                    }
                                    else
                                    {
                                        // the client command is different from activate file
                                        package.result = -9;
                                        senditem.packageSend = package;
                                        return senditem;
                                    }
                                }
                                else
                                {
                                    // the current updat is newr than the offline update
                                    package.result = -18;
                                    senditem.packageSend = package;
                                    return senditem;
                                }
                            }
                            else
                            {
                                // method not match // online or offline
                                package.result = -7;
                                senditem.packageSend = package;

                                return senditem;
                            }
                        }
                        else
                        {
                            // not payed

                            serialList = new List<PosSerialSend>();
                            package = new packagesSend();

                            senditem = new SendDetail();
                            package.activeres = activeres;
                            package.pocrDate = packuserfile.pocrDate;
                            package.poId = packuserfile.poId;
                            package.upnum = "";

                            senditem.packageSend = package;
                            senditem.PosSerialSendList = serialList;

                            package.result = -4;

                            senditem.packageSend = package;

                            return senditem;
                        }
                    }
                    else
                    {
                        //serial not found
                        serialList = new List<PosSerialSend>();
                        package = new packagesSend();

                        senditem = new SendDetail();
                        package.activeres = activeres;
                        senditem.packageSend = package;
                        senditem.PosSerialSendList = serialList;

                        package.result = -5;

                        senditem.packageSend = package;

                        return senditem;

                        //senditem = new SendDetail();
                        //packagesSend ps = new packagesSend();
                        //ps.posCount = -3;
                        //senditem.packageSend = ps;

                        //// senditem.packageSend.posCount = -3;
                        //return TokenManager.GenerateToken(senditem);
                    }
                }
            }
            catch (Exception ex)
            {
                //error
                SendDetail senditem = new SendDetail();

                packagesSend ps = new packagesSend();
                ps.result = 0;
                senditem.packageSend = ps;

                return senditem;
            }
            //   return TokenManager.GenerateToken("00");
        }

        //[HttpPost]
        //[Route("offserialstest")]
        //public async Task<string> offserialstest(string token)
        //{
        //    //  getIncSite();
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {



        //        string activeState = "";

        //        SendDetail sendDetailItem = new SendDetail();



        //        try
        //        {
        //            SendDetail returnsend = new SendDetail();
        //            List<PosSerialSend> plis = new List<PosSerialSend>();
        //            //  sd = getinfo();


        //            plis = getserialsinfo();
        //            returnsend.PosSerialSendList = plis;

        //            return TokenManager.GenerateToken(returnsend);
        //        }
        //        catch (Exception ex)
        //        {
        //            // connection error
        //            SendDetail senditem = new SendDetail();

        //            packagesSend ps = new packagesSend();
        //            ps.result = 0;
        //            senditem.packageSend = ps;
        //            return TokenManager.GenerateToken(senditem);
        //        }
        //    }
        //}
    }
}
