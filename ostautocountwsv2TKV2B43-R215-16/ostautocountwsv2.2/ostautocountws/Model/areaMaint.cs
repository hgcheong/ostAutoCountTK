using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
    public class areaMaint
    {
        public string AreaCode { get; set; }
        public string Description { get; set; }
        public int LastUpdate { get; set; }

        public Guid Guid { get; set; }
    }
}
