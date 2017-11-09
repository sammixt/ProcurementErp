using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.BLL
{
    public class UpdateRequestBLL
    {
        public decimal TempId;
        public RFQREQViewModel model = new RFQREQViewModel();
        public List<RFQITEMViewModel> items = new List<RFQITEMViewModel>();
        public RFQITEMViewModel _item = new RFQITEMViewModel();
        public RFIRFPComboViewModel _model = new RFIRFPComboViewModel();
        // retrieve rfq request
        #region RFQ BLL
        public RFQREQViewModel GetRFQRequest()
        {
            try
            {
                using(var context = new Entities())
                {
                    var query = context.SOURCING_RFQ_REQ.Where(m => m.TEMP_NO == this.TempId).FirstOrDefault();
                    if(query != null)
                    {
                        this.model.TEMP_NO = query.TEMP_NO;
                        this.model.RFQ_CLOSE_DATE = query.RFQ_CLOSE_DATE;
                        this.model.RFQ_START_DATE = query.RFQ_START_DATE;
                        this.model.DELIVERY_ADDRESS = query.DELIVERY_ADDRESS;
                        return this.model;
                    }else{
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + "-" + ex.StackTrace, "error");
                return null;
            }
        }
        //update rfq request
        public decimal UpdateRFQRequest()
        {
            SOURCING_RFQ_REQ _request = new SOURCING_RFQ_REQ();
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_RFQ_REQ.Where(m => m.TEMP_NO == this.model.TEMP_NO).FirstOrDefault();
                    if (query != null)
                    {
                        query.DELIVERY_ADDRESS = this.model.DELIVERY_ADDRESS;
                        query.RFQ_START_DATE = this.model.RFQ_START_DATE;
                        query.RFQ_CLOSE_DATE = this.model.RFQ_CLOSE_DATE;
                        context.SaveChanges();
                        return this.model.TEMP_NO;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + "-" + ex.StackTrace, "error");
                return 0;
            }
        }

        //Get items 
        public List<RFQITEMViewModel> GetItems()
        {
            try{
             using (var context = new Entities())
                {
                    var query = context.SOURCING_RFQ_ITEM.Where(m => m.TEMP_NO == this.TempId).ToList();
                    if (query != null)
                    {
                       foreach(var item in query)
                       {
                          this.items.Add(new RFQITEMViewModel 
                          {
                              DESCRIPTION = item.DESCRIPTION,
                              UNIT_OF_MEAS = item.UNIT_OF_MEAS,
                              QUANTITY = item.QUANTITY,
                              ITEM_NO = item.ITEM_NO,
                              TEMP_NO = item.TEMP_NO
                          });
                       }
                       return this.items;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + "-" + ex.StackTrace, "error");
                return null;
            }
        }

        public decimal UpdateREQItems()
        {
            decimal ReturnVal;
            SOURCING_RFQ_ITEM _item = new SOURCING_RFQ_ITEM();
            var TempId = HttpContext.Current.Session["TempId"].ToString();
            decimal _tempId = Convert.ToDecimal(TempId);
            var context = new Entities();
            try
            {
                
                foreach (var item in this.items)
                {
                   
                    if (item.ITEM_NO == 0)
                    {
                        _item.TEMP_NO = _tempId;
                        _item.DESCRIPTION = item.DESCRIPTION;
                        _item.QUANTITY = item.QUANTITY;
                        _item.UNIT_OF_MEAS = item.UNIT_OF_MEAS;
                        context.SOURCING_RFQ_ITEM.Add(_item);
                        context.SaveChanges();
                    }
                    
                    var query = context.SOURCING_RFQ_ITEM.Where(m => m.ITEM_NO == item.ITEM_NO).FirstOrDefault();
                    if (query != null)
                    {
                        query.DESCRIPTION = item.DESCRIPTION;
                        query.QUANTITY = item.QUANTITY;
                        query.UNIT_OF_MEAS = item.UNIT_OF_MEAS;
                        context.SaveChanges();
                    }
                    
                }
                ReturnVal = this.TempId;
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message + "-" + ex.StackTrace, "error");
                ReturnVal = 0;
            }
            finally
            {
                context.Dispose();
            }
            return ReturnVal;
        }
        #endregion
        #region RFP
        
        public  string UpdateRFPIndex()
        {
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_RFP_RFI_REQ.Where(m => m.TEMP_ID == this._model.Request.TEMP_ID).FirstOrDefault();
                    if (query != null)
                    {
                        query.PRJECT_TITLE = this._model.Request.PRJECT_TITLE;
                        query.PROJECT_OBJECTIVE = this._model.Request.PROJECT_OBJECTIVE;
                        query.WORK_SCOPE = this._model.Request.WORK_SCOPE;
                        query.UBN_OVERVIEW = this._model.Request.UBN_OVERVIEW;
                        query.TECHNICAL_PROPOSAL = this._model.Request.TECHNICAL_PROPOSAL;
                        query.ISSUE_DATE = this._model.Request.ISSUE_DATE;
                        query.DUE_DATE = this._model.Request.DUE_DATE;
                        query.DUE_TIME = this._model.Request.DUE_TIME;
                        query.LST_QRY_RECEIPT_DATE = this._model.Request.LST_QRY_RECEIPT_DATE;
                        query.BANK_QRY_RES_DATE = this._model.Request.BANK_QRY_RES_DATE;
                        query.LST_RPF_RECPT_DATE = this._model.Request.LST_RPF_RECPT_DATE;
                        query.STATUS = "Awaiting Vendor Response";
                        context.SaveChanges();

                    }
                    var commercial = context.SOURCING_CONTACTPERSON.Where(m => m.ID == this._model.Commercial.ID && m.CONTACT_TYPE == "Commercial").FirstOrDefault();
                    if (commercial != null)
                    {
                        commercial.NAME = this._model.Commercial.NAME;
                        commercial.DEPARTMENT = this._model.Commercial.DEPARTMENT;
                        commercial.DESIGNATION = this._model.Commercial.DESIGNATION;
                        commercial.TELEPHONE = this._model.Commercial.TELEPHONE;
                        commercial.EMAIL = this._model.Commercial.EMAIL;
                        commercial.CONTACT_TYPE = "Commercial";
                        context.SaveChanges();
                    }

                    var _tehcnical = context.SOURCING_CONTACTPERSON.Where(m => m.ID == this._model.Technical.ID && m.CONTACT_TYPE == "Technical").FirstOrDefault();
                    if (_tehcnical != null)
                    {
                        _tehcnical.NAME = this._model.Technical.NAME;
                        _tehcnical.DEPARTMENT = this._model.Technical.DEPARTMENT;
                        _tehcnical.DESIGNATION = this._model.Technical.DESIGNATION;
                        _tehcnical.TELEPHONE = this._model.Technical.TELEPHONE;
                        _tehcnical.EMAIL = this._model.Technical.EMAIL;
                        _tehcnical.CONTACT_TYPE = "Technical";
                        context.SaveChanges();
                    }
                    string _tempId = Convert.ToString(this.TempId);
                    return _tempId;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }
        #endregion
        #region RFI

        public RFQITEMViewModel GetRFIInfo()
        {
            try
            {
                using (var context = new Entities())
                {
                    var getItems = context.SOURCING_RFQ_ITEM.Where(it => it.TEMP_NO == this.TempId).FirstOrDefault();
                    if (getItems != null)
                    {
                        _item.ITEM_NO = getItems.ITEM_NO;
                        _item.RFI_INFO = getItems.RFI_INFO;
                        _item.TEMP_NO = getItems.TEMP_NO;
                        return this._item;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message, "error");
                return null;
            }
            
        }

        public string UpdateRFIInfor()
        {
            try
            {
                using (var context = new Entities())
                {
                    var query = context.SOURCING_RFQ_ITEM.Where(m => m.ITEM_NO == _item.ITEM_NO).FirstOrDefault();
                    if (query != null)
                    {
                        query.RFI_INFO = _item.RFI_INFO;
                        context.SaveChanges();
                        return this.TempId.ToString();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message +
                    "------------------------------------------------" + ex.StackTrace,
                    "error");
                return null;
            }
        }


        #endregion
    }
}