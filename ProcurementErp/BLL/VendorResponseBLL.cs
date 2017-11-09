using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class VendorResponseBLL
    {
        //retrieve vendor response using the request temporary number
        public string TempNo;
        public string VendorId;
        public VendorResponseViewModel GetVendorResponseList()
        {
            VendorResponseViewModel response = new VendorResponseViewModel();
            decimal _TempNo = Convert.ToDecimal(this.TempNo);
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == _TempNo && (m.STATUS != null &&  m.STATUS != "NEW REQUEST")).ToList();
                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            response.ContactedVendor.Add(new ContactedVendorViewModel
                            {
                                VENDOR_ID = item.VENDOR_ID,
                                VENDOR_NAME = item.VENDOR_NAME,
                                AUTO_EMAIL = item.AUTO_EMAIL,
                                STATUS = item.STATUS,
                                RESPONSE_DATE = item.RESPONSE_DATE,
                                TEMP_NO = item.TEMP_NO
                            });
                        }
                    }
                    else
                    {
                        response.ContactedVendor = null;
                    }

                    var getRefNo = context.SOURCING_REF_TEMP_LINK
                       .Join(context.SOURCING_REQUEST_TYPE, tl => tl.REQ_TYPE, rtype => rtype.REQUEST_ID, (tl, rtype) => new { _tl = tl, _rtype = rtype })
                   .Where(m => m._tl.TEMP_NO == _TempNo).FirstOrDefault();
                    if (getRefNo != null)
                    {
                        response.RefNum.REF_NO = getRefNo._tl.REF_NO;
                        response.RefNum.REQ_TYPE = getRefNo._tl.REQ_TYPE;
                        response.RequestType.REQUEST_NAME = getRefNo._rtype.REQUEST_NAME;
                        response.RequestType.REQUEST_SHORTCODE = getRefNo._rtype.REQUEST_SHORTCODE;
                        response.RefNum.REQ_TYPE = getRefNo._tl.REQ_TYPE;
                    }
                    else
                    {
                        response.RefNum = null;
                        response.RequestType = null;
                    }
                    return response;
                   
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }

        //Retreive RFQ vendor response summary
        #region RFQ vendor response summary
        public RFQSummaryDisplay GetRFQQuotationSummaryAfter()
        {
            decimal _TempNo = Convert.ToDecimal(this.TempNo);
            decimal VendorId = Convert.ToDecimal(this.VendorId);
            RFQSummaryDisplay Summary = new RFQSummaryDisplay();
            ContactedVendorViewModel _Vendors = new ContactedVendorViewModel();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            List<QuotationViewModel> Quotes = new List<QuotationViewModel>();
            List<NegotiationsViewModel> Negotiations = new List<NegotiationsViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == _TempNo && vend.VENDOR_ID == VendorId).FirstOrDefault();
                    if (getVendors != null)
                    {
                        _Vendors.VENDOR_ID = getVendors.VENDOR_ID;
                        _Vendors.VENDOR_NAME = getVendors.VENDOR_NAME;
                        _Vendors.AUTO_EMAIL = getVendors.AUTO_EMAIL;
                        _Vendors.VAT = getVendors.VAT;
                        _Vendors.TOTALBFTAX = getVendors.TOTALBFTAX;
                        _Vendors.GRANDTOTAL = getVendors.GRANDTOTAL;
                        _Vendors.RESPONSE_DATE = getVendors.RESPONSE_DATE;
                        _Vendors.ADDRESS = getVendors.ADDRESS;
                        _Vendors.EMAIL = getVendors.EMAIL;
                        _Vendors.EXPECTED_DELIVERY_DATE = getVendors.EXPECTED_DELIVERY_DATE;
                        _Vendors.SUPPLIERS_QUOTE = getVendors.SUPPLIERS_QUOTE;
                        _Vendors.PAYMENT_TERMS = getVendors.PAYMENT_TERMS;
                        _Vendors.CONTACT_NAME = getVendors.CONTACT_NAME;
                        _Vendors.STATUS = getVendors.STATUS;
                        _Vendors.NEG_STATUS = getVendors.NEG_STATUS;
                        _Vendors.TELEPHONE = getVendors.TELEPHONE;
                        _Vendors.VERDICT_ACCEPT_DATE = getVendors.VERDICT_ACCEPT_DATE;
                        _Vendors.VERDICT_ISSUE_DATE = getVendors.VERDICT_ISSUE_DATE;
                        Summary._Vendor = _Vendors;
                    }

                    IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f =>f.SOURCE_REQ_ID == _TempNo && f.VENDORID == VendorId).ToList();
                    Summary._Files = _queryFile;

                    /*********************************************************************************************/
                    //get list from negotiation table, then iteriate items through the list. 
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _TempNo).ToList();
                    var getQuotesFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == _TempNo && m.VENDOR_ID == VendorId).OrderBy(m => m.NEG_NO).ToList();
                    foreach (var _quote in getQuotesFromNegTable)
                    {
                       
                        if (getItems != null)
                        {
                            foreach (var singleItem in getItems)
                            {
                                var getQuotes = context.SOURCING_NEG_PRICE.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.NEG_NO == _quote.NEG_NO ).FirstOrDefault();
                                if (getQuotes != null)
                                {

                                    _Item.Add(new RFQITEMViewModel
                                    {
                                        ID = getQuotes.ID,
                                        QUANTITY = singleItem.QUANTITY,
                                        UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                        DESCRIPTION = singleItem.DESCRIPTION,
                                        ITEM_NO = singleItem.ITEM_NO,
                                        UNIT_PRICE = getQuotes.UNIT_PRICE,
                                        TOTAL_PRICE = getQuotes.TOTAL_PRICE,
                                        NegNum = _quote.NEG_NO
                                    });
                                }
                            }
                        }

                        Negotiations.Add(new NegotiationsViewModel
                        {
                            TOTAL_AMT = _quote.TOTAL_AMT,
                            GRANDTOTAL = _quote.GRANDTOTAL,
                            VAT = _quote.VAT,
                            VATVALUE = _quote.VATVALUE,
                            NEG_NO = _quote.NEG_NO,
                            NEGOTIATOR = _quote.NEGOTIATOR,
                            RESPONSE_NO = _quote.RESPONSE_NO,
                            STATUS = _quote.STATUS,
                            TEMP_ID = _quote.TEMP_ID,
                            RES_DATE = _quote.RES_DATE,
                            VENDOR_ID = _quote.VENDOR_ID,
                            NegItems = _Item
                        });
                    }

                    Summary._Negotiations = Negotiations;

                    /*********************************************************************************************/
                    //get items
                    //var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _TempNo).ToList();
                    //if (getItems != null)
                    //{
                    //    foreach (var singleItem in getItems)
                    //    {
                    //        var getQuotes = context.SOURCING_RFQ_QUOTATION.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.VENDOR_ID == VendorId).FirstOrDefault();
                    //        _Item.Add(new RFQITEMViewModel
                    //        {
                    //            QUANTITY = singleItem.QUANTITY,
                    //            UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                    //            DESCRIPTION = singleItem.DESCRIPTION,
                    //            ITEM_NO = singleItem.ITEM_NO,
                    //            UNIT_PRICE = getQuotes.UNIT_PRICE,
                    //            TOTAL_PRICE = getQuotes.TOTAL_PRICE
                    //        });
                    //    }
                    //    Summary.Items = _Item;
                    //}

                    var query = context.SOURCING_RFQ_REQ.Join(
                        context.SOURCING_USERS, rq => rq.INITIATOR_ID, user => user.USER_ID, (rq, user) => new { Req = rq, Users = user })
                        .Join(context.SOURCING_REF_TEMP_LINK, link => link.Req.TEMP_NO, temp => temp.TEMP_NO, (link, temp) => new { _link = link, _temp = temp })
                        .Where(reqID => reqID._link.Req.TEMP_NO == _TempNo).FirstOrDefault();
                    if (query != null)
                    {
                        Summary.Initiator.NAME = query._link.Users.NAME;
                        Summary.Initiator.TELEPHONE = query._link.Users.TELEPHONE;
                        Summary.Initiator.EMAIL = query._link.Users.EMAIL;
                        Summary.Request.DELIVERY_ADDRESS = query._link.Req.DELIVERY_ADDRESS;
                        Summary.Request.RFQ_START_DATE = query._link.Req.RFQ_START_DATE;
                        Summary.Request.RFQ_CLOSE_DATE = query._link.Req.RFQ_CLOSE_DATE;
                        Summary.RefNum.REF_NO = query._temp.REF_NO;
                        Summary.Request.TEMP_NO = query._link.Req.TEMP_NO;
                        Summary.Request.NEGVATVALUE = query._link.Req.NEGVATVALUE;
                        Summary.Request.NEGVAT = query._link.Req.NEGVAT;
                        Summary.Request.NEGTOTALAMT = query._link.Req.NEGTOTALAMT;
                        Summary.Request.NEGGRANDTOTAL = query._link.Req.NEGGRANDTOTAL;
                    }
                    return Summary;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }
        #endregion

        //Retrieve RFI vendor response summary
        #region RFI vendor response summary
        public RFIRFPComboViewModel GetRFISummaryInfo()
        {
            decimal _TempId = Convert.ToDecimal(this.TempNo);
            decimal VendorId = Convert.ToDecimal(this.VendorId);
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            ContactedVendorViewModel _Vendors = new ContactedVendorViewModel();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == _TempId && vend.VENDOR_ID == VendorId).FirstOrDefault();
                    if (getVendors != null)
                    {
                        _Vendors.VENDOR_ID = getVendors.VENDOR_ID;
                        _Vendors.VENDOR_NAME = getVendors.VENDOR_NAME;
                        _Vendors.AUTO_EMAIL = getVendors.AUTO_EMAIL;
                        _Vendors.VAT = getVendors.VAT;
                        _Vendors.TOTALBFTAX = getVendors.TOTALBFTAX;
                        _Vendors.GRANDTOTAL = getVendors.GRANDTOTAL;
                        _Vendors.RESPONSE_DATE = getVendors.RESPONSE_DATE;
                        _Vendors.ADDRESS = getVendors.ADDRESS;
                        _Vendors.EMAIL = getVendors.EMAIL;
                        _Vendors.EXPECTED_DELIVERY_DATE = getVendors.EXPECTED_DELIVERY_DATE;
                        _Vendors.SUPPLIERS_QUOTE = getVendors.SUPPLIERS_QUOTE;
                        _Vendors.PAYMENT_TERMS = getVendors.PAYMENT_TERMS;
                        _Vendors.CONTACT_NAME = getVendors.CONTACT_NAME;
                        _Vendors.STATUS = getVendors.STATUS;


                        Summary.Vendor = _Vendors;
                    }
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _TempId).ToList();
                    if (getItems != null)
                    {
                        foreach (var singleItem in getItems)
                        {
                            _Item.Add(new RFQITEMViewModel
                            {
                                QUANTITY = singleItem.QUANTITY,
                                UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                DESCRIPTION = singleItem.DESCRIPTION,
                                RFI_INFO = singleItem.RFI_INFO
                            });
                        }
                        Summary.Item = _Item;
                    }
                    //get Reference Number

                    var getRefNum = context.SOURCING_REF_TEMP_LINK.Where(m => m.TEMP_NO == _TempId).FirstOrDefault();
                    if (getRefNum != null)
                    {
                        Summary.RefNum.REF_NO = getRefNum.REF_NO;
                    }


                    var getContactPerson = context.SOURCING_CONTACTPERSON.Where(m => m.TEMP_NO == _TempId).ToList();
                    var getTechnicalPerson = getContactPerson.Where(m => m.CONTACT_TYPE == "Technical").FirstOrDefault();
                    if (getTechnicalPerson != null)
                    {
                        Summary.Technical.TEMP_NO = getTechnicalPerson.TEMP_NO;
                        Summary.Technical.NAME = getTechnicalPerson.NAME;
                        Summary.Technical.DEPARTMENT = getTechnicalPerson.DEPARTMENT;
                        Summary.Technical.DESIGNATION = getTechnicalPerson.DESIGNATION;
                        Summary.Technical.TELEPHONE = getTechnicalPerson.TELEPHONE;
                        Summary.Technical.EMAIL = getTechnicalPerson.EMAIL;
                        Summary.Technical.CONTACT_TYPE = getTechnicalPerson.CONTACT_TYPE;
                    }
                    var getCommercialPerson = getContactPerson.Where(m => m.CONTACT_TYPE == "Commercial").FirstOrDefault();
                    if (getCommercialPerson != null)
                    {
                        Summary.Commercial.TEMP_NO = getCommercialPerson.TEMP_NO;
                        Summary.Commercial.NAME = getCommercialPerson.NAME;
                        Summary.Commercial.DEPARTMENT = getCommercialPerson.DEPARTMENT;
                        Summary.Commercial.DESIGNATION = getCommercialPerson.DESIGNATION;
                        Summary.Commercial.TELEPHONE = getCommercialPerson.TELEPHONE;
                        Summary.Commercial.EMAIL = getCommercialPerson.EMAIL;
                        Summary.Commercial.CONTACT_TYPE = getCommercialPerson.CONTACT_TYPE;
                    }

                    var query = context.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == _TempId).FirstOrDefault();
                    if (query != null)
                    {
                        Summary.Request.TEMP_ID = query.TEMP_ID;
                        Summary.Request.PRJECT_TITLE = query.PRJECT_TITLE;
                        Summary.Request.INITIATOR_NAME = query.INITIATOR_NAME;
                        Summary.Request.INITIATOR = query.INITIATOR;
                        Summary.Request.INITIATION_DATE = query.INITIATION_DATE;
                        Summary.Request.PROJECT_OBJECTIVE = query.PROJECT_OBJECTIVE;
                        Summary.Request.WORK_SCOPE = query.WORK_SCOPE;
                        Summary.Request.UBN_OVERVIEW = query.UBN_OVERVIEW;
                        Summary.Request.TECHNICAL_PROPOSAL = query.TECHNICAL_PROPOSAL;
                        Summary.Request.ISSUE_DATE = query.ISSUE_DATE;
                        Summary.Request.DUE_DATE = query.DUE_DATE;
                        Summary.Request.DUE_TIME = query.DUE_TIME;
                        Summary.Request.LST_QRY_RECEIPT_DATE = query.LST_QRY_RECEIPT_DATE;
                        Summary.Request.BANK_QRY_RES_DATE = query.BANK_QRY_RES_DATE;
                        Summary.Request.LST_RPF_RECPT_DATE = query.LST_RPF_RECPT_DATE;
                        Summary.Request.STATUS = query.STATUS;

                    }

                    var queryProposal = context.SOURCING_PROPOSAL.Where(m => m.TEMPID == _TempId && m.VENDORID == VendorId && m.REQUEST_TYPE == 1).FirstOrDefault();
                    if (queryProposal != null)
                    {
                        Summary.proposal.PROPOSAL = queryProposal.PROPOSAL;
                    }
                    return Summary;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message, "error");
                return null;
            }
        }
        #endregion

        //Rerieve RFP vendor response summary
        #region RFP vendor response summary
        internal RFIRFPComboViewModel GetRFPSummaryInfo()
        {
            decimal TempId = Convert.ToDecimal(this.TempNo);
            decimal VendorId = Convert.ToDecimal(this.VendorId);
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            List<NegotiationsViewModel> Negotiations = new List<NegotiationsViewModel>();
            ProposalTemplateViewModel _proposal = new ProposalTemplateViewModel();
            ContactedVendorViewModel _Vendors = new ContactedVendorViewModel();

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == TempId && vend.VENDOR_ID == VendorId).FirstOrDefault();
                    if (getVendors != null)
                    {
                        _Vendors.VENDOR_ID = getVendors.VENDOR_ID;
                        _Vendors.VENDOR_NAME = getVendors.VENDOR_NAME;
                        _Vendors.AUTO_EMAIL = getVendors.AUTO_EMAIL;
                        _Vendors.VAT_PROPOSAL = getVendors.VAT_PROPOSAL;
                        _Vendors.TOTALBFTAX_PROPOSAL = getVendors.TOTALBFTAX_PROPOSAL;
                        _Vendors.GRANDTOTAL_PROPOSAL = getVendors.GRANDTOTAL_PROPOSAL;
                        _Vendors.RESPONSE_DATE = getVendors.RESPONSE_DATE;
                        _Vendors.ADDRESS = getVendors.ADDRESS;
                        _Vendors.EMAIL = getVendors.EMAIL;
                        _Vendors.EXPECTED_DELIVERY_DATE = getVendors.EXPECTED_DELIVERY_DATE;
                        _Vendors.SUPPLIERS_QUOTE = getVendors.SUPPLIERS_QUOTE;
                        _Vendors.PAYMENT_TERMS = getVendors.PAYMENT_TERMS;
                        _Vendors.CONTACT_NAME = getVendors.CONTACT_NAME;
                        _Vendors.STATUS = getVendors.STATUS;
                        _Vendors.NEG_STATUS = getVendors.NEG_STATUS;
                        _Vendors.VERDICT_ACCEPT_DATE = getVendors.VERDICT_ACCEPT_DATE;
                        _Vendors.VERDICT_ISSUE_DATE = getVendors.VERDICT_ISSUE_DATE;


                        Summary.Vendor = _Vendors;
                    }
                    var getRefNum = context.SOURCING_REF_TEMP_LINK.Where(m => m.TEMP_NO == TempId).FirstOrDefault();
                    if (getRefNum != null)
                    {
                        Summary.RefNum.REF_NO = getRefNum.REF_NO;
                    }

                    //get FIles
                    IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f => f.SOURCE_REQ_ID == TempId && f.VENDORID == VendorId).ToList();
                    Summary._Files = _queryFile;
                    //get items


                    var getQuotesFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == TempId && m.VENDOR_ID == VendorId).OrderBy(m => m.NEG_NO).ToList();
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
                    foreach (var _quote in getQuotesFromNegTable)
                    {

                        if (getItems != null)
                        {
                            foreach (var singleItem in getItems)
                            {
                                var getQuotes = context.SOURCING_NEG_PRICE.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.NEG_NO == _quote.NEG_NO).FirstOrDefault();
                                if (getQuotes != null)
                                {
                                    _Item.Add(new RFQITEMViewModel
                                    {
                                        ID = getQuotes.ID,
                                        QUANTITY = singleItem.QUANTITY,
                                        UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                        DESCRIPTION = singleItem.DESCRIPTION,
                                        ITEM_NO = singleItem.ITEM_NO,
                                        UNIT_PRICE = getQuotes.UNIT_PRICE,
                                        TOTAL_PRICE = getQuotes.TOTAL_PRICE,
                                        NegNum = _quote.NEG_NO
                                    });
                                }
                            }
                        }

                        Negotiations.Add(new NegotiationsViewModel
                        {
                            TOTAL_AMT = _quote.TOTAL_AMT,
                            GRANDTOTAL = _quote.GRANDTOTAL,
                            VAT = _quote.VAT,
                            VATVALUE = _quote.VATVALUE,
                            NEG_NO = _quote.NEG_NO,
                            NEGOTIATOR = _quote.NEGOTIATOR,
                            RESPONSE_NO = _quote.RESPONSE_NO,
                            STATUS = _quote.STATUS,
                            TEMP_ID = _quote.TEMP_ID,
                            VENDOR_ID = _quote.VENDOR_ID,
                            RES_DATE = _quote.RES_DATE,
                            NegItems = _Item
                        });
                    }

                    Summary._Negotiations = Negotiations;

                    //var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
                    //if (getItems != null)
                    //{
                    //    foreach (var singleItem in getItems)
                    //    {
                    //        var getQuotes = context.SOURCING_RFQ_QUOTATION.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.VENDOR_ID == VendorId).FirstOrDefault();
                    //        _Item.Add(new RFQITEMViewModel
                    //        {
                    //            QUANTITY = singleItem.QUANTITY,
                    //            UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                    //            DESCRIPTION = singleItem.DESCRIPTION,
                    //            ITEM_NO = singleItem.ITEM_NO,
                    //            UNIT_PRICE = getQuotes.UNIT_PRICE,
                    //            TOTAL_PRICE = getQuotes.TOTAL_PRICE
                    //        });
                    //    }
                        
                    //    Summary.Item = _Item;
                    //}

                    var getContactPerson = context.SOURCING_CONTACTPERSON.Where(m => m.TEMP_NO == TempId).ToList();
                    var getTechnicalPerson = getContactPerson.Where(m => m.CONTACT_TYPE == "Technical").FirstOrDefault();
                    if (getTechnicalPerson != null)
                    {
                        Summary.Technical.TEMP_NO = getTechnicalPerson.TEMP_NO;
                        Summary.Technical.NAME = getTechnicalPerson.NAME;
                        Summary.Technical.DEPARTMENT = getTechnicalPerson.DEPARTMENT;
                        Summary.Technical.DESIGNATION = getTechnicalPerson.DESIGNATION;
                        Summary.Technical.TELEPHONE = getTechnicalPerson.TELEPHONE;
                        Summary.Technical.EMAIL = getTechnicalPerson.EMAIL;
                        Summary.Technical.CONTACT_TYPE = getTechnicalPerson.CONTACT_TYPE;
                    }
                    var getCommercialPerson = getContactPerson.Where(m => m.CONTACT_TYPE == "Commercial").FirstOrDefault();
                    if (getCommercialPerson != null)
                    {
                        Summary.Commercial.TEMP_NO = getCommercialPerson.TEMP_NO;
                        Summary.Commercial.NAME = getCommercialPerson.NAME;
                        Summary.Commercial.DEPARTMENT = getCommercialPerson.DEPARTMENT;
                        Summary.Commercial.DESIGNATION = getCommercialPerson.DESIGNATION;
                        Summary.Commercial.TELEPHONE = getCommercialPerson.TELEPHONE;
                        Summary.Commercial.EMAIL = getCommercialPerson.EMAIL;
                        Summary.Commercial.CONTACT_TYPE = getCommercialPerson.CONTACT_TYPE;
                    }

                    var query = context.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == TempId).FirstOrDefault();
                    if (query != null)
                    {
                        Summary.Request.TEMP_ID = query.TEMP_ID;
                        Summary.Request.PRJECT_TITLE = query.PRJECT_TITLE;
                        Summary.Request.INITIATOR_NAME = query.INITIATOR_NAME;
                        Summary.Request.INITIATOR = query.INITIATOR;
                        Summary.Request.INITIATION_DATE = query.INITIATION_DATE;
                        Summary.Request.PROJECT_OBJECTIVE = query.PROJECT_OBJECTIVE;
                        Summary.Request.WORK_SCOPE = query.WORK_SCOPE;
                        Summary.Request.UBN_OVERVIEW = query.UBN_OVERVIEW;
                        Summary.Request.TECHNICAL_PROPOSAL = query.TECHNICAL_PROPOSAL;
                        Summary.Request.ISSUE_DATE = query.ISSUE_DATE;
                        Summary.Request.DUE_DATE = query.DUE_DATE;
                        Summary.Request.DUE_TIME = query.DUE_TIME;
                        Summary.Request.LST_QRY_RECEIPT_DATE = query.LST_QRY_RECEIPT_DATE;
                        Summary.Request.BANK_QRY_RES_DATE = query.BANK_QRY_RES_DATE;
                        Summary.Request.LST_RPF_RECPT_DATE = query.LST_RPF_RECPT_DATE;
                        Summary.Request.STATUS = query.STATUS;
                        Summary.Request.NEGVATVALUE = query.NEGVATVALUE;
                        Summary.Request.NEGVAT = query.NEGVAT;
                        Summary.Request.NEGTOTALAMT = query.NEGTOTALAMT;
                        Summary.Request.NEGGRANDTOTAL = query.NEGGRANDTOTAL;

                    }

                    var queryProposal = context.SOURCING_PROPOSAL.Where(m => m.TEMPID == TempId && m.VENDORID == VendorId && m.REQUEST_TYPE == 2).FirstOrDefault();
                    if (queryProposal != null)
                    {
                        Summary.proposal.PROPOSAL = queryProposal.PROPOSAL;
                    }
                    return Summary;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }
        #endregion

        //Get negotiate price
        public  List<RFQITEMViewModel> getNegotiatedPrice(decimal? TempId, decimal? _vendorId)
        {


            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            using (var context = new Entities())
            {

                var getQuotesFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == TempId && m.VENDOR_ID == _vendorId && m.STATUS == "ACCEPTED").OrderBy(m => m.NEG_NO).FirstOrDefault();
                var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
                

                    if (getItems != null)
                    {
                        foreach (var singleItem in getItems)
                        {
                            var getQuotes = context.SOURCING_NEG_PRICE.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.NEG_NO == getQuotesFromNegTable.NEG_NO).FirstOrDefault();
                            if (getQuotes != null)
                            {
                                _Item.Add(new RFQITEMViewModel
                                {
                                    ID = getQuotes.ID,
                                    QUANTITY = singleItem.QUANTITY,
                                    UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                    DESCRIPTION = singleItem.DESCRIPTION,
                                    ITEM_NO = singleItem.ITEM_NO,
                                    UNIT_PRICE = getQuotes.UNIT_PRICE,
                                    TOTAL_PRICE = getQuotes.TOTAL_PRICE,
                                    NegNum = getQuotesFromNegTable.NEG_NO
                                });
                            }
                        }
                    }
                    return _Item;

                    //Negotiations.Add(new NegotiationsViewModel
                    //{
                    //    TOTAL_AMT = _quote.TOTAL_AMT,
                    //    GRANDTOTAL = _quote.GRANDTOTAL,
                    //    VAT = _quote.VAT,
                    //    VATVALUE = _quote.VATVALUE,
                    //    NEG_NO = _quote.NEG_NO,
                    //    NEGOTIATOR = _quote.NEGOTIATOR,
                    //    RESPONSE_NO = _quote.RESPONSE_NO,
                    //    STATUS = _quote.STATUS,
                    //    TEMP_ID = _quote.TEMP_ID,
                    //    VENDOR_ID = _quote.VENDOR_ID,
                    //    RES_DATE = _quote.RES_DATE,
                    //    NegItems = _Item
                    //});
                

                //Summary._Negotiations = Negotiations;




                //var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
                //if (getItems != null)
                //{
                //    foreach (var singleItem in getItems)
                //    {
                //        var getQuotes = context.SOURCING_NEG_PRICE.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.TEMP_NO == TempId).FirstOrDefault();
                //        _Item.Add(new RFQITEMViewModel
                //        {
                //            QUANTITY = singleItem.QUANTITY,
                //            UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                //            DESCRIPTION = singleItem.DESCRIPTION,
                //            ITEM_NO = singleItem.ITEM_NO,
                //            UNIT_PRICE = getQuotes.UNIT_PRICE,
                //            TOTAL_PRICE = getQuotes.TOTAL_PRICE
                //        });
                //    }

                //    return  _Item;
                //}
                //else
                //{
                //    return null;
                //}
            }
            
        }
            
    }
}