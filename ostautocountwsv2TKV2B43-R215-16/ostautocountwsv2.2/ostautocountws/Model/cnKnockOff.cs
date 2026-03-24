using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
  public  class cnKnockOff
    {
        public string CREDITNUMBER { get; set; }
        public string INVOICENUMBER { get; set; }

        public string CURRENCYCODE { get; set; }
        public decimal APPLIEDAMOUNT { get; set; }
        public decimal PAYMENTEXCHANGERATE { get; set;}



    }
}
