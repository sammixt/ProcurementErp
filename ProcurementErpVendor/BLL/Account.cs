using ProcurementErpVendor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.BLL
{
    public class Account
    {
        public static LoginViewModel AuthenticateUser(LoginViewModel model)
        {
            LoginViewModel user = new LoginViewModel();
            try
            {
                using (Entities context = new Entities())
                {
                    var query = context.SOURCING_VENDOR_LOGIN_DETAILS.Where(m => m.USERNAME == model.UserName && m.PASSWORD == model.Password).FirstOrDefault();
                    if (query != null)
                    {
                        user.VendorId = (decimal)query.VENDOR_ID;
                        user.UserName = query.USERNAME;
                        user.ID = (decimal)query.ID;
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log("Error Authenticating User. Error: " + ex.StackTrace, "error");
                return null;
            }
        }

        public static VendorViewModel VendorDetails(decimal UserId)
        {
            VendorViewModel Vendor = new VendorViewModel();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == UserId).FirstOrDefault();
                    if (query != null)
                    {
                        Vendor.VENDOR_NAME = query.VENDOR_NAME;
                        return Vendor;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
                return null;
            }

        }
    }
}