using ProcurementErpVendor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.BLL
{
    public class GetRequest
    {
        //display new request on the landing page
        public static List<RequestViewModel> GetAllRequest(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            var RfqRequest = GetRFQRequset(VendorID);
            var RfqRfpRequest = GetRFIRFPRequset(VendorID);
            if (RfqRequest != null)
            {
                foreach (var item in RfqRequest)
                {
                    _request.Add(new RequestViewModel
                    {
                        ReferenceNumber = item.ReferenceNumber,
                        Status = item.Status,
                        StartDate = item.StartDate,
                        DueDate = item.DueDate,
                        RequestType = item.RequestType,
                        TempNo = item.TempNo
                    });
                }
            }
            if (RfqRfpRequest != null)
            {
                foreach (var item in RfqRfpRequest)
                {
                    _request.Add(new RequestViewModel
                    {
                        ReferenceNumber = item.ReferenceNumber,
                        Status = item.Status,
                        StartDate = item.StartDate,
                        DueDate = item.DueDate,
                        RequestType = item.RequestType,
                        TempNo = item.TempNo
                    });
                }
            }
            return _request??null;
        }

        public static List<RequestViewModel> GetRFQRequset(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFQ_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_NO, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_NO, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && (v.Linkrfq._RFQ.STATUS == "Awaiting Vendor Response" || v.Linkrfq._RFQ.STATUS == "CLOSED")).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.RFQ_START_DATE,
                                DueDate = item.Linkrfq._RFQ.RFQ_CLOSE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_NO
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        public static List<RequestViewModel> GetRFQRequsetHistory(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFQ_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_NO, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_NO, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && v.Linkrfq._RFQ.STATUS == "AWARDED").ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.RFQ_START_DATE,
                                DueDate = item.Linkrfq._RFQ.RFQ_CLOSE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_NO
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        public static List<RequestViewModel> GetRFIRFPRequset(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFP_RFI_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_ID, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_ID, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && v.Linkrfq._RFQ.STATUS == "Awaiting Vendor Response").ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.INITIATION_DATE,
                                DueDate = item.Linkrfq._RFQ.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_ID
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        /*** Begin...Retrieve and process RFQ Request **/
        //check if vendor has replied
        public static bool CheckVendorResponse(decimal _TempNo, decimal _vendorId)
        {
            try
            {
                using (var context = new Entities())
                {
                    int _count = context.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == _vendorId && m.TEMP_NO == _TempNo && (m.STATUS != "CLOSED" && m.STATUS == "NEW REQUEST")).Count();
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
            catch(Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
                return false;
            }
        }

        public static RFQSummaryViewModel GetRFQSummaryInfo(decimal _TempNo)
        {
            RFQSummaryViewModel Summary = new RFQSummaryViewModel();
            List<ContactedVendorViewModel> _Vendors = new List<ContactedVendorViewModel>();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
           

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == _TempNo).ToList();
                    if (getVendors != null)
                    {
                        foreach (var singleVendor in getVendors)
                        {
                            _Vendors.Add(new ContactedVendorViewModel
                            {
                                VENDOR_NAME = singleVendor.VENDOR_NAME,
                                AUTO_EMAIL = singleVendor.AUTO_EMAIL,
                                STATUS = singleVendor.STATUS
                            });
                        }
                        Summary.Vendors = _Vendors;
                    }

                    //get items
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _TempNo).ToList();
                    if (getItems != null)
                    {
                        foreach (var singleItem in getItems)
                        {
                            _Item.Add(new RFQITEMViewModel
                            {
                                QUANTITY = singleItem.QUANTITY,
                                UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                DESCRIPTION = singleItem.DESCRIPTION,
                                ITEM_NO = singleItem.ITEM_NO
                            });
                        }
                        Summary.Items = _Item;
                    }

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
                        Summary.Request.STATUS = query._link.Req.STATUS;
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

        public static string GetRequestType(decimal ReqID)
        {
            try
            {
                using (var context = new Entities())
                {
                    string query = context.SOURCING_REQUEST_TYPE.Where(m => m.REQUEST_ID == ReqID).Select(m => m.REQUEST_NAME).FirstOrDefault().ToString();
                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
                return null;
            }
        }

        //retreive summary after vendor submits quote.
        public static RFQSummaryDisplay GetRFQQuotationSummaryAfter(decimal _TempNo,decimal VendorId)
        {
            RFQSummaryDisplay Summary = new RFQSummaryDisplay();
            ContactedVendorViewModel _Vendors = new ContactedVendorViewModel();
            List<RFQITEMViewModel> _Items = new List<RFQITEMViewModel>();
            List<QuotationViewModel> Quotes = new List<QuotationViewModel>();
            List<NegotiationsViewModel> Negotiations = new List<NegotiationsViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == _TempNo && vend.VENDOR_ID == VendorId).FirstOrDefault();
                    if (getVendors != null)
                    {
                        
                            _Vendors.VENDOR_NAME = getVendors.VENDOR_NAME;
                            _Vendors.AUTO_EMAIL = getVendors.AUTO_EMAIL;
                            //_Vendors.VAT = getVendors.VAT;
                            //_Vendors.TOTALBFTAX = getVendors.TOTALBFTAX;
                            //_Vendors.GRANDTOTAL = getVendors.GRANDTOTAL;
                            //_Vendors.RESPONSE_DATE = getVendors.RESPONSE_DATE;
                            _Vendors.ADDRESS = getVendors.ADDRESS;
                            _Vendors.EMAIL = getVendors.EMAIL;
                            _Vendors.EXPECTED_DELIVERY_DATE = getVendors.EXPECTED_DELIVERY_DATE;
                            _Vendors.SUPPLIERS_QUOTE = getVendors.SUPPLIERS_QUOTE;
                            _Vendors.PAYMENT_TERMS = getVendors.PAYMENT_TERMS;
                            _Vendors.TELEPHONE = getVendors.TELEPHONE;
                            _Vendors.CONTACT_NAME = getVendors.CONTACT_NAME;
                            _Vendors.STATUS = getVendors.STATUS;
                            _Vendors.NEG_STATUS = getVendors.NEG_STATUS;
                            _Vendors.VENDOR_ID = getVendors.VENDOR_ID;
                            _Vendors.VERDICT_ISSUE_DATE = getVendors.VERDICT_ISSUE_DATE;
                            _Vendors.VERDICT_ACCEPT_DATE = getVendors.VERDICT_ACCEPT_DATE;
                            
                        
                        Summary._Vendor = _Vendors;
                    }
                    //get FIles
                    IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f =>f.SOURCE_REQ_ID == _TempNo && f.VENDORID == VendorId).ToList();
                    Summary._Files = _queryFile;
                    //get list from negotiation table, then iteriate items through the list. 

                    var getQuotesFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == _TempNo && m.VENDOR_ID == VendorId).OrderBy(m => m.NEG_NO).ToList();
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _TempNo).ToList();
                    foreach (var _quote in getQuotesFromNegTable)
                    {
                       

                        if (getItems != null)
                        {
                            foreach (var singleItem in getItems)
                            {
                                var getQuotes = context.SOURCING_NEG_PRICE.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.NEG_NO == _quote.NEG_NO ).FirstOrDefault();
                                if (getQuotes != null)
                                {
                                    _Items.Add(new RFQITEMViewModel
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
                            _Item = _Items
                        });
                    }

                    Summary._Negotiations = Negotiations;

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
                        Summary.Request.STATUS = query._link.Req.STATUS;
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


        /*** End...Retrieve and process RFQ Request **/

        /*** Begin...Retrieve and process RFI Request **/
        public static List<RequestViewModel> GetRFIRequset(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFP_RFI_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_ID, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_ID, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && (v.Linkrfq._RFQ.STATUS == "Awaiting Vendor Response" || v.Linkrfq._RFQ.STATUS == "CLOSED") && v.LinkrefTemp.REQ_TYPE == 1).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.INITIATION_DATE,
                                DueDate = item.Linkrfq._RFQ.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_ID
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        public static List<RequestViewModel> GetRFIRequsetHistory(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFP_RFI_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_ID, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_ID, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && v.Linkrfq._RFQ.STATUS == "Awaiting Vendor Response" && v.LinkrefTemp.REQ_TYPE == 1).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.INITIATION_DATE,
                                DueDate = item.Linkrfq._RFQ.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_ID
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }


        public static RFIRFPComboViewModel GetRFISummaryInfo(decimal _TempId)
        {
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            List<ContactedVendorViewModel> _Vendors = new List<ContactedVendorViewModel>();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == _TempId).ToList();
                    if (getVendors != null)
                    {
                        foreach (var singleVendor in getVendors)
                        {
                            _Vendors.Add(new ContactedVendorViewModel
                            {
                                VENDOR_NAME = singleVendor.VENDOR_NAME,
                                AUTO_EMAIL = singleVendor.AUTO_EMAIL,
                            });
                        }
                        Summary.Vendors = _Vendors;
                    }

                    //get Reference Number

                    var getRefNum = context.SOURCING_REF_TEMP_LINK.Where(m => m.TEMP_NO == _TempId).FirstOrDefault();
                    if (getRefNum != null)
                    {
                        Summary.RefNum.REF_NO = getRefNum.REF_NO;
                    }
                    //get items
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
                    return Summary;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message, "error");
                return null;
            }
        }

        public static RFIRFPComboViewModel GetRFISummaryInfo(decimal _TempId, decimal VendorId)
        {
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            List<ContactedVendorViewModel> _Vendors = new List<ContactedVendorViewModel>();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();

            try
            {
                using (var context = new Entities())
                {

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

        //Check if the vendor has responded to the RFI 

        public static bool CheckVendorResponseToRFI(decimal VendorID, decimal TempId)
        {
            using (var context = new Entities())
            {
                var query = context.SOURCING_MESSAGE.Where(m => m.TEMP_ID == TempId && m.CREATORID == VendorID && m.CREATORTYPE == "Vendor").Count();
                if (query > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /*** End...Retrieve and process RFI Request **/

        /** Begin... Retrieve and process RFP Request **/
        public static List<RequestViewModel> GetRFPRequset(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFP_RFI_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_ID, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_ID, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && (v.Linkrfq._RFQ.STATUS == "Awaiting Vendor Response" || v.Linkrfq._RFQ.STATUS == "CLOSED") && v.LinkrefTemp.REQ_TYPE == 2).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.INITIATION_DATE,
                                DueDate = item.Linkrfq._RFQ.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_ID
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        public static List<RequestViewModel> GetRFPRequsetHistory(decimal VendorID)
        {
            List<RequestViewModel> _request = new List<RequestViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Join(context.SOURCING_RFP_RFI_REQ, vendor => vendor.TEMP_NO, rfq => rfq.TEMP_ID, (vendor, rfq) => new { Vendor = vendor, _RFQ = rfq })
                        .Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq._RFQ.TEMP_ID, _refTemp => _refTemp.TEMP_NO, (_rfq, _refTemp) => new { Linkrfq = _rfq, LinkrefTemp = _refTemp })
                        .Where(v => v.Linkrfq.Vendor.VENDOR_ID == VendorID && v.Linkrfq._RFQ.STATUS == "AWARDED" && v.LinkrefTemp.REQ_TYPE == 2).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _request.Add(new RequestViewModel
                            {
                                ReferenceNumber = item.LinkrefTemp.REF_NO,
                                Status = item.Linkrfq._RFQ.STATUS,
                                StartDate = item.Linkrfq._RFQ.INITIATION_DATE,
                                DueDate = item.Linkrfq._RFQ.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.LinkrefTemp.REQ_TYPE) ?? null,
                                TempNo = item.Linkrfq._RFQ.TEMP_ID
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        /** End .. Retrieve and process RFP Request**/


        internal static RFIRFPComboViewModel GetRFPSummaryInfo(decimal TempId)
        {
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var getRefNum = context.SOURCING_REF_TEMP_LINK.Where(m => m.TEMP_NO == TempId).FirstOrDefault();
                    if (getRefNum != null)
                    {
                        Summary.RefNum.REF_NO = getRefNum.REF_NO;
                    }
                    
                    //get items
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
                    if (getItems != null)
                    {
                        foreach (var singleItem in getItems)
                        {
                            _Item.Add(new RFQITEMViewModel
                            {
                                QUANTITY = singleItem.QUANTITY,
                                ITEM_NO = singleItem.ITEM_NO,
                                UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                DESCRIPTION = singleItem.DESCRIPTION,
                                RFI_INFO = singleItem.RFI_INFO
                            });
                        }
                        Summary.Item = _Item;
                    }

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
                    return Summary;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message, "error");
                return null;
            }
        }

        internal static RFIRFPComboViewModel GetRFPSummaryInfo(decimal TempId, decimal VendorId)
        {
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            ProposalTemplateViewModel _proposal = new ProposalTemplateViewModel();
            ContactedVendorViewModel _Vendors = new ContactedVendorViewModel();
            List<NegotiationsViewModel> Negotiations = new List<NegotiationsViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == TempId && vend.VENDOR_ID == VendorId).FirstOrDefault();
                    if (getVendors != null)
                    {

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
                        _Vendors.VENDOR_ID = (decimal)getVendors.VENDOR_ID;
                        _Vendors.NEG_STATUS = getVendors.NEG_STATUS;
                        _Vendors.VERDICT_ISSUE_DATE = getVendors.VERDICT_ISSUE_DATE;
                        _Vendors.VERDICT_ACCEPT_DATE = getVendors.VERDICT_ACCEPT_DATE;

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


                    //get list from negotiation table, then iteriate items through the list. 

                    var getQuotesFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == TempId && m.VENDOR_ID == VendorId).OrderBy(m => m.NEG_NO).ToList();
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
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
                            VENDOR_ID = _quote.VENDOR_ID,
                            RES_DATE = _quote.RES_DATE,
                            _Item = _Item
                        });
                    }

                    Summary._Negotiations = Negotiations;


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
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message, "error");
                return null;
            }
        }

        public static ProposalTemplateViewModel getProposal(decimal TempId, decimal VendorId)
        {
            ProposalTemplateViewModel _proposal = new ProposalTemplateViewModel();
            try
            {
                using (var context = new Entities())
                {
                    var queryProposal = context.SOURCING_PROPOSAL.Where(m => m.TEMPID == TempId && m.VENDORID == VendorId && m.REQUEST_TYPE == 2).FirstOrDefault();
                    if (queryProposal != null)
                    {
                        _proposal.ID = queryProposal.ID;
                        _proposal.TEMPID = queryProposal.TEMPID;
                        _proposal.VENDORID = queryProposal.VENDORID;
                        _proposal.PROPOSAL = queryProposal.PROPOSAL;
                        return _proposal;
                    }
                    else
                    {
                        _proposal.TEMPID = TempId;
                        _proposal.VENDORID = VendorId;
                        return _proposal;
                    }
                    
                }

            }  
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for proposal table. Error: " + ex.Message, "error");

                _proposal.TEMPID = TempId;
                _proposal.VENDORID = VendorId;
                return _proposal;
            }
        }

        //May not be required
        public static List<RFQITEMViewModel> getNegotiatedPrice(decimal? TempId, decimal _vendorId)
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

                //var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == TempId).ToList();
                //if (getItems != null)
                //{
                //    foreach (var singleItem in getItems)
                //    {
                //        var getQuotes = context.SOURCING_NEG_PRICE.Where(it => it.ITEM_NO == singleItem.ITEM_NO && it.TEMP_NO == TempId).FirstOrDefault();
                //        if (getQuotes != null)
                //        {
                //            _Item.Add(new RFQITEMViewModel
                //            {
                //                QUANTITY = singleItem.QUANTITY,
                //                UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                //                DESCRIPTION = singleItem.DESCRIPTION,
                //                ITEM_NO = singleItem.ITEM_NO,
                //                UNIT_PRICE = getQuotes.UNIT_PRICE,
                //                TOTAL_PRICE = getQuotes.TOTAL_PRICE
                //            });
                //        }
                        
                //    }

                //    return _Item;
                //}
                //else
                //{
                //    return null;
                //}
            }

        }

        internal static bool UpdateVendorNegResponse(string Response, decimal TempNo, decimal VendorId, string VendorName = null)
        {
            string contentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\NegotiationResponse.txt";
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == VendorId && m.TEMP_NO == TempNo).FirstOrDefault();
                    if (query != null)
                    {
                        query.NEG_STATUS = Response;
                        context.SaveChanges();
                        Common.NotifyProcurement(VendorName, TempNo, contentPath);
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
                Logger.Log("Error " + ex.StackTrace, "error");
                return false;
            }
        }

        
    }

}