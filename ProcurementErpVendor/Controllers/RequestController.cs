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
    public class RequestController : Controller
    {
        // GET: Request
        public ActionResult RFQ(decimal TempId)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            bool vendorResponse = GetRequest.CheckVendorResponse(TempId, _VendorId);
            if (vendorResponse == true)
            {
                var _request = GetRequest.GetRFQSummaryInfo(TempId);
                return View(_request);
            }
            else
            {
                return RedirectToAction("RFQSummary", "Request", new { id = TempId });
            }
            
        }


        [HttpPost]
        public ActionResult ProcessQuotation(RFQSummaryViewModel model, FormCollection collections,IEnumerable<HttpPostedFileBase> Docs)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            string VendorName = principal.FindFirst(ClaimTypes.GivenName).Value;
            decimal _id = Convert.ToDecimal(collections["TEMP_NO"].ToString());
            string FileUploadResponse = DocumentUploadDownload.UploadDocument(Docs, collections["REF_NO"].ToString(), _VendorId, _id);
            ProcessResponse response = new ProcessResponse();
            response.items = model.Items;
            response.vendor = collections;
            response._VendorId = _VendorId;
            response.VendorName = VendorName;
            response.RefNUmber = collections["REF_NO"].ToString();
            response.InsertVendorResponseToItems();
            response.InsertSuppliesInfo();
            TempData["Message"] = FileUploadResponse;
            return RedirectToAction("RFQSummary", "Request", new {id = _id });
        }

        [HttpPost]
        public ActionResult UpdateQuotation(RFQSummaryDisplay model, FormCollection collections, IEnumerable<HttpPostedFileBase> Docs)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            string VendorName = principal.FindFirst(ClaimTypes.GivenName).Value;
            decimal _id = Convert.ToDecimal(collections["TEMP_NO"].ToString());
            string FileUploadResponse = DocumentUploadDownload.UploadDocument(Docs, collections["REF_NO"].ToString(), _VendorId, _id);
            ProcessResponse response = new ProcessResponse();
            response.items = model.Items;
            response.vendor = collections;
            response._VendorId = _VendorId;
            response.VendorName = VendorName;
            response.RefNUmber = collections["REF_NO"].ToString();
            response.UpdateVendorResponseToItems();
            response.InsertSuppliesInfo();
            TempData["Message"] = FileUploadResponse;
            return RedirectToAction("RFQSummary", "Request", new { id = _id });
        }

        public ActionResult EditQuotation(decimal TempId)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            var output = GetRequest.GetRFQQuotationSummaryAfter(TempId, _VendorId);
            return View(output);
        }

        public ActionResult RFQSummary(decimal id)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            var output = GetRequest.GetRFQQuotationSummaryAfter(id, _VendorId);
            ViewBag.Message = TempData["Message"] != null ? TempData["Message"].ToString() : null;
            
            return View(output);
        }

        public ActionResult RFQList()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetRFQRequset(VendorID);
            return View(_request);
        }

        public ActionResult RFIList()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetRFIRequset(VendorID);
            return View(_request);
        }

        public ActionResult RFI(decimal TempId)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            bool vendorResponse = GetRequest.CheckVendorResponse(TempId, _VendorId);
            if (vendorResponse == true)
            {
                var _request = GetRequest.GetRFISummaryInfo(TempId);
                return View(_request);
            }
            else
            {
                return RedirectToAction("RFISummary", "Request", new { Temp = TempId });
            }

        }

        public ActionResult RFIResponse(string TempId)
        {
            if (TempId == null)
            {
                return RedirectToAction("RFIList");
            }
            ViewBag.TempNo = TempId;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProcessRFIResponse(ProposalTemplateViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            ProcessResponseRFP response = new ProcessResponseRFP();
            response._VendorId = _VendorId;
            response._requestType = 1;
            response.proposal = model;
            string outcome = response.ProcessRFPProposal();
            if (outcome != null)
            {
                response.UpdateVendorResponse();
            }
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RFISummary(string Temp = null)
        {
            if (Temp == null)
            {
                return RedirectToAction("RFIList");
            }
            decimal TempId = Convert.ToDecimal(Temp);
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            var _request = GetRequest.GetRFISummaryInfo(TempId, _VendorId);
            return View(_request);
        }

        public ActionResult RFPList()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetRFPRequset(VendorID);
            return View(_request);
        }

        public ActionResult RFP(decimal TempId)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            bool vendorResponse = GetRequest.CheckVendorResponse(TempId, _VendorId);
            if (vendorResponse == true)
            {
                var _request = GetRequest.GetRFPSummaryInfo(TempId);
                return View(_request);   
            }
            else
            {
                return RedirectToAction("RFPSummary", "Request", new { Temp = TempId });
            }
            
        }

        //posted RFP response from vendor
        [HttpPost]
        public ActionResult RFP(RFIRFPComboViewModel model, FormCollection collections, IEnumerable<HttpPostedFileBase> Docs)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            string VendorName = principal.FindFirst(ClaimTypes.GivenName).Value;
            decimal _id = Convert.ToDecimal(collections["TEMP_ID"].ToString());
            string FileUploadResponse = DocumentUploadDownload.UploadDocument(Docs, collections["REF_NO"].ToString(), _VendorId, _id);
            ProcessResponseRFP response = new ProcessResponseRFP();
            response.items = model.Item;
            response.vendor = collections;
            response._VendorId = _VendorId;
            response.VendorName = VendorName;
            response.RefNUmber = collections["REF_NO"].ToString();
            response.InsertVendorResponseToItems();
            response.InsertSuppliesInfo();
            return RedirectToAction("RFPTechnicalPro", "Request", new { id = _id });
        }

        
        public ActionResult RFPTechnicalPro(string id)
        {
            ProposalTemplateViewModel _proposal = new ProposalTemplateViewModel();
            if (id == null)
            {
                return RedirectToAction("RFPList");
            }
            ViewBag.TempNo = id;
            decimal _id = Convert.ToDecimal(id);
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
             _proposal = GetRequest.getProposal(_id, _VendorId);
             _proposal.TEMPID = _proposal.TEMPID == null ? _id : _proposal.TEMPID;
             _proposal.VENDORID = _proposal.VENDORID == null ? _VendorId : _proposal.VENDORID;
            return View(_proposal);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProcessRFPTechnicalPro(ProposalTemplateViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            ProcessResponseRFP response = new ProcessResponseRFP();
            response._VendorId = _VendorId;
            response._requestType = 2;
            response.proposal = model;
            string outcome = response.ProcessRFPProposal();
            return Json(outcome,JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditRFPQuotation(string Temp = null)
        {
            if (Temp == null)
            {
                return RedirectToAction("RFPList");
            }
            decimal TempId = Convert.ToDecimal(Temp);
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            var _request = GetRequest.GetRFPSummaryInfo(TempId, _VendorId);
            return View(_request);
        }

        [HttpPost]
        public ActionResult EditRFPQuotationSubmitted(RFIRFPComboViewModel model, FormCollection collections, IEnumerable<HttpPostedFileBase> Docs)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            string VendorName = principal.FindFirst(ClaimTypes.GivenName).Value;
            decimal _id = Convert.ToDecimal(collections["TEMP_ID"].ToString());
            string FileUploadResponse = DocumentUploadDownload.UploadDocument(Docs, collections["REF_NO"].ToString(), _VendorId, _id);
            ProcessResponseRFP response = new ProcessResponseRFP();
            response.items = model.Item;
            response.vendor = collections;
            response._VendorId = _VendorId;
            response.VendorName = VendorName;
            response.RefNUmber = collections["REF_NO"].ToString();
            response.UpdateVendorResponseToItems();
            response.InsertSuppliesInfo();
            return RedirectToAction("RFPTechnicalPro", "Request", new { id = _id });
        }

        public ActionResult RFPSummary(string Temp = null)
        {
            if (Temp == null)
            {
                return RedirectToAction("RFPList");
            }
            decimal TempId = Convert.ToDecimal(Temp);
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            var _request = GetRequest.GetRFPSummaryInfo(TempId, _VendorId);
            //if (_request.Vendor.STATUS != "RESPONDED" && _request.Vendor.STATUS != null)
            //{
            //    ViewBag.Response = GetRequest.getNegotiatedPrice(_request.Request.TEMP_ID);
            //}
            return View(_request);
        }

        public JsonResult UpdateNegotiationResponse(string Response, decimal TempNo, decimal VendorId)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorName = principal.FindFirst(ClaimTypes.GivenName).Value;
            var _response = GetRequest.UpdateVendorNegResponse(Response, TempNo, VendorId,VendorName);
            return Json(_response, JsonRequestBehavior.AllowGet);
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
            else if(FileName.Contains(".xls"))
            {
                ContentType = "application/vnd.ms-excel";
            }
            return File(FileName, ContentType, FileName);
        }


    }
}