using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
    public class arinvoice
    {
        public string DocNo { get; set; }

        public DateTime DocDate { get; set; }
        public string DebtorCode { get; set; }
        public string DisplayTerm { get; set; }

        public string SalesAgent { get; set; }
        public string JournalType { get; set; }
        public string HeaderDescription { get; set; }
        public string Description { get; set; }

        public string ProjNo { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxRate { get; set; }
        public string TaxType { get; set; }
        public string AccNo { get; set; }
        public string BATCHFILENO { get; set; }
        public string CURRENCYCODE { get; set; }
        public decimal EXCHANGERATE { get; set; }
        public decimal Amount { get; set; }
    }
}
