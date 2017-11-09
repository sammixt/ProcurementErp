using ProcurementErp.Services;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.BLL
{
    public class ProcessUser
    {
        public static List<SelectListItem> GetRoles()
        {
            var _roles = ProcessRoles.GetRoles();
            var listItems = new List<SelectListItem>();
            foreach(var item in _roles)
                {
                listItems.Add(new SelectListItem {
                    Text = item.ROLE_NAME,
                    Value = Convert.ToString(item.ROLE_ID)
                    });
                };
            return listItems;
        }

        public static List<SelectListItem> GetCategory()
        {
            var context = new Entities();
            var _category = context.SOURCING_CATEGORIES.Where(m => m.CATEGORY_PARENT == 0).OrderByDescending(m => m.CATEGORY_NUM).ToList();
            var listItems = new List<SelectListItem>();
            foreach (var item in _category)
            {
                listItems.Add(new SelectListItem
                {
                    Text = item.CATEGORY_NAME,
                    Value = Convert.ToString(item.CATEGORY_NUM)
                });
            };
            return listItems;
        }

        public static string CreateUser(UserViewModel model)
        {
            //check if user exist 
            //if no retrieve uses information 
            //add to database.
            SOURCING_USERS _Users = new SOURCING_USERS();
            var profile = AuthenticationService.GetUserProfile(model.USERNAME);
            if (profile.EmployeeNumber != null)
            {
                try
                {
                    using (var context = new Entities())
                    {
                        var check = context.SOURCING_USERS.Where(m => m.USERNAME == model.USERNAME).FirstOrDefault();
                        if (check != null)
                        {
                            return "exist";
                        }
                        else
                        {
                            decimal _RoleId = Convert.ToDecimal(model.ROLE);
                            decimal _CategoryId = Convert.ToDecimal(model.CATEGORY);
                            _Users.USERNAME = model.USERNAME;
                            _Users.ROLE_ID = _RoleId;
                            _Users.EMP_NUM = profile.EmployeeNumber;
                            _Users.EMAIL = profile.Email;
                            _Users.NAME = profile.FullName;
                            _Users.ROLE = context.SOURCING_ROLES.Where(m => m.ROLE_ID == _RoleId).Select(m => m.ROLE_NAME).FirstOrDefault().ToString();
                            _Users.CATEGORY_ID = _CategoryId;
                            _Users.CATEGORY = context.SOURCING_CATEGORIES.Where(m => m.CATEGORY_NUM == _CategoryId).Select(m => m.CATEGORY_NAME).FirstOrDefault().ToString();
                            _Users.DEPARTMENT = profile.Department;
                            _Users.DESIGNATION = profile.JobTitle;
                            _Users.TELEPHONE = profile.MobileNumber;
                            _Users.STATUS = model.STATUS;
                            context.SOURCING_USERS.Add(_Users);
                            context.SaveChanges();
                            return "success";
                        }

                    }
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Logger.Log("Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:", "error");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Logger.Log("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage + " ", "error");
                        }
                    }

                    // Logger.Log("An error occured upon inserting new request into database. Error: " + ex.StackTrace, "error");
                    return "error";
                }
                //catch (Exception ex)
                //{
                //    Logger.Log("An error occurred while creating user. Error: " + ex.Message, "error");
                //    return "error";
                //}
            }
            else
            {
                return "Invalid Username";
            }
            
        }

        public static List<UserViewModel> GetAllUsers()
        {
            List<UserViewModel> _users = new List<UserViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_USERS.ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _users.Add(new UserViewModel
                            {
                               CATEGORY = item.CATEGORY,
                               EMAIL = item.EMAIL,
                               EMP_NUM = item.EMP_NUM,
                               NAME = item.NAME,
                               USERNAME = item.USERNAME,
                               DESIGNATION = item.DESIGNATION,
                               DEPARTMENT = item.DEPARTMENT,
                               TELEPHONE = item.TELEPHONE,
                               ROLE = item.ROLE,
                               STATUS = item.STATUS,
                               USER_ID = item.USER_ID
                            });
                        }
                        return _users;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving users list. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static UserViewModel GetAllUsers(int id)
        {
            UserViewModel _user = new UserViewModel();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_USERS.Where(m => m.USER_ID == id).FirstOrDefault();
                    if (query != null)
                    {
                       
                                _user.CATEGORY = query.CATEGORY;
                                _user.CATEGORY_ID = query.CATEGORY_ID;
                                _user.ROLE_ID = query.ROLE_ID;
                                _user.EMAIL = query.EMAIL;
                                _user.EMP_NUM = query.EMP_NUM;
                                _user.NAME = query.NAME;
                                _user.USERNAME = query.USERNAME;
                                _user.DESIGNATION = query.DESIGNATION;
                                _user.DEPARTMENT = query.DEPARTMENT;
                                _user.TELEPHONE = query.TELEPHONE;
                                _user.ROLE = query.ROLE;
                                _user.STATUS = query.STATUS;
                                _user.USER_ID = query.USER_ID;
                        return _user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving user. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        internal static bool EditUser(int id, UserViewModel collection)
        {
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_USERS.Where(u => u.USER_ID == id).FirstOrDefault();
                    if (query != null)
                    {
                        query.CATEGORY = context.SOURCING_CATEGORIES.Where(c => c.CATEGORY_NUM == collection.CATEGORY_ID).Select(c => c.CATEGORY_NAME).FirstOrDefault().ToString();
                        query.CATEGORY_ID = collection.CATEGORY_ID;
                        query.ROLE = context.SOURCING_ROLES.Where(m => m.ROLE_ID == collection.ROLE_ID).Select(m => m.ROLE_NAME).FirstOrDefault().ToString();
                        query.ROLE_ID = collection.ROLE_ID;
                        query.STATUS = collection.STATUS;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving role. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return false;
            }
        }
    }
}