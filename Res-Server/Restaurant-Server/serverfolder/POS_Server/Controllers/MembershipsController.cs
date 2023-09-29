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
    [RoutePrefix("api/memberships")]
    public class MembershipsController : ApiController
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
                    var List = entity.memberships
                        .Select(
                            S =>
                                new MembershipsModel
                                {
                                    membershipId = S.membershipId,
                                    name = S.name,
                                    notes = S.notes,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    isActive = S.isActive,
                                    subscriptionType = S.subscriptionType,
                                    code = S.code,
                                    subscriptionFee = S.subscriptionFees.FirstOrDefault().Amount,
                                    isFreeDelivery = S.isFreeDelivery,
                                    deliveryDiscountPercent = S.deliveryDiscountPercent,
                                    couponsCount = S.couponsMemberships
                                        .Where(X => X.membershipId == S.membershipId)
                                        .ToList()
                                        .Count(),
                                    offersCount = S.membershipsOffers
                                        .Where(X => X.membershipId == S.membershipId)
                                        .ToList()
                                        .Count(),
                                    invoicesClassesCount = S.invoicesClassMemberships
                                        .Where(X => X.membershipId == S.membershipId)
                                        .ToList()
                                        .Count(),
                                    customersCount = S.agents
                                        .Where(X => X.membershipId == S.membershipId)
                                        .ToList()
                                        .Count(),
                                }
                        )
                        .ToList();
                    /*
                         public Nullable<int> couponsCounts { get; set; }
        public Nullable<int> offersCounts { get; set; }
        public Nullable<int> invoicesClassCounts { get; set; }
        public Nullable<int> agentsCounts { get; set; }

                        public int customersCount { get; set; }
        public int couponsCount { get; set; }
        public int offersCount { get; set; }
        public int invoicesClassesCount { get; set; }
                     * */
                    if (List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            canDelete = false;
                            if (List[i].isActive == 1)
                            {
                                long membershipId = (long)List[i].membershipId;
                                //var itemsI = entity.agentMemberships.Where(x => x.membershipId == membershipId).Select(b => new { b.agentMembershipsId }).FirstOrDefault();
                                var items2 = entity.subscriptionFees
                                    .Where(x => x.membershipId == membershipId)
                                    .Select(b => new { b.subscriptionFeesId })
                                    .FirstOrDefault();
                                var items3 = entity.couponsMemberships
                                    .Where(x => x.membershipId == membershipId)
                                    .Select(b => new { b.couponMembershipId })
                                    .FirstOrDefault();
                                var items4 = entity.membershipsOffers
                                    .Where(x => x.membershipId == membershipId)
                                    .Select(b => new { b.membershipOfferId })
                                    .FirstOrDefault();
                                var items5 = entity.invoicesClassMemberships
                                    .Where(x => x.membershipId == membershipId)
                                    .Select(b => new { b.invClassMemberId })
                                    .FirstOrDefault();

                                if (
                                    (
                                        items2 is null
                                        && items3 is null
                                        && items4 is null
                                        && items5 is null
                                    )
                                )
                                    canDelete = true;
                            }
                            List[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(List);
                }
            }
        }

        /*
 
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
                long membershipId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        membershipId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.memberships
                        .Where(S => S.membershipId == membershipId)
                        .Select(
                            S =>
                                new
                                {
                                    S.membershipId,
                                    S.name,
                                    S.notes,
                                    S.createDate,
                                    S.updateDate,
                                    S.createUserId,
                                    S.updateUserId,
                                    S.isActive,
                                    S.subscriptionType,
                                    S.code,
                                    S.isFreeDelivery,
                                    S.deliveryDiscountPercent,
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
                string membershipId = "";
                memberships newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        membershipId = c.Value.Replace("\\", string.Empty);
                        membershipId = membershipId.Trim('"');
                        newObject = JsonConvert.DeserializeObject<memberships>(
                            membershipId,
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        memberships tmpObject = new memberships();
                        var bankEntity = entity.Set<memberships>();
                        if (newObject.membershipId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            tmpObject = bankEntity.Add(newObject);
                            entity.SaveChanges();
                            message = tmpObject.membershipId.ToString();
                            ;
                        }
                        else
                        {
                            tmpObject = entity.memberships
                                .Where(p => p.membershipId == newObject.membershipId)
                                .FirstOrDefault();

                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.membershipId = newObject.membershipId;
                            tmpObject.name = newObject.name;

                            tmpObject.notes = newObject.notes;
                            //  tmpObject.createDate = newObject.createDate;

                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.updateUserId = newObject.updateUserId;
                            tmpObject.isActive = newObject.isActive;

                            tmpObject.subscriptionType = newObject.subscriptionType;
                            tmpObject.code = newObject.code;
                            tmpObject.isFreeDelivery = newObject.isFreeDelivery;
                            tmpObject.deliveryDiscountPercent = newObject.deliveryDiscountPercent;
                            entity.SaveChanges();
                            message = tmpObject.membershipId.ToString();
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
                long membershipId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        membershipId = long.Parse(c.Value);
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
                            memberships objDelete = entity.memberships.Find(membershipId);
                            entity.memberships.Remove(objDelete);
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
                            memberships objDelete = entity.memberships.Find(membershipId);
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

        public long Save(memberships newObject)
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
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    memberships tmpObject = new memberships();
                    var Entity = entity.Set<memberships>();
                    if (newObject.membershipId == 0)
                    {
                        newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateUserId = newObject.createUserId;
                        tmpObject = Entity.Add(newObject);
                        entity.SaveChanges();
                        message = tmpObject.membershipId;
                    }
                    else
                    {
                        tmpObject = entity.memberships
                            .Where(p => p.membershipId == newObject.membershipId)
                            .FirstOrDefault();

                        tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                        tmpObject.membershipId = newObject.membershipId;
                        tmpObject.name = newObject.name;

                        tmpObject.notes = newObject.notes;
                        //  tmpObject.createDate = newObject.createDate;

                        tmpObject.createUserId = newObject.createUserId;
                        tmpObject.updateUserId = newObject.updateUserId;
                        tmpObject.isActive = newObject.isActive;

                        tmpObject.subscriptionType = newObject.subscriptionType;
                        tmpObject.code = newObject.code;
                        tmpObject.isFreeDelivery = newObject.isFreeDelivery;
                        tmpObject.deliveryDiscountPercent = newObject.deliveryDiscountPercent;
                        entity.SaveChanges();
                        message = tmpObject.membershipId;
                    }
                    return message;
                }
            }
            catch
            {
                message = 0;
                return message;
            }
        }

        [HttpPost]
        [Route("SaveMemberAndSub")]
        public string SaveMemberAndSub(string token)
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
                long res = 0;
                long subres = 0;
                string membershipId = "";
                MembershipsModel newObjectModel = null;

                subscriptionFees newsubscrOb = new subscriptionFees();
                subscriptionFeesController subscCntrller = new subscriptionFeesController();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        membershipId = c.Value.Replace("\\", string.Empty);
                        membershipId = membershipId.Trim('"');
                        newObjectModel = JsonConvert.DeserializeObject<MembershipsModel>(
                            membershipId,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        memberships tmpObject = new memberships();

                        tmpObject.membershipId = newObjectModel.membershipId;

                        tmpObject.membershipId = newObjectModel.membershipId;
                        tmpObject.name = newObjectModel.name;

                        tmpObject.notes = newObjectModel.notes;
                        tmpObject.createDate = newObjectModel.createDate;

                        tmpObject.createUserId = newObjectModel.createUserId;
                        tmpObject.updateUserId = newObjectModel.updateUserId;
                        tmpObject.isActive = newObjectModel.isActive;

                        tmpObject.subscriptionType = newObjectModel.subscriptionType;
                        tmpObject.code = newObjectModel.code;
                        tmpObject.isFreeDelivery = newObjectModel.isFreeDelivery;
                        tmpObject.deliveryDiscountPercent = newObjectModel.deliveryDiscountPercent;

                        if (newObjectModel.membershipId == 0)
                        {
                            //new add membership
                            res = Save(tmpObject);
                            if (res > 0)
                            {
                                //new add subscriptionFees row

                                //   newsubscrOb.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                newsubscrOb.subscriptionFeesId = 0;
                                newsubscrOb.membershipId = res;

                                if (newObjectModel.subscriptionType == "o")
                                {
                                    newsubscrOb.monthsCount = 1;
                                    newsubscrOb.Amount = (decimal)newObjectModel.subscriptionFee;
                                    newsubscrOb.createUserId = newObjectModel.createUserId;
                                    newsubscrOb.updateUserId = newObjectModel.updateUserId;
                                    newsubscrOb.notes = newObjectModel.notes;
                                    newsubscrOb.isActive = newObjectModel.isActive;
                                    subres = subscCntrller.Save(newsubscrOb);
                                    if (subres > 0)
                                    {
                                        return TokenManager.GenerateToken(res.ToString());
                                    }
                                    else
                                    {
                                        return TokenManager.GenerateToken("-1");
                                    }
                                }
                                else if (newObjectModel.subscriptionType == "f")
                                {
                                    newsubscrOb.monthsCount = 1;
                                    newsubscrOb.Amount = 0;
                                    newsubscrOb.createUserId = newObjectModel.createUserId;
                                    newsubscrOb.updateUserId = newObjectModel.updateUserId;
                                    newsubscrOb.notes = newObjectModel.notes;
                                    newsubscrOb.isActive = newObjectModel.isActive;
                                    subres = subscCntrller.Save(newsubscrOb);
                                    if (subres > 0)
                                    {
                                        return TokenManager.GenerateToken(res.ToString());
                                    }
                                    else
                                    {
                                        return TokenManager.GenerateToken("-1");
                                    }
                                }
                                else
                                {
                                    return TokenManager.GenerateToken(res.ToString());
                                }
                            }
                            else
                            {
                                return TokenManager.GenerateToken("-1");
                            }
                        }
                        else
                        {
                            //update

                            memberships tmpObjectdb = new memberships();
                            List<subscriptionFees> tmpsubListdb = new List<subscriptionFees>();
                            subscriptionFees tmpSubObjdb = new subscriptionFees();

                            tmpObjectdb = entity.memberships
                                .Where(p => p.membershipId == newObjectModel.membershipId)
                                .FirstOrDefault();
                            tmpsubListdb = entity.subscriptionFees
                                .Where(p => p.membershipId == newObjectModel.membershipId)
                                .ToList();
                            tmpSubObjdb = tmpsubListdb.OrderBy(S => S.updateDate).LastOrDefault();
                            //   string oldtype = tmpObjectdb.subscriptionType;
                            res = Save(tmpObject);
                            if (res > 0)
                            {
                                if (tmpObjectdb.subscriptionType == newObjectModel.subscriptionType)
                                {
                                    if (tmpObjectdb.subscriptionType == "o")
                                    {
                                        if (tmpSubObjdb.Amount != newObjectModel.subscriptionFee)
                                        {
                                            //the price changed so we have to save the change

                                            tmpSubObjdb.Amount = (decimal)
                                                newObjectModel.subscriptionFee;

                                            subres = subscCntrller.Save(tmpSubObjdb);
                                            if (subres > 0)
                                            {
                                                return TokenManager.GenerateToken(res.ToString());
                                            }
                                            else
                                            {
                                                return TokenManager.GenerateToken("-1");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //no change on subscrb table
                                        return TokenManager.GenerateToken(res.ToString());
                                    }
                                }
                                else
                                {
                                    long delres = 0;
                                    delres = subscCntrller.DeleteByMembershipId(
                                        newObjectModel.membershipId
                                    );

                                    //the old not like new
                                    if (newObjectModel.subscriptionType == "o")
                                    {
                                        //the new is "o"
                                        newsubscrOb.membershipId = newObjectModel.membershipId;
                                        newsubscrOb.monthsCount = 1;
                                        newsubscrOb.Amount = (decimal)
                                            newObjectModel.subscriptionFee;
                                        newsubscrOb.createUserId = newObjectModel.createUserId;
                                        newsubscrOb.updateUserId = newObjectModel.updateUserId;
                                        newsubscrOb.notes = newObjectModel.notes;
                                        newsubscrOb.isActive = newObjectModel.isActive;
                                        subres = subscCntrller.Save(newsubscrOb);
                                        if (subres > 0)
                                        {
                                            return TokenManager.GenerateToken(res.ToString());
                                        }
                                        else
                                        {
                                            return TokenManager.GenerateToken("-1");
                                        }
                                    }
                                    else if (newObjectModel.subscriptionType == "f")
                                    {
                                        newsubscrOb.membershipId = newObjectModel.membershipId;
                                        newsubscrOb.monthsCount = 1;
                                        newsubscrOb.Amount = 0;
                                        newsubscrOb.createUserId = newObjectModel.createUserId;
                                        newsubscrOb.updateUserId = newObjectModel.updateUserId;
                                        newsubscrOb.notes = newObjectModel.notes;
                                        newsubscrOb.isActive = newObjectModel.isActive;
                                        subres = subscCntrller.Save(newsubscrOb);
                                        if (subres > 0)
                                        {
                                            return TokenManager.GenerateToken(res.ToString());
                                        }
                                        else
                                        {
                                            return TokenManager.GenerateToken("-1");
                                        }
                                    }
                                    else
                                    {
                                        // "m"
                                        return TokenManager.GenerateToken(res.ToString());
                                    }
                                }
                            }
                            else
                            {
                                return TokenManager.GenerateToken("-1");
                            }

                            //tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

                            //tmpObject.membershipId = newObject.membershipId;
                            //tmpObject.name = newObject.name;

                            //tmpObject.notes = newObject.notes;
                            //tmpObject.createDate = newObject.createDate;

                            //tmpObject.createUserId = newObject.createUserId;
                            //tmpObject.updateUserId = newObject.updateUserId;
                            //tmpObject.isActive = newObject.isActive;

                            //tmpObject.subscriptionType = newObject.subscriptionType;
                            //tmpObject.code = newObject.code;


                            message = tmpObject.membershipId.ToString();
                        }
                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    message = "-1";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        [HttpPost]
        [Route("GetmembershipByAgentId")]
        public string GetmembershipByAgentId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long agentId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        agentId = long.Parse(c.Value);
                    }
                }
                try
                {
                    DateTime dtnow = coctrlr.AddOffsetTodate(DateTime.Now);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var List1 = (
                            from M in entity.memberships
                            join A in entity.agents on M.membershipId equals A.membershipId

                            join S in entity.subscriptionFees
                                on M.membershipId equals S.membershipId
                                into SU
                            join CSH in entity.agentMembershipCash
                                on M.membershipId equals CSH.membershipId
                                into CS
                            from JCS in CS.DefaultIfEmpty()

                            where
                                (
                                    agentId == A.agentId
                                    && M.isActive == 1
                                    && (
                                        M.subscriptionType == "f"
                                        || (M.subscriptionType == "o" && JCS.cashTransId > 0)
                                        || (
                                            (
                                                JCS.subscriptionType == "m"
                                                || JCS.subscriptionType == "y"
                                            )
                                            && JCS.cashTransId > 0
                                            && JCS.endDate >= dtnow
                                            && A.membershipId == JCS.membershipId
                                        )
                                    )
                                )
                            from JSU in SU.DefaultIfEmpty()

                            select new AgenttoPayCashModel
                            {
                                //transNum = JCTR.transNum,
                                //transType = JCTR.transType,
                                // agentMembershipsId = AM.agentMembershipsId,
                                agentMembershipCashId = JCS.agentMembershipCashId,
                                subscriptionFeesId = JSU.subscriptionFeesId,
                                cashTransId = JCS.cashTransId,
                                membershipId = M.membershipId,
                                // agentId = G.agentId,
                                startDate = JCS.startDate,
                                endDate = JCS.endDate,
                                Amount = JSU.Amount,
                                membershipName = M.name,
                                membershipisActive = M.isActive,
                                monthsCount = JSU.monthsCount,
                                subscriptionType = M.subscriptionType,
                                updateDate = JCS.updateDate,
                                cashsubscriptionType = JCS.subscriptionType,
                            }
                        ).OrderBy(X => X.updateDate).ToList();

                        //var List = List1.GroupBy(S => S.agentId).Select(S => new AgenttoPayCashModel
                        //{
                        //    transNum = S.LastOrDefault().transNum,
                        //    transType = S.LastOrDefault().transType,
                        //    //  agentMembershipsId = S.LastOrDefault().agentMembershipsId,
                        //    agentMembershipCashId = S.LastOrDefault().agentMembershipCashId,
                        //    subscriptionFeesId = S.LastOrDefault().subscriptionFeesId,
                        //    cashTransId = S.LastOrDefault().cashTransId,
                        //    membershipId = S.LastOrDefault().membershipId,
                        //    agentId = S.LastOrDefault().agentId,
                        //    startDate = S.LastOrDefault().startDate,
                        //    endDate = S.LastOrDefault().endDate,

                        //    Amount = S.LastOrDefault().Amount,
                        //    agentName = S.LastOrDefault().agentName,
                        //    membershipName = S.LastOrDefault().membershipName,
                        //    membershipisActive = S.LastOrDefault().membershipisActive,
                        //    monthsCount = S.LastOrDefault().monthsCount,
                        //    subscriptionType = S.LastOrDefault().subscriptionType,
                        //    updateDate = S.LastOrDefault().updateDate,
                        //    cashsubscriptionType = S.LastOrDefault().cashsubscriptionType,
                        //}
                        //).Where(S => (S.agentMembershipCashId == null || S.agentMembershipCashId == 0)
                        //|| (S.subscriptionType == "o" && S.agentMembershipCashId > 0 && (S.cashTransId == 0 || S.cashTransId == null) && S.subscriptionType == S.cashsubscriptionType)
                        //|| ((S.subscriptionType == "m" || S.subscriptionType == "y") && (S.agentMembershipCashId != null && S.agentMembershipCashId > 0) && S.subscriptionType == S.cashsubscriptionType)

                        //).ToList();

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
                        var List = List1.LastOrDefault();

                        return TokenManager.GenerateToken(List);
                    }
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        [HttpPost]
        [Route("GetmembershipStateByAgentId")]
        public string GetmembershipStateByAgentId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long agentId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        agentId = long.Parse(c.Value);
                    }
                }
                try
                {
                    DateTime dtnow = coctrlr.AddOffsetTodate(DateTime.Now);

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var List1 = (
                            from M in entity.memberships.Where(x => x.isActive == 1)
                            join A in entity.agents on M.membershipId equals A.membershipId
                            join S in entity.subscriptionFees
                                on M.membershipId equals S.membershipId
                                into SU
                            // join CSH in entity.agentMembershipCash on M.membershipId equals CSH.membershipId into CS
                            //  from JCS in CS.DefaultIfEmpty()
                            from JSU in SU.DefaultIfEmpty()
                            //where (agentId == A.agentId && M.isActive == 1 &&
                            //(M.subscriptionType == "f" ||
                            //(M.subscriptionType == "o" && JCS.cashTransId > 0)
                            //|| ((JCS.subscriptionType == "m" || JCS.subscriptionType == "y") && JCS.cashTransId > 0 && JCS.endDate >= dtnow && A.membershipId == JCS.membershipId)))
                            where (agentId == A.agentId && M.membershipId == A.membershipId)
                            select new AgenttoPayCashModel
                            {
                                //  transNum = JCTR.transNum,
                                //transType = JCTR.transType,
                                // agentMembershipsId = AM.agentMembershipsId,
                                // agentMembershipCashId = A.agentMembershipCash.Where(x => x.agentId==agentId && M.membershipId==x.membershipId).ToList().LastOrDefault().agentMembershipCashId,


                                //agentMembershipCashId = JCS.agentMembershipCashId,
                                agentMembershipcashobjList = A.agentMembershipCash
                                    .Where(x => x.agentId == agentId)
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
                                //  cashTransId = JCS.cashTransId,
                                membershipId = M.membershipId,
                                isFreeDelivery = M.isFreeDelivery,
                                deliveryDiscountPercent = M.deliveryDiscountPercent,
                                agentId = A.agentId,
                                //startDate = JCS.startDate,
                                //endDate = JCS.endDate,

                                Amount = JSU.Amount,
                                membershipName = M.name,
                                membershipcode = M.code,
                                membershipisActive = M.isActive,
                                monthsCount = JSU.monthsCount,
                                subscriptionType = M.subscriptionType,
                                //updateDate = JCS.updateDate,
                                //createDate = JCS.createDate,
                                //cashsubscriptionType = JCS.subscriptionType,

                                invoicesClassesCount = M.invoicesClassMemberships
                                    .Where(X => X.membershipId == M.membershipId)
                                    .ToList()
                                    .Count(),
                                offersCount = M.membershipsOffers
                                    .Where(x => x.membershipId == M.membershipId)
                                    .ToList()
                                    .Count(),

                                //cachpayrowCount = entity.agentMembershipCash.Where(X => X.agentId == agentId).Count(),
                            }
                        )
                        //.OrderBy(X => X.createDate)
                        .ToList();

                        foreach (AgenttoPayCashModel row in List1)
                        {
                            row.agentMembershipcashobj =
                                row.agentMembershipcashobjList.LastOrDefault();
                            if (row.subscriptionType != "f" && row.agentMembershipcashobj != null)
                            {
                                row.endDate = row.agentMembershipcashobj.endDate;
                            }
                            //    row.endDate = row.agentMembershipcashobj==null?null:row.agentMembershipcashobj.endDate;

                            if (row.membershipisActive == 0)
                            {
                                row.membershipStatus = "notactive";
                            }
                            else if (row.subscriptionType == "f")
                            {
                                row.membershipStatus = "valid";
                            }
                            else if (
                                (
                                    row.subscriptionType == "o"
                                    || row.subscriptionType == "y"
                                    || row.subscriptionType == "m"
                                ) && (row.agentMembershipcashobjList.Count() == 0)
                            )
                            {
                                // //no cash record
                                row.membershipStatus = "notpayed";
                            }
                            else if (
                                row.agentMembershipcashobj != null
                                && (row.agentMembershipcashobj.membershipId != row.membershipId)
                            )
                            {
                                //membership changed
                                row.membershipStatus = "notpayed";
                            }
                            else if (
                                row.agentMembershipcashobj != null
                                && (row.agentMembershipcashobj.membershipId == row.membershipId)
                            )
                            { //membership not changed and subscriptionType changed
                                if (
                                    (
                                        (
                                            row.agentMembershipcashobj.subscriptionType == "m"
                                            || row.agentMembershipcashobj.subscriptionType == "y"
                                        ) && (row.subscriptionType == "o")
                                    )
                                    || (
                                        (row.agentMembershipcashobj.subscriptionType == "o")
                                        && (
                                            row.subscriptionType == "y"
                                            || row.subscriptionType == "m"
                                        )
                                    )
                                )
                                {
                                    row.membershipStatus = "notpayed";
                                }
                                else if (
                                    row.subscriptionType == "o"
                                    && row.agentMembershipcashobjList.Count() > 0
                                )
                                {
                                    //subscriptionType =o and not changed and  payed
                                    row.membershipStatus = "valid";
                                }
                                else if (
                                    (row.subscriptionType == "m" || row.subscriptionType == "y")
                                    && !(row.agentMembershipcashobj.endDate >= dtnow)
                                )
                                {
                                    //subscriptionType =y or m  and not changed and  payed and  endDate < dtnow
                                    row.membershipStatus = "expired";
                                }
                                else
                                {
                                    row.membershipStatus = "valid";
                                }

                                // row.membershipStatus = "notpayed";
                            }

                            //else if (row.subscriptionType == "o" && (!(row.cashTransId > 0) || row.cachpayrowCount == 0))
                            //{
                            //    row.membershipStatus = "notpayed";
                            //}
                            //else if ((row.subscriptionType == "m" || row.subscriptionType == "y") && (!(row.cashTransId > 0) || row.cachpayrowCount == 0))
                            //{
                            //    row.membershipStatus = "notpayed";


                            //}
                            //else if ((row.subscriptionType == "m" || row.subscriptionType == "y") && !(row.endDate >= dtnow))
                            //{
                            //    row.membershipStatus = "expired";
                            //}
                            //else
                            //{
                            //    if ((row.subscriptionType == "m" || row.subscriptionType == "y") && (row.cashTransId > 0) && !(row.endDate >= dtnow))
                            //    {
                            //        row.membershipStatus = "expired";
                            //    }
                            //    else
                            //    {
                            //        row.membershipStatus = "valid";
                            //    }

                            //}


                            row.couponsCount = entity.couponsMemberships
                                .Where(x => x.membershipId == row.membershipId)
                                .Count();
                        }
                        var List = List1.LastOrDefault();

                        return TokenManager.GenerateToken(List);
                    }
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }
    }
}
