using ProcurementErp.ADService;
using ProcurementErp.BLL;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProcurementErp.Services
{
    public class AuthenticationService
    {
        //private static UBN_SMS_ServiceClient client = new UBN_SMS_ServiceClient();
        private static UBN_SMS_ServiceClient client = new UBN_SMS_ServiceClient();
        private static string moduleId = ConfigurationManager.AppSettings["ModuleId"];
        //System.Configuration.ConfigurationManager.AppSettings["ModuleId"].ToString()
        public static bool Validate(string username, string password)
        {

            bool value = client.AdAuthenticate(username.ToLower(), password, moduleId);
            if (value == true)
            {
                return value;
            }
            else
            {
                return false;
            }

        }

        public static IEnumerable<string> GetRole(string username)
        {

            var roles = client.GetRolesForUser(username, moduleId).ToList();
            return roles;
        }

        public static UserProfileViewModel GetUserProfile(string username)
        {
            try
            {
                UserProfile user = client.GetUserProfile(username.ToLower(), moduleId);

                return new UserProfileViewModel
                {
                    UserName = username.ToLower(),
                    FullName = user.DisplayName,
                    EmployeeNumber = user.EmployeeId,
                    Email = user.Email,
                    Title = user.JobTitle,
                    BranchCode = user.BranchCode,
                    Branch = user.BranchName,
                    Department = user.Department,
                    JobTitle = user.JobTitle,
                    MobileNumber = user.MobileNumber,
                    TelephoneNumber = user.TelephoneNumber
                };
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving userprofile from AD. ErrorInfo : " + ex.Message + "+ -------------------------------- + " + ex.StackTrace, "Error");
                return null;
            }
        }

        public static bool GetState(string username)
        {
            bool userStat = client.IsUserExist(username, moduleId);
            return userStat;
        }
    }
}