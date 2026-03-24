using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
   public class agentMaint
    {
     
        public string SalesAgent { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }

        public Guid Guid { get; set; }
        public int LastUpdate { get; set; }
    }
}
