using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class ProcessRFP
    {
        internal static string ProcessRFPIndex(ViewModels.RFIRFPComboViewModel model)
        {
            SOURCING_RFP_RFI_REQ _Request = new SOURCING_RFP_RFI_REQ();
            SOURCING_CONTACTPERSON _Commercial = new SOURCING_CONTACTPERSON();
            SOURCING_CONTACTPERSON _Technical = new SOURCING_CONTACTPERSON();
            decimal ItemId = ProcessRequest.GetSequence();
            try
            {
                using (var context = new Entities())
                {
                    _Request.TEMP_ID = ItemId;
                    _Request.PRJECT_TITLE = model.Request.PRJECT_TITLE;
                    _Request.INITIATOR_NAME = model.Request.INITIATOR_NAME;
                    _Request.INITIATOR = model.Request.INITIATOR;
                    _Request.INITIATION_DATE = model.Request.INITIATION_DATE;
                    _Request.PROJECT_OBJECTIVE = model.Request.PROJECT_OBJECTIVE;
                    _Request.WORK_SCOPE = model.Request.WORK_SCOPE;
                    _Request.UBN_OVERVIEW = model.Request.UBN_OVERVIEW;
                    _Request.TECHNICAL_PROPOSAL = model.Request.TECHNICAL_PROPOSAL;
                    _Request.ISSUE_DATE = model.Request.ISSUE_DATE;
                    _Request.DUE_DATE = model.Request.DUE_DATE;
                    _Request.DUE_TIME = model.Request.DUE_TIME;
                    _Request.LST_QRY_RECEIPT_DATE = model.Request.LST_QRY_RECEIPT_DATE;
                    _Request.MAPPING = model.Request.MAPPING;
                    _Request.BANK_QRY_RES_DATE = model.Request.BANK_QRY_RES_DATE;
                    _Request.LST_RPF_RECPT_DATE = model.Request.LST_RPF_RECPT_DATE;
                    _Request.STATUS = "Awaiting Vendor Response";

                    context.SOURCING_RFP_RFI_REQ.Add(_Request);
                    context.SaveChanges();
                    _Commercial.TEMP_NO = ItemId;
                    _Commercial.NAME = model.Commercial.NAME;
                    _Commercial.DEPARTMENT = model.Commercial.DEPARTMENT;
                    _Commercial.DESIGNATION = model.Commercial.DESIGNATION;
                    _Commercial.TELEPHONE = model.Commercial.TELEPHONE;
                    _Commercial.EMAIL = model.Commercial.EMAIL;
                    _Commercial.CONTACT_TYPE = "Commercial";

                    _Technical.TEMP_NO = ItemId;
                    _Technical.NAME = model.Technical.NAME;
                    _Technical.DEPARTMENT = model.Technical.DEPARTMENT;
                    _Technical.DESIGNATION = model.Technical.DESIGNATION;
                    _Technical.TELEPHONE = model.Technical.TELEPHONE;
                    _Technical.EMAIL = model.Technical.EMAIL;
                    _Technical.CONTACT_TYPE = "Technical";
                    Common.SaveChanges(_Commercial);
                    Common.SaveChanges(_Technical);
                    if (model.Request.MAPPING != null)
                    {
                        Common.UpdateSourcingRequestTableWithMappingInfo((decimal)model.Request.MAPPING, ItemId);
                    }
                    
                    string _tempId = Convert.ToString(ItemId);
                    return _tempId;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to Insert into SOURCING_RFP_RFI_REQ table. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static RFIRFPComboViewModel GetSummaryInfo(string _TempId = null)
        {
            string TempId = "";
            RFIRFPComboViewModel Summary = new RFIRFPComboViewModel();
            List<ContactedVendorViewModel> _Vendors = new List<ContactedVendorViewModel>();
            List<RFQITEMViewModel> _Item = new List<RFQITEMViewModel>();
            if (_TempId == null)
            {
                TempId = HttpContext.Current.Session["TempId"].ToString();
            }
            else
            {
                TempId = _TempId;
                HttpContext.Current.Session["TempId"] = _TempId;
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
                            _Vendors.Add(new ContactedVendorViewModel
                            {
                                VENDOR_NAME = singleVendor.VENDOR_NAME,
                                AUTO_EMAIL = singleVendor.AUTO_EMAIL,
                                VENDOR_ID = singleVendor.VENDOR_ID,
                                STATUS = singleVendor.STATUS??null
                            });
                        }
                        Summary.Vendors = _Vendors;
                    }

                    //get Reference Number

                    var getRefNum = context.SOURCING_REF_TEMP_LINK.Where(m => m.TEMP_NO == _tempId).FirstOrDefault();
                    if (getRefNum != null)
                    {
                        Summary.RefNum.REF_NO = getRefNum.REF_NO;
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
                                DESCRIPTION = singleItem.DESCRIPTION,
                                RFI_INFO = singleItem.RFI_INFO,
                                ITEM_NO = singleItem.ITEM_NO
                            });
                        }
                        Summary.Item = _Item;
                    }

                    var getContactPerson = context.SOURCING_CONTACTPERSON.Where(m => m.TEMP_NO == _tempId).ToList();
                    var getTechnicalPerson = getContactPerson.Where(m => m.CONTACT_TYPE == "Technical").FirstOrDefault();
                    if (getTechnicalPerson != null)
                    {
                        Summary.Technical.ID = getTechnicalPerson.ID;
                        Summary.Technical.TEMP_NO = getTechnicalPerson.TEMP_NO;
                        Summary.Technical.NAME = getTechnicalPerson.NAME;
                        Summary.Technical.DEPARTMENT = getTechnicalPerson.DEPARTMENT;
                        Summary.Technical.DESIGNATION = getTechnicalPerson.DESIGNATION;
                        Summary.Technical.TELEPHONE = getTechnicalPerson.TELEPHONE;
                        Summary.Technical.EMAIL = getTechnicalPerson.EMAIL;
                        Summary.Technical.CONTACT_TYPE = getTechnicalPerson.CONTACT_TYPE; 
                    }
                    var getCommercialPerson = getContactPerson.Where(m => m.CONTACT_TYPE == "Commercial").FirstOrDefault();
                    if (getCommercialPerson != null)
                    {
                        Summary.Commercial.ID = getCommercialPerson.ID;
                        Summary.Commercial.TEMP_NO = getCommercialPerson.TEMP_NO;
                        Summary.Commercial.NAME = getCommercialPerson.NAME;
                        Summary.Commercial.DEPARTMENT = getCommercialPerson.DEPARTMENT;
                        Summary.Commercial.DESIGNATION = getCommercialPerson.DESIGNATION;
                        Summary.Commercial.TELEPHONE = getCommercialPerson.TELEPHONE;
                        Summary.Commercial.EMAIL = getCommercialPerson.EMAIL;
                        Summary.Commercial.CONTACT_TYPE = getCommercialPerson.CONTACT_TYPE;
                    }

                    var query = context.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == _tempId).FirstOrDefault();
                    if (query != null)
                    {
                        Summary.Request.TEMP_ID = query.TEMP_ID;
                        Summary.Request.PRJECT_TITLE = query.PRJECT_TITLE;
                        Summary.Request.INITIATOR_NAME = query.INITIATOR_NAME;
                        Summary.Request.INITIATOR  = query.INITIATOR ;
                        Summary.Request.INITIATION_DATE = query.INITIATION_DATE;
                        Summary.Request.PROJECT_OBJECTIVE = query.PROJECT_OBJECTIVE ;
                        Summary.Request.WORK_SCOPE = query.WORK_SCOPE ;
                        Summary.Request.UBN_OVERVIEW = query.UBN_OVERVIEW ;
                        Summary.Request.TECHNICAL_PROPOSAL = query.TECHNICAL_PROPOSAL ;
                        Summary.Request.ISSUE_DATE = query.ISSUE_DATE;
                        Summary.Request.DUE_DATE = query.DUE_DATE;
                        Summary.Request.DUE_TIME = query.DUE_TIME ;
                        Summary.Request.LST_QRY_RECEIPT_DATE = query.LST_QRY_RECEIPT_DATE ;
                        Summary.Request.BANK_QRY_RES_DATE = query.BANK_QRY_RES_DATE ;
                        Summary.Request.LST_RPF_RECPT_DATE = query.LST_RPF_RECPT_DATE;
                        Summary.Request.STATUS = query.STATUS;

                    }
                    return Summary;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving data for summary display. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }

        public static SOURCING_REF_TEMP_LINK SearchRequest(string RefNum)
        {
            SOURCING_REF_TEMP_LINK links = new SOURCING_REF_TEMP_LINK();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_REF_TEMP_LINK.Where(m => m.REF_NO == RefNum).FirstOrDefault();
                    if(query != null)
                    {
                        links.REQ_TYPE = query.REQ_TYPE;
                        links.TEMP_NO = query.TEMP_NO;
                        return links;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error retrieving Reference Number. Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }
    }
}