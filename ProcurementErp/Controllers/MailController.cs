using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail
        public ActionResult Compose()
        {
            return View();
        }

        public ActionResult Inbox()
        {
            return View();
        }

        public ActionResult Inbox(string id)
        {
            return View();
        }

        public ActionResult Sent()
        {
            return View();
        }

        public ActionResult NewMailCount()
        {
            return Json(true,JsonRequestBehavior.AllowGet);
        }
    }
}