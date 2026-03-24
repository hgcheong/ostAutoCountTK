using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
    public class glTran
    {
        public DateTime DocDate { get; set; }
        public string DocNo { get; set; }
        public string DocNo2 { get; set; }
        public string Description { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }

        public string ProjNo { get; set; }
        public int UNIQUEID { get; set; }
        public int BATCHFILENO { get; set; }
        public decimal FINANCIALCOST { get; set; }
    }
}
