using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class AnalysisBLL
    {
        public string TempId;

        public List<NegotiationsViewModel> GetItemsFromNegTable()
        {
            List<NegotiationsViewModel> _Negs = new List<NegotiationsViewModel>();
            decimal _TempId = Convert.ToDecimal(TempId);
            try
            {
                using (var context = new Entities())
                {
                    var getQuoteFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == _TempId && m.RESPONSE_NO == 0).OrderBy(m => m.NEG_NO).ToList();
                    if (getQuoteFromNegTable != null)
                    {
                        foreach (var item in getQuoteFromNegTable)
                        {
                            _Negs.Add(new NegotiationsViewModel
                            {
                                NEG_NO = item.NEG_NO,
                                RESPONSE_NO = item.RESPONSE_NO,
                                NEGOTIATOR = item.NEGOTIATOR,
                                TEMP_ID = item.TEMP_ID,
                                VENDOR_ID = item.VENDOR_ID,
                                RES_DATE = item.RES_DATE,
                                TOTAL_AMT = item.TOTAL_AMT,
                                VAT = item.VAT,
                                VATVALUE = item.VATVALUE,
                                GRANDTOTAL =item.GRANDTOTAL,
                                STATUS = item.STATUS

                            });
                        }
                        return _Negs;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }

        #region get summary of all vendors quote and buys accepted price
        public List<NegotiationsViewModel> GetItemsFromNegTableForSummaryView()
        {
            List<NegotiationsViewModel> _Negs = new List<NegotiationsViewModel>();
            decimal _TempId = Convert.ToDecimal(TempId);
            try
            {
                using (var context = new Entities())
                {
                    var getQuoteFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == _TempId).OrderBy(m => m.NEG_NO).ToList();
                    var groupByVendorId = getQuoteFromNegTable.Where(p => p.NEGOTIATOR == "VENDOR").GroupBy(p => p.VENDOR_ID).ToList();
                    if (getQuoteFromNegTable != null)
                    {
                        foreach (var VendorsResponseGroup in groupByVendorId)
                        {
                            var item = VendorsResponseGroup.OrderByDescending(p => p.NEG_NO).FirstOrDefault(); ;
                            _Negs.Add(new NegotiationsViewModel
                            {
                                NEG_NO = item.NEG_NO,
                                RESPONSE_NO = item.RESPONSE_NO,
                                NEGOTIATOR = item.NEGOTIATOR,
                                TEMP_ID = item.TEMP_ID,
                                VENDOR_ID = item.VENDOR_ID,
                                RES_DATE = item.RES_DATE,
                                TOTAL_AMT = item.TOTAL_AMT,
                                VAT = item.VAT,
                                VATVALUE = item.VATVALUE,
                                GRANDTOTAL = item.GRANDTOTAL,
                                STATUS = item.STATUS

                            });
                        }

                        var GetAwardedVendorId = context.SOURCING_CONTACTEDVENDOR.Where(t => t.TEMP_NO == _TempId && t.STATUS == "PROCESSING").Select(v => v.VENDOR_ID).First();
                        var SelectedVendor = _Negs.Where(p => p.VENDOR_ID == GetAwardedVendorId).First();
                        _Negs.Add(new NegotiationsViewModel
                            {
                                NEG_NO = SelectedVendor.NEG_NO,
                                RESPONSE_NO = SelectedVendor.RESPONSE_NO,
                                NEGOTIATOR = SelectedVendor.NEGOTIATOR,
                                TEMP_ID = SelectedVendor.TEMP_ID,
                                VENDOR_ID = SelectedVendor.VENDOR_ID,
                                RES_DATE = SelectedVendor.RES_DATE,
                                TOTAL_AMT = SelectedVendor.TOTAL_AMT,
                                VAT = SelectedVendor.VAT,
                                VATVALUE = SelectedVendor.VATVALUE,
                                GRANDTOTAL = SelectedVendor.GRANDTOTAL,
                                STATUS = "PROCESSING"

                            });
                        return _Negs;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }
        #endregion

        public List<AnalysisViewModel> GetVendorResponseAnalysis(string Source = null)
        {
            List<AnalysisViewModel> _Analysis = new List<AnalysisViewModel>();
            decimal _TempId = Convert.ToDecimal(TempId);
            try
            {
                using (var context = new Entities())
                {
                    var getQuoteFromNegTable = context.SOURCING_NEGOTIATIONS.Where(m => m.TEMP_ID == _TempId && m.RESPONSE_NO == 0).OrderBy(m => m.NEG_NO).ToList();
                    var query = context.SOURCING_RFQ_ITEM.Where(m => m.TEMP_NO == _TempId).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _Analysis.Add(new AnalysisViewModel
                            {
                                ITEM_NO = item.ITEM_NO,
                                DESCRIPTION = item.DESCRIPTION??"",
                                UNIT_OF_MEAS = item.UNIT_OF_MEAS??"",
                                QUANTITY = item.QUANTITY??"",
                                _VendorsQuote = GetVendorsQuotePerItem(item.ITEM_NO, Source) 

                            });
                        }

                        return _Analysis;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }

        

        public List<QuotationViewModel> GetVendorsQuotePerItem(decimal _itemNo, string Source = null)
        {
            List<QuotationViewModel> VendorQuotes = new List<QuotationViewModel>();
            var negitems = Source == null ? GetItemsFromNegTable() : GetItemsFromNegTableForSummaryView();
            try
            {
                using (var context = new Entities())
                {
                    foreach (var item in negitems)
                    {
                        var query = context.SOURCING_NEG_PRICE.Where(m => m.ITEM_NO == _itemNo && m.NEG_NO == item.NEG_NO).ToList();
                    
                        foreach (var items in query)
                        {
                            VendorQuotes.Add(new QuotationViewModel
                            {
                                ID = items.ID,
                               
                                ITEM_NO = items.ITEM_NO,
                                UNIT_PRICE = items.UNIT_PRICE??0,
                                TOTAL_PRICE = items.TOTAL_PRICE??0,
                                _VendorDetails = GetVendorsDetails((decimal)item.VENDOR_ID,item.NEG_NO,item.STATUS)
                                
                                
                            });
                        }
                       
                    
                    }

                    return VendorQuotes;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }
        #region get vendor response summary to be exported to erp
        public List<QuotationViewModel> GetVendorsQuotePerItemSummaryView(decimal _itemNo)
        {
            List<QuotationViewModel> VendorQuotes = new List<QuotationViewModel>();
            var negitems = GetItemsFromNegTableForSummaryView();// GetItemsFromNegTable();
            try
            {
                using (var context = new Entities())
                {
                    foreach (var item in negitems)
                    {
                        var query = context.SOURCING_NEG_PRICE.Where(m => m.ITEM_NO == _itemNo && m.NEG_NO == item.NEG_NO).ToList();


                        foreach (var items in query)
                        {
                            VendorQuotes.Add(new QuotationViewModel
                            {
                                ID = items.ID,

                                ITEM_NO = items.ITEM_NO,
                                UNIT_PRICE = items.UNIT_PRICE ?? 0,
                                TOTAL_PRICE = items.TOTAL_PRICE ?? 0,
                                _VendorDetails = GetVendorsDetails((decimal)item.VENDOR_ID, item.NEG_NO, item.STATUS)


                            });
                        }


                    }

                    return VendorQuotes;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }
        #endregion
        public ContactedVendorViewModel GetVendorsDetails(decimal VendorId, decimal negNo, string Status = null)
        {
            ContactedVendorViewModel vend = new ContactedVendorViewModel();
            try{
                using (var context = new Entities())
                {
                    var getVendorQuotePerNegNo = context.SOURCING_NEGOTIATIONS.Where(m => m.VENDOR_ID == VendorId && m.NEG_NO == negNo).FirstOrDefault();
                    var query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.VENDOR_ID == VendorId).FirstOrDefault();
                    if (query != null && getVendorQuotePerNegNo != null)
                    {
                        
                        vend.VENDOR_NAME = query.VENDOR_NAME;
                        vend.STATUS = Status;
                        vend.TOTALBFTAX = getVendorQuotePerNegNo.TOTAL_AMT;
                        vend.VAT = getVendorQuotePerNegNo.VATVALUE;
                        vend.GRANDTOTAL = getVendorQuotePerNegNo.GRANDTOTAL;
                        
                      
                        return vend;
                    }    
                    else
                    {
                        return null;
                    }
                }
            }catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }

        public List<KeyValuePair<string, string>> GetRefNumberAndRequestType()
        {
            List<KeyValuePair<string, string>>  _ReqInfo = new List<KeyValuePair<string, string>>();
            decimal _TempId = Convert.ToDecimal(this.TempId);
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_REF_TEMP_LINK.Join(context.SOURCING_REQUEST_TYPE, link => link.REQ_TYPE, reqT => reqT.REQUEST_ID, (link, reqT) => new { _link = link, _reqT = reqT }).Where(m => m._link.TEMP_NO == _TempId).FirstOrDefault();
                    _ReqInfo.Add(new KeyValuePair<string, string>("RequestTypeName", query._reqT.REQUEST_NAME));
                    _ReqInfo.Add(new KeyValuePair<string, string>("RequestTypeShortCode", query._reqT.REQUEST_SHORTCODE));
                    _ReqInfo.Add(new KeyValuePair<string, string>("RequestTypeID", Convert.ToString(query._reqT.REQUEST_ID)));
                    _ReqInfo.Add(new KeyValuePair<string, string>("RefNumber", query._link.REF_NO));
                    return _ReqInfo;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + " - " + ex.StackTrace, "error");
                return null;
            }
        }
    }
}