using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class ManageVendor
    {
        /*
         * Generate username and password, for vendors 
         * if non exist, send email to selected vendors
         */

        //check if vendor has username on the table
        private static bool CheckVendorLoginDetails(decimal _VendorID)
        {
            try
            {
                using (var context = new Entities())
                {
                    int _count = context.SOURCING_VENDOR_LOGIN_DETAILS.Where(m => m.VENDOR_ID == _VendorID).Count();
                    if (_count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to check SOURCING_VENDOR_LOGIN_DETAILS table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return false;
            }
        }

        //retreive the username and password 
        private static VendorLoginViewModel GetUsernamePassword(decimal _VendorID)
        {
            VendorLoginViewModel details = new VendorLoginViewModel();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_VENDOR_LOGIN_DETAILS.Where(m => m.VENDOR_ID == _VendorID).FirstOrDefault();
                    if (query != null)
                    {
                        details.USERNAME = query.USERNAME;
                        details.PASSWORD = query.PASSWORD;
                        return details;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to retrieve vendor username and passowrd from SOURCING_VENDOR_LOGIN_DETAILS table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        private static VendorLoginViewModel GenerateUsernamePassword(decimal _VendorID)
        {
            SOURCING_VENDOR_LOGIN_DETAILS _vendor = new SOURCING_VENDOR_LOGIN_DETAILS();
            VendorLoginViewModel vendor = new VendorLoginViewModel();
            var context = new Entities();
            vendor.USERNAME = "vendor_" + _VendorID;
            vendor.PASSWORD = Common.RandomChar(8) + _VendorID;

            _vendor.USERNAME = vendor.USERNAME;
            _vendor.PASSWORD = vendor.PASSWORD;
            _vendor.VENDOR_ID = _VendorID;
            context.SOURCING_VENDOR_LOGIN_DETAILS.Add(_vendor);
            context.SaveChanges();
            return vendor;        
        }

        //if vendorId is null 


        public static string ProcessVendorEmail(decimal _VendorID, decimal _TempNo, string CompanyName,string UpdateVendor = null)
        {
            string contentPath = string.Empty;
            if (UpdateVendor == null)
            {
                contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\content.txt";
            }
            else
            {
                contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\updatecontent.txt";
            }
             
            string content = System.IO.File.ReadAllText(contentPath);
            VendorLoginViewModel vendor = new VendorLoginViewModel();
            string query = "";
            bool CheckLoginDetails = CheckVendorLoginDetails(_VendorID);
            if (CheckLoginDetails == true)
            {
                vendor = GetUsernamePassword(_VendorID);
            }
            else
            {
                vendor = GenerateUsernamePassword(_VendorID);
            }

            using (var context = new Entities())
            {
                try
                {
                    query = (from a in context.SOURCING_REF_TEMP_LINK
                             join b in context.SOURCING_REQUEST_TYPE
                             on a.REQ_TYPE equals b.REQUEST_ID
                             where a.TEMP_NO == _TempNo
                             select b.REQUEST_NAME).First().ToString();

                }
                catch (Exception ex)
                {
                    Logger.Log("Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                    return null;
                }
            }
            content = content.Replace("{RequestType}", query);
            content = content.Replace("{username}", vendor.USERNAME);
            content = content.Replace("{password}", vendor.PASSWORD);
            content = content.Replace("{CompanyName}", CompanyName);
            return content;
        }
    }
}