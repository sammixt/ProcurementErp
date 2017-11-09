using ProcurementErp.BLL;
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
    public class RequestController : Controller
    {
        // GET: Request
        public ActionResult Index()
        {
            RequestTypeViewModel _request = new RequestTypeViewModel();
            _request._Request = AdminRequestProcesses.GetRequestType();
            return View(_request);
        }

        public ActionResult RFQForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RFQForm(RFQREQViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            var FullName = principal.FindFirst(ClaimTypes.GivenName).Value;
            var UserId = principal.FindFirst(ClaimTypes.Actor).Value;

            model.INITIATOR_NAME = FullName;
            model.INITIATOR_ID = Convert.ToDecimal(UserId);
            if (ModelState.IsValid)
            {
                string process = ProcessRFQ.ProcessRfqFormOne(model);
                if (process != null)
                {
                    HttpContext.Session["TempId"] = process;
                    return RedirectToAction("RFQFormII");
                }

            }
            return View();
        }


        public ActionResult RFQFormII()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RFQFormII(RFQITEMViewModel[] item)
        {
            string process = ProcessRFQ.ProcessRfqFormTwo(item);
            if (process != null)
            {
                HttpContext.Session["TempId"] = process;
                return RedirectToAction("RFQFormV");
            }
            return View();
        }


        public ActionResult RFQFormV()
        {

            ViewBag.SelectItems = Common.GetVendorsList();
            return View();
        }

        [HttpPost]
        public ActionResult RFQFormV(ContactedVendorViewModel[] item)
        {
            string process = Common.InsertVendor(item, "RFQ", 3);
            if (process != null)
            {
                return RedirectToAction("RFQSummary");
            }
            return View();
        }

       

        public ActionResult RFQSummary(string TempId = null)
        {
            var _Summary = ProcessRFQ.GetSummaryInfo(TempId);
            if (TempId != null)
            {
                ViewBag.ShowSubmit = "false";
            }
            
            return View(_Summary);

            //send email to vendors//
        }

        [HttpPost]
        public ActionResult Index(RequestTypeViewModel model)
        {
            if (model.REQUEST_ID == 3)
            {
                return RedirectToAction("RFQForm");
            }
            else if (model.REQUEST_ID == 2)
            {
                return RedirectToAction("RFPIndex");
            }
            else if (model.REQUEST_ID == 1)
            {
                return RedirectToAction("RFI");
            }
            return RedirectToAction("Index");

        }

        public ActionResult RFPIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RFPIndex(RFIRFPComboViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            var FullName = principal.FindFirst(ClaimTypes.GivenName).Value;
            var UserId = principal.FindFirst(ClaimTypes.Actor).Value;

            model.Request.INITIATOR_NAME = FullName;
            model.Request.INITIATOR = Convert.ToDecimal(UserId);
            string process = ProcessRFP.ProcessRFPIndex(model);
            if (process != null)
            {
                HttpContext.Session["TempId"] = process;
                return RedirectToAction("RFPFormII");
            }
            return View();
        }

        public ActionResult RFPFormII()
        {
           // HttpContext.Session["TempId"] = "191"; //for testing remove later
            return View();
        }

        [HttpPost]
        public ActionResult RFPFormII(RFQITEMViewModel[] item)
        {
            string process = ProcessRFQ.ProcessRfqFormTwo(item);
            if (process != null)
            {
                HttpContext.Session["TempId"] = process;
                return RedirectToAction("RFPFormIII");
            }
            return View();
        }

        public ActionResult RFPFormIII()
        {
            ViewBag.SelectItems = Common.GetVendorsList();
            return View();
        }

        [HttpPost]
        public ActionResult RFPFormIII(ContactedVendorViewModel[] item)
        {
            string process = Common.InsertVendor(item, "RFP", 2);
            if (process != null)
            {
                return RedirectToAction("RFPSummary");
            }
            return View();
        }

       

        public ActionResult RFPSummary(string TempId = null)
        {
            var _summary = ProcessRFP.GetSummaryInfo(TempId);
            if (TempId != null)
            {
                ViewBag.ShowSubmit = "false";
            }
            return View(_summary);
        }

        public ActionResult RFI()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RFI(RFIRFPComboViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            var FullName = principal.FindFirst(ClaimTypes.GivenName).Value;
            var UserId = principal.FindFirst(ClaimTypes.Actor).Value;

            model.Request.INITIATOR_NAME = FullName;
            model.Request.INITIATOR = Convert.ToDecimal(UserId);
            string process = ProcessRFP.ProcessRFPIndex(model);
            if (process != null)
            {
                HttpContext.Session["TempId"] = process;
                return RedirectToAction("RFIInfo");
            }
            return View();
        }

        public ActionResult RFIInfo()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RFIInfo(RFQITEMViewModel item)
        {
            string process = ProcessRFQ.ProcessRfiInfo(item);
            if (process != null)
            {
                HttpContext.Session["TempId"] = process;
                return RedirectToAction("RFIVendors");
            }
            return View();
        }

        public ActionResult RFIVendors()
        {
            ViewBag.SelectItems = Common.GetVendorsList();
            return View();
        }

        [HttpPost]
        public ActionResult RFIVendors(ContactedVendorViewModel[] item)
        {
            string process = Common.InsertVendor(item, "RFI", 1);
            if (process != null)
            {
                return RedirectToAction("RFISummary");
            }
            return View();
        }


        public ActionResult RFISummary(string TempId = null)
        {
            var _summary = ProcessRFP.GetSummaryInfo(TempId);
            string checker = (TempData["ShowSubmit"] != null) ? TempData["ShowSubmit"].ToString() : null;
            if (TempId != null && checker == null)
            {
                ViewBag.ShowSubmit = "false";
            }
            else
            {
                ViewBag.ShowSubmit = null;
            }
           
            return View(_summary);
        }

        public ActionResult ProcessSummaryPage(string UpdateVendor = null)
        {
            bool outcome = Common.ProcessSummary(UpdateVendor);
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Completed()
        {
            return View();
        }

        public JsonResult Close(string TempNo)
        {
            var closedState = Common.CloseRequest(TempNo);
            return Json(closedState, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Search(string RefNum)
        {
            var Search = ProcessRFP.SearchRequest(RefNum);
            if (Search == null)
            {
                
                return Json("No Record Found",JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Search, JsonRequestBehavior.AllowGet);
            }
            
        }
      
    }
}