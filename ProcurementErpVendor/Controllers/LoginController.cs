using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Owin.Security;
using ProcurementErpVendor.BLL;
using ProcurementErpVendor.Models;


namespace ProcurementErpVendor.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            var claims = new List<Claim>();
            string _VendorName = "";
            try
            {
                if (ModelState.IsValid)
                {
                    var user = Account.AuthenticateUser(model);
                    if (user == null)
                    {
                        ViewBag.ErrorMessage = "Invalid Username or Password Combination";
                        return View();
                    }
                    var vendor = Account.VendorDetails(user.VendorId);
                    if (vendor != null)
                    {
                        _VendorName = vendor.VENDOR_NAME != null ? vendor.VENDOR_NAME : "Vendor";
                    }
                    else 
                    {
                        _VendorName = "Vendor";
                    }
                    claims.Add(new Claim(ClaimTypes.GivenName, _VendorName));
                    claims.Add(new Claim(ClaimTypes.Email, user.UserName));
                    claims.Add(new Claim(ClaimTypes.UserData, Convert.ToString(user.VendorId)));
                    claims.Add(new Claim(ClaimTypes.Actor, Convert.ToString(user.ID)));
                    SignInAsync(new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie));
                    return RedirectToAction("Index", "Home");
                }
                return View(); ;
            }
            catch
            {
                return View(); ;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SignInAsync(ClaimsIdentity id)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}