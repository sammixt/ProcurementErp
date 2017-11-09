using ProcurementErpVendor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.BLL
{
    public class ProcessResponse
    {
        public  List<RFQITEMViewModel> items;
        public  FormCollection vendor;
        public string VendorName;
        public string RefNUmber;
        public decimal _VendorId;
        public  void InsertVendorResponseToItems()
        {
            NegotiationsViewModel model = new NegotiationsViewModel();
             //SOURCING_RFQ_QUOTATION query = new SOURCING_RFQ_QUOTATION();
            SOURCING_NEGOTIATIONS _negotiations = new SOURCING_NEGOTIATIONS();
            SOURCING_NEG_PRICE query = new SOURCING_NEG_PRICE();
            model.VAT = Convert.ToDecimal(vendor["VAT"].ToString());
            model.VATVALUE = Convert.ToDecimal(vendor["VATVALUE"].ToString());
            model.TOTAL_AMT = Convert.ToDecimal(vendor["TOTALBFTAX"].ToString());
            model.GRANDTOTAL = Convert.ToDecimal(vendor["GRANDTOTAL"].ToString());
            model.TEMP_ID = Convert.ToDecimal(vendor["TEMP_NO"].ToString());
            model.RES_DATE = DateTime.Now;
            decimal _negNo = Common.GetSequenceB();
            _negotiations.NEG_NO = _negNo;
            _negotiations.GRANDTOTAL = model.GRANDTOTAL;
            _negotiations.TOTAL_AMT = model.TOTAL_AMT;
            _negotiations.VAT = model.VAT;
            _negotiations.VATVALUE = model.VATVALUE;
            _negotiations.RES_DATE = model.RES_DATE;
            _negotiations.RESPONSE_NO = model.RESPONSE_NO ?? 0;
            _negotiations.VENDOR_ID = _VendorId;
            _negotiations.NEGOTIATOR = "VENDOR";
            _negotiations.TEMP_ID = model.TEMP_ID;
            _negotiations.STATUS = "";
            try
            {
                using (var context = new Entities())
                {
                    context.SOURCING_NEGOTIATIONS.Add(_negotiations);
                    context.SaveChanges();
                    foreach (var item in this.items)
                    {
                        query.ITEM_NO = item.ITEM_NO;
                        query.NEG_NO = _negNo;
                        query.UNIT_PRICE = item.UNIT_PRICE;
                        query.TOTAL_PRICE = item.TOTAL_PRICE;
                        context.SOURCING_NEG_PRICE.Add(query);
                        context.SaveChanges();           
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
            }
        }

        public void UpdateVendorResponseToItems()
        {
            SOURCING_NEG_PRICE query = new SOURCING_NEG_PRICE();
            NegotiationsViewModel model = new NegotiationsViewModel();
            //SOURCING_RFQ_QUOTATION query = new SOURCING_RFQ_QUOTATION();
            model.VAT = Convert.ToDecimal(vendor["VAT"].ToString());
            model.VATVALUE = Convert.ToDecimal(vendor["VATVALUE"].ToString());
            model.TOTAL_AMT = Convert.ToDecimal(vendor["TOTALBFTAX"].ToString());
            model.GRANDTOTAL = Convert.ToDecimal(vendor["GRANDTOTAL"].ToString());
            //model.TEMP_ID = Convert.ToDecimal(vendor["TEMP_NO"].ToString());
            model.NEG_NO = Convert.ToDecimal(vendor["NEGNO"].ToString());
            try
            {
                using (var context = new Entities())
                {
                    var updateNegotiationTable = context.SOURCING_NEGOTIATIONS.Where(m => m.NEG_NO == model.NEG_NO).FirstOrDefault();
                    if (updateNegotiationTable != null)
                    {
                        updateNegotiationTable.VAT = model.VAT;
                        updateNegotiationTable.VATVALUE = model.VATVALUE;
                        updateNegotiationTable.GRANDTOTAL = model.GRANDTOTAL;
                        updateNegotiationTable.TOTAL_AMT = model.TOTAL_AMT;
                        context.SaveChangesAsync();
                    }

                    foreach (var item in this.items)
                    {
                        var addItem = context.SOURCING_NEG_PRICE.Where(m => m.ID == item.ID).FirstOrDefault();
                        if (addItem != null)
                        {
                            addItem.UNIT_PRICE = item.UNIT_PRICE;
                            addItem.TOTAL_PRICE = item.TOTAL_PRICE;
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
            }
        }

        public void InsertSuppliesInfo()
        {
            string contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\VendorResponse.txt";

            ContactedVendorViewModel model = new ContactedVendorViewModel();
            model.TEMP_NO = Convert.ToDecimal(vendor["TEMP_NO"].ToString());
            model.RESPONSE_DATE = DateTime.Now;
            model.ADDRESS = vendor["ADDRESS"].ToString() ?? null;
            model.EMAIL = vendor["EMAIL"].ToString() ?? null;
            model.TELEPHONE = vendor["TELEPHONE"].ToString();
            model.EXPECTED_DELIVERY_DATE = Common.ParseDate((vendor["EXPECTED_DELIVERY_DATE"] != null) ? vendor["EXPECTED_DELIVERY_DATE"].ToString() : null); 
            
            model.SUPPLIERS_QUOTE = vendor["SUPPLIERS_QUOTE"].ToString();
           model.PAYMENT_TERMS = vendor["PAYMENT_TERMS"].ToString()??null ;
            model.CONTACT_NAME = vendor["CONTACT_NAME"].ToString() ?? null;
            model.STATUS = "RESPONDED";
            try
            {
                using (var context = new Entities())
                {
                    
                        var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == model.TEMP_NO && m.VENDOR_ID == _VendorId).FirstOrDefault();
                        if (query != null)
                        {
                            query.RESPONSE_DATE = model.RESPONSE_DATE;
                            query.ADDRESS = model.ADDRESS;
                            query.EMAIL = model.EMAIL;
                            query.TELEPHONE = model.TELEPHONE;
                            query.EXPECTED_DELIVERY_DATE = model.EXPECTED_DELIVERY_DATE;
                            query.SUPPLIERS_QUOTE = model.SUPPLIERS_QUOTE;
                            query.PAYMENT_TERMS = model.PAYMENT_TERMS;
                            query.CONTACT_NAME = model.CONTACT_NAME;
                            query.STATUS = model.STATUS;
                            context.SaveChanges();
                        }
                    
                }

                Common.NotifyProcurement(VendorName, (decimal)model.TEMP_NO, contentPath,RefNUmber);


            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
               
            }
        }
    }

    
    public class ProcessResponseRFP
    {
        public List<RFQITEMViewModel> items;
        public FormCollection vendor;
        public decimal _VendorId;
        public ProposalTemplateViewModel proposal;
        public decimal _requestType;
        public string VendorName;
        public string RefNUmber;
        string contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\VendorResponse.txt";
        public void InsertVendorResponseToItems()
        {

            NegotiationsViewModel model = new NegotiationsViewModel();
            //SOURCING_RFQ_QUOTATION query = new SOURCING_RFQ_QUOTATION();
            SOURCING_NEGOTIATIONS _negotiations = new SOURCING_NEGOTIATIONS();
            SOURCING_NEG_PRICE query = new SOURCING_NEG_PRICE();
            model.VAT = Convert.ToDecimal(vendor["VAT"].ToString());
            model.VATVALUE = Convert.ToDecimal(vendor["VATVALUE"].ToString());
            model.TOTAL_AMT = Convert.ToDecimal(vendor["TOTALBFTAX"].ToString());
            model.GRANDTOTAL = Convert.ToDecimal(vendor["GRANDTOTAL"].ToString());
            model.TEMP_ID = Convert.ToDecimal(vendor["TEMP_ID"].ToString());
            model.RES_DATE = DateTime.Now;
            decimal _negNo = Common.GetSequenceB();
            _negotiations.NEG_NO = _negNo;
            _negotiations.GRANDTOTAL = model.GRANDTOTAL;
            _negotiations.TOTAL_AMT = model.TOTAL_AMT;
            _negotiations.VAT = model.VAT;
            _negotiations.VATVALUE = model.VATVALUE;
            _negotiations.RES_DATE = model.RES_DATE;
            _negotiations.RESPONSE_NO = model.RESPONSE_NO ?? 0;
            _negotiations.VENDOR_ID = _VendorId;
            _negotiations.NEGOTIATOR = "VENDOR";
            _negotiations.TEMP_ID = model.TEMP_ID;
            _negotiations.STATUS = "";
            try
            {
                using (var context = new Entities())
                {
                    context.SOURCING_NEGOTIATIONS.Add(_negotiations);
                    context.SaveChanges();
                    foreach (var item in this.items)
                    {
                        query.ITEM_NO = item.ITEM_NO;
                        query.NEG_NO = _negNo;
                        query.UNIT_PRICE = item.UNIT_PRICE;
                        query.TOTAL_PRICE = item.TOTAL_PRICE;
                        context.SOURCING_NEG_PRICE.Add(query);
                        context.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
            }
          
        }

        public void InsertSuppliesInfo()
        {
            ContactedVendorViewModel model = new ContactedVendorViewModel();
            //model.VAT_PROPOSAL = Convert.ToDecimal(vendor["VAT"].ToString());
            //model.TOTALBFTAX_PROPOSAL = Convert.ToDecimal(vendor["TOTALBFTAX"].ToString());
            //model.GRANDTOTAL_PROPOSAL = Convert.ToDecimal(vendor["GRANDTOTAL"].ToString());
            model.TEMP_NO = Convert.ToDecimal(vendor["TEMP_ID"].ToString());
            model.RESPONSE_DATE = DateTime.Now;
          
            model.STATUS = "RESPONDED";
            try
            {
                using (var context = new Entities())
                {
                    //var query = context.SOURCING_CONTACTEDVENDOR.ToList();

                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == model.TEMP_NO && m.VENDOR_ID == _VendorId).FirstOrDefault();
                    if (query != null)
                    {
                        
                        query.RESPONSE_DATE = model.RESPONSE_DATE;
                        query.STATUS = model.STATUS;
                        context.SaveChanges();
                    }
                    Common.NotifyProcurement(VendorName, (decimal)model.TEMP_NO, contentPath, RefNUmber);
                }

            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");

            }
        }

        public string ProcessRFPProposal()
        {
            
            try
            {
                SOURCING_PROPOSAL _proposal = new SOURCING_PROPOSAL();         
                using (var context = new Entities())
                {
                     var check = context.SOURCING_PROPOSAL.Where(m => m.TEMPID == proposal.TEMPID && m.VENDORID == _VendorId && m.REQUEST_TYPE == _requestType && m.ID == proposal.ID).FirstOrDefault();
                    if(check != null)
                    {
                        check.PROPOSAL = proposal.PROPOSAL;

                    }else{
                        _proposal.TEMPID = proposal.TEMPID;
                        _proposal.VENDORID = _VendorId;
                        _proposal.REQUEST_TYPE = _requestType;
                        _proposal.PROPOSAL = proposal.PROPOSAL;
                        context.SOURCING_PROPOSAL.Add(_proposal);
                    }                 
                    context.SaveChanges();
                    return Convert.ToString(proposal.TEMPID);
                }

            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
                return null;
            }
        }

        //used for RFI request
        public void UpdateVendorResponse()
        {
            ContactedVendorViewModel model = new ContactedVendorViewModel();

            model.TEMP_NO = proposal.TEMPID;
            model.RESPONSE_DATE = DateTime.Now;
            model.STATUS = "RESPONDED";
            try
            {
                using (var context = new Entities())
                {
                    //var query = context.SOURCING_CONTACTEDVENDOR.ToList();

                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == model.TEMP_NO && m.VENDOR_ID == _VendorId).FirstOrDefault();
                    if (query != null)
                    {  
                        query.RESPONSE_DATE = model.RESPONSE_DATE;
                        query.STATUS = model.STATUS;
                        context.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");

            }
        }

        public void UpdateVendorResponseToItems()
        {
            SOURCING_NEG_PRICE query = new SOURCING_NEG_PRICE();
            NegotiationsViewModel model = new NegotiationsViewModel();
            //SOURCING_RFQ_QUOTATION query = new SOURCING_RFQ_QUOTATION();
            model.VAT = Convert.ToDecimal(vendor["VAT"].ToString());
            model.VATVALUE = Convert.ToDecimal(vendor["VATVALUE"].ToString());
            model.TOTAL_AMT = Convert.ToDecimal(vendor["TOTALBFTAX"].ToString());
            model.GRANDTOTAL = Convert.ToDecimal(vendor["GRANDTOTAL"].ToString());
            //model.TEMP_ID = Convert.ToDecimal(vendor["TEMP_NO"].ToString());
            model.NEG_NO = Convert.ToDecimal(vendor["NEGNO"].ToString());
            try
            {
                using (var context = new Entities())
                {
                    var updateNegotiationTable = context.SOURCING_NEGOTIATIONS.Where(m => m.NEG_NO == model.NEG_NO).FirstOrDefault();
                    if (updateNegotiationTable != null)
                    {
                        updateNegotiationTable.VAT = model.VAT;
                        updateNegotiationTable.VATVALUE = model.VATVALUE;
                        updateNegotiationTable.GRANDTOTAL = model.GRANDTOTAL;
                        updateNegotiationTable.TOTAL_AMT = model.TOTAL_AMT;
                        context.SaveChangesAsync();
                    }

                    foreach (var item in this.items)
                    {
                        var addItem = context.SOURCING_NEG_PRICE.Where(m => m.ID == item.ID).FirstOrDefault();
                        if (addItem != null)
                        {
                            addItem.UNIT_PRICE = item.UNIT_PRICE;
                            addItem.TOTAL_PRICE = item.TOTAL_PRICE;
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
            }
        }

        public  void NotifyProcurement(string Email)
        {
            string contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\NegotiationResponse.txt";
            string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
            string content = System.IO.File.ReadAllText(contentPath);
            try
            {


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

    }
}