using ProcurementErp.Services;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.BLL
{
    public class NegotiateBLL
    {
        public static string InsertNegotiationForRequest(RFIRFPComboViewModel inputs, FormCollection collection)
        {
            NegotiationsViewModel model = new NegotiationsViewModel();
            //SOURCING_RFQ_QUOTATION query = new SOURCING_RFQ_QUOTATION();
            SOURCING_NEGOTIATIONS _negotiations = new SOURCING_NEGOTIATIONS();
            SOURCING_NEG_PRICE query = new SOURCING_NEG_PRICE();

            try
            {
                decimal ResponseNumber = Convert.ToDecimal(collection["NegNumber"].ToString());
                var context = new Entities();
                foreach (var vendor in inputs.Vendors)
                {
                    decimal _negNo = Common.GetSequenceB();
                    model.VAT = inputs.Request.NEGVAT;
                    model.VATVALUE = inputs.Request.NEGVATVALUE;
                    model.TOTAL_AMT = inputs.Request.NEGTOTALAMT;
                    model.GRANDTOTAL = inputs.Request.NEGGRANDTOTAL;
                    model.TEMP_ID = inputs.Request.TEMP_ID;
                    model.RES_DATE = DateTime.Now;
                    
                    _negotiations.NEG_NO = _negNo;
                    _negotiations.RESPONSE_NO = ResponseNumber;
                    _negotiations.GRANDTOTAL = model.GRANDTOTAL;
                    _negotiations.TOTAL_AMT = model.TOTAL_AMT;
                    _negotiations.VAT = model.VAT;
                    _negotiations.VATVALUE = model.VATVALUE;
                    _negotiations.RES_DATE = model.RES_DATE;
                    _negotiations.VENDOR_ID = vendor.VENDOR_ID;
                    _negotiations.NEGOTIATOR = "BUYER";
                    _negotiations.TEMP_ID = model.TEMP_ID;
                    _negotiations.STATUS = "";
                    context.SOURCING_NEGOTIATIONS.Add(_negotiations);
                    context.SaveChanges();
                    foreach (var item in inputs.Item)
                    {
                        InsertNegPrice(item, inputs.Request.TEMP_ID, _negNo);
                    }

                    UpdateVendorStatus((decimal)vendor.VENDOR_ID, inputs.Request.TEMP_ID, ResponseNumber);
                    NotifyVendor((decimal)vendor.VENDOR_ID, inputs.Request.TEMP_ID);
                }
                return "success";
            }
            catch (Exception ex)
            {
                Logger.Log("An error occurred, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return "error";
            }
        }

        public static void NotifyVendor(decimal _vendorId,decimal _TempId)
        {
            string contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Negotiate.txt";
            string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
            string content = System.IO.File.ReadAllText(contentPath);
            try
            {
                using(var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(v => v.VENDOR_ID == _vendorId && v.TEMP_NO == _TempId).FirstOrDefault();
                    if(query != null)
                    {
                        content = content.Replace("{CompanyName}", query.VENDOR_NAME);
                        bool MailResponse = SendMail.prepMail(content, null, query.AUTO_EMAIL??query.EMAIL, emailTemplatePath);
                        if (MailResponse)
                        {
                            Logger.Log("Email Sent to " + query.AUTO_EMAIL ?? query.EMAIL, "Info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("An error occurred, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
            }
        }

        public static void UpdateVendorStatus(decimal VendorId, decimal _TempId,decimal NegNo)
        {
            using (var db = new Entities())
            {
                var query = db.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == VendorId && m.TEMP_NO == _TempId).FirstOrDefault();
                if (query != null)
                {
                    query.STATUS = "NEGOTIATING";
                    db.SaveChanges();
                }

                var getNegNum = db.SOURCING_NEGOTIATIONS.Where(m => m.NEG_NO == NegNo).FirstOrDefault();
                if (getNegNum != null)
                {
                    getNegNum.STATUS = "NEGOTIATING";
                    db.SaveChanges();
                }
            }
        }

        public static void InsertNegPrice(RFQITEMViewModel item, decimal? tempId, decimal NegotiationNumber)
        {
            SOURCING_NEG_PRICE neg = new SOURCING_NEG_PRICE();
            using (var context = new Entities())
            {
                neg.ITEM_NO = item.ITEM_NO;
                neg.NEG_NO = NegotiationNumber;
                neg.UNIT_PRICE = item.UNIT_PRICE;
                neg.TOTAL_PRICE = item.TOTAL_PRICE;
                context.SOURCING_NEG_PRICE.Add(neg);
                context.SaveChanges();
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

        internal static bool UpdateNegStatus(string TempNo, string NegNum, string Status)
        {
            decimal _TempNo = Convert.ToDecimal(TempNo);
            decimal _NegNum = Convert.ToDecimal(NegNum);
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_NEGOTIATIONS.Where(m => m.NEG_NO == _NegNum && m.TEMP_ID == _TempNo).FirstOrDefault();
                    if (query != null)
                    {
                        query.STATUS = Status;
                        context.SaveChanges();
                        return true;
                    }
                    else return false;

                }
            }
            catch (Exception ex)
            {
                Logger.Log("An error occurred, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return false;
            }
        }
    }
}