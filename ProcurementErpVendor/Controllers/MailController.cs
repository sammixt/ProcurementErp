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
    public class MailController : Controller
    {
        // GET: Mail
        public ActionResult Compose(string RefNum = null, string Title = null, string ParentMsgId = null, decimal TempId = 0, string Init = null)
        {
            if (Title != null && RefNum != null)
            {
                ViewBag.TempId = TempId;
                ViewBag.RefNum = RefNum;
                ViewBag.ProjectTitle = Title;
            }
            if (ParentMsgId != null)
            {
                ViewBag.ParentMsgId = ParentMsgId;
            } if (Init != null) { ViewBag.Init = Init; }
            return View();
        }

        public ActionResult Inbox(string id)
        {
            if (id != null)
            {
                return View("ReadMail");
            }
            return View();
        }

        public ActionResult Sent(string TempId = null)
        {
            List<MessageViewModel> msg = new List<MessageViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            ProcessMail mail = new ProcessMail(_VendorId, "Vendor", "Admin");
            if (TempId != null) 
            {
                msg = mail.GetSentMails(TempId);
            }
            else
            {
                msg = mail.GetSentMails();
            }
            return View(msg);
        }

        public ActionResult SentRead(string id = null)
        {
            MessageViewModel msg = new MessageViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            ProcessMail mail = new ProcessMail(_VendorId, "Vendor", "Admin");
            if (id != null)
            {
                 var _msg = mail.GetSentMails(null, id);
                 msg = _msg.FirstOrDefault();
            }

            return View(msg);
        }

        public ActionResult NewMailCount()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SendMail(MessageViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            ProcessMail mail = new ProcessMail(_VendorId, "Vendor","Admin");
            bool sendmail = mail.SendMail(model);
            return Json(sendmail, JsonRequestBehavior.AllowGet);
        }
    }
}