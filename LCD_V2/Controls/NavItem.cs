using System;
using System.Windows;
using System.Windows.Media;

namespace LCD_V2.Controls
{
    /// <summary>
    /// Navigation list row model. Each item binds to a sidebar ListBoxItem.
    /// </summary>
    public sealed class NavItem
    {
        public string Title { get; set; }
        public Geometry Icon { get; set; }
        public Type PageType { get; set; }
        public string Badge { get; set; }
    }
}
