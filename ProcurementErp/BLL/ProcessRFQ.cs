using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public static class ProcessRFQ
    {
        public static string ProcessRfqFormOne(RFQREQViewModel model)
        {
            SOURCING_RFQ_REQ _request = new SOURCING_RFQ_REQ();
            try
            {
                decimal ItemId = ProcessRequest.GetSequence();
                using (var context = new Entities())
                {
                    _request.MAPPING = model.MAPPING;
                    _request.TEMP_NO = ItemId;
                    _request.RFQ_START_DATE = model.RFQ_START_DATE;
                    _request.RFQ_CLOSE_DATE = model.RFQ_CLOSE_DATE;
                    _request.DELIVERY_ADDRESS = model.DELIVERY_ADDRESS;
                    _request.INITIATOR_ID = model.INITIATOR_ID;
                    _request.INITIATOR_NAME = model.INITIATOR_NAME;
                    _request.STATUS = "Awaiting Vendor Response";
                    context.SOURCING_RFQ_REQ.Add(_request);
                    context.SaveChanges();
                }
                Common.UpdateSourcingRequestTableWithMappingInfo((decimal)model.MAPPING, ItemId);
                return Convert.ToString(ItemId);
            }
            catch (Exception ex)
            {
                Logger.Log("Error insert into RFQ-REQ table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static string ProcessRfqFormTwo(RFQITEMViewModel[] model)
        {
            SOURCING_RFQ_ITEM _item = new SOURCING_RFQ_ITEM();
            try
            {
                var TempId = HttpContext.Current.Session["TempId"].ToString();
                decimal _tempId = Convert.ToDecimal(TempId);
                
                foreach (var item in model)
                {
                     using (var context = new Entities())
                    {
                        _item.TEMP_NO = _tempId;
                        _item.DESCRIPTION = item.DESCRIPTION;
                        _item.QUANTITY = item.QUANTITY;
                        _item.UNIT_OF_MEAS = item.UNIT_OF_MEAS;
                        context.SOURCING_RFQ_ITEM.Add(_item);
                        context.SaveChanges();
                    }
                }

                return TempId;
            }
            catch (Exception ex)
            {
                Logger.Log("Error insert into RFQ-Item table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static RFQSummaryViewModel GetSummaryInfo(string _TempId = null)
        {
            string TempId = "";
            
            RFQSummaryViewModel  Summary = new RFQSummaryViewModel();
            List<ContactedVendorViewModel> _Vendors = new List<ContactedVendorViewModel>();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            if (_TempId == null)
            {
                TempId = HttpContext.Current.Session["TempId"].ToString();
            }
            else
            {
                TempId = _TempId;
            }
            decimal _tempId = Convert.ToDecimal(TempId);

            try
            {
                using (var context = new Entities())
                {
                    var getVendors = context.SOURCING_CONTACTEDVENDOR.Where(vend => vend.TEMP_NO == _tempId).ToList();
                    if (getVendors != null)
                    {
                        foreach (var singleVendor in getVendors)
                        {
                            _Vendors.Add( new ContactedVendorViewModel {
                                VENDOR_NAME = singleVendor.VENDOR_NAME,
                                AUTO_EMAIL = singleVendor.AUTO_EMAIL,
                            });
                        }
                        Summary.Vendors = _Vendors;
                    }

                    //get items
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == _tempId).ToList();
                    if (getItems != null)
                    {
                        foreach (var singleItem in getItems)
                        {
                            _Item.Add(new RFQITEMViewModel
                            {
                                QUANTITY = singleItem.QUANTITY,
                                UNIT_OF_MEAS = singleItem.UNIT_OF_MEAS,
                                DESCRIPTION = singleItem.DESCRIPTION
                            });
                        }
                        Summary.Items = _Item;
                    }

                    var query = context.SOURCING_RFQ_REQ.Join(
                        context.SOURCING_USERS, rq => rq.INITIATOR_ID, user => user.USER_ID, (rq, user) => new { Req = rq, Users = user })
                        .Join(context.SOURCING_REF_TEMP_LINK, link => link.Req.TEMP_NO, temp => temp.TEMP_NO, (link, temp) => new { _link = link, _temp = temp })
                        .Where(reqID => reqID._link.Req.TEMP_NO == _tempId).FirstOrDefault();
                    if (query != null)
                    {
                        Summary.Initiator.NAME = query._link.Users.NAME;
                        Summary.Initiator.TELEPHONE = query._link.Users.TELEPHONE;
                        Summary.Initiator.EMAIL = query._link.Users.EMAIL;
                        Summary.Request.TEMP_NO = query._link.Req.TEMP_NO;
                        Summary.Request.DELIVERY_ADDRESS = query._link.Req.DELIVERY_ADDRESS;
                        Summary.Request.RFQ_START_DATE = query._link.Req.RFQ_START_DATE;
                        Summary.Request.RFQ_CLOSE_DATE = query._link.Req.RFQ_CLOSE_DATE;
                        Summary.RefNum.REF_NO = query._temp.REF_NO;

                    }
                    return Summary;
                } 
            }
            catch(Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static string ProcessRfiInfo(RFQITEMViewModel model)
        {
            SOURCING_RFQ_ITEM _item = new SOURCING_RFQ_ITEM();
            try
            {
                var TempId = HttpContext.Current.Session["TempId"].ToString();
                decimal _tempId = Convert.ToDecimal(TempId);

                
                    using (var context = new Entities())
                    {
                        _item.TEMP_NO = _tempId;
                        _item.DESCRIPTION = model.DESCRIPTION;
                        _item.QUANTITY = model.QUANTITY;
                        _item.UNIT_OF_MEAS = model.UNIT_OF_MEAS;
                        _item.RFI_INFO = model.RFI_INFO;
                        context.SOURCING_RFQ_ITEM.Add(_item);
                        context.SaveChanges();
                    }
                

                return TempId;
            }
            catch (Exception ex)
            {
                Logger.Log("Error insert into RFQ-Item table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }
        

        
        
    }
}