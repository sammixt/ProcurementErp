using ProcurementErp.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    [Authorize]
    public class VendorResponseController : Controller
    {
        // GET: VendorResponse
        public ActionResult ResponseList(string ReqType,string TempNo = null)
        {
            if (TempNo == null)
            {
                return RedirectToAction(ReqType, "RunningRequest");
            }
            else
            {
                VendorResponseBLL _VendorResponse = new VendorResponseBLL();
                _VendorResponse.TempNo = TempNo;
                var response = _VendorResponse.GetVendorResponseList();
                return View(response);
            }   
        }

        public ActionResult RFQVendorResponse(string TempId = null, string VendorId = null)
        {
            if (TempId != null && VendorId != null)
            {
                VendorResponseBLL _VendorResponse = new VendorResponseBLL();
                _VendorResponse.TempNo = TempId;
                _VendorResponse.VendorId = VendorId;
                var response = _VendorResponse.GetRFQQuotationSummaryAfter();
                
                ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
                ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"].ToString() : null;
                return View(response);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        public ActionResult RFIVendorResponse(string TempId = null, string VendorId = null)
        {
            if (TempId != null && VendorId != null)
            {
                VendorResponseBLL _VendorResponse = new VendorResponseBLL();
                _VendorResponse.TempNo = TempId;
                _VendorResponse.VendorId = VendorId;
                var response = _VendorResponse.GetRFISummaryInfo();
                return View(response);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        public ActionResult RFPVendorResponse(string TempId = null, string VendorId = null)
        {
            if (TempId != null && VendorId != null)
            {
                VendorResponseBLL _VendorResponse = new VendorResponseBLL();
                _VendorResponse.TempNo = TempId;
                _VendorResponse.VendorId = VendorId;
                var response = _VendorResponse.GetRFPSummaryInfo();
                
                ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
                ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"].ToString() : null;
                return View(response);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        public FileResult DownloadFile(string FIleId)
        {
            string ContentType = string.Empty;
            decimal _FileId = Convert.ToDecimal(FIleId);
            string FileName = Common.GetFilePath(_FileId);
            if (FileName.Contains(".pdf"))
            {
                ContentType = "application/pdf";
            }
            else if (FileName.Contains(".xlsx"))
            {
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else if (FileName.Contains(".xls"))
            {
                ContentType = "application/vnd.ms-excel";
            }
            return File(FileName, ContentType, FileName);
        }
    }
}