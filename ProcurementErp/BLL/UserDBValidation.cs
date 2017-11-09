using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class UserDBValidation
    {
        public static UserViewModel CheckUserExist(string UserName)
        {
            UserViewModel _user = new UserViewModel();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_USERS.Where(m => m.USERNAME == UserName && m.STATUS == "Active").FirstOrDefault();
                    if (query != null)
                    {
                        _user.ROLE = query.ROLE;
                        _user.CATEGORY = query.CATEGORY;
                        _user.ROLE_ID = query.ROLE_ID;
                        _user.CATEGORY_ID = query.CATEGORY_ID;
                        _user.USER_ID = query.USER_ID;
                    }
                    else
                    {
                        _user = null;
                    }

                    return _user;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error validatiing user against DB. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }
    }
}