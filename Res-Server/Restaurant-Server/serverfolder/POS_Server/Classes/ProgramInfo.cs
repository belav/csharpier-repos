using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Classes
{
    public class ProgramInfo
    {
        public int getBranchCount()
        {
            int branchCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                branchCount = entity.ProgramDetails.Select(x => x.branchCount).SingleOrDefault();
            }
            return branchCount;
        }
        public int getStroeCount()
        {
            int storeCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                storeCount = entity.ProgramDetails.Select(x => x.storeCount).SingleOrDefault();
            }
            return storeCount;
        }
        public int getPosCount()
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
               var posCount = entity.ProgramDetails.Select(x => x.posCount).SingleOrDefault();
                return (int)posCount;
            }
        }
        public int getUserCount()
        {
            int userCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                userCount = entity.ProgramDetails.Select(x => x.userCount).SingleOrDefault();
            }
            return userCount;
        }
        public int getVendorCount()
        {
            int vendorCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                vendorCount = entity.ProgramDetails.Select(x => x.vendorCount).SingleOrDefault();
            }
            return vendorCount;
        }
        public int getCustomerCount()
        {
            int customerCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                customerCount = entity.ProgramDetails.Select(x => x.customerCount).SingleOrDefault();
            }
            return customerCount;
        }
        public int getItemCount()
        {
            int itemCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                itemCount = entity.ProgramDetails.Select(x => x.itemCount).SingleOrDefault();
            }
            return itemCount;
        }
        public int getSaleinvCount()
        {
            int invCount = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                invCount = entity.ProgramDetails.Select(x => x.saleinvCount).SingleOrDefault();
            }
            return invCount;
        }
    }
}