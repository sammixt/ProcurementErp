using ProcurementErp.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    [Authorize]
    public class ResponseAnalysisController : Controller
    {
        // GET: ResponseAnalysis for both RFP and RFQ
        public ActionResult RFPAnalysis(string TempNo)
        {
            AnalysisBLL _Analysis = new AnalysisBLL();
            _Analysis.TempId = TempNo;

           ViewBag.ReferenceNum = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RefNumber").FirstOrDefault().Value;
           ViewBag.ReqType = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RequestTypeName").FirstOrDefault().Value;
           ViewBag.ReqShortCode = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RequestTypeShortCode").FirstOrDefault().Value;
           ViewBag.ReqID = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RequestTypeID").FirstOrDefault().Value;
           ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
           ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"].ToString() : null;
            var _Analyse = _Analysis.GetVendorResponseAnalysis();
            ViewBag.TempNo = TempNo;
            return View(_Analyse);
        }

        [HttpGet]
        public FileContentResult ExportToExcel(string TempNo)
        {
            byte[] fileContent = ExportToExcelBLL.ExportExcel(TempNo);
            return File(fileContent, ExportToExcelBLL.ExcelContentType, "Report.xlsx");
        }

        [HttpGet]
        public FileContentResult ExportToExcelII(string TempNo)
        {
            byte[] fileContent = ExportToExcelBLL.ExportExcel(TempNo,"Originator");
            return File(fileContent, ExportToExcelBLL.ExcelContentType, "Report.xlsx");
        }
    }
}