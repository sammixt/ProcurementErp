using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace ProcurementErp.BLL
{
    public class AdminRequestProcesses
    {
        public static List<SelectListItem> GetRequestType()
        {
            var context = new Entities();
            var listItems = new List<SelectListItem>();
            try
            {
                var _category = context.SOURCING_REQUEST_TYPE.OrderByDescending(m => m.REQUEST_ID).ToList();
                var FilterCategory = _category.Where(m => m.REQUEST_ID <= 3).ToList();

                foreach (var item in FilterCategory)
                {
                    listItems.Add(new SelectListItem
                    {
                        Text = item.REQUEST_NAME + " - " + item.REQUEST_SHORTCODE,
                        Value = Convert.ToString(item.REQUEST_ID)
                    });
                };
            }
            catch(Exception ex)
            {
                Logger.Log("An Error Occurred Retrieving Request type. Error: " + ex.StackTrace, "error");
                listItems = null;
            }
            finally
            {
                context.Dispose();
            }
            return listItems;
        }
    }
}