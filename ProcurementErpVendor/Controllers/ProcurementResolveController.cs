using ProcurementErpVendor.BLL;
using ProcurementErpVendor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.Controllers
{
    [Authorize]
    public class ProcurementResolveController : Controller
    {
        // GET: ProcurementResolve
        public ActionResult AuthorityToProceed(string TempId = null)
        {
            decimal _TempId = Convert.ToDecimal(TempId);
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            decimal? ReqType = Common.getRequestType(_TempId);
            if (ReqType == 2) 
            {
                var _request = GetRequest.GetRFPSummaryInfo(_TempId, _VendorId);
                if (_request.Vendor.STATUS != "RESPONDED" && _request.Vendor.STATUS != null)
                {
                    ViewBag.Response = GetRequest.getNegotiatedPrice(_request.Request.TEMP_ID, _VendorId);
                }
                return View(_request);
            }
            else
            {
                var output = GetRequest.GetRFQQuotationSummaryAfter(_TempId, _VendorId);
                if (output._Vendor.STATUS != "RESPONDED" && output._Vendor.STATUS != null)
                {
                    ViewBag.Response = GetRequest.getNegotiatedPrice(output.Request.TEMP_NO, _VendorId);
                }
                return View("AuthorityToProceedII",output);
            }
            
        }

        public ActionResult RegLetter(string TempId = null)
        {
            RFIRFPComboViewModel response = new RFIRFPComboViewModel();
            RFQSummaryDisplay _response = new RFQSummaryDisplay();
            LorVendorViewModel _vend = new LorVendorViewModel();
             var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            decimal? ReqType;
            decimal _TempId = Convert.ToDecimal(TempId);
            if (TempId == null && VendorId == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.TempId = TempId;
                ViewBag.VendorId = VendorId;
                ReqType = Common.getRequestType(_TempId);
                if (ReqType == 2)
                {
                    response = GetRequest.GetRFPSummaryInfo(_TempId, _VendorId);
                    _vend.InitiatorName = response.Request.INITIATOR_NAME;
                    _vend.RefNum = response.RefNum.REF_NO;
                    _vend.VendorName = response.Vendor.VENDOR_NAME;
                }
                else
                {
                    _response = GetRequest.GetRFQQuotationSummaryAfter(_TempId, _VendorId);
                    _vend.InitiatorName = _response.Initiator.NAME;
                    _vend.RefNum = _response.RefNum.REF_NO;
                    _vend.VendorName = _response._Vendor.VENDOR_NAME;
                }
                return View(_vend);

            }
        }
    }
}