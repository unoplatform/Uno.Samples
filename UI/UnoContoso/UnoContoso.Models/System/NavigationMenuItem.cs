using System;
using System.Collections.Generic;
using System.Text;

namespace UnoContoso.Models
{
    /// <summary>
    /// Navigation Menu Item model
    /// </summary>
    public class NavigationMenuItem
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public string Icon { get; set; }

        public string Path { get; set; }
    }
}
