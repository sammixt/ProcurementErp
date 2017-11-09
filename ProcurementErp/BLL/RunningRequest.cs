using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class RunningRequest
    {
        private decimal InitiatorId;
        private RunningRequestViewModel model;
        private decimal RequestType;

        public RunningRequest(decimal _InitiatorId, RunningRequestViewModel _model, decimal _requestType)
        {
            this.InitiatorId = _InitiatorId;
            this.model = _model;
            this.RequestType = _requestType;
        }

        //get running request for both RFP and RFI 
        public  List<RunningRequestViewModel> GetRFIRequset()
        {
            List<RunningRequestViewModel> _request = new List<RunningRequestViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getRunningReq = context.SOURCING_RFP_RFI_REQ.Join(context.SOURCING_REF_TEMP_LINK, _rfirfp => _rfirfp.TEMP_ID, _tempLink => _tempLink.TEMP_NO, (_rfirfp, _tempLink) => new { rfirfi = _rfirfp, tempLink = _tempLink })
                        .Where(req => req.rfirfi.INITIATOR == this.InitiatorId && req.rfirfi.STATUS != "AWARDED" && req.tempLink.REQ_TYPE == this.RequestType).ToList();
                    if (getRunningReq != null)
                    {
                        foreach (var item in getRunningReq)
                        {
                            _request.Add(new RunningRequestViewModel
                            {
                                ReferenceNumber = item.tempLink.REF_NO,
                                Status = item.rfirfi.STATUS,
                                StartDate = item.rfirfi.INITIATION_DATE,
                                DueDate = item.rfirfi.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.tempLink.REQ_TYPE) ?? null,
                                TempNo = item.rfirfi.TEMP_ID,
                                _RequestTypeNum = (decimal)item.tempLink.REQ_TYPE,
                                VendorResponseCount = GetVendorResponseCount(item.rfirfi.TEMP_ID)
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        //get running request for both RFP and RFI (Admin View)
        public List<RunningRequestViewModel> GetRFIRequsetAdmin()
        {
            List<RunningRequestViewModel> _request = new List<RunningRequestViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getRunningReq = context.SOURCING_RFP_RFI_REQ.Join(context.SOURCING_REF_TEMP_LINK, _rfirfp => _rfirfp.TEMP_ID, _tempLink => _tempLink.TEMP_NO, (_rfirfp, _tempLink) => new { rfirfi = _rfirfp, tempLink = _tempLink })
                        .Where(req => req.rfirfi.STATUS != "AWARDED" && req.tempLink.REQ_TYPE == this.RequestType).ToList();
                    if (getRunningReq != null)
                    {
                        foreach (var item in getRunningReq)
                        {
                            _request.Add(new RunningRequestViewModel
                            {
                                ReferenceNumber = item.tempLink.REF_NO,
                                Status = item.rfirfi.STATUS,
                                StartDate = item.rfirfi.INITIATION_DATE,
                                DueDate = item.rfirfi.DUE_DATE,
                                RequestType = GetRequestType((decimal)item.tempLink.REQ_TYPE) ?? null,
                                TempNo = item.rfirfi.TEMP_ID,
                                _RequestTypeNum = (decimal)item.tempLink.REQ_TYPE,
                                VendorResponseCount = GetVendorResponseCount(item.rfirfi.TEMP_ID)
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        //get running request for RFQ
        public List<RunningRequestViewModel> GetRFQRequset()
        {
            List<RunningRequestViewModel> _request = new List<RunningRequestViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getRunningReq = context.SOURCING_RFQ_REQ.Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq.TEMP_NO, _tempLink => _tempLink.TEMP_NO, (_rfq, _tempLink) => new { rfq = _rfq, tempLink = _tempLink })
                        .Where(req => req.rfq.INITIATOR_ID == this.InitiatorId && req.rfq.STATUS != "AWARDED" && req.tempLink.REQ_TYPE == this.RequestType).ToList();
                    if (getRunningReq != null)
                    {
                        foreach (var item in getRunningReq)
                        {
                            _request.Add(new RunningRequestViewModel
                            {
                                ReferenceNumber = item.tempLink.REF_NO,
                                Status = item.rfq.STATUS,
                                StartDate = item.rfq.RFQ_START_DATE,
                                DueDate = item.rfq.RFQ_CLOSE_DATE,
                                RequestType = GetRequestType((decimal)item.tempLink.REQ_TYPE) ?? null,
                                TempNo = item.rfq.TEMP_NO,
                                _RequestTypeNum = (decimal)item.tempLink.REQ_TYPE,
                                VendorResponseCount = GetVendorResponseCount(item.rfq.TEMP_NO)
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        //get running request for RFQ (Admin View)
        public List<RunningRequestViewModel> GetRFQRequsetAdmin()
        {
            List<RunningRequestViewModel> _request = new List<RunningRequestViewModel>();

            try
            {
                using (var context = new Entities())
                {
                    var getRunningReq = context.SOURCING_RFQ_REQ.Join(context.SOURCING_REF_TEMP_LINK, _rfq => _rfq.TEMP_NO, _tempLink => _tempLink.TEMP_NO, (_rfq, _tempLink) => new { rfq = _rfq, tempLink = _tempLink })
                        .Where(req => req.rfq.STATUS != "AWARDED" && req.tempLink.REQ_TYPE == this.RequestType).ToList();
                    if (getRunningReq != null)
                    {
                        foreach (var item in getRunningReq)
                        {
                            _request.Add(new RunningRequestViewModel
                            {
                                ReferenceNumber = item.tempLink.REF_NO,
                                Status = item.rfq.STATUS,
                                StartDate = item.rfq.RFQ_START_DATE,
                                DueDate = item.rfq.RFQ_CLOSE_DATE,
                                RequestType = GetRequestType((decimal)item.tempLink.REQ_TYPE) ?? null,
                                TempNo = item.rfq.TEMP_NO,
                                _RequestTypeNum = (decimal)item.tempLink.REQ_TYPE,
                                VendorResponseCount = GetVendorResponseCount(item.rfq.TEMP_NO)
                            });
                        }
                        return _request;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
            }
            return _request;
        }

        public static decimal GetVendorResponseCount(decimal TempId)
        {
            try
            {
                using (var context = new Entities())
                {
                    decimal query = context.SOURCING_CONTACTEDVENDOR.Where(m => m.TEMP_NO == TempId && (m.STATUS != "NEW REQUEST" && m.STATUS != null)).Count();
                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
                return 0;
            }
        }

        public static string GetRequestType(decimal ReqID)
        {
            try
            {
                using (var context = new Entities())
                {
                    string query = context.SOURCING_REQUEST_TYPE.Where(m => m.REQUEST_ID == ReqID).Select(m => m.REQUEST_NAME).FirstOrDefault().ToString();
                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: ", ex.StackTrace);
                return null;
            }
        }
    }
}