using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
   public class cdeposit
    {
        public DateTime DocDate { get; set; }
        public string DebtorCode { get; set; }

        public string DocKey { get; set; }
        public string DocNo { get; set; }
        public string DocNo2 { get; set; }
        public string DebitAcc { get; set; }
        public string DrAccType { get; set; }
        public string ChequeNo { get; set; }
        public string CreditAcc { get; set; }
        public string CrAccType { get; set; }
        public string PAYMENTACCOUNT { get; set; }
        public string PAYMENTMETHOD { get; set; }
        public string CURRENCYCODE { get; set; }
        public decimal EXCHANGERATE { get; set; }
        public string UNIQUEID { get; set; }
        public decimal PAYMENTAMOUNT { get; set; }

        public decimal UNAPPLIEDAMT { get; set; }
        public decimal FXVALUE { get; set; }

        public string Cancelled { get; set; }

        public string Description { get; set; }
    }
}
