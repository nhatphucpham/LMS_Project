using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Project.Model
{
    public class NavItem
    {
        public int Index;
        public object Glyph { get; set; }
        public string Text { get; set; }
        public Type Page { get; set; }
        public NavItem(int index, object glyph = null, string text = "", Type page = null)
        {
            Index = index;
            Glyph = glyph;
            Text = text;
            Page = page;
        }
    }

}
