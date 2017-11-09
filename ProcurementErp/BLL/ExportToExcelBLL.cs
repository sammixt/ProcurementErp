using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web;

namespace ProcurementErp.BLL
{
    public class ExportToExcelBLL
    {
        public static string ExcelContentType
        {
            get
            { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        public static byte[] ExportExcel(string TempNo,string Source = null)
        {
            byte[] result = null;
            using (ExcelPackage _excelReader = new ExcelPackage())
            {
                AnalysisBLL _Analysis = new AnalysisBLL();
                _Analysis.TempId = TempNo;

                var ReferenceNum = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RefNumber").FirstOrDefault().Value;
                var ReqType = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RequestTypeName").FirstOrDefault().Value;
                var ReqShortCode = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RequestTypeShortCode").FirstOrDefault().Value;
                var ReqID = _Analysis.GetRefNumberAndRequestType().Where(m => m.Key == "RequestTypeID").FirstOrDefault().Value;

                var _Analyse = _Analysis.GetVendorResponseAnalysis(Source);
                
                var worksheet = _excelReader.Workbook.Worksheets.Add("Report for " + ReferenceNum);
                //Head section
                worksheet.Cells[1, 1, 1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2) / 2)].Value = ReqType;
                worksheet.Cells[1, 1, 1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2) / 2)].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2) / 2)].Merge = true;
                worksheet.Cells[1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2) / 2)+1, 1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2))].Value = ReferenceNum;
                worksheet.Cells[1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2) / 2) +1, 1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2))].Style.Font.Bold = true;
                worksheet.Cells[1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2) / 2) + 1, 1, ((_Analyse.FirstOrDefault()._VendorsQuote.Count() * 2 + 2))].Merge = true;
                worksheet.Cells[2, 1, 2, 2].Merge = true;
                worksheet.Cells[2, 1, 2, 2].Value = "";
                int indexColumn = 3;
                int startRow = 2;
                int increment = 2;
                foreach (var vendor in _Analyse.FirstOrDefault()._VendorsQuote)
                {
                    worksheet.Cells[startRow, indexColumn, startRow, indexColumn + 1].Merge = true;
                    worksheet.Cells[startRow, indexColumn, startRow, indexColumn + 1].Value = (vendor._VendorDetails.STATUS == "PROCESSING")?"NEGOTIATED COST":vendor._VendorDetails.VENDOR_NAME;
                    worksheet.Cells[startRow, indexColumn, startRow, indexColumn + 1].Style.Font.Bold = true;
                    indexColumn = indexColumn + increment;
                }
                //end of head section
                //sub head section
                worksheet.Cells[3, 1].Value = "Item"; worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells[3, 2].Value = "Quantity"; worksheet.Cells[3, 2].Style.Font.Bold = true;
                int _startColUp = 3;
                int _startColTp = 4;
                
                foreach (var vendor in _Analyse.FirstOrDefault()._VendorsQuote)
                {
                    worksheet.Cells[3, _startColUp].Value = "Unit Price";
                    worksheet.Cells[3, _startColUp].Style.Font.Bold = true;
                    worksheet.Cells[3, _startColTp].Value = "Total Price";
                    worksheet.Cells[3, _startColTp].Style.Font.Bold = true;
                    _startColUp = _startColUp + increment;
                    _startColTp = _startColTp + increment;
                }
                //end of subhead section
                //items section
                int itemStartRow = 4;
                int itemUpcolIndex = 3;
                int itemTpcolIndex = 4;
                foreach(var item in _Analyse)
                {
                    worksheet.Cells[itemStartRow, 1].Value = item.DESCRIPTION;
                    worksheet.Cells[itemStartRow, 2].Value = item.QUANTITY;

                    foreach(var vendor in item._VendorsQuote)
                    {
                        worksheet.Cells[itemStartRow, itemUpcolIndex].Value = vendor.UNIT_PRICE;
                        worksheet.Cells[itemStartRow, itemTpcolIndex].Value = vendor.TOTAL_PRICE;
                        itemUpcolIndex = itemUpcolIndex + 2;
                        itemTpcolIndex = itemTpcolIndex + 2;
                    }
                    itemUpcolIndex = 3;
                    itemTpcolIndex = 4;


                    itemStartRow++;
                }
                var count = worksheet.Dimension.End.Row;
                worksheet.Cells[count+1, 1, count + 1, 2].Merge = true;
                worksheet.Cells[count + 1, 1, count + 1, 2].Value = "";
                worksheet.Cells[count + 2, 1, count + 2, 2].Merge = true;
                worksheet.Cells[count + 2, 1, count + 2, 2].Value = "";
                worksheet.Cells[count + 3, 1, count + 3, 2].Merge = true;
                worksheet.Cells[count + 3, 1, count + 3, 2].Value = "";

                int __startColTagName = 3;
                int __startColAmtName = 4;
                foreach (var vendor in _Analyse.FirstOrDefault()._VendorsQuote)
                {
                    worksheet.Cells[count + 1, __startColTagName].Value = "Total";
                    worksheet.Cells[count + 1, __startColTagName].Style.Font.Bold = true;
                    worksheet.Cells[count + 1, __startColAmtName].Value = vendor._VendorDetails.TOTALBFTAX;
                    worksheet.Cells[count + 2, __startColTagName].Value = "VAT";
                    worksheet.Cells[count + 2, __startColTagName].Style.Font.Bold = true;
                    worksheet.Cells[count + 2, __startColAmtName].Value = vendor._VendorDetails.VAT;
                    worksheet.Cells[count + 3, __startColTagName].Value = "Grand Total";
                    worksheet.Cells[count + 3, __startColTagName].Style.Font.Bold = true;
                    worksheet.Cells[count + 3, __startColAmtName].Value = vendor._VendorDetails.GRANDTOTAL;
                    __startColTagName = __startColTagName + 2;
                    __startColAmtName = __startColAmtName + 2;
                }//end items section
                    result =  _excelReader.GetAsByteArray();
                return result;
            }
        }
    }
}