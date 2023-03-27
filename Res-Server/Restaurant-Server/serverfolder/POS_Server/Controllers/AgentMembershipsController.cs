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
    [RoutePrefix("api/AgentMemberships")]
    public class AgentMembershipsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        //[HttpPost]
        //[Route("GetAll")]
        //public string GetAll(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    Boolean canDelete = false;
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var List = entity.agentMemberships.Select(S => new AgentMembershipsModel
        //            {
        //                agentMembershipsId = S.agentMembershipsId,

        //                membershipId = S.membershipId,
        //                agentId = S.agentId,

        //                notes = S.notes,
        //                createDate = S.createDate,
        //                updateDate = S.updateDate,
        //                createUserId = S.createUserId,
        //                updateUserId = S.updateUserId,
        //                isActive = S.isActive,


        //            })
        //            .ToList();


        //            return TokenManager.GenerateToken(List);

        //        }
        //    }
        //}
        /*
   
         * */
        //[HttpPost]
        //[Route("GetById")]
        //public string GetById(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        int agentMembershipsId = 0;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "itemId")
        //            {
        //                agentMembershipsId = int.Parse(c.Value);
        //            }
        //        }
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var bank = entity.agentMemberships
        //           .Where(S => S.agentMembershipsId == agentMembershipsId)
        //           .Select(S => new
        //           {
        //               S.agentMembershipsId,

        //               S.membershipId,
        //               S.agentId,

        //               S.notes,
        //               S.createDate,
        //               S.updateDate,
        //               S.createUserId,
        //               S.updateUserId,
        //               S.isActive,



        //           })
        //           .FirstOrDefault();
        //            return TokenManager.GenerateToken(bank);

        //        }
        //    }
        //}
        // [HttpPost]
        //[Route("GetAgentMemberShip")]
        //public string GetAgentMemberShip(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        long agentId = 0;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "agentId")
        //            {
        //                agentId = int.Parse(c.Value);
        //            }
        //        }
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var agentMemberShip = entity.agentMemberships
        //           .Where(S => S.agentId == agentId)
        //           .Select(S => new AgentMembershipsModel()
        //           {
        //               agentMembershipsId = S.agentMembershipsId,

        //               membershipId = S.membershipId,
        //               agentId = S.agentId,

        //               notes = S.notes,
        //               createDate = S.createDate,
        //               updateDate = S.updateDate,
        //               createUserId = S.createUserId,
        //               updateUserId = S.updateUserId,
        //               isActive = S.isActive,
        //               memberShip = entity.memberships.Where(x => x.membershipId == S.membershipId)
        //                                                .Select(x => new MembershipsModel()
        //                                                   {
        //                                                    membershipId = x.membershipId,
        //                                                   code = x.code,
        //                                                   createDate = x.createDate,
        //                                                   updateDate = x.updateDate,
        //                                                   isActive = x.isActive,
        //                                                   isFreeDelivery = x.isFreeDelivery,
        //                                                   deliveryDiscountPercent = x.deliveryDiscountPercent,
        //                                                   name = x.name,
        //                                                   notes=  x.notes,
        //                                                   subscriptionFee = x.subscriptionFees.FirstOrDefault().Amount,
        //                                                   subscriptionType = x.subscriptionType,
        //                                                   }).FirstOrDefault()
        //           }).FirstOrDefault();
        //            return TokenManager.GenerateToken(agentMemberShip);

        //        }
        //    }
        //}

        //[HttpPost]
        //[Route("Save")]
        //public string Save(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    string message = "";
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        string agentMembershipsId = "";
        //        agentMemberships newObject = null;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "itemObject")
        //            {
        //                agentMembershipsId = c.Value.Replace("\\", string.Empty);
        //                agentMembershipsId = agentMembershipsId.Trim('"');
        //                newObject = JsonConvert.DeserializeObject<agentMemberships>(agentMembershipsId, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
        //                break;
        //            }
        //        }
        //        if (newObject.updateUserId == 0 || newObject.updateUserId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.updateUserId = id;
        //        }
        //        if (newObject.createUserId == 0 || newObject.createUserId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.createUserId = id;
        //        }

        //        if (newObject.membershipId == 0 || newObject.membershipId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.membershipId = id;
        //        }
        //        if (newObject.agentId == 0 || newObject.agentId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.agentId = id;
        //        }
        //        try
        //        {
        //            using (incposdbEntities entity = new incposdbEntities())
        //            {
        //                agentMemberships tmpObject = new agentMemberships();
        //                var bankEntity = entity.Set<agentMemberships>();
        //                if (newObject.agentMembershipsId == 0)
        //                {
        //                    newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateUserId = newObject.createUserId;
        //                    tmpObject = bankEntity.Add(newObject);
        //                    entity.SaveChanges();
        //                    message = tmpObject.agentMembershipsId.ToString(); ;
        //                }
        //                else
        //                {
        //                    tmpObject = entity.agentMemberships.Where(p => p.agentMembershipsId == newObject.agentMembershipsId).FirstOrDefault();

        //                    tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

        //                    tmpObject.agentMembershipsId = newObject.agentMembershipsId;

        //                    tmpObject.membershipId = newObject.membershipId;
        //                    tmpObject.agentId = newObject.agentId;

        //                    tmpObject.notes = newObject.notes;
        //                  //  tmpObject.createDate = newObject.createDate;
        //                    tmpObject.updateDate = newObject.updateDate;
        //                    tmpObject.createUserId = newObject.createUserId;
        //                    tmpObject.updateUserId = newObject.updateUserId;
        //                    tmpObject.isActive = newObject.isActive;



        //                    entity.SaveChanges();
        //                    message = tmpObject.agentMembershipsId.ToString();

        //                }
        //                return TokenManager.GenerateToken(message);
        //            }
        //        }

        //        catch
        //        {
        //            message = "0";
        //            return TokenManager.GenerateToken(message);
        //        }
        //    }
        //}

        //[HttpPost]
        //[Route("Delete")]
        //public string Delete(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    string message = "";
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        int agentMembershipsId = 0;
        //        long userId = 0;
        //        Boolean final = false;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "itemId")
        //            {
        //                agentMembershipsId = int.Parse(c.Value);
        //            }
        //            else if (c.Type == "userId")
        //            {
        //                userId = long.Parse(c.Value);
        //            }
        //            else if (c.Type == "final")
        //            {
        //                final = bool.Parse(c.Value);
        //            }
        //        }
        //        if (final)
        //        {
        //            try
        //            {
        //                using (incposdbEntities entity = new incposdbEntities())
        //                {

        //                    agentMemberships objDelete = entity.agentMemberships.Find(agentMembershipsId);
        //                    entity.agentMemberships.Remove(objDelete);
        //                    message = entity.SaveChanges().ToString();
        //                    return TokenManager.GenerateToken(message);
        //                }
        //            }
        //            catch
        //            {
        //                return TokenManager.GenerateToken("0");
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                using (incposdbEntities entity = new incposdbEntities())
        //                {

        //                    agentMemberships objDelete = entity.agentMemberships.Find(agentMembershipsId);
        //                    objDelete.isActive = 0;
        //                    objDelete.updateUserId = userId;
        //                    objDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    message = entity.SaveChanges().ToString();
        //                    return TokenManager.GenerateToken(message);
        //                }
        //            }
        //            catch
        //            {
        //                return TokenManager.GenerateToken("0");
        //            }
        //        }
        //    }
        //}

        [HttpPost]
        [Route("UpdateAgentsByMembershipId")]
        public string UpdateAgentsByMembershipId(string token)
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
                string strObject = "";
                List<AgentMembershipsModel> newListObj = null;
                long membershipId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<AgentMembershipsModel>>(
                            strObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "membershipId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                    else if (c.Type == "updateUserId")
                    {
                        updateUserId = long.Parse(c.Value);
                    }
                }

                try
                {
                    AgentController agcont = new AgentController();
                    agcont.resetMembershipId(membershipId);
                    foreach (var row in newListObj)
                    {
                        agcont.UpdateMembershipId((int)row.agentId, (int)row.membershipId);
                    }

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

        /*
         *        [HttpPost]
        [Route("UpdateAgentsByMembershipId")]
        public string UpdateAgentsByMembershipId(string token)
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
                string strObject = "";
                List<agentMemberships> newListObj = null;
                long membershipId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<agentMemberships>>(strObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "membershipId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                    else
                  if (c.Type == "updateUserId")
                    {
                        updateUserId = long.Parse(c.Value);
                    }
                }

                List<agentMemberships> items = null;
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity.agentMemberships.Where(x => x.membershipId == membershipId).ToList();
                    if (items != null)
                    {
                        entity.agentMemberships.RemoveRange(items);
                        try
                        { entity.SaveChanges(); }
                        catch (Exception ex)
                        {
                            message = "-2";
                            return TokenManager.GenerateToken(message);
                        }
                    }


                }
                try
                {
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
                            if (newListObj[i].membershipId == 0 || newListObj[i].membershipId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].membershipId = id;
                            }
                            if (newListObj[i].agentId == 0 || newListObj[i].agentId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].agentId = id;
                            }
                         
                            var branchEntity = entity.Set<agentMemberships>();

                            newListObj[i].createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newListObj[i].updateDate = newListObj[i].createDate;
                            newListObj[i].updateUserId = updateUserId;
                            newListObj[i].membershipId = membershipId;
                            branchEntity.Add(newListObj[i]);
                            entity.SaveChanges();
                        }

                        entity.SaveChanges();


                    }



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
         * */
        //[HttpPost]
        //[Route("UpdateAgentsByMembershipId")]
        //public string UpdateAgentsByMembershipId(string token)
        //{
        //    token = TokenManager.readToken(HttpContext.Current.Request);
        //    string message = "";
        //    var strP = TokenManager.GetPrincipal(token);
        //    if (strP != "0") //invalid authorization
        //    {
        //        return TokenManager.GenerateToken(strP);
        //    }
        //    else
        //    {
        //        string strObject = "";
        //        List<agentMemberships> newListObj = null;
        //        long membershipId = 0;
        //        long updateUserId = 0;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "newList")
        //            {
        //                strObject = c.Value.Replace("\\", string.Empty);
        //                strObject = strObject.Trim('"');
        //                newListObj = JsonConvert.DeserializeObject<List<agentMemberships>>(strObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

        //            }
        //            else if (c.Type == "membershipId")
        //            {
        //                membershipId = long.Parse(c.Value);
        //            }
        //            else
        //          if (c.Type == "updateUserId")
        //            {
        //                updateUserId = long.Parse(c.Value);
        //            }
        //        }

        //        List<agentMemberships> olditems = null;
        //        List<agentMemberships> oldActiveitems = null;
        //        List<agentMemberships> newitems = null;
        //        agentMemberships tempob  = new agentMemberships();
        //        // delete old invoice items
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            olditems = entity.agentMemberships.Where(x => x.membershipId == membershipId).ToList();

        //            if (olditems != null)
        //            {
        //                //  entity.agentMemberships.RemoveRange(items);
        //           //     oldActiveitems = olditems.Where(M=>M.agentMembershipsId!= cont)
        //                try
        //                {
        //                    foreach (var oldrow in olditems)
        //                    {
        //                        oldActiveitems =  null;
        //                        oldActiveitems = newListObj.Where(M => M.agentMembershipsId == oldrow.agentMembershipsId).ToList();
        //                        if (oldActiveitems == null)
        //                        {
        //                            //isactive=0
        //                            oldrow.isActive = 0;
        //                            entity.SaveChanges();
        //                        }
        //                        else if(oldActiveitems.Count == 0)
        //                        {
        //                            //isactive=0
        //                            oldrow.isActive = 0;
        //                            entity.SaveChanges();

        //                        }



        //                    }


        //                }
        //                catch (Exception ex)
        //                {
        //                    message = "-2";
        //                    return TokenManager.GenerateToken(message);
        //                }
        //            }
        //            //add new items
        //            newitems = newListObj.Where(M => M.agentMembershipsId == 0).ToList();

        //            foreach (var newrow in newitems)
        //            {
        //                newrow.membershipId = membershipId;
        //                Save(newrow);


        //            }
        //            return TokenManager.GenerateToken("1");


        //        }


        //    }


        //}


        //public int Save(agentMemberships newObject)
        //{
        //    if (newObject != null)
        //    {

        //    int message = 0;
        //        if (newObject.updateUserId == 0 || newObject.updateUserId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.updateUserId = id;
        //        }
        //        if (newObject.createUserId == 0 || newObject.createUserId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.createUserId = id;
        //        }
        //        if (newObject.subscriptionFeesId == 0 || newObject.subscriptionFeesId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.subscriptionFeesId = id;
        //        }
        //        if (newObject.cashTransId == 0 || newObject.cashTransId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.cashTransId = id;
        //        }
        //        if (newObject.membershipId == 0 || newObject.membershipId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.membershipId = id;
        //        }
        //        if (newObject.agentId == 0 || newObject.agentId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.agentId = id;
        //        }
        //        try
        //        {
        //            using (incposdbEntities entity = new incposdbEntities())
        //            {
        //                agentMemberships tmpObject = new agentMemberships();
        //                var bankEntity = entity.Set<agentMemberships>();
        //                if (newObject.agentMembershipsId == 0)
        //                {
        //                    newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateUserId = newObject.createUserId;
        //                    tmpObject = bankEntity.Add(newObject);
        //                    entity.SaveChanges();
        //                    message = tmpObject.agentMembershipsId ;
        //                }
        //                else
        //                {
        //                    tmpObject = entity.agentMemberships.Where(p => p.agentMembershipsId == newObject.agentMembershipsId).FirstOrDefault();

        //                    tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

        //                    tmpObject.agentMembershipsId = newObject.agentMembershipsId;
        //                    tmpObject.subscriptionFeesId = newObject.subscriptionFeesId;
        //                    tmpObject.cashTransId = newObject.cashTransId;
        //                    tmpObject.membershipId = newObject.membershipId;
        //                    tmpObject.agentId = newObject.agentId;
        //                    tmpObject.startDate = newObject.startDate;
        //                    tmpObject.EndDate = newObject.EndDate;
        //                    tmpObject.notes = newObject.notes;
        //                 //   tmpObject.createDate = newObject.createDate;
        //                    tmpObject.updateDate = newObject.updateDate;
        //                    tmpObject.createUserId = newObject.createUserId;
        //                    tmpObject.updateUserId = newObject.updateUserId;
        //                    tmpObject.isActive = newObject.isActive;

        //                    entity.SaveChanges();
        //                    message = tmpObject.agentMembershipsId ;

        //                }
        //                return  message;
        //            }
        //        }

        //        catch
        //        {
        //            message = 0;
        //            return message;
        //        }
        //    }
        //    else
        //        {

        //            return 0;
        //        }

        //}
    }
}
