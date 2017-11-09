using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProcurementErp.BLL;

namespace ProcurementErp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            var _user = ProcessUser.GetAllUsers();
            return View(_user);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            UserViewModel item = new UserViewModel();
            item._Roles = ProcessUser.GetRoles();
            item._Category = ProcessUser.GetCategory();
            ViewBag.Users = Common.GetUsersInProcurement();
            return View(item);
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(UserViewModel collection)
        {
            UserViewModel item = new UserViewModel();
            item._Roles = ProcessUser.GetRoles();
            item._Category = ProcessUser.GetCategory();
            try
            {
                var status = ProcessUser.CreateUser(collection);
                if (status == "success")
                {
                    return RedirectToAction("Index");
                }
                if (status == "exist")
                {
                    ViewBag.ErrorMessage = "User Already Exist";
                    return View(item);
                }
                else if (status == "error")
                {
                    ViewBag.ErrorMessage = "An Error Occurred during Creation";
                    return View(item);
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid Username";
                    return View(item); 
                }

                
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            var _user = ProcessUser.GetAllUsers(id);
            _user._Category = ProcessUser.GetCategory();
            _user._Roles = ProcessUser.GetRoles();
            return View(_user);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, UserViewModel collection)
        {
            var _user = ProcessUser.GetAllUsers(id);
            _user._Category = ProcessUser.GetCategory();
            _user._Roles = ProcessUser.GetRoles();
            try
            {
                // TODO: Add update logic here
                var status = ProcessUser.EditUser(id, collection);
                if(status == true)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "An Error Occurred";
                    return View(_user);
                }
                   
            }
            catch
            {
                return View(_user);
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
