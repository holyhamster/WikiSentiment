using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiLibrary.API
{
    internal static class Parsing
    {
        public static string NormalizeTitle(string title)
        {
            return title.Trim().Replace(' ', '_');
        }

        public static string StripSubmenuLink(string title)
        {
            return title.Split('#', 2)[0];
        }
    }
}
