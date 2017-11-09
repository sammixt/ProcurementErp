using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcurementErpVendor.Models;
using System.Web.Mvc;

namespace ProcurementErpVendor.BLL
{
    public class NegotiateBLL
    {
        public static List<RFQITEMViewModel> GetItems(string TempNo)
        {
            decimal _TempNo = Convert.ToDecimal(TempNo);
            List<RFQITEMViewModel> _items = new List<RFQITEMViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _TempNo).ToList();
                    if (getItems != null)
                    {
                        foreach (var singleItem in getItems)
                        {
                            _items.Add(new RFQITEMViewModel
                            {
                                QUANTITY = singleItem.QUANTITY,
                                UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                DESCRIPTION = singleItem.DESCRIPTION,
                                RFI_INFO = singleItem.RFI_INFO,
                                ITEM_NO = singleItem.ITEM_NO
                            });
                        }
                        return _items;
                    }
                    else return null;
                }
            }
            
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.StackTrace, "error");
                return null;
            }
            
        }

        public static string InsertNegotiationForRequest(List<RFQITEMViewModel> inputs, FormCollection collection,decimal _vendorId)
        {
            NegotiationsViewModel model = new NegotiationsViewModel();
            //SOURCING_RFQ_QUOTATION query = new SOURCING_RFQ_QUOTATION();
            SOURCING_NEGOTIATIONS _negotiations = new SOURCING_NEGOTIATIONS();
            SOURCING_NEG_PRICE query = new SOURCING_NEG_PRICE();

            try
            {
                decimal ResponseNumber = Convert.ToDecimal(collection["NegNumber"].ToString());
                var context = new Entities();
                
                    decimal _negNo = Common.GetSequenceB();
                    model.VAT = Convert.ToDecimal(collection["VAT"].ToString());
                    model.VATVALUE = Convert.ToDecimal(collection["VATVALUE"].ToString());
                    model.TOTAL_AMT = Convert.ToDecimal(collection["TOTALAMT"].ToString());
                    model.GRANDTOTAL = Convert.ToDecimal(collection["GRANDTOTAL"].ToString());
                    model.TEMP_ID = Convert.ToDecimal(collection["TempNo"].ToString());
                    model.RESPONSE_NO = ResponseNumber;
                    model.VENDOR_ID = _vendorId;
                    model.RES_DATE = DateTime.Now;

                    _negotiations.NEG_NO = _negNo;
                    _negotiations.RESPONSE_NO = model.RESPONSE_NO;
                    _negotiations.GRANDTOTAL = model.GRANDTOTAL;
                    _negotiations.TOTAL_AMT = model.TOTAL_AMT;
                    _negotiations.VAT = model.VAT;
                    _negotiations.VATVALUE = model.VATVALUE;
                    _negotiations.RES_DATE = model.RES_DATE;
                    _negotiations.VENDOR_ID = _vendorId;
                    _negotiations.NEGOTIATOR = "VENDOR";
                    _negotiations.TEMP_ID = model.TEMP_ID;
                    _negotiations.STATUS = "";
                    context.SOURCING_NEGOTIATIONS.Add(_negotiations);
                    context.SaveChanges();
                    foreach (var item in inputs)
                    {
                        InsertNegPrice(item, model.TEMP_ID, _negNo);
                    }

                    UpdateVendorStatus(_vendorId, (decimal)model.TEMP_ID, ResponseNumber);
                
                return "success";
            }
            catch (Exception ex)
            {
                Logger.Log("An error occurred, Error: " + ex.StackTrace, "error");
                return "error";
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

        public static void UpdateVendorStatus(decimal VendorId, decimal _TempId, decimal NegNo)
        {
            using (var db = new Entities())
            {
                var getNegNum = db.SOURCING_NEGOTIATIONS.Where(m => m.NEG_NO == NegNo).FirstOrDefault();
                if (getNegNum != null)
                {
                    getNegNum.STATUS = "NEGOTIATING";
                    db.SaveChanges();
                }
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
                Logger.Log("An error occurred, Error: " + ex.StackTrace, "error");
                return false;
            }
        }
    }
}