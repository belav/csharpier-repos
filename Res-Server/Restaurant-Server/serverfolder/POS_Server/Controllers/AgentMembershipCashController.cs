using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/AgentMembershipCash")]
    public class AgentMembershipCashController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

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
                    var List = (
                        from CSH in entity.agentMembershipCash
                        join G in entity.agents on CSH.agentId equals G.agentId
                        join CT in entity.cashTransfer on CSH.cashTransId equals CT.cashTransId
                        join M in entity.memberships on CSH.membershipId equals M.membershipId

                        //from JCTR in CTR.DefaultIfEmpty()
                        //  from JSU in SU.DefaultIfEmpty()

                        select new AgentMembershipCashModel
                        {
                            transNum = CT.transNum,
                            transType = CT.transType,
                            agentMembershipCashId = CSH.agentMembershipCashId,
                            subscriptionFeesId = CSH.subscriptionFeesId,
                            cashTransId = CSH.cashTransId,
                            membershipId = M.membershipId,
                            agentId = G.agentId,
                            startDate = CSH.startDate,
                            endDate = CSH.endDate,
                            Amount = CT.cash,
                            agentName = G.name,
                            agentcompany = G.company,
                            agenttype = G.type,
                            agentcode = G.code,
                            membershipName = M.name,
                            membershipcode = M.code,
                            membershipisActive = M.isActive,
                            monthsCount = CSH.monthsCount,
                            updateDate = CSH.updateDate,
                            subscriptionType = CSH.subscriptionType,
                            payDate = CT.updateDate,
                            discountValue = CSH.discountValue,
                            total = CSH.total,
                            processType = CT.processType,
                            cardId = CT.cardId,
                            cardName = CT.cards.name,
                            docNum = CT.docNum,
                        }
                    ).OrderBy(X => X.updateDate).ToList();

                    return TokenManager.GenerateToken(List);
                }
            }
        }

        [HttpPost]
        [Route("GetAgentToPay")]
        public string GetAgentToPay(string token)
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
                try
                {
                    List<AgenttoPayCashModel> List1 = new List<AgenttoPayCashModel>();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List1 = (
                            from G in entity.agents

                            join M in entity.memberships on G.membershipId equals M.membershipId
                            join S in entity.subscriptionFees
                                on M.membershipId equals S.membershipId
                                into SU
                            // //  join JCS in entity.agentMembershipCash on M.membershipId equals JCS.membershipId
                            // join CSH in entity.agentMembershipCash on M.membershipId equals CSH.membershipId into CS
                            //  from JCS in CS.DefaultIfEmpty()
                            // join CSH2 in entity.agentMembershipCash on G.agentId equals CSH2.agentId
                            //   join CT in entity.cashTransfer on JCS.cashTransId equals CT.cashTransId into CTR


                            from JSU in SU.DefaultIfEmpty()
                            where
                                (
                                    M.subscriptionType != "f"
                                    && M.isActive == 1
                                    && G.membershipId == M.membershipId
                                )
                            select new AgenttoPayCashModel
                            {
                                //transNum = JCTR.transNum,
                                //transType = JCTR.transType,
                                // agentMembershipsId = AM.agentMembershipsId,
                                //   agentMembershipCashId = JCS.agentMembershipCashId,
                                //agentMembershipCashId=   JCS.agentMembershipCashId,
                                //   agentMembershipCashId = G.agentMembershipCash.Where(x => x.agentId == G.agentId && x.membershipId == M.membershipId).FirstOrDefault().agentMembershipCashId,
                                agentMembershipcashobjList = G.agentMembershipCash
                                    .Where(x => x.agentId == G.agentId)
                                    .OrderBy(x => x.updateDate)
                                    .Select(
                                        x =>
                                            new AgentMembershipCashModel
                                            {
                                                agentMembershipCashId = x.agentMembershipCashId,
                                                subscriptionFeesId = x.subscriptionFeesId,
                                                cashTransId = x.cashTransId,
                                                membershipId = x.membershipId,
                                                agentId = x.agentId,
                                                startDate = x.startDate,
                                                endDate = x.endDate,
                                                notes = x.notes,
                                                updateUserId = x.updateUserId,
                                                isActive = x.isActive,
                                                createDate = x.createDate,
                                                updateDate = x.updateDate,
                                                createUserId = x.createUserId,
                                                subscriptionType = x.subscriptionType,
                                                total = x.total,
                                                monthsCount = x.monthsCount,
                                            }
                                    )
                                    .ToList(),
                                subscriptionFeesId = JSU.subscriptionFeesId,
                                //cashTransId = JCS.cashTransId,
                                membershipId = M.membershipId,
                                agentId = G.agentId,
                                //startDate = JCS.startDate,
                                //endDate = JCS.endDate,

                                Amount = JSU.Amount == null ? 0 : JSU.Amount,
                                agentName = G.name,
                                agentcompany = G.company,
                                agenttype = G.type,
                                agentcode = G.code,
                                membershipName = M.name,
                                membershipisActive = M.isActive,
                                monthsCount = JSU.monthsCount,
                                subscriptionType = M.subscriptionType,
                                //updateDate = JCS.updateDate,
                                //cashsubscriptionType = JCS.subscriptionType,
                                //discountValue = JCS.discountValue == null ? 0 : JCS.discountValue,
                                //total = JCS.total == null ? 0 : JCS.total,
                                //cashmembId= JCS.membershipId
                            }
                        ).ToList();
                    }

                    //(row.subscriptionType == "o" && (!(row.cashTransId > 0) || row.cachpayrowCount == 0))
                    var List2 = List1
                        //.Where(S =>
                        // //notpayed
                        //(S.subscriptionType == "o" || S.subscriptionType == "y" || S.subscriptionType == "m")&&(S.agentMembershipcashobjList.Count()==0)

                        // )
                        .ToList()
                        .GroupBy(S => S.agentId)
                        .Select(
                            S =>
                                new AgenttoPayCashModel
                                {
                                    transNum = S.LastOrDefault().transNum,
                                    transType = S.LastOrDefault().transType,
                                    //  agentMembershipsId = S.LastOrDefault().agentMembershipsId,
                                    agentMembershipCashId = S.LastOrDefault().agentMembershipCashId,
                                    subscriptionFeesId = S.LastOrDefault().subscriptionFeesId,
                                    cashTransId = S.LastOrDefault().cashTransId,
                                    membershipId = S.LastOrDefault().membershipId,
                                    agentId = S.LastOrDefault().agentId,
                                    startDate = S.LastOrDefault().startDate,
                                    endDate = S.LastOrDefault().endDate,
                                    Amount = S.LastOrDefault().Amount,
                                    agentName = S.LastOrDefault().agentName,
                                    membershipName = S.LastOrDefault().membershipName,
                                    membershipisActive = S.LastOrDefault().membershipisActive,
                                    monthsCount = S.LastOrDefault().monthsCount,
                                    subscriptionType = S.LastOrDefault().subscriptionType,
                                    //updateDate = S.LastOrDefault().updateDate,
                                    //cashsubscriptionType = S.LastOrDefault().cashsubscriptionType,
                                    //discountValue = S.LastOrDefault().discountValue,
                                    //total = S.LastOrDefault().total,
                                    agentMembershipcashobj = S.LastOrDefault()
                                        .agentMembershipcashobjList.LastOrDefault(),
                                    agentMembershipcashobjList =
                                        S.LastOrDefault().agentMembershipcashobjList,
                                }
                        )
                        .ToList();
                    // (S.subscriptionType == "o" || S.subscriptionType == "y" || S.subscriptionType == "m") && (S.agentMembershipcashobjList.Count() == 0)

                    // foreach (var X in List2)
                    // {
                    //     if ((X.subscriptionType == "o" || X.subscriptionType == "y" || X.subscriptionType == "m") && (X.agentMembershipcashobjList.Count() == 0))
                    //     {
                    //         X.isShow = 1;

                    //     }
                    //     else if (X.agentMembershipcashobj != null && (X.agentMembershipcashobj.membershipId != X.membershipId) ||
                    //  ((X.agentMembershipcashobj.membershipId == X.membershipId)
                    //  && (
                    //((X.agentMembershipcashobj.subscriptionType == "m" || X.agentMembershipcashobj.subscriptionType == "y") && (X.subscriptionType == "o"))
                    //  || ((X.agentMembershipcashobj.subscriptionType == "o") && (X.subscriptionType == "y" || X.subscriptionType == "m"))
                    //  || (X.subscriptionType == "y" || X.subscriptionType == "m")
                    //  )))
                    //     {
                    //         X.isShow = 1;
                    //     }
                    //     else
                    //     {
                    //         X.isShow = 0;
                    //     }
                    // }

                    var List = List2
                        .Where(
                            X =>
                                //no cash record
                                (
                                    (
                                        X.subscriptionType == "o"
                                        || X.subscriptionType == "y"
                                        || X.subscriptionType == "m"
                                    ) && (X.agentMembershipcashobjList.Count() == 0)
                                )
                                ||
                                //membership changed
                                X.agentMembershipcashobj != null
                                    && (X.agentMembershipcashobj.membershipId != X.membershipId)
                                ||
                                //membership not changed and subscriptionType changed
                                (
                                    (X.agentMembershipcashobj.membershipId == X.membershipId)
                                    && (
                                        (
                                            (
                                                X.agentMembershipcashobj.subscriptionType == "m"
                                                || X.agentMembershipcashobj.subscriptionType == "y"
                                            ) && (X.subscriptionType == "o")
                                        )
                                        || (
                                            (X.agentMembershipcashobj.subscriptionType == "o")
                                            && (
                                                X.subscriptionType == "y"
                                                || X.subscriptionType == "m"
                                            )
                                        )
                                        || (X.subscriptionType == "y" || X.subscriptionType == "m")
                                    )
                                )
                        )
                        .ToList();
                    //  var List = List2;


                    return TokenManager.GenerateToken(List);
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        /*
   old
     using (incposdbEntities entity = new incposdbEntities())
                {

                        var List1 = (from AM in entity.agentMemberships
                                    join G in entity.agents on AM.agentId equals G.agentId

                                    join M in entity.memberships on AM.membershipId equals M.membershipId
                                    join S in entity.subscriptionFees on M.membershipId equals S.membershipId into SU
                                    join CSH in entity.agentMembershipCash on G.agentId equals CSH.agentId into CS
                                    from JCS in CS.DefaultIfEmpty()
                                        // join CSH2 in entity.agentMembershipCash on G.agentId equals CSH2.agentId
                                    join CT in entity.cashTransfer on JCS.cashTransId equals CT.cashTransId into CTR

                                    where (M.subscriptionType != "F" && M.isActive == 1)
                                    //  from I in entity.invoices.Where(I => I.invoiceId == IT.invoiceId)


                                    //  join IUL in entity.itemsLocations on L.locationId equals IUL.locationId


                                    from JCTR in CTR.DefaultIfEmpty()
                                    from JSU in SU.DefaultIfEmpty()

                                    select new AgenttoPayCashModel
                                    {
                                        transNum = JCTR.transNum,
                                        transType = JCTR.transType,
                                        agentMembershipsId = AM.agentMembershipsId,
                                        agentMembershipCashId = JCS.agentMembershipCashId,
                                        subscriptionFeesId = JSU.subscriptionFeesId,
                                        cashTransId = JCS.cashTransId,
                                        membershipId = M.membershipId,
                                        agentId = G.agentId,
                                        startDate = JCS.startDate,
                                        endDate = JCS.endDate,

                                        Amount = JSU.Amount,
                                        agentName = G.name,
                                        membershipName = M.name,
                                        membershipisActive = M.isActive,
                                        monthsCount = JSU.monthsCount,
                                        subscriptionType = M.subscriptionType,
                                        updateDate = JCS.updateDate,

                                    }
                                    ).OrderBy(X=>X.updateDate).ToList();
                        var List = List1.GroupBy(S => S.agentMembershipsId).Select(S => new AgenttoPayCashModel
                        {
                            transNum = S.LastOrDefault().transNum,
                            transType = S.LastOrDefault().transType,
                            agentMembershipsId = S.LastOrDefault().agentMembershipsId,
                            agentMembershipCashId = S.LastOrDefault().agentMembershipCashId,
                            subscriptionFeesId = S.LastOrDefault().subscriptionFeesId,
                            cashTransId = S.LastOrDefault().cashTransId,
                            membershipId = S.LastOrDefault().membershipId,
                            agentId = S.LastOrDefault().agentId,
                            startDate = S.LastOrDefault().startDate,
                            endDate = S.LastOrDefault().endDate,

                            Amount = S.LastOrDefault().Amount,
                            agentName = S.LastOrDefault().agentName,
                            membershipName = S.LastOrDefault().membershipName,
                            membershipisActive = S.LastOrDefault().membershipisActive,
                            monthsCount = S.LastOrDefault().monthsCount,
                            subscriptionType = S.LastOrDefault().subscriptionType,
                            updateDate = S.LastOrDefault().updateDate,
                        }
                        ).Where(S=>(S.agentMembershipCashId==null || S.agentMembershipCashId == 0 )
                        || (S.subscriptionType == "o" &&  S.agentMembershipCashId >0  && ( S.cashTransId ==0 || S.cashTransId == null))
                        || ((S.subscriptionType == "m"|| S.subscriptionType == "y") && (S.agentMembershipCashId != null && S.agentMembershipCashId > 0))
                        
                        ).ToList();

                    //                var List = entity.agentMembershipCash.Select(S => new AgentMembershipCashModel
                    //{
                    //    agentMembershipCashId = S.agentMembershipCashId,
                    //    subscriptionFeesId = S.subscriptionFeesId,
                    //    cashTransId = S.cashTransId,
                    //    membershipId = S.membershipId,
                    //    agentId = S.agentId,
                    //    startDate = S.startDate,
                    //    endDate = S.endDate,
                    //    notes = S.notes,
                    //    createDate = S.createDate,
                    //    updateDate = S.updateDate,
                    //    createUserId = S.createUserId,
                    //    updateUserId = S.updateUserId,
                    //    isActive = S.isActive,


                        //})
                        //.ToList();


                        return TokenManager.GenerateToken(List);

                }
         * */
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
                long agentMembershipCashId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        agentMembershipCashId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.agentMembershipCash
                        .Where(S => S.agentMembershipCashId == agentMembershipCashId)
                        .Select(
                            S =>
                                new
                                {
                                    S.agentMembershipCashId,
                                    S.subscriptionFeesId,
                                    S.cashTransId,
                                    S.membershipId,
                                    S.agentId,
                                    S.startDate,
                                    S.endDate,
                                    S.notes,
                                    S.createDate,
                                    S.updateDate,
                                    S.createUserId,
                                    S.updateUserId,
                                    S.isActive,
                                    S.discountValue,
                                    S.total,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(bank);
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
                string agentMembershipCashId = "";
                agentMembershipCash newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        agentMembershipCashId = c.Value.Replace("\\", string.Empty);
                        agentMembershipCashId = agentMembershipCashId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<agentMembershipCash>(
                            agentMembershipCashId,
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
                if (newObject.subscriptionFeesId == 0 || newObject.subscriptionFeesId == null)
                {
                    Nullable<long> id = null;
                    newObject.subscriptionFeesId = id;
                }
                if (newObject.cashTransId == 0 || newObject.cashTransId == null)
                {
                    Nullable<long> id = null;
                    newObject.cashTransId = id;
                }
                if (newObject.membershipId == 0 || newObject.membershipId == null)
                {
                    Nullable<long> id = null;
                    newObject.membershipId = id;
                }
                if (newObject.agentId == 0 || newObject.agentId == null)
                {
                    Nullable<long> id = null;
                    newObject.agentId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        agentMembershipCash tmpObject = new agentMembershipCash();
                        var bankEntity = entity.Set<agentMembershipCash>();
                        if (newObject.agentMembershipCashId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.agentMembershipCashId.ToString();
                            ;
                        }
                        else
                        {
                            tmpObject = entity.agentMembershipCash
                                .Where(
                                    p => p.agentMembershipCashId == newObject.agentMembershipCashId
                                )
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.agentMembershipCashId = newObject.agentMembershipCashId;
                            tmpObject.subscriptionFeesId = newObject.subscriptionFeesId;
                            tmpObject.cashTransId = newObject.cashTransId;
                            tmpObject.membershipId = newObject.membershipId;
                            tmpObject.agentId = newObject.agentId;
                            tmpObject.startDate = newObject.startDate;
                            tmpObject.endDate = newObject.endDate;
                            tmpObject.notes = newObject.notes;
                            tmpObject.createDate = newObject.createDate;
                            tmpObject.updateDate = newObject.updateDate;
                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;
                            tmpObject.isActive = newObject.isActive;
                            tmpObject.monthsCount = newObject.monthsCount;
                            tmpObject.subscriptionType = newObject.subscriptionType;
                            tmpObject.discountValue = newObject.discountValue;
                            tmpObject.total = newObject.total;
                            entity.SaveChanges();
                            message = tmpObject.agentMembershipCashId.ToString();
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
                long agentMembershipCashId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        agentMembershipCashId = long.Parse(c.Value);
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
                            agentMembershipCash objDelete = entity.agentMembershipCash.Find(
                                agentMembershipCashId
                            );
                            entity.agentMembershipCash.Remove(objDelete);
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
                            agentMembershipCash objDelete = entity.agentMembershipCash.Find(
                                agentMembershipCashId
                            );
                            objDelete.isActive = 0;
                            objDelete.updateUserId = userId;
                            objDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
                List<agentMembershipCash> newListObj = null;
                long membershipId = 0;
                long updateUserId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        strObject = c.Value.Replace("\\", string.Empty);
                        strObject = strObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<agentMembershipCash>>(
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

                List<agentMembershipCash> items = null;
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    items = entity.agentMembershipCash
                        .Where(x => x.membershipId == membershipId)
                        .ToList();
                    if (items != null)
                    {
                        entity.agentMembershipCash.RemoveRange(items);
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        for (int i = 0; i < newListObj.Count; i++)
                        {
                            if (
                                newListObj[i].updateUserId == 0
                                || newListObj[i].updateUserId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].updateUserId = id;
                            }
                            if (
                                newListObj[i].createUserId == 0
                                || newListObj[i].createUserId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].createUserId = id;
                            }
                            if (
                                newListObj[i].membershipId == 0
                                || newListObj[i].membershipId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].membershipId = id;
                            }
                            if (newListObj[i].agentId == 0 || newListObj[i].agentId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].agentId = id;
                            }
                            if (
                                newListObj[i].subscriptionFeesId == 0
                                || newListObj[i].subscriptionFeesId == null
                            )
                            {
                                Nullable<long> id = null;
                                newListObj[i].subscriptionFeesId = id;
                            }
                            if (newListObj[i].cashTransId == 0 || newListObj[i].cashTransId == null)
                            {
                                Nullable<long> id = null;
                                newListObj[i].cashTransId = id;
                            }
                            var branchEntity = entity.Set<agentMembershipCash>();

                            newListObj[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
        //        List<agentMembershipCash> newListObj = null;
        //        long membershipId = 0;
        //        long updateUserId = 0;
        //        IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
        //        foreach (Claim c in claims)
        //        {
        //            if (c.Type == "newList")
        //            {
        //                strObject = c.Value.Replace("\\", string.Empty);
        //                strObject = strObject.Trim('"');
        //                newListObj = JsonConvert.DeserializeObject<List<agentMembershipCash>>(strObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

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

        //        List<agentMembershipCash> olditems = null;
        //        List<agentMembershipCash> oldActiveitems = null;
        //        List<agentMembershipCash> newitems = null;
        //        agentMembershipCash tempob  = new agentMembershipCash();
        //        // delete old invoice items
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            olditems = entity.agentMembershipCash.Where(x => x.membershipId == membershipId).ToList();

        //            if (olditems != null)
        //            {
        //                //  entity.agentMembershipCash.RemoveRange(items);
        //           //     oldActiveitems = olditems.Where(M=>M.agentMembershipCashId!= cont)
        //                try
        //                {
        //                    foreach (var oldrow in olditems)
        //                    {
        //                        oldActiveitems =  null;
        //                        oldActiveitems = newListObj.Where(M => M.agentMembershipCashId == oldrow.agentMembershipCashId).ToList();
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
        //            newitems = newListObj.Where(M => M.agentMembershipCashId == 0).ToList();

        //            foreach (var newrow in newitems)
        //            {
        //                newrow.membershipId = membershipId;
        //                Save(newrow);


        //            }
        //            return TokenManager.GenerateToken("1");


        //        }


        //    }


        //}


        //public int Save(agentMembershipCash newObject)
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
        //                agentMembershipCash tmpObject = new agentMembershipCash();
        //                var bankEntity = entity.Set<agentMembershipCash>();
        //                if (newObject.agentMembershipCashId == 0)
        //                {
        //                    newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateUserId = newObject.createUserId;
        //                    tmpObject = bankEntity.Add(newObject);
        //                    entity.SaveChanges();
        //                    message = tmpObject.agentMembershipCashId ;
        //                }
        //                else
        //                {
        //                    tmpObject = entity.agentMembershipCash.Where(p => p.agentMembershipCashId == newObject.agentMembershipCashId).FirstOrDefault();

        //                    tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

        //                    tmpObject.agentMembershipCashId = newObject.agentMembershipCashId;
        //                    tmpObject.subscriptionFeesId = newObject.subscriptionFeesId;
        //                    tmpObject.cashTransId = newObject.cashTransId;
        //                    tmpObject.membershipId = newObject.membershipId;
        //                    tmpObject.agentId = newObject.agentId;
        //                    tmpObject.startDate = newObject.startDate;
        //                    tmpObject.endDate = newObject.endDate;
        //                    tmpObject.notes = newObject.notes;
        //                 //   tmpObject.createDate = newObject.createDate;
        //                    tmpObject.updateDate = newObject.updateDate;
        //                    tmpObject.createUserId = newObject.createUserId;
        //                    tmpObject.updateUserId = newObject.updateUserId;
        //                    tmpObject.isActive = newObject.isActive;

        //                    entity.SaveChanges();
        //                    message = tmpObject.agentMembershipCashId ;

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



        public long Saveamc(agentMembershipCash newObject)
        {
            long message = 0;
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
            if (newObject.subscriptionFeesId == 0 || newObject.subscriptionFeesId == null)
            {
                Nullable<long> id = null;
                newObject.subscriptionFeesId = id;
            }
            if (newObject.cashTransId == 0 || newObject.cashTransId == null)
            {
                Nullable<long> id = null;
                newObject.cashTransId = id;
            }
            if (newObject.membershipId == 0 || newObject.membershipId == null)
            {
                Nullable<long> id = null;
                newObject.membershipId = id;
            }
            if (newObject.agentId == 0 || newObject.agentId == null)
            {
                Nullable<long> id = null;
                newObject.agentId = id;
            }
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    agentMembershipCash tmpObject = new agentMembershipCash();
                    var bankEntity = entity.Set<agentMembershipCash>();
                    if (newObject.agentMembershipCashId == 0)
                    {
                        newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateUserId = newObject.createUserId;
                        tmpObject = bankEntity.Add(newObject);
                        entity.SaveChanges();
                        message = tmpObject.agentMembershipCashId;
                    }
                    else
                    {
                        tmpObject = entity.agentMembershipCash
                            .Where(p => p.agentMembershipCashId == newObject.agentMembershipCashId)
                            .FirstOrDefault();

                        tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                        tmpObject.agentMembershipCashId = newObject.agentMembershipCashId;
                        tmpObject.subscriptionFeesId = newObject.subscriptionFeesId;
                        tmpObject.cashTransId = newObject.cashTransId;
                        tmpObject.membershipId = newObject.membershipId;
                        tmpObject.agentId = newObject.agentId;
                        tmpObject.startDate = newObject.startDate;
                        tmpObject.endDate = newObject.endDate;
                        tmpObject.notes = newObject.notes;
                        tmpObject.createDate = newObject.createDate;
                        tmpObject.updateDate = newObject.updateDate;
                        tmpObject.createUserId = newObject.createUserId;
                        tmpObject.updateUserId = newObject.updateUserId;
                        tmpObject.isActive = newObject.isActive;
                        tmpObject.monthsCount = newObject.monthsCount;
                        tmpObject.subscriptionType = newObject.subscriptionType;
                        tmpObject.discountValue = newObject.discountValue;
                        tmpObject.total = newObject.total;
                        entity.SaveChanges();
                        message = tmpObject.agentMembershipCashId;
                    }
                    return message;
                }
            }
            catch
            {
                message = -126;
                return message;
            }
        }

        [HttpPost]
        [Route("Savepay")]
        public string Savepay(string token)
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
                string agentMembershipCashId = "";
                string cashTransferObjectstring = "";
                agentMembershipCash newObject = null;
                cashTransfer cashTransferObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        agentMembershipCashId = c.Value.Replace("\\", string.Empty);
                        agentMembershipCashId = agentMembershipCashId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<agentMembershipCash>(
                            agentMembershipCashId,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cashtransferobject")
                    {
                        cashTransferObjectstring = c.Value.Replace("\\", string.Empty);
                        cashTransferObjectstring = cashTransferObjectstring.Trim('"');
                        cashTransferObject = JsonConvert.DeserializeObject<cashTransfer>(
                            cashTransferObjectstring,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }

                try
                {
                    long cashTransId = 0;
                    DateTime lastenddate = coctrlr.AddOffsetTodate(DateTime.Now);
                    DateTime DTNow = coctrlr.AddOffsetTodate(DateTime.Now);
                    DateTime tmp;
                    CashTransferController cashcntrlr = new CashTransferController();
                    AgenttoPayCashModel LastAgentmcash = new AgenttoPayCashModel();
                    subscriptionFees subscriptionmodel = new subscriptionFees();
                    LastAgentmcash = GetLastAgentmcash(newObject);
                    //  return TokenManager.GenerateToken(LastAgentmcash);
                    subscriptionFeesController subctrlr = new subscriptionFeesController();
                    // get month count

                    subscriptionmodel = subctrlr.GetById((long)newObject.subscriptionFeesId);
                    // return TokenManager.GenerateToken(subscriptionmodel);
                    newObject.monthsCount = subscriptionmodel.monthsCount;

                    ///////////////////////////////////////////////////////////
                    if (LastAgentmcash != null)
                    {
                        if (LastAgentmcash.agentMembershipCashId > 0)
                        {
                            if (
                                (
                                    LastAgentmcash.cashsubscriptionType == "m"
                                    || LastAgentmcash.cashsubscriptionType == "y"
                                )
                                && (
                                    newObject.subscriptionType == "m"
                                    || newObject.subscriptionType == "y"
                                )
                            )
                            {
                                //last type has expire date AND current type has expire
                                if (LastAgentmcash.endDate < DTNow)
                                {
                                    //expired
                                    lastenddate = DTNow;
                                    newObject.startDate = DTNow;
                                }
                                else
                                {
                                    // not expired yet
                                    lastenddate = (DateTime)LastAgentmcash.endDate;
                                }

                                //check type to extend endDate
                                if (newObject.subscriptionType == "m")
                                {
                                    newObject.endDate = lastenddate.AddMonths(
                                        (int)newObject.monthsCount
                                    );
                                }
                                else if (newObject.subscriptionType == "y")
                                {
                                    newObject.endDate = lastenddate.AddYears(
                                        (int)newObject.monthsCount
                                    );
                                }
                            }
                            else if (
                                LastAgentmcash.cashsubscriptionType == "o"
                                && (
                                    newObject.subscriptionType == "m"
                                    || newObject.subscriptionType == "y"
                                )
                            )
                            {
                                //last type dont have expire date AND current type has expire
                                lastenddate = DTNow;
                                newObject.startDate = DTNow;
                                //check type to extend endDate
                                if (newObject.subscriptionType == "m")
                                {
                                    newObject.endDate = lastenddate.AddMonths(
                                        (int)newObject.monthsCount
                                    );
                                }
                                else if (newObject.subscriptionType == "y")
                                {
                                    newObject.endDate = lastenddate.AddYears(
                                        (int)newObject.monthsCount
                                    );
                                }
                            }
                        }
                        else
                        {
                            //new pay or the last type is free
                            lastenddate = DTNow;
                            newObject.startDate = DTNow;
                            //check type to extend endDate
                            if (newObject.subscriptionType == "m")
                            {
                                newObject.endDate = lastenddate.AddMonths(
                                    (int)newObject.monthsCount
                                );
                                newObject.startDate = DTNow;
                            }
                            else if (newObject.subscriptionType == "y")
                            {
                                newObject.endDate = lastenddate.AddYears(
                                    (int)newObject.monthsCount
                                );
                                newObject.startDate = DTNow;
                            }
                            else
                            {
                                newObject.endDate = DTNow;
                                //   newObject.endDate = DTNow.AddMonths(1);
                            }
                        }
                    }
                    else
                    {
                        //new pay or the last type is free
                        lastenddate = DTNow;
                        newObject.startDate = DTNow;
                        //check type to extend endDate
                        if (newObject.subscriptionType == "m")
                        {
                            newObject.endDate = lastenddate.AddMonths((int)newObject.monthsCount);
                            newObject.startDate = DTNow;
                        }
                        else if (newObject.subscriptionType == "y")
                        {
                            newObject.endDate = lastenddate.AddYears((int)newObject.monthsCount);
                        }
                        else
                        {
                            newObject.endDate = DTNow;
                            newObject.startDate = DTNow;
                            //   newObject.endDate = DTNow.AddMonths(2);
                        }
                    }
                    if (newObject.subscriptionType == "o")
                    {
                        newObject.endDate = DTNow;
                    }

                    tmp = (DateTime)newObject.endDate;
                    tmp = tmp.AddDays(-1);
                    newObject.endDate = tmp.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    ////////////////////////////////////////////////////////
                    //save cash trans
                    cashTransId = cashcntrlr.Save(cashTransferObject);
                    //string ms = cashcntrlr.Save(cashTransferObject);
                    //return TokenManager.GenerateToken(ms);
                    if (cashTransId > 0)
                    {
                        newObject.cashTransId = cashTransId;
                        //save AgentMembershipCash
                        long res = Saveamc(newObject);
                        return TokenManager.GenerateToken(res.ToString());
                    }
                    else
                    {
                        return TokenManager.GenerateToken("-687");
                    }
                }
                catch
                {
                    message = "-79";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        public AgenttoPayCashModel GetLastAgentmcash(agentMembershipCash currentObject)
        {
            AgenttoPayCashModel lastrow = new AgenttoPayCashModel();
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List1 = (
                        from CSH in entity.agentMembershipCash

                        join M in entity.memberships on CSH.membershipId equals M.membershipId
                        join S in entity.subscriptionFees
                            on CSH.subscriptionFeesId equals S.subscriptionFeesId
                            into SU
                        // join CSH2 in entity.agentMembershipCash on G.agentId equals CSH2.agentId
                        join CT in entity.cashTransfer
                            on CSH.cashTransId equals CT.cashTransId
                            into CTR

                        where
                            (
                                M.subscriptionType != "F"
                                && currentObject.membershipId == CSH.membershipId
                                && currentObject.agentId == CSH.agentId
                            )
                        from JCTR in CTR.DefaultIfEmpty()
                        from JSU in SU.DefaultIfEmpty()

                        select new AgenttoPayCashModel
                        {
                            transNum = JCTR.transNum,
                            transType = JCTR.transType,
                            // agentMembershipsId = AM.agentMembershipsId,
                            agentMembershipCashId = CSH.agentMembershipCashId,
                            subscriptionFeesId = CSH.subscriptionFeesId,
                            cashTransId = CSH.cashTransId,
                            membershipId = M.membershipId,
                            agentId = CSH.agentId,
                            startDate = CSH.startDate,
                            endDate = CSH.endDate,
                            Amount = JCTR.cash,
                            ////agentName = G.name,
                            ////agentcompany = G.company,
                            ////agenttype = G.type,
                            ////agentcode = G.code,
                            //membershipName = M.name,
                            membershipisActive = M.isActive,
                            monthsCount = CSH.monthsCount,
                            subscriptionType = M.subscriptionType,
                            updateDate = CSH.updateDate,
                            createDate = CSH.createDate,
                            cashsubscriptionType = CSH.subscriptionType,
                            discountValue = CSH.discountValue,
                            total = CSH.total,
                        }
                    ).OrderBy(X => X.createDate).ToList();

                    lastrow = List1.LastOrDefault();
                    return lastrow;
                }
            }
            catch (Exception ex)
            {
                lastrow.agentName = ex.ToString();
                return lastrow;
            }
        }
    }
}
