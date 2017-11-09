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
    public class ProcurementResolveController : Controller
    {
        // GET: ProcurementResolve
        public ActionResult AuthorityToProceed(string TempId = null,string VendorId = null)
        {
            RFIRFPComboViewModel response = new RFIRFPComboViewModel();
            RFQSummaryDisplay _response = new RFQSummaryDisplay();
            VendorResponseBLL _VendorResponse = new VendorResponseBLL();
            ProcurementResolveBLL _ProcurementResolve = new ProcurementResolveBLL();
            decimal? ReqType;
            decimal _TempId = Convert.ToDecimal(TempId);
            if (TempId == null && VendorId == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                _VendorResponse.TempNo = TempId;
                _ProcurementResolve.TempID = TempId;
                _VendorResponse.VendorId = VendorId;
                _ProcurementResolve.VendorID = VendorId;
                _ProcurementResolve.Resolve = "Authority To Proceed";
                _ProcurementResolve.UpdateVendorResolve();
                ViewBag.TempId = TempId;
                ViewBag.VendorId = VendorId;
                decimal _vendorId = Convert.ToDecimal(VendorId);
                ReqType = NegotiateBLL.getRequestType(_TempId);
                if (ReqType == 2)
                {
                    response = _VendorResponse.GetRFPSummaryInfo();
                    if (response.Vendor.STATUS != "RESPONDED" && response.Vendor.STATUS != null)
                    {
                        ViewBag.Response = _VendorResponse.getNegotiatedPrice(response.Request.TEMP_ID, _vendorId);
                    }
                    return View(response);
                }
                else
                {
                     _response = _VendorResponse.GetRFQQuotationSummaryAfter();
                    if (_response._Vendor.STATUS != "RESPONDED" && _response._Vendor.STATUS != null)
                    {
                        ViewBag.Response = _VendorResponse.getNegotiatedPrice(_response.Request.TEMP_NO, _vendorId);
                    }
                    return View("AuthorityToProceedII",_response);
                }
                
            }
           
        }

        public ActionResult RegLetter(string TempId = null, string VendorId = null)
        {
            RFIRFPComboViewModel response = new RFIRFPComboViewModel();
            RFQSummaryDisplay _response = new RFQSummaryDisplay();
            LorVendorViewModel _vend = new LorVendorViewModel();
            VendorResponseBLL _VendorResponse = new VendorResponseBLL();
            ProcurementResolveBLL _ProcurementResolve = new ProcurementResolveBLL();
            decimal? ReqType;
            decimal _TempId = Convert.ToDecimal(TempId);
            if (TempId == null && VendorId == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                _VendorResponse.TempNo = TempId;
                _ProcurementResolve.TempID = TempId;
                _VendorResponse.VendorId = VendorId;
                _ProcurementResolve.VendorID = VendorId;
                _ProcurementResolve.Resolve = "Letter of Rejection";
                _ProcurementResolve.UpdateVendorResolve();
                ViewBag.TempId = TempId;
                ViewBag.VendorId = VendorId;
                ReqType = NegotiateBLL.getRequestType(_TempId);
                if (ReqType == 2)
                {
                    response = _VendorResponse.GetRFPSummaryInfo();
                    _vend.InitiatorName = response.Request.INITIATOR_NAME;
                    _vend.RefNum = response.RefNum.REF_NO;
                    _vend.VendorName = response.Vendor.VENDOR_NAME;
                }
                else
                {
                    _response = _VendorResponse.GetRFQQuotationSummaryAfter();
                    _vend.InitiatorName = _response.Initiator.NAME;
                    _vend.RefNum = _response.RefNum.REF_NO;
                    _vend.VendorName = _response._Vendor.VENDOR_NAME;
                }
                return View(_vend);

            }
        }

        public ActionResult AwardContract(string TempId = null, string VendorId = null)
        {
            if (TempId == null && VendorId == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ProcurementResolveBLL _ProcurementResolve = new ProcurementResolveBLL();
            _ProcurementResolve.VendorID = VendorId;
            _ProcurementResolve.TempID = TempId;
            _ProcurementResolve.Resolve = "PROCESSING";
            _ProcurementResolve.Resolution = "AWARDED"; 
            bool outcome = _ProcurementResolve.UpdateVendorResolveII();
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }
    }
}