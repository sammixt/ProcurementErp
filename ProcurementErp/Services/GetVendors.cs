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
    public class GetVendors
    {
        private static string conn = ConfigurationManager.ConnectionStrings["HRERP"].ConnectionString;
        private static string GetVendorQuery = AppDomain.CurrentDomain.BaseDirectory + @"\Queries\GetVendors.txt";

        public static List<VendorViewModel> GetAllVendors()
        {
            List<VendorViewModel> _Vendors = new List<VendorViewModel>();
            string _GetVendorQuery = System.IO.File.ReadAllText(GetVendorQuery);
            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = conn;
                try
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    command.CommandText = _GetVendorQuery;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        _Vendors.Add(new VendorViewModel
                        {
                            VENDOR_NAME = reader["VENDOR_NAME"] != DBNull.Value?(string)reader["VENDOR_NAME"]:"",
                            VENDOR_ID = (decimal)reader["VENDOR_ID"],
                            EMAIL_ADDRESS = reader["email_address"] != DBNull.Value ? (string)reader["email_address"] : "",
                            PHONE_NUMBER = reader["PHONE_NUMBER"] != DBNull.Value ? (string)reader["PHONE_NUMBER"] : "",
                            VENDOR_NUMBER = reader["VENDOR_NUMBER"] != DBNull.Value ? (string)reader["VENDOR_NUMBER"] : ""
                        });
                    }

                    return _Vendors;
                }
                catch (Exception ex)
                {
                    var d = ex.Message.ToString();
                    Logger.Log("Unable to retrieve vendors from xxubn_suppliers_info_v. Error: " + d, "error");
                    return null;
                }
            }
        }
    }
}