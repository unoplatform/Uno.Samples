using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeEntryRia.Web.Models
{
    public class SummaryRow
    {
        public int? ProjectId { get; set; }
        public string Name { get; set; }
        public bool IsTotalRow { get; set; }
        public double MonTotal { get; set; }
        public double TueTotal { get; set; }
        public double WedTotal { get; set; }
        public double ThuTotal { get; set; }
        public double FriTotal { get; set; }
        public double SatTotal { get; set; }
        public double SunTotal { get; set; }
        public double Total { get; set; }
    }
}