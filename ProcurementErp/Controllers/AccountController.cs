using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ProcurementErp.ViewModels;
using ProcurementErp.BLL;
using ProcurementErp.Services;

namespace ProcurementErp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        
        public ActionResult Login(LoginViewModel model, string urlReturn)
        {
            string _Role = "";
            //var pass = AuthenticationService.Validate(model.UserName, model.Password);
            var profile = AuthenticationService.GetUserProfile(model.UserName);
            if (profile.EmployeeNumber != null)
            {
                var claims = new List<Claim>();
                //var profile = AuthenticationService.GetUserProfile(model.UserName);
                var userDetailsFrmDb = UserDBValidation.CheckUserExist(model.UserName);// Check is user exist in the database
                if (userDetailsFrmDb != null)
                {
                    _Role = userDetailsFrmDb.ROLE;
                    claims.Add(new Claim(ClaimTypes.Role, _Role));
                    claims.Add(new Claim(ClaimTypes.Rsa, Convert.ToString(userDetailsFrmDb.ROLE_ID)));
                    claims.Add(new Claim(ClaimTypes.Dns, userDetailsFrmDb.CATEGORY));
                    claims.Add(new Claim(ClaimTypes.Dsa, Convert.ToString(userDetailsFrmDb.CATEGORY_ID)));
                    claims.Add(new Claim(ClaimTypes.Actor, Convert.ToString(userDetailsFrmDb.USER_ID)));
                }
                else
                {
                    _Role = "Public";
                    claims.Add(new Claim(ClaimTypes.Role, _Role));
                }
               
               var GetBranchCodeAndCostCenter = GetInfoFromHRMS.GetCostCenterWithEmpNumber(profile.EmployeeNumber);
               var GetDeptCodeDesc = GetInfoFromHRMS.GetDeptCodeDesc(GetBranchCodeAndCostCenter.CostCenterCode);
               var Branches = GetInfoFromHRMS.GetBranches(GetBranchCodeAndCostCenter.BranchCode);
               string MainBranch = Branches.Branch;
               var BranchCode = GetBranchCodeAndCostCenter.BranchCode ?? profile.BranchCode;
               
                //if (BranchCode == Category.HQCODE)
                //{
                //    claims.Add(new Claim(ClaimTypes.Actor, GetBranchCodeAndCostCenter.CostCenterCode));
                //}
                claims.Add(new Claim(ClaimTypes.Name, profile.UserName));
                claims.Add(new Claim(ClaimTypes.GivenName, profile.FullName));
                claims.Add(new Claim(ClaimTypes.SerialNumber, profile.EmployeeNumber));
                claims.Add(new Claim(ClaimTypes.UserData, GetDeptCodeDesc.DeptName));
                claims.Add(new Claim(ClaimTypes.Email, profile.Email));
                claims.Add(new Claim(ClaimTypes.StateOrProvince,MainBranch));
                claims.Add(new Claim(ClaimTypes.PostalCode, BranchCode));
                claims.Add(new Claim(ClaimTypes.Actor, GetBranchCodeAndCostCenter.CostCenterCode));
                claims.Add(new Claim(ClaimTypes.Locality, GetDeptCodeDesc.DeptName));


                SignInAsync(new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie));
                Logger.Log("Successful Login by " + profile.FullName + " Employee's Number " + profile.EmployeeNumber, "info");
                return RedirectToLocal(urlReturn,_Role);
            }
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View(model);
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

        private ActionResult RedirectToLocal(string returnUrl, string role = null)
        {
            var principal = (ClaimsIdentity)User.Identity;
          
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                if (role == "Public")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
                    

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }

}