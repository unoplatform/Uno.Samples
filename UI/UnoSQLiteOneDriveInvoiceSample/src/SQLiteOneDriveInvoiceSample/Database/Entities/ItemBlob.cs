using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteOneDriveInvoiceSample.Database.Entities
{
    public class ItemBlob
    {
        public string ItemType { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
