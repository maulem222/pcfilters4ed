using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTML_voor_gevorderden_VI
{
    public class Menu
    {
        public int id;
        public string href;
        public string text;
        public int ParentId;
        public List<Menu> children;
    }
}