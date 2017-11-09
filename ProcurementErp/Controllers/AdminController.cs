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
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult PendingSourceReq()
        {
            List<SourceRequestViewModel> _req = new List<SourceRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            if (UserRole == "Administrator")
            {
                _req = RetrieveRequest.GetNewRequestFromHODToProcurementBuyer();
            }
            else
            {
                _req = RetrieveRequest._GetNewRequestFromHODToProcurementBuyer(null,UserId);
            }
            return View(_req);
        }

        public ActionResult PendingSourceReqPopUp(string ItemId)
        {
            var RequestHistory = RetrieveRequest.GetNewRequestFromHODToProcurementBuyer(ItemId);
            return PartialView(RequestHistory);
        }

        public ActionResult TreatedSourceReq()
        {
            //check for request id on the request table
            List<SourceRequestViewModel> RequestHistory = new List<SourceRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            if (UserRole == "Administrator")
            {
                RequestHistory = RetrieveRequest.GetSourceRequestHistoryAdmin();
            }
            else
            {
                RequestHistory = RetrieveRequest.GetSourceRequestHistoryAdmin(UserId);
            }
            
            return View(RequestHistory);
        }

        //Pops up when creating a new RFQ or RFP
        //enables buyer mapp request initiated from branch being mapped to 
        //new RFQ or RFP request.

        public ActionResult MappTreatedSourceReq()
        {
            List<SourceRequestViewModel> RequestHistory = new List<SourceRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            if (UserRole == "Administrator")
            {
                RequestHistory = RetrieveRequest.GetSourceRequestHistoryAdmin(null,null,"Mapp");
            }
            else
            {
                RequestHistory = RetrieveRequest.GetSourceRequestHistoryAdmin(UserId,null,"Mapp");
            }

            return PartialView(RequestHistory);
        }

        public ActionResult TreatedSourceReqPopUp(string ItemId)
        {
            var RequestHistory = RetrieveRequest.GetSourceRequestHistoryAdmin(null, ItemId);
            return PartialView(RequestHistory);
        }

        public JsonResult PendingSourceReqApproval(SourceRequestViewModel model)
        {
            //check for request id on the request on the request table
            var outcome = ProcessRequest.UpdateSourceRequestApproval(model);
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CountPendingSourceReq()
        {
            //later check for request by userid on the request table
            List<SourceRequestViewModel> _req = new List<SourceRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            if (UserRole == "Administrator")
            {
                _req = RetrieveRequest.GetNewRequestFromHODToProcurementBuyer();
            }
            else
            {
                _req = RetrieveRequest._GetNewRequestFromHODToProcurementBuyer(null, UserId);
            }
            decimal Req = _req != null ? _req.Count() : 0;
            string _count = Convert.ToString(Req);
            return Json(_count, JsonRequestBehavior.AllowGet);
        }
    }
}