using ProcurementErp.BLL;
using ProcurementErp.Services;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string EmpNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            string _Count = RetrieveRequest.GetPendingRequestCount(EmpNum);
            string _HistoryCount = RetrieveRequest.GetRequestHistoryCount(EmpNum);
            ViewBag.Pending = _Count;
            ViewBag.HistCount = _HistoryCount;
            return View();
        }

        public ActionResult NewRequest()
        {
            SourceRequestViewModel item = new SourceRequestViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string _dept = principal.FindFirst(ClaimTypes.UserData).Value;
            string _deptCode = principal.FindFirst(ClaimTypes.Actor).Value;
            string _branch = principal.FindFirst(ClaimTypes.StateOrProvince).Value;
            string _branchCode = principal.FindFirst(ClaimTypes.PostalCode).Value;
            
            item.INITIATING_DEPT = _dept;
            item.INITIATING_DEPTCODE = _deptCode;
            item.INITIATING_BRANCH = _branch;
            item.INITIATING_BRANCHCODE = _branchCode;
            ViewBag.ItemCategories = Common.GetMainCategory();
            ViewBag.Vendors = GetVendors.GetAllVendors();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRequest(SourceRequestViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string EmpNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            string FullName = principal.FindFirst(ClaimTypes.GivenName).Value;
            string Email = principal.FindFirst(ClaimTypes.Email).Value;
            if (ModelState.IsValid)
            {
                model.INITIATOR_NAME = FullName;
                model.INITIATOR_NUMBER = EmpNum;
                model.INITIATOR_EMAIL = Email;
                ProcessRequest.ProcessNewRequest(model, EmpNum);
                return RedirectToAction("RequestHistory");
            }
            else 
            {
                SourceRequestViewModel item = new SourceRequestViewModel();
                string _dept = principal.FindFirst(ClaimTypes.UserData).Value;
                string _deptCode = principal.FindFirst(ClaimTypes.Actor).Value;
                string _branch = principal.FindFirst(ClaimTypes.StateOrProvince).Value;
                string _branchCode = principal.FindFirst(ClaimTypes.PostalCode).Value;

                item.INITIATING_DEPT = _dept;
                item.INITIATING_DEPTCODE = _deptCode;
                item.INITIATING_BRANCH = _branch;
                item.INITIATING_BRANCHCODE = _branchCode;
                ViewBag.ItemCategories = Common.GetMainCategory();
                ViewBag.Vendors = GetVendors.GetAllVendors();
                ViewBag.Error = "An Error Occurred";
                return View(item);
            }

            
        }

        public ActionResult EditRequest(string RequestId =null) 
        {
            if (RequestId == null)
            {
                return RedirectToAction("RequestHistory");
            }
            SourceRequestViewModel item = new SourceRequestViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string _dept = principal.FindFirst(ClaimTypes.UserData).Value;
            string _deptCode = principal.FindFirst(ClaimTypes.Actor).Value;
            string _branch = principal.FindFirst(ClaimTypes.StateOrProvince).Value;
            string _branchCode = principal.FindFirst(ClaimTypes.PostalCode).Value;
            var RequestHistory = RetrieveRequest.GetRequestHistory(null, RequestId).FirstOrDefault();
            
            ViewBag.ItemCategories = Common.GetMainCategory();
            ViewBag.Vendors = GetVendors.GetAllVendors();
            return View(RequestHistory);
        }

        [HttpPost]
        public ActionResult EditRequest(SourceRequestViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _dept = principal.FindFirst(ClaimTypes.UserData).Value;
            string _deptCode = principal.FindFirst(ClaimTypes.Actor).Value;
            string _branch = principal.FindFirst(ClaimTypes.StateOrProvince).Value;
            string _branchCode = principal.FindFirst(ClaimTypes.PostalCode).Value;
            SourceRequestViewModel item = new SourceRequestViewModel();
            if (ModelState.IsValid)
            {
                ProcessRequest.EditRequest(model, _branchCode);
                return RedirectToAction("RequestHistory");
            }
            else 
            {
                return RedirectToAction("EditRequest", "Home", new { RequestId = model.TEMP_ID });
            }
        }
        public ActionResult DeleteFile(string FileId, string RequestId)
        {
            bool outcome = DocumentUploadDownload.DeleteFileFromServer(FileId);
            
                return RedirectToAction("EditRequest", "Home", new { RequestId = RequestId });
        }

        public ActionResult RequestPendingApproval()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string EmpNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            var RequestHistory = RetrieveRequest.GetRequestPendingApproval(EmpNum);
            return View(RequestHistory);
        }

        public ActionResult RequestPendingApprovalPopUp(string ItemId)
        {
            var RequestHistory = RetrieveRequest.GetRequestPendingApproval(null, ItemId);
            return PartialView(RequestHistory);
        }

        public JsonResult Approval(SourceRequestViewModel model)
        {
            var outcome = ProcessRequest.UpdateRequestApproval(model);
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RequestHistory()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string EmpNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            var RequestHistory = RetrieveRequest.GetRequestHistory(EmpNum);
            return View(RequestHistory);
        }

        public ActionResult RequestHistoryPopup(string ItemId)
        {
            var RequestHistory = RetrieveRequest.GetRequestHistory(null, ItemId);
            return PartialView(RequestHistory);
        }

        public JsonResult GetCategoryById(int ParentId)
        {
            decimal? _ParentId = ParentId;
            var _Categories = Common.GetCategoryByParentId(_ParentId);
            if (_Categories != null)
            {
                return Json(_Categories, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
                
            }
        }

        public JsonResult GetRequestHistoryCount()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string EmpNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            string _Count = RetrieveRequest.GetRequestHistoryCount(EmpNum);
            return Json(_Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingRequestCount()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string EmpNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            string _Count = RetrieveRequest.GetPendingRequestCount(EmpNum);
            return Json(_Count, JsonRequestBehavior.AllowGet);
        }


    }
}