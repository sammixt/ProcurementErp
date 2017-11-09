using ProcurementErp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class ProcurementResolveBLL
    {
        public string TempID;
        public string VendorID;
        public string Resolve;
        public string Resolution;

        public void UpdateVendorResolve()
        {
            decimal? _vendorId = Convert.ToDecimal(VendorID);
            decimal? _tempId = Convert.ToDecimal(TempID);
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == _vendorId && m.TEMP_NO == _tempId).FirstOrDefault();
                    if (query != null)
                    {
                        query.STATUS = Resolve;
                        query.VERDICT_ISSUE_DATE = DateTime.Now;
                        context.SaveChanges();
                    } 
                }

                NotifyVendor((decimal)_vendorId, (decimal)_tempId, "a Authority to Proceed");
            }
            catch (Exception ex)
            {
                Logger.Log("Error : " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
            }
        }


        public bool UpdateVendorResolveII()
        {
            decimal? _vendorId = Convert.ToDecimal(VendorID);
            decimal? _tempId = Convert.ToDecimal(TempID);
            decimal? ReqType;
            ReqType = NegotiateBLL.getRequestType((decimal)_tempId);
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == _vendorId && m.TEMP_NO == _tempId).FirstOrDefault();
                    if (query != null)
                    {
                        query.STATUS = Resolve;
                        query.VERDICT_ISSUE_DATE = DateTime.Now;
                        context.SaveChanges();
                    }

                    if (ReqType == 2)
                    {
                        var rfpReq = context.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == _tempId).FirstOrDefault();
                        if (rfpReq != null)
                        {
                            rfpReq.STATUS = Resolution;
                            context.SaveChanges();
                            UpdateBranchAndDeptRequestTable((decimal)rfpReq.MAPPING);
                        }
                    }
                    else if (ReqType == 3)
                    {
                        var rfqReq = context.SOURCING_RFQ_REQ.Where(m => m.TEMP_NO == _tempId).FirstOrDefault();
                        if (rfqReq != null)
                        {
                            rfqReq.STATUS = Resolution;
                            context.SaveChanges();
                            UpdateBranchAndDeptRequestTable((decimal)rfqReq.MAPPING);
                        }
                    }

                    return true;
                    //NotifyVendor((decimal)_vendorId, (decimal)_tempId, "a Authority to Proceed");
                }

              
            }
            catch (Exception ex)
            {
                Logger.Log("Error : " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return false;
            }
        }

        public static void UpdateBranchAndDeptRequestTable(decimal MappingId)
        {
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == MappingId).FirstOrDefault();
                    if (query != null)
                    {
                        query.BUYER_STATUS = "TREATED";
                        query.BUYER_COMMENT = "View the request and Log on ERP";
                        context.SaveChanges();

                        NotifyRequester(query.INITIATOR_NAME, query.LINE_MANAGER_NAME, (DateTime)query.INITIATION_DATE, (DateTime)query.LINE_MANAGER_APPR_DATE, query.INITIATOR_EMAIL, query.LINE_MANAGER_EMAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error : " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
            }
        }


        public static void NotifyVendor(decimal _vendorId, decimal _TempId, string resolution)
        {
            string contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Resolution.txt";
            string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
            string content = System.IO.File.ReadAllText(contentPath);
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(v => v.VENDOR_ID == _vendorId && v.TEMP_NO == _TempId).FirstOrDefault();
                    if (query != null)
                    {
                        content = content.Replace("{CompanyName}", query.VENDOR_NAME);
                        content = content.Replace("{Resolve}", resolution);
                        bool MailResponse = SendMail.prepMail(content, null, query.AUTO_EMAIL ?? query.EMAIL, emailTemplatePath);
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

        public static void NotifyRequester(string InitiatorsName, string Approver, DateTime InitiationDate,DateTime ApprovalDate,string InitiatorsEmail, string ApproversEmail)
        {  
             string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
             string Content = "The sourcing request initiated by " + InitiatorsName;
             Content += " on " + InitiationDate.ToShortDateString() + "and approved by " + Approver +" on";
             Content += ApprovalDate.ToShortDateString() + "has been treated. Log on to the sourcing application in order to log the request on ERP.";
             bool MailResponse = SendMail.prepMail(Content, InitiatorsName, InitiatorsEmail, emailTemplatePath);
             bool MailResponseII = SendMail.prepMail(Content, Approver, ApproversEmail, emailTemplatePath);
             if (MailResponse)
             {
                 Logger.Log("Email Sent to " + InitiatorsEmail + " and " + ApproversEmail, "Info");
             }
        }
    }
}