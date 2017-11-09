using Oracle.ManagedDataAccess.Client;
using ProcurementErp.BLL;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProcurementErp.Services
{
    public class GetInfoFromHRMS
    {
        private static string conn = ConfigurationManager.ConnectionStrings["COG_NOS"].ConnectionString;
        private static string conn_memo = ConfigurationManager.ConnectionStrings["FCUBSMEM"].ConnectionString;
        private static string GetUserCostCenterQuery = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetCostCenterForEachUser.txt";
        private static string GetSupervisorsIdQuery = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetSupervisorsId.txt";
        private static string GetSuperVisorDetaisQuery = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetSupervisorsDetails.txt";
        private static string GetDeptCodeHqQuery = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetDeptCodeHq.txt";
        private static string GetDeptCodeDescQuery = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetDeptCodeDesc.txt";
        private static string GetBranchPerCode = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetBranches.txt";
        private static string GetUserInProcurement = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetUserInProcurement.txt";


        public static UserProfileViewModel GetCostCenterWithEmpNumber(string EmpNumber)
        {
            UserProfileViewModel UserProfile = new UserProfileViewModel();
            string _GetUserCostCenterQuery = System.IO.File.ReadAllText(GetUserCostCenterQuery);
            _GetUserCostCenterQuery += AddQuoteToString(EmpNumber);

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetUserCostCenterQuery;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        UserProfile.CostCenterCode = (string)reader["segment3"];
                        UserProfile.BranchCode = (string)reader["segment2"];
                    }

                    return UserProfile;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve Cost Center For Emp Number " + EmpNumber + ". Error: " + d, "error");
                    return null;
                }
            }
        }

        //called upon submission of new movment request
        public static UserProfileViewModel GetSupervisorsDetails(string EmpNumber)
        {
            string _GetSuperVisorDetaisQuery = System.IO.File.ReadAllText(GetSuperVisorDetaisQuery);
            _GetSuperVisorDetaisQuery = _GetSuperVisorDetaisQuery.Replace("{EMP_NUM}", EmpNumber);
            UserProfileViewModel UserProfile = new UserProfileViewModel();

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetSuperVisorDetaisQuery;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        UserProfile.CostCenterCode = (string)reader["segment3"];
                        UserProfile.Email = (string)reader["EMAIL_ADDRESS"];
                        UserProfile.FullName = (string)reader["FULL_NAME"];
                        UserProfile.EmployeeNumber = (string)reader["EMPLOYEE_NUMBER"];
                    }

                    return UserProfile;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve Supervisors Details. Error: " + d, "error");
                    return null;
                }
            }
        }

        public static string GetSupervisorIdWithEmpNumber(string EmpNumber)
        {
            string SupervisorsId = "";
            string _GetSupervisorsIdQuery = System.IO.File.ReadAllText(GetUserCostCenterQuery);
            _GetSupervisorsIdQuery += EmpNumber;

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetSupervisorsIdQuery;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SupervisorsId = (string)reader["SUPERVISOR_ID"];
                    }

                    return SupervisorsId;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve Supervisors ID. Error: " + d, "error");
                    return null;
                }
            }
        }

        public static List<DeptCodeViewModel> GetDeptCodeHQ()
        {
            List<DeptCodeViewModel> DeptCode = new List<DeptCodeViewModel>();
            string _GetDeptCodeHqQuery = System.IO.File.ReadAllText(GetDeptCodeHqQuery);

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetDeptCodeHqQuery;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DeptCode.Add(new DeptCodeViewModel
                        {
                            DeptCostCode = (string)reader["segment3"]
                        });

                    }

                    return DeptCode;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve Dept Cost Codes. Error: " + d, "error");
                    return null;
                }
            }
        }

        public static DeptCodeViewModel GetDeptCodeDesc(string cost_code)
        {
            DeptCodeViewModel CostCodeDesc = new DeptCodeViewModel();
            string _GetDeptCodeDescQuery = System.IO.File.ReadAllText(GetDeptCodeDescQuery);
            _GetDeptCodeDescQuery += AddQuoteToString(cost_code);
            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn_memo;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetDeptCodeDescQuery;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        CostCodeDesc.DeptCostCode = (string)reader["MIS_CODE"];
                        CostCodeDesc.DeptName = (string)reader["CODE_DESC"];
                    }
                    return CostCodeDesc;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve Cost code desc from gltm_mis_CODE. Error: " + d, "error");
                    return null;
                }
            }
        }

        //get the branch name, input argument branch code
        public static UserProfileViewModel GetBranches(string branch_code)
        {
            UserProfileViewModel BranchDesc = new UserProfileViewModel();
            string _GetBranchPerCode = System.IO.File.ReadAllText(GetBranchPerCode);
            _GetBranchPerCode += AddQuoteToString(branch_code);
            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn_memo;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetBranchPerCode;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        BranchDesc.BranchCode = (string)reader["BRANCH_CODE"];
                        BranchDesc.Branch = ((string)reader["BRANCH_NAME"]) + ", " + ((string)reader["BRANCH_ADDR1"]);
                    }
                    return BranchDesc;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve branch details from sttm_branch. Error: " + d, "error");
                    return null;
                }
            }
        }

        public static string AddQuoteToString(string variable)
        {
            string output = "'" + variable + "'";
            return output;
        }

        public static List<UserProfileViewModel> GetUsersInProcurementDept()
        {
            List<UserProfileViewModel> UserProfile = new List<UserProfileViewModel>();
            string _GetUserInProcurement = System.IO.File.ReadAllText(GetUserInProcurement);
           

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetUserInProcurement;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        UserProfile.Add(new UserProfileViewModel
                        {
                            FullName = (string)reader["full_name"],
                            Email = (string)reader["email_address"],
                            EmployeeNumber = (string)reader["employee_number"]
                        });
                       
                    }
                    return UserProfile;
                }
                catch (Exception ex)
                {
                    var d = ex.StackTrace.ToString();
                    Logger.Log("Error: " + d, "error");
                    return null;
                }
            }
        }
    }
}