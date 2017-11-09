using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class ProcessRoles
    {
        public static bool CreateRole(RolesViewModel model)
        {
            SOURCING_ROLES _roles = new SOURCING_ROLES();
           
            try
            {
                using (var context = new Entities())
                {
                    _roles.ROLE_NAME = model.ROLE_NAME;
                    _roles.STATUS = model.STATUS;
                    context.SOURCING_ROLES.Add(_roles);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error creating role. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return false;
            }
        }

        public static List<RolesViewModel> GetRoles()
        {
            List<RolesViewModel> _roles = new List<RolesViewModel>();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_ROLES.ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _roles.Add(new RolesViewModel
                            {
                                ROLE_ID = item.ROLE_ID,
                                ROLE_NAME = item.ROLE_NAME,
                                STATUS = item.STATUS
                            });
                        }
                        return _roles;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving role. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static RolesViewModel GetRoles(int id)
        {
            RolesViewModel _roles = new RolesViewModel();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_ROLES.Where(r => r.ROLE_ID == id).FirstOrDefault();
                    if (query != null)
                    {
                        _roles.ROLE_ID = query.ROLE_ID;
                        _roles.ROLE_NAME = query.ROLE_NAME;
                        _roles.STATUS = query.STATUS;
                        return _roles;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving role. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        internal static bool EditRoles(RolesViewModel model)
        {
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_ROLES.Where(r => r.ROLE_ID == model.ROLE_ID).FirstOrDefault();
                    if (query != null)
                    {
                        query.ROLE_NAME = model.ROLE_NAME;
                        query.STATUS = model.STATUS;
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