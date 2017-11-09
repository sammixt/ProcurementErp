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
    public class UpdateRequestController : Controller
    {
        // GET: UpdateRequest
        #region RFQ UPDATE
        public ActionResult EditRFQForm(string TempId = null)
        {
            decimal _TempId;
            UpdateRequestBLL _model = new UpdateRequestBLL();
            if (TempId == null)
            {
                return RedirectToAction("RFQList","RunningRequest");
            }
            else
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] != null?TempData["ErrorMessage"].ToString():null;
                _TempId = Convert.ToDecimal(TempId);
                _model.TempId = _TempId;
                var getRequest = _model.GetRFQRequest();
                return View(getRequest);
            }
        }
        [HttpPost]
        public ActionResult UpdateRFQForm(RFQREQViewModel model)
        {
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.model = model;
            decimal output = _model.UpdateRFQRequest();
            if (output != 0)
            {
                HttpContext.Session["TempId"] = Convert.ToString(output);
                return RedirectToAction("EditRFQItem");
            }
            else
            {
                TempData["ErrorMessage"] = "An Error Occurred";
                return RedirectToAction("EditRFQForm", "UpdateRequest", new { TempId = Convert.ToString(model.TEMP_NO) });
            }

        }

        public ActionResult EditRFQItem()
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
            var getRequest = _model.GetItems();
            return View(getRequest);      
        }

        [HttpPost]
        public ActionResult UpdateRFQItem(RFQITEMViewModel[] item)
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            _model.items = item.ToList();
            decimal output = _model.UpdateREQItems();
            if (output != 0)
            {
                HttpContext.Session["TempId"] = Convert.ToString(output);
                return RedirectToAction("EditRFQVendor");
            }
            else
            {
                TempData["ErrorMessage"] = "An Error Occurred";
                return RedirectToAction("EditRFQItem");
            }
        }

        public ActionResult EditRFQVendor()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
            ViewBag.SelectItems = Common.GetVendorsList();
            var ContactedVendorList = Common.GetContactedVendor();
            return View(ContactedVendorList);
        }

        [HttpPost]
        public ActionResult UpdateRFQVendor(ContactedVendorViewModel[] item)
        {
            string process = Common.InsertVendorForUpdate(item, "RFQ", 3);
            if (process != null)
            {
                return RedirectToAction("RFQSummary");
            }
            return View();
        }

        public ActionResult RFQSummary(string TempId = null)
        {
            var _Summary = ProcessRFQ.GetSummaryInfo(TempId);
            
                ViewBag.ShowSubmit = "contactVendor";
            
            return View("~/Views/Request/RFQSummary.cshtml", _Summary);

            //send email to vendors//
        }
        #endregion

        #region RFP UPDATE
        public ActionResult EditRFPForm(string TempId = null)
        {
            decimal _TempId;

            if (TempId == null)
            {
                return RedirectToAction("RFPList", "RunningRequest");
            }
            else
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
                _TempId = Convert.ToDecimal(TempId);
                var _summary = ProcessRFP.GetSummaryInfo(TempId);
                return View(_summary);
            }
        }

        [HttpPost]
        public ActionResult UpdateRFPForm(RFIRFPComboViewModel model)
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            _model._model = model;

            string output = _model.UpdateRFPIndex();
            if (output != null)
            {
                HttpContext.Session["TempId"] = Convert.ToString(output);
                return RedirectToAction("EditRFPItem");
            }
            else
            {
                TempData["ErrorMessage"] = "An Error Occurred";
                return RedirectToAction("EditRFPForm", "UpdateRequest", new { TempId = TempId });
            }
        }

        public ActionResult EditRFPItem()
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;

            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
            var getRequest = _model.GetItems();
            return View(getRequest);  
        }

        [HttpPost]
        public ActionResult UpdateRFPItem(RFQITEMViewModel[] item)
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            _model.items = item.ToList();
            decimal output = _model.UpdateREQItems();
            if (output != 0)
            {
                HttpContext.Session["TempId"] = Convert.ToString(output);
                return RedirectToAction("EditRFPVendor");
            }
            else
            {
                TempData["ErrorMessage"] = "An Error Occurred";
                return RedirectToAction("EditRFPItem");
            }
        }

        public ActionResult EditRFPVendor()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
            ViewBag.SelectItems = Common.GetVendorsList();
            var ContactedVendorList = Common.GetContactedVendor();
            return View(ContactedVendorList);
        }

        [HttpPost]
        public ActionResult UpdateRFPVendor(ContactedVendorViewModel[] item)
        {
            string process = Common.InsertVendorForUpdate(item, "RFP", 2);
            if (process != null)
            {
                return RedirectToAction("RFPSummary");
            }
            return View();
        }

        public ActionResult RFPSummary(string TempId = null)
        {
            var _summary = ProcessRFP.GetSummaryInfo(TempId);
            ViewBag.ShowSubmit = "contactVendor";
            return View("~/Views/Request/RFPSummary.cshtml",_summary);
        }

        #endregion

        #region Update RFI
        public ActionResult EditRFIForm(string TempId = null)
        {
            decimal _TempId;

            if (TempId == null)
            {
                return RedirectToAction("RFIList", "RunningRequest");
            }
            else
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
                _TempId = Convert.ToDecimal(TempId);
                var _summary = ProcessRFP.GetSummaryInfo(TempId);
                return View(_summary);
            }
           
        }

        [HttpPost]
        public ActionResult UpdateRFIForm(RFIRFPComboViewModel model)
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            _model._model = model;

            string output = _model.UpdateRFPIndex();
            if (output != null)
            {
                HttpContext.Session["TempId"] = Convert.ToString(output);
                return RedirectToAction("GetRFIInfo");
            }
            else
            {
                TempData["ErrorMessage"] = "An Error Occurred";
                return RedirectToAction("EditRFIForm", "UpdateRequest", new { TempId= TempId });
            }
        }

        public ActionResult GetRFIInfo()
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            var info = _model.GetRFIInfo();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
            return View(info);
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult UpdateRFIInfo(RFQITEMViewModel model)
        {
            decimal _TempId;
            string TempId = HttpContext.Session["TempId"].ToString();
            _TempId = Convert.ToDecimal(TempId);
            UpdateRequestBLL _model = new UpdateRequestBLL();
            _model.TempId = _TempId;
            _model._item = model;
            string output = _model.UpdateRFIInfor();
            if (output != null)
            {
                return RedirectToAction("EditRFIVendor");
            }
            else
            {
                TempData["ErrorMessage"] = "An Error Occurred";
                return RedirectToAction("GetRFIInfo");
            }
        }

        public ActionResult EditRFIVendor()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
            ViewBag.SelectItems = Common.GetVendorsList();
            var ContactedVendorList = Common.GetContactedVendor();
            return View(ContactedVendorList);
        }

        [HttpPost]
        public ActionResult UpdateRFIVendor(ContactedVendorViewModel[] item)
        {
            string process = Common.InsertVendorForUpdate(item, "RFP", 1);
            if (process != null)
            {
                TempData["ShowSubmit"] = "Show";
                return RedirectToAction("RFISummary", "Request", new { TempId = process });
            }
            return View();
        }      
        #endregion
    }
}