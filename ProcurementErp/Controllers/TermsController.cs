using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    [Authorize]
    public class TermsController : Controller
    {
        // GET: Conditions
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EligibleBidders()
        {
            return PartialView();
        }

        public ActionResult RFIConfidentiality()
        {
            return PartialView();
        }
    }
}