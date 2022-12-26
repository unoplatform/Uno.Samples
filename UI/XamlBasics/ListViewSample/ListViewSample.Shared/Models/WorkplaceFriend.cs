using System;
using System.Runtime.InteropServices;

namespace ListViewSample.Models
{
    public class WorkplaceFriend
    {
        public string Name { get; set; }
        public string Occupation { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
    }
}