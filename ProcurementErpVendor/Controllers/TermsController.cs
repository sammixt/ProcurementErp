using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.Controllers
{
    public class TermsController : Controller
    {
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