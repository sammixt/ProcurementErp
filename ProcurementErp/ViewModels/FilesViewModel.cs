using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class FilesViewModel
    {
        public decimal FIILE_ID { get; set; }
        public Nullable<decimal> SOURCE_REQ_ID { get; set; }
        public string FILENAME { get; set; }
        public string FILETYPE { get; set; }
        public string FILE_PATH { get; set; }
        
    }
}