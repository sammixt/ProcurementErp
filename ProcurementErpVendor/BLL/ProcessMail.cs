using ProcurementErpVendor.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.BLL
{
    public class ProcessMail
    {
        private decimal VendorId;
        private string CreatorType;
        private string RecipientType;
        private DateTime date;
        private decimal ID;

        public ProcessMail(decimal _vendorId, string _creatorType,string _recipientType)
        {
            this.VendorId = _vendorId;
            this.CreatorType = _creatorType;
            this.RecipientType = _recipientType;
            this.date = DateTime.Now;
            this.ID = Common.GetSequence();
        }

        public bool SendMail(MessageViewModel model)
        {
            model.CREATORID = this.VendorId;
            model.CREATORTYPE = this.CreatorType;
            model.DATE_CREATED = this.date;
            model.ID = this.ID;

            SOURCING_MESSAGE _Message = new SOURCING_MESSAGE();
            SOURCING_MESSAGE_RECIPIENT _Recipient = new SOURCING_MESSAGE_RECIPIENT();
            _Message.ID = model.ID;
            _Message.CREATORID = model.CREATORID;
            _Message.CREATORTYPE = model.CREATORTYPE;
            _Message.DATE_CREATED = model.DATE_CREATED;
            _Message.MAIL_SUBJECT = model.MAIL_SUBJECT;
            _Message.MESSAGE_BODY = model.MESSAGE_BODY;
            _Message.TEMP_ID = model.TEMP_ID;
            _Recipient.IS_READ = 0;
            _Recipient.MESSAGE_ID = model.ID;
            _Recipient.RECIPIENT_ID = model.INITIALIZER;
            _Recipient.RECIPIENT_TYPE = this.RecipientType;
            using (var context = new Entities())
            {
                try
                {
                    context.SOURCING_MESSAGE.Add(_Message);
                    context.SOURCING_MESSAGE_RECIPIENT.Add(_Recipient);
                    context.SaveChanges();
                    return true;
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
                    //Logger.Log("Error: " + ex.StackTrace, "error");
                    return false;
                }
                
            }
        }

        public List<MessageViewModel> GetSentMails()
        {
            List<MessageViewModel> msg = new List<MessageViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_MESSAGE.Where(m => m.CREATORID == this.VendorId && m.CREATORTYPE == this.CreatorType).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            msg.Add(new MessageViewModel
                            {
                                ID = item.ID,
                                MAIL_SUBJECT = item.MAIL_SUBJECT,
                                MESSAGE_BODY = item.MESSAGE_BODY,
                                DATE_CREATED = item.DATE_CREATED
                            });
                        }
                        return msg;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " " + ex.StackTrace, "error");
                return null;
            }
        } 

        public List<MessageViewModel> GetSentMails(string TempId = null, string MsgId = null)
        {
            List<MessageViewModel> msg = new List<MessageViewModel>();
            List<SOURCING_MESSAGE> query = new List<SOURCING_MESSAGE>();
            try
            {
                using (var context = new Entities())
                {
                    if (TempId != null)
                    {
                        decimal _TempId = Convert.ToDecimal(TempId);
                        query = context.SOURCING_MESSAGE.Where(m => m.TEMP_ID == _TempId && m.CREATORID == this.VendorId && m.CREATORTYPE == this.CreatorType).ToList();
                    }
                    else if (MsgId != null)
                    {
                        decimal _MsgId = Convert.ToDecimal(MsgId);
                        query = context.SOURCING_MESSAGE.Where(m => m.ID == _MsgId && m.CREATORID == this.VendorId && m.CREATORTYPE == this.CreatorType).ToList();
                    }
                   
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            msg.Add(new MessageViewModel
                            {
                                ID = item.ID,
                                MAIL_SUBJECT = item.MAIL_SUBJECT,
                                MESSAGE_BODY = item.MESSAGE_BODY,
                                DATE_CREATED = item.DATE_CREATED
                            });
                        }
                        return msg;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " " + ex.StackTrace, "error");
                return null;
            }
        } 

    }
}