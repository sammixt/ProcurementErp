using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class RetrieveRequest
    {
        public static List<SourceRequestViewModel> GetRequestHistory(string _EmpNum = null, string temp_id = null)
        {
            try
            {
                List<SourceRequestViewModel> query = new List<SourceRequestViewModel>();
                List<SOURCING_REQUEST> _query = new List<SOURCING_REQUEST>();
                using (Entities context = new Entities())
                {
                    if (_EmpNum != null)
                    {
                        _query = context.SOURCING_REQUEST.Where(m => m.INITIATOR_NUMBER == _EmpNum || (m.LINE_MANAGER_NUM == _EmpNum && (m.LINE_MANAGER_APPR == "APPROVED" || m.LINE_MANAGER_APPR == "DECLINED"))).OrderByDescending(n => n.TEMP_ID).ToList();
                    }
                    else if (temp_id != null)
                    {
                        decimal _temp_id = Convert.ToDecimal(temp_id);
                        _query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == _temp_id ).ToList();
                    }
                   
                   if (_query != null)

                   {

                       
                       foreach (var item in _query)
                       {

                           IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f => f.SOURCE_REQ_ID == item.TEMP_ID).ToList();
                           query.Add(new SourceRequestViewModel
                           {
                               BUYER_COMMENT = item.BUYER_COMMENT,
                               BUYER_STATUS = item.BUYER_STATUS,
                               EXPECTED_DELIVERY_DATE = item.EXPECTED_DELIVERY_DATE,
                               INITIATING_BRANCH = item.INITIATING_BRANCH,
                               INITIATING_DEPT = item.INITIATING_DEPT,
                               INITIATOR_NAME = item.INITIATOR_NAME,
                               ITEM_DESCRIPTION = item.ITEM_DESCRIPTION,
                               ITEM_CATEGORY = item.ITEM_CATEGORY,
                               ITEM_CATEGORY_ID = item.ITEM_CATEGORY_ID,
                               INITIATION_DATE = item.INITIATION_DATE,
                               LINE_MANAGER_APPR = item.LINE_MANAGER_APPR,
                               LINE_MANAGER_APPR_DATE = item.LINE_MANAGER_APPR_DATE,
                               LINE_MANAGER_NAME = item.LINE_MANAGER_NAME,
                               LINE_MANAGER_COMMENT = item.LINE_MANAGER_COMMENT,
                               PREFERED_VENDOR = item.PREFERED_VENDOR,
                               PREFERED_VENDOR_ID = item.PREFERED_VENDOR_ID,
                               PROCUREMENT_BUYER = item.PROCUREMENT_BUYER,
                               MAPTOREQUEST = item.MAPTOREQUEST,
                               TEMP_ID = item.TEMP_ID,
                               _Files = _queryFile
                           });
                       }
                       return query;
                   }
                   else
                   {
                       return null;
                   }
                   
                }

                
            }
            catch (Exception ex)
            {
                Logger.Log("Request cannot be retrieved from database, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error"); ;
                return null;
            }
        }

        /* retrieve count of total request inititiated or approved for display in request history label */

        public static string GetRequestHistoryCount(string EmpNum)
        {
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_REQUEST.Where(x => x.INITIATOR_NUMBER == EmpNum || (x.LINE_MANAGER_NUM == EmpNum && (x.LINE_MANAGER_APPR == "APPROVED" || x.LINE_MANAGER_APPR == "DECLINED"))).Count();
                    return Convert.ToString(query);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving count from DB. Error: " + ex.Message, "error");
                return null;
            }
        }

        public static string GetPendingRequestCount(string EmpNum)
        {
            try
            {
                 using (var context = new Entities())
                {
                var query = context.SOURCING_REQUEST.Where(x => x.LINE_MANAGER_NUM == EmpNum && x.LINE_MANAGER_APPR == "Pending" ).Count();
                    return Convert.ToString(query);
                 }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving count. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        internal static object GetRequestPendingApproval(string _EmpNum = null, string temp_id = null)
        {
            try
            {
                List<SourceRequestViewModel> query = new List<SourceRequestViewModel>();
                List<SOURCING_REQUEST> _query = new List<SOURCING_REQUEST>();
                using (Entities context = new Entities())
                {
                    if (_EmpNum != null)
                    {
                        _query = context.SOURCING_REQUEST.Where(m => m.LINE_MANAGER_NUM == _EmpNum && m.LINE_MANAGER_APPR == "Pending" ).OrderByDescending(n => n.TEMP_ID).ToList();
                    }
                    else if (temp_id != null)
                    {
                        decimal _temp_id = Convert.ToDecimal(temp_id);
                        _query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == _temp_id).ToList();
                    }

                    if (_query != null)
                    {


                        foreach (var item in _query)
                        {

                            IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f => f.SOURCE_REQ_ID == item.TEMP_ID).ToList();
                            query.Add(new SourceRequestViewModel
                            {
                                BUYER_COMMENT = item.BUYER_COMMENT,
                                BUYER_STATUS = item.BUYER_STATUS,
                                EXPECTED_DELIVERY_DATE = item.EXPECTED_DELIVERY_DATE,
                                INITIATING_BRANCH = item.INITIATING_BRANCH,
                                INITIATING_DEPT = item.INITIATING_DEPT,
                                INITIATOR_NAME = item.INITIATOR_NAME,
                                ITEM_DESCRIPTION = item.ITEM_DESCRIPTION,
                                ITEM_CATEGORY = item.ITEM_CATEGORY,
                                ITEM_CATEGORY_ID = item.ITEM_CATEGORY_ID,
                                INITIATION_DATE = item.INITIATION_DATE,
                                LINE_MANAGER_APPR = item.LINE_MANAGER_APPR,
                                LINE_MANAGER_APPR_DATE = item.LINE_MANAGER_APPR_DATE,
                                LINE_MANAGER_NAME = item.LINE_MANAGER_NAME,
                                PREFERED_VENDOR = item.PREFERED_VENDOR,
                                TEMP_ID = item.TEMP_ID,
                                _Files = _queryFile
                            });
                        }
                        return query;
                    }
                    else
                    {
                        return null;
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.Log("Request cannot be retrieved from database, Error: " + ex.Message, "error");
                return null;
            }
        }

        //retrieves all request approved by hod for display on the Procurement buyers page
        public static List<SourceRequestViewModel> GetNewRequestFromHODToProcurementBuyer(string temp_id = null)
        {
            try
            {
                List<SourceRequestViewModel> query = new List<SourceRequestViewModel>();
                List<SOURCING_REQUEST> _query = new List<SOURCING_REQUEST>();
                using (Entities context = new Entities())
                {
                    if (temp_id == null)
                    {
                        _query = context.SOURCING_REQUEST.Where(m => m.LINE_MANAGER_APPR == "APPROVED" && (m.BUYER_STATUS != "RECEIVED" && m.BUYER_STATUS != "CLOSED")).OrderByDescending(n => n.TEMP_ID).ToList();
                    }   
                    else if (temp_id != null)
                    {
                        decimal _temp_id = Convert.ToDecimal(temp_id);
                        _query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == _temp_id).ToList();
                    }
                    if (_query != null)
                    {
                        foreach (var item in _query)
                        {

                            IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f => f.SOURCE_REQ_ID == item.TEMP_ID).ToList();
                            query.Add(new SourceRequestViewModel
                            {
                                BUYER_COMMENT = item.BUYER_COMMENT,
                                BUYER_STATUS = item.BUYER_STATUS,
                                EXPECTED_DELIVERY_DATE = item.EXPECTED_DELIVERY_DATE,
                                INITIATING_BRANCH = item.INITIATING_BRANCH,
                                INITIATING_DEPT = item.INITIATING_DEPT,
                                INITIATOR_NAME = item.INITIATOR_NAME,
                                ITEM_DESCRIPTION = item.ITEM_DESCRIPTION,
                                ITEM_CATEGORY = item.ITEM_CATEGORY,
                                ITEM_CATEGORY_ID = item.ITEM_CATEGORY_ID,
                                INITIATION_DATE = item.INITIATION_DATE,
                                LINE_MANAGER_APPR = item.LINE_MANAGER_APPR,
                                LINE_MANAGER_APPR_DATE = item.LINE_MANAGER_APPR_DATE,
                                LINE_MANAGER_NAME = item.LINE_MANAGER_NAME,
                                LINE_MANAGER_COMMENT = item.LINE_MANAGER_COMMENT,
                                PREFERED_VENDOR = item.PREFERED_VENDOR,
                                PROCUREMENT_BUYER = item.PROCUREMENT_BUYER,
                                TEMP_ID = item.TEMP_ID,
                                _Files = _queryFile
                            });
                        }
                        return query;
                    }
                    else
                    {
                        return null;
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.Log("Request cannot be retrieved from database, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        //get new request with the users role as filter
        public static List<SourceRequestViewModel> _GetNewRequestFromHODToProcurementBuyer(string temp_id = null, string UserId = null)
        {
            decimal _UserId;
            if (Decimal.TryParse(UserId, out _UserId))
            {

                try
                {
                    List<SourceRequestViewModel> query = new List<SourceRequestViewModel>();
                    List<SOURCING_REQUEST> _query = new List<SOURCING_REQUEST>();
                    using (Entities context = new Entities())
                    {
                        if (temp_id == null)
                        {
                            _query = context.SOURCING_REQUEST.Where(m => m.LINE_MANAGER_APPR == "APPROVED" && (m.BUYER_STATUS != "RECEIVED" && m.BUYER_STATUS != "CLOSED") && m.PROCUREMENT_BUYER_ID == _UserId).OrderByDescending(n => n.TEMP_ID).ToList();
                        }
                        else if (temp_id != null)
                        {
                            decimal _temp_id = Convert.ToDecimal(temp_id);
                            _query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == _temp_id).ToList();
                        }
                        if (_query != null)
                        {
                            foreach (var item in _query)
                            {

                                IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f => f.SOURCE_REQ_ID == item.TEMP_ID).ToList();
                                query.Add(new SourceRequestViewModel
                                {
                                    BUYER_COMMENT = item.BUYER_COMMENT,
                                    BUYER_STATUS = item.BUYER_STATUS,
                                    EXPECTED_DELIVERY_DATE = item.EXPECTED_DELIVERY_DATE,
                                    INITIATING_BRANCH = item.INITIATING_BRANCH,
                                    INITIATING_DEPT = item.INITIATING_DEPT,
                                    INITIATOR_NAME = item.INITIATOR_NAME,
                                    ITEM_DESCRIPTION = item.ITEM_DESCRIPTION,
                                    ITEM_CATEGORY = item.ITEM_CATEGORY,
                                    ITEM_CATEGORY_ID = item.ITEM_CATEGORY_ID,
                                    INITIATION_DATE = item.INITIATION_DATE,
                                    LINE_MANAGER_APPR = item.LINE_MANAGER_APPR,
                                    LINE_MANAGER_APPR_DATE = item.LINE_MANAGER_APPR_DATE,
                                    LINE_MANAGER_NAME = item.LINE_MANAGER_NAME,
                                    PREFERED_VENDOR = item.PREFERED_VENDOR,
                                    PROCUREMENT_BUYER = item.PROCUREMENT_BUYER,
                                    TEMP_ID = item.TEMP_ID,
                                    _Files = _queryFile
                                });
                            }
                            return query;
                        }
                        else
                        {
                            return null;
                        }

                    }


                }
                catch (Exception ex)
                {
                    Logger.Log("Request cannot be retrieved from database, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                    return null;
                }
            }
            else { return null; }
        }

        public static List<SourceRequestViewModel> GetSourceRequestHistoryAdmin(string procurementBuyerId = null, string temp_id = null,string MappingPopUp = null)
        {
           
            try
            {
                List<SourceRequestViewModel> query = new List<SourceRequestViewModel>();
                List<SOURCING_REQUEST> _query = new List<SOURCING_REQUEST>();
                using (Entities context = new Entities())
                {
                    if (procurementBuyerId == null && temp_id == null)
                    {
                        _query = context.SOURCING_REQUEST.Where(m => m.BUYER_STATUS == "RECEIVED" || m.BUYER_STATUS == "CLOSED" || m.BUYER_STATUS == "TREATED").OrderByDescending(n => n.TEMP_ID).ToList();
                    }
                    if (procurementBuyerId != null && MappingPopUp == null)
                    {
                        decimal _procurementBuyerId = Convert.ToDecimal(procurementBuyerId);
                        _query = context.SOURCING_REQUEST.Where(m => m.BUYER_STATUS == "RECEIVED" || m.BUYER_STATUS == "CLOSED" || m.BUYER_STATUS == "TREATED")
                           .Where(r => r.PROCUREMENT_BUYER_ID == _procurementBuyerId).OrderByDescending(n => n.TEMP_ID).ToList();
                    }
                    else if (temp_id != null)
                    {
                        decimal _temp_id = Convert.ToDecimal(temp_id);
                        _query = context.SOURCING_REQUEST.Where(m => m.TEMP_ID == _temp_id).ToList();
                    }
                    else if (MappingPopUp != null && procurementBuyerId == null)
                    {
                        _query = context.SOURCING_REQUEST.Where(m => m.BUYER_STATUS == "RECEIVED" && m.MAPTOREQUEST == null).OrderByDescending(n => n.TEMP_ID).ToList();
                    }
                    else if (MappingPopUp != null && procurementBuyerId != null)
                    {
                        decimal _procurementBuyerId = Convert.ToDecimal(procurementBuyerId);
                        _query = context.SOURCING_REQUEST
                            .Where(m => m.BUYER_STATUS == "RECEIVED")
                            .Where(r => r.PROCUREMENT_BUYER_ID == _procurementBuyerId)
                            .Where(m => m.MAPTOREQUEST == null).OrderByDescending(n => n.TEMP_ID).ToList();
                    }
                    

                    if (_query != null)
                    {


                        foreach (var item in _query)
                        {

                            IEnumerable<SOURCING_REQUEST_FILES> _queryFile = context.SOURCING_REQUEST_FILES.Where(f => f.SOURCE_REQ_ID == item.TEMP_ID).ToList();
                            query.Add(new SourceRequestViewModel
                            {
                                BUYER_COMMENT = item.BUYER_COMMENT,
                                BUYER_STATUS = item.BUYER_STATUS,
                                EXPECTED_DELIVERY_DATE = item.EXPECTED_DELIVERY_DATE,
                                INITIATING_BRANCH = item.INITIATING_BRANCH,
                                INITIATING_DEPT = item.INITIATING_DEPT,
                                INITIATOR_NAME = item.INITIATOR_NAME,
                                ITEM_DESCRIPTION = item.ITEM_DESCRIPTION,
                                ITEM_CATEGORY = item.ITEM_CATEGORY,
                                INITIATION_DATE = item.INITIATION_DATE,
                                LINE_MANAGER_APPR = item.LINE_MANAGER_APPR,
                                LINE_MANAGER_APPR_DATE = item.LINE_MANAGER_APPR_DATE,
                                LINE_MANAGER_NAME = item.LINE_MANAGER_NAME,
                                LINE_MANAGER_COMMENT = item.LINE_MANAGER_COMMENT,
                                PROCUREMENT_BUYER = item.PROCUREMENT_BUYER,
                                PREFERED_VENDOR = item.PREFERED_VENDOR,
                                TEMP_ID = item.TEMP_ID,
                                _Files = _queryFile
                            });
                        }
                        return query;
                    }
                    else
                    {
                        return null;
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.Log("Request cannot be retrieved from database, Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }


    }
}