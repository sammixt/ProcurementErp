﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.Models
{
    public class RequestViewModel
    {
        public string RequestType { get; set; }

        public String ReferenceNumber { get; set; }

        public Nullable<System.DateTime> StartDate { get; set; }

        public Nullable<System.DateTime> DueDate { get; set; }

        public string Status { get; set; }

        public decimal TempNo { get; set; }
    }
}