using ProcurementErp.Services;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class ProcessRequest
    {
        private static string DbSchema = ConfigurationManager.AppSettings["DbSchema"];
        private static string Seq = ConfigurationManager.AppSettings["Sequence"];
        private static string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
        public static void ProcessNewRequest(SourceRequestViewModel model, string _EmpNum)
        {
            string Content = "A new sourcing request has been initiated by " + model.INITIATOR_NAME;
                   Content += " , log on to the Procurement application to approve/decline the request.";
            try
            {
                SOURCING_REQUEST _NewRequest = new SOURCING_REQUEST();
                decimal ItemId = GetSequence();
                var InfoFromHRMS = GetInfoFromHRMS.GetSupervisorsDetails(_EmpNum);
                using(Entities context = new Entities())
                {
                    _NewRequest.TEMP_ID = ItemId;
                    _NewRequest.ITEM_CATEGORY = model.ITEM_CATEGORY;
                    _NewRequest.ITEM_DESCRIPTION = model.ITEM_DESCRIPTION;
                    _NewRequest.PREFERED_VENDOR = model.PREFERED_VENDOR;
                    _NewRequest.PREFERED_VENDOR_ID = model.PREFERED_VENDOR_ID;
                    _NewRequest.INITIATING_BRANCH = model.INITIATING_BRANCH;
                    _NewRequest.INITIATOR_NAME = model.INITIATOR_NAME;
                    _NewRequest.INITIATOR_NUMBER = model.INITIATOR_NUMBER;
                    _NewRequest.INITIATING_BRANCHCODE = model.INITIATING_BRANCHCODE;
                    _NewRequest.INITIATING_DEPT = model.INITIATING_DEPT;
                    _NewRequest.INITIATION_DATE = DateTime.Now;
                    _NewRequest.INITIATOR_EMAIL = model.INITIATOR_EMAIL;
                    _NewRequest.EXPECTED_DELIVERY_DATE = model.EXPECTED_DELIVERY_DATE;
                    _NewRequest.INITIATING_DEPTCODE = model.INITIATING_DEPTCODE;
                    _NewRequest.LINE_MANAGER_NAME = InfoFromHRMS.FullName;
                    _NewRequest.LINE_MANAGER_NUM = InfoFromHRMS.EmployeeNumber;
                    _NewRequest.LINE_MANAGER_APPR = "Pending";
                    _NewRequest.LINE_MANAGER_EMAIL = InfoFromHRMS.Email;
                    _NewRequest.ITEM_CATEGORY_ID = model.ITEM_CATEGORY_ID;
                    context.SOURCING_REQUEST.Add(_NewRequest);
                    context.SaveChanges();
                }

                DocumentUploadDownload.UploadDocument(model.Files, model.INITIATING_BRANCHCODE, ItemId);
                bool MailResponse = SendMail.prepMail(Content, InfoFromHRMS.FullName, InfoFromHRMS.Email,emailTemplatePath);
                if (MailResponse)
                {
                    Logger.Log("Email Sent to " + InfoFromHRMS.Email, "Info");
                }
                      
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Logger.Log("Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:", "error");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Logger.Log("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage + " ", "error");
                    }
                }
                 
               // Logger.Log("An error occured upon inserting new request into database. Error: " + ex.StackTrace, "error");
            }
        }

        public static void EditRequest(SourceRequestViewModel model,string branchcode)
        {
            try
            {
                var context = new Entities();
                var query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == model.TEMP_ID).FirstOrDefault();
                if (query != null)
                {

                    query.ITEM_CATEGORY = model.ITEM_CATEGORY;
                    query.ITEM_DESCRIPTION = model.ITEM_DESCRIPTION;
                    query.PREFERED_VENDOR = model.PREFERED_VENDOR;
                    query.PREFERED_VENDOR_ID = model.PREFERED_VENDOR_ID;
                    query.EXPECTED_DELIVERY_DATE = model.EXPECTED_DELIVERY_DATE;
                    query.ITEM_CATEGORY_ID = model.ITEM_CATEGORY_ID;
                    context.SaveChanges();
                    DocumentUploadDownload.UploadDocument(model.Files, branchcode, model.TEMP_ID);
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Logger.Log("Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:", "error");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Logger.Log("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage + " ", "error");
                    }
                }

                // Logger.Log("An error occured upon inserting new request into database. Error: " + ex.StackTrace, "error");
            }
        }

        public static bool UpdateRequestApproval(SourceRequestViewModel model)
        {
            string Content = "A new sourcing request has been initiated by ";
            Content += " , log on to the Procurement application to treat the request.";
            try
            {
                DateTime ApprovalDate = DateTime.Now;

                string[] split = model.ITEM_CATEGORY_ID.Split('>');
                var _categoryId = Convert.ToDecimal(split[0].ToString());

                using (var context = new Entities())
                {
                    //gets the procurement buyer based on the category ID.

                    var getBuyer = context.SOURCING_USERS.Where(u => u.CATEGORY_ID == _categoryId).FirstOrDefault();
                    var query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == model.TEMP_ID).FirstOrDefault();
                    if (query != null)
                    {
                        query.LINE_MANAGER_APPR = model.LINE_MANAGER_APPR;
                        query.LINE_MANAGER_APPR_DATE = ApprovalDate;
                        query.LINE_MANAGER_COMMENT = model.LINE_MANAGER_COMMENT;
                        query.PROCUREMENT_BUYER = getBuyer.NAME;
                        query.PROCUREMENT_BUYER_ID = getBuyer.USER_ID;
                        context.SaveChanges();
                        //send to procurement buyer based on category
                    }

                    if (model.LINE_MANAGER_APPR != "DECLINED")
                    {
                        bool MailResponse = SendMail.prepMail(Content, getBuyer.NAME, getBuyer.EMAIL, emailTemplatePath);
                        if (MailResponse)
                        {
                            Logger.Log("Email Sent to " + getBuyer.EMAIL, "Info");
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error updating approval. Error: "+ ex.Message + 
                    "------------------------------------------------"+ ex.StackTrace, 
                    "error");
                return false;
            }
        }


        public static bool UpdateSourceRequestApproval(SourceRequestViewModel model)
        {
            try
            {
                DateTime ApprovalDate = DateTime.Now;

                

                using (var context = new Entities())
                {
                    //gets the procurement buyer based on the category ID.

                   // var getBuyer = context.SOURCING_USERS.Where(u => u.CATEGORY_ID == _categoryId).FirstOrDefault();
                    var query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == model.TEMP_ID).FirstOrDefault();
                    if (query != null)
                    {
                        query.BUYER_STATUS = model.BUYER_STATUS;
                        query.BUYER_COMMENT = model.BUYER_COMMENT;
                        context.SaveChanges();
                        //send to procurement buyer based on category
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error updating approval. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error"); 
                return false;
            }
        }

        //process Document Upload 

        
        public static int GetSequence()
        {
            using (var db = new Entities())
            {
                decimal val = db.Database.SqlQuery<decimal>("select " + DbSchema + "." + Seq + ".NEXTVAL from dual", new object[0]).FirstOrDefault<decimal>();
                return Convert.ToInt32(val);
            }
        }
    }
}