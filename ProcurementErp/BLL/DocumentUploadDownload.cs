using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class DocumentUploadDownload
    {
        //process upload on a new sourcing request.
        static string path = ConfigurationManager.AppSettings["FilePath"];
        public static void UploadDocument(IEnumerable<HttpPostedFileBase> _Files, string BranchCode, decimal RequestId)
        {
            List<SOURCING_REQUEST_FILES> Files = new List<SOURCING_REQUEST_FILES>();
            
            if (_Files != null)
            {
                bool exists = Directory.Exists(path + /*BranchCode +*/ "/");
                if (!exists)
                {
                    Directory.CreateDirectory(path + /*BranchCode +*/ "/");
                }
                foreach (var file in _Files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        
                        var fileName = Path.GetFileName(file.FileName);
                        string fileExtension = Path.GetExtension(file.FileName);

                        string finalFileName = fileName + fileExtension;
                        string SplitPath = (Path.Combine(path + /*BranchCode +*/ @"\", Guid.NewGuid() + fileExtension));
                        string imagepath = SplitPath;
                        file.SaveAs(SplitPath);
                       
                        Files.Add(new SOURCING_REQUEST_FILES
                        {
                            SOURCE_REQ_ID = RequestId,
                            FILENAME = fileName,
                            FILETYPE = fileExtension,
                            FILE_PATH = imagepath
                        });
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
                
            }
        }

        public static bool DeleteFileFromServer(string FileId)
        {
            SOURCING_REQUEST_FILES Files = new SOURCING_REQUEST_FILES();
            decimal _FileId = Convert.ToDecimal(FileId);
            Entities context = new Entities();
            
            Files = context.SOURCING_REQUEST_FILES.Where(m => m.FIILE_ID == _FileId).FirstOrDefault();
            
            if (Files != null)
            {
                string fullPath = Files.FILE_PATH;
                bool exists = File.Exists(fullPath);
                if (!exists) return false;

                try //Maybe error could happen like Access denied or Presses Already User used
                {
                    System.IO.File.Delete(fullPath);
                    context.SOURCING_REQUEST_FILES.Remove(Files);
                    context.SaveChanges();
                    context.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log("An error occurred " + e.StackTrace, "error");
                    return false;
                }

            }
            else
            {
                return false;
            }

        }
    }
}