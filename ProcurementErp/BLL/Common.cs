using ProcurementErp.Services;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
//using System.Web.WebPages.Html;
using System.Web.Mvc;

namespace ProcurementErp.BLL
{
    public static class Common
    {

        private static string DbSchema = ConfigurationManager.AppSettings["DbSchema"];
        private static string SeqB = ConfigurationManager.AppSettings["SequenceB"];

        public static List<Category> GetMainCategory()
        {
            List<Category> _Categories = new List<Category>();
            try
            {
                using (Entities context = new Entities())
                {
                    _Categories = (from x in context.SOURCING_CATEGORIES
                                   where x.CATEGORY_PARENT == 0
                                   select new Category
                                   {
                                       CATEGORY_NUM = x.CATEGORY_NUM,
                                       CATEGORY_NAME = x.CATEGORY_NAME
                                   }).ToList();
                }
                return _Categories;
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to retrieve categories. Error Msg: " + ex.Message, "error");
                return null;
            }
        }

        public static List<Category> GetCategoryByParentId(decimal? id)
        {
            List<Category> _Categories = new List<Category>();
            try
            {
                using (Entities context = new Entities())
                {
                    _Categories = (from x in context.SOURCING_CATEGORIES
                                   where x.CATEGORY_PARENT == id
                                   select new Category
                                   {
                                       CATEGORY_NUM = x.CATEGORY_NUM,
                                       CATEGORY_NAME = x.CATEGORY_NAME
                                   }).ToList();
                }
                return _Categories;
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to retrieve categories. Error Msg: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static List<SelectListItem> GetVendorsList()
        {
            List<SelectListItem> _listItems = new List<SelectListItem>();
            var _GetAllVendors = GetVendors.GetAllVendors();
            foreach (var item in _GetAllVendors)
            {
                _listItems.Add(new SelectListItem
                {
                    Text = item.VENDOR_NAME + " ----- "+ item.EMAIL_ADDRESS,
                    Value = Convert.ToString(item.VENDOR_ID)
                });
            }
            return _listItems;
            
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomChar(int length)
        {
            Random random = new Random();
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz@#*$£";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string CheckReferenceNum(string _refNum)
        {
            try
            {
                using (var context = new Entities())
                {
                    int _count = context.SOURCING_REF_TEMP_LINK.Where(m => m.REF_NO == _refNum).Count();
                    if (_count > 0)
                    {
                        return null;
                    }
                    else
                    {
                        return _refNum;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to check ref num from Ref_tempL=_link Table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return "error"; 
            }
        }

        //Saves to sourcing contact person table
        public static void SaveChanges(SOURCING_CONTACTPERSON model)
        {
            try
            {
                using (var context = new Entities())
                {
                    context.SOURCING_CONTACTPERSON.Add(model);
                    context.SaveChanges();
                }
               
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to save to ContactPerson Table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                
            }
        }

        internal static string GenerateReferenceNumber(string _Type)
        {
            string RandomNum = Common.RandomString(6);
            string RefNum = _Type + RandomNum;
            return RefNum;
        }

        public static string InsertVendor(ContactedVendorViewModel[] item, string RequestType, decimal? RequestTypeCode)
        {
            SOURCING_CONTACTEDVENDOR _item = new SOURCING_CONTACTEDVENDOR();
            SOURCING_REF_TEMP_LINK _refTempLink = new SOURCING_REF_TEMP_LINK();
            string ReferenceNumber = "";
            try
            {
                var TempId = HttpContext.Current.Session["TempId"].ToString();
                decimal _tempId = Convert.ToDecimal(TempId);

                foreach (var vendor in item)
                {
                    using (var context = new Entities())
                    {
                        _item.TEMP_NO = _tempId;
                        _item.VENDOR_ID = vendor.VENDOR_ID;
                        _item.AUTO_EMAIL = vendor.AUTO_EMAIL;
                        _item.VENDOR_NAME = vendor.VENDOR_NAME;
                        _item.STATUS = "NEW REQUEST";
                        context.SOURCING_CONTACTEDVENDOR.Add(_item);
                        context.SaveChanges();
                    }
                }
                //generate reference number and ensure its unique.
                do
                {
                    string _ReferenceNumber = GenerateReferenceNumber(RequestType);
                    ReferenceNumber = CheckReferenceNum(_ReferenceNumber);
                } while (ReferenceNumber == null);

                if (!string.IsNullOrWhiteSpace(ReferenceNumber))
                {
                    _refTempLink.REF_NO = ReferenceNumber;
                    _refTempLink.REQ_TYPE = RequestTypeCode;
                    _refTempLink.TEMP_NO = _tempId;
                    using (var db = new Entities())
                    {
                        db.SOURCING_REF_TEMP_LINK.Add(_refTempLink);
                        db.SaveChanges();
                    }
                }
                return TempId;
            }
            catch (Exception ex)
            {
                Logger.Log("Error inserting into Contacted Vendors Table table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static string InsertVendorForUpdate(ContactedVendorViewModel[] item, string RequestType, decimal? RequestTypeCode)
        {
            SOURCING_CONTACTEDVENDOR _item = new SOURCING_CONTACTEDVENDOR();
            SOURCING_REF_TEMP_LINK _refTempLink = new SOURCING_REF_TEMP_LINK();
            var TempId = HttpContext.Current.Session["TempId"].ToString();
            decimal _tempId = Convert.ToDecimal(TempId);
            var context = new Entities();
            try
            {
                
                if (item != null)
                {
                    foreach (var vendor in item)
                    {

                        if (vendor.ID == 0)
                        {
                            _item.TEMP_NO = _tempId;
                            _item.VENDOR_ID = vendor.VENDOR_ID;
                            _item.AUTO_EMAIL = vendor.AUTO_EMAIL;
                            _item.VENDOR_NAME = vendor.VENDOR_NAME;
                            _item.STATUS = "NEW REQUEST";
                            context.SOURCING_CONTACTEDVENDOR.Add(_item);
                            context.SaveChanges();
                        }
                        
                    }

                    
                }
               
            }
            catch (Exception ex)
            {
                Logger.Log("Error inserting into Contacted Vendors Table table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");  
            }
            finally
            {
                context.Dispose();
            }
            return TempId;
        }

        public static List<ContactedVendorViewModel> GetContactedVendor()
        {
            List<ContactedVendorViewModel> _CONTACTEDVENDOR = new List<ContactedVendorViewModel>();
            var TempId = HttpContext.Current.Session["TempId"].ToString();
            decimal _tempId = Convert.ToDecimal(TempId);
            try
            {
                var context = new Entities();
                var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == _tempId).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        _CONTACTEDVENDOR.Add(new ContactedVendorViewModel
                        {
                            ID = item.ID,
                            VENDOR_ID = item.VENDOR_ID,
                            VENDOR_NAME = item.VENDOR_NAME,
                            AUTO_EMAIL = item.AUTO_EMAIL
                        });
                    }
                }
                else
                {
                    _CONTACTEDVENDOR = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error inserting into Contacted Vendors Table table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                _CONTACTEDVENDOR = null;
            }
            return _CONTACTEDVENDOR;
        }

        public static bool ProcessSummary(string UpdateVendor = null)
        {
            string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
            List<ContactedVendorViewModel> _ContactedVendors = new List<ContactedVendorViewModel>();
            string EmailContent = "";
            var TempId = HttpContext.Current.Session["TempId"].ToString();
            decimal _tempId = Convert.ToDecimal(TempId);
            try
            {
                var context = new Entities();
                var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == _tempId).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        EmailContent = ManageVendor.ProcessVendorEmail((decimal)item.VENDOR_ID, _tempId, item.VENDOR_NAME, UpdateVendor);
                        bool MailResponse = SendMail.prepMail(EmailContent, null, item.AUTO_EMAIL, emailTemplatePath);
                        if (MailResponse)
                        {
                            Logger.Log("Email Sent to " + item.EMAIL, "Info");
                        }
                    }

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                Logger.Log("Unable to retrieve Contacted Vendor. Error: " + ex.Message, "error");
                return false;
            }
        }

        public static string GetFilePath(decimal FileID)
        {
            using (var context = new Entities())
            {
                string query = context.SOURCING_REQUEST_FILES.Where(m => m.FIILE_ID == FileID).Select(m => m.FILE_PATH).First().ToString();
                return query;
            }
        }


        internal static object CloseRequest(string TempNo)
        {
            decimal _TempNo  = Convert.ToDecimal(TempNo);
            decimal? ReqType = NegotiateBLL.getRequestType(_TempNo);
            using (var db = new Entities())
            {
                if(ReqType == 2 || ReqType == 1)
                {
                    var query = db.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == _TempNo).FirstOrDefault();
                    query.STATUS = "CLOSED";
                    db.SaveChanges();
                }
                else if (ReqType == 3)
                {
                    var query = db.SOURCING_RFQ_REQ.Where(m => m.TEMP_NO == _TempNo).FirstOrDefault();
                    query.STATUS = "CLOSED";
                    db.SaveChanges();
                }
                return true;
                
            }
        }

        public static void UpdateSourcingRequestTableWithMappingInfo(decimal TempId, decimal RequestId)
        {
            using (var context = new Entities())
            {
                var query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == TempId).FirstOrDefault();
                if (query != null)
                {
                    query.MAPTOREQUEST = RequestId;
                    context.SaveChanges();
                }
            }
        }

        public static int GetSequenceB()
        {
            using (var db = new Entities())
            {
                decimal val = db.Database.SqlQuery<decimal>("select " + DbSchema + "." + SeqB + ".NEXTVAL from dual", new object[0]).FirstOrDefault<decimal>();
                return Convert.ToInt32(val);
            }
        }

        //get users in Procurement
        public static List<SelectListItem> GetUsersInProcurement()
        {

            List<SelectListItem> _listItems = new List<SelectListItem>();
            var UserInProcurement = GetInfoFromHRMS.GetUsersInProcurementDept();
            foreach (var item in UserInProcurement)
            {
                _listItems.Add(new SelectListItem
                {
                    Text = item.FullName,
                    Value = GetUserName(item.Email)
                });
            }
            return _listItems;

        }

        public static string GetUserName(string _email = null)
        {
            string _UserName = string.Empty;
            if (_email != null)
            {
                string[] _InputEmail = _email.Split('@');
                _UserName = _InputEmail[0].ToString();
            }
            else
            {
                _UserName = null;
            }
            return _UserName;
        }

        public static MvcHtmlString FormatNumber(this HtmlHelper helper, decimal? Amount)
        {
            var formatter = new System.Globalization.CultureInfo("HA-LATN-NG");
            formatter.NumberFormat.CurrencySymbol = "₦";
            string _Amount = string.Format(formatter,"{0:C2}", Amount);
            return new MvcHtmlString(_Amount);
        }

        public static MvcHtmlString GetPathForAjaxCall(this HtmlHelper helper)
        {
            string _path = ConfigurationManager.AppSettings["PathForAjaxCall"].ToString();
            return new MvcHtmlString(_path);
        }
    }
}