using ProcurementErp.BLL;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    [Authorize]
    public class NegotiateController : Controller
    {
        // GET: Negotiate... May be removed
        public ActionResult Index(string TempNo)
        {
            var getItems = ProcessRFP.GetSummaryInfo(TempNo);
            var respondedVendors = getItems.Vendors.Where(m => m.STATUS == "RESPONDED").ToList();
            getItems.Vendors = respondedVendors;
            if (getItems.Request.TEMP_ID == 0)
            {
                getItems.Request.TEMP_ID = Convert.ToDecimal(TempNo);
            }
            return PartialView(getItems);
        }

        //Negotiate with a single vendor for RFP
        public ActionResult NegotiatePerVendor(string TempNo, string VendorId, string NegNum = null)
        {
            decimal? _vendorId = Convert.ToDecimal(VendorId);
            var getItems = ProcessRFP.GetSummaryInfo(TempNo);
            var respondedVendors = getItems.Vendors.Where(m => m.VENDOR_ID == _vendorId).ToList();
            getItems.Vendors = respondedVendors;
            if (getItems.Request.TEMP_ID == 0)
            {
                getItems.Request.TEMP_ID = Convert.ToDecimal(TempNo);
            }
            TempData["TempNo"] = TempNo;
            TempData["VendorId"] = VendorId;
            ViewBag.NegNum = (NegNum != null) ? NegNum : "0";
            return PartialView("Index",getItems);
        }

        [HttpPost]
        public ActionResult Negotiate(RFIRFPComboViewModel inputs,FormCollection collections) 
        {
            decimal? ReqType;
            string status;
            var TempNo = TempData["TempNo"] != null?TempData["TempNo"].ToString():null;
            var _VendorId = TempData["VendorId"] != null?TempData["VendorId"].ToString():null;
            ReqType = NegotiateBLL.getRequestType(inputs.Request.TEMP_ID);
            
            status = NegotiateBLL.InsertNegotiationForRequest(inputs, collections);
            if (status == "error")
            {
                TempData["ErrorMessage"] = "An Error Occurred";
            }
            else
            {
                TempData["SuccessMessage"] = "Negotiation Sent to selected vendors";
            }
            if (ReqType == 2)
            {
                if (TempNo != null && _VendorId != null)
                {
                    return RedirectToAction("RFPVendorResponse", "VendorResponse", new { TempId = TempNo, VendorId = _VendorId });
                }
                return RedirectToAction("RFPAnalysis", "ResponseAnalysis", new { TempNo = inputs.Request.TEMP_ID });
            }
            else 
            {
                if (TempNo != null && _VendorId != null)
                {
                    return RedirectToAction("RFQVendorResponse", "VendorResponse", new { TempId = TempNo, VendorId = _VendorId });
                }
                return RedirectToAction("RFPAnalysis", "ResponseAnalysis", new { TempNo = inputs.Request.TEMP_ID });
            }              
            
        }

        public ActionResult UpdateNegStatus(string TempNo, string NegNum = null, string Status = null)
        {
            var outcome = NegotiateBLL.UpdateNegStatus(TempNo, NegNum, Status);
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }
    }
}