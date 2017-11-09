using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.BLL
{
    public static class Common
    {
        private static string DbSchema = ConfigurationManager.AppSettings["DbSchema"];
        private static string Seq = ConfigurationManager.AppSettings["Sequence"];
        private static string SeqB = ConfigurationManager.AppSettings["SequenceB"];

        
        public static DateTime? ParseDate(string data)
        {
            DateTime? NulDate = null;
            DateTime date = new DateTime();
            var formats = new[] { "MM/dd/yyyy"};
            if (DateTime.TryParseExact(data, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            else
            {
                return NulDate;
            }
        }

        public static int GetSequence()
        {
            using (var db = new Entities())
            {
                decimal val = db.Database.SqlQuery<decimal>("select " + DbSchema + "." + Seq + ".NEXTVAL from dual", new object[0]).FirstOrDefault<decimal>();
                return Convert.ToInt32(val);
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

        public static decimal? getRequestType(decimal TempId)
        {
            using (var context = new Entities())
            {
                decimal? query = context.SOURCING_REF_TEMP_LINK.Where(m => m.TEMP_NO == TempId).Select(t => t.REQ_TYPE).FirstOrDefault();
                return query;
            }
        }

        public static decimal? getInitiatorsId(decimal TEMP_NO)
        {
            decimal? RequestType = getRequestType(TEMP_NO);
            decimal? InitiatorsId = 0;
            using (var context = new Entities())
            {
                if(RequestType == 2)
                {
                    InitiatorsId = context.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == TEMP_NO).Select(m => m.INITIATOR).FirstOrDefault();
                }else if(RequestType == 3)
                {
                    InitiatorsId = context.SOURCING_RFQ_REQ.Where(m => m.TEMP_NO == TEMP_NO).Select(m => m.INITIATOR_ID).FirstOrDefault();
                }
                return InitiatorsId;
            }
        }

        public static string getInitiatorsEmail(decimal Temp_No)
        {
            string Email = string.Empty;
            decimal InitiatorsId = (decimal)getInitiatorsId(Temp_No);
            using (var context = new Entities())
            {
                Email = context.SOURCING_USERS.Where(m => m.USER_ID == InitiatorsId).Select(m => m.EMAIL).FirstOrDefault();
            }
            return Email;
        }

        public static void NotifyProcurement(string VendorName, decimal TempNo, string contentPath, string RefNum = null)
        {
            string Email = getInitiatorsEmail(TempNo);
            string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
            string content = System.IO.File.ReadAllText(contentPath);
            try
            {

                if(RefNum != null)
                {
                    content = content.Replace("{RefNum}", RefNum);
                }
                content = content.Replace("{VendorName}", VendorName);
                bool MailResponse = SendMail.prepMail(content, null, Email, emailTemplatePath);
                if (MailResponse)
                {
                    Logger.Log("Email Sent to " + Email, "Info");
                }


            }
            catch (Exception ex)
            {
                Logger.Log("An error occurred, Error: " + ex.StackTrace, "error");
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

        public static MvcHtmlString FormatNumber(this HtmlHelper helper, decimal? Amount)
        {
            var formatter = new System.Globalization.CultureInfo("HA-LATN-NG");
            formatter.NumberFormat.CurrencySymbol = "₦";
            string _Amount = string.Format(formatter, "{0:C2}", Amount);
            return new MvcHtmlString(_Amount);
        }

        public static MvcHtmlString GetPathForAjaxCall(this HtmlHelper helper)
        {
            string _path = ConfigurationManager.AppSettings["PathForAjaxCall"].ToString();
            return new MvcHtmlString(_path);
        }
    }
}