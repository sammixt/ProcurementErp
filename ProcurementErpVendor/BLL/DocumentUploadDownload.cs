using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.BLL
{
    public class DocumentUploadDownload
    {
        static string path = ConfigurationManager.AppSettings["FilePath"];
        public static string UploadDocument(IEnumerable<HttpPostedFileBase> _Files, string RefNum, decimal _VendorId, decimal TempId)
        {
            string Msg = null;
            List<SOURCING_REQUEST_FILES> Files = new List<SOURCING_REQUEST_FILES>();
            var supportedTypes = new[] { ".pdf", ".xls", ".xlsx" };
            if (_Files != null)
            {
                bool exists = Directory.Exists(path + /*RefNum + */"/");
                if (!exists)
                {
                    Directory.CreateDirectory(path +  /*RefNum + */"/");
                }
                foreach (var file in _Files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        string fileExtension = Path.GetExtension(file.FileName);
                        if (supportedTypes.Contains(fileExtension))
                        {
                            string finalFileName = fileName + fileExtension;
                            string SplitPath = (Path.Combine(path +/* RefNum +*/ "/", Guid.NewGuid() + fileExtension));
                            string imagepath = SplitPath;
                            file.SaveAs(SplitPath);
                            Files.Add(new SOURCING_REQUEST_FILES
                            {
                                SOURCE_REQ_ID = TempId,
                                FILENAME = fileName,
                                FILETYPE = fileExtension,
                                FILE_PATH = imagepath,
                                VENDORID = _VendorId
                            });
                        }
                        else
                        {
                            Msg = "Not all document where uploaded File Format not Supportted";
                        }
                    }
                }
                foreach (var item in Files)
                {
                    using (Entities context = new Entities())
                    {
                        context.SOURCING_REQUEST_FILES.Add(item);
                        context.SaveChanges();
                    }
                }
                Msg = "Quotation Submitted";
                return Msg;
            }
            else { return null; }
        }
    }
}