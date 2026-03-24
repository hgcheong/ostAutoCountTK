using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
   public class termMaint
    {
        public string DisplayTerm { get; set; }
        public string Terms { get; set; }
        public string TermDaysFrom { get; set; }
        public int TermDays { get; set; }

        public int LastUpdate { get; set; }
    }
}
