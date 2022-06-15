using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;

namespace ThreeLevelListDetailsSample.Data
{
    public class WorkRequestItem : IItem
    {
        public List<AttachmentItem> AttachmentItems { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public string Title { get; set; }
    }

    public class AttachmentItem : IItem
    {
        public BitmapImage Photo { get; set; }
        public DateTime DateAdded { get; set; }
        public string Title { get; set; }
    }

    public interface IItem
    {
        DateTime DateAdded { get; set; }

        string Title { get; set; }
    }
}
