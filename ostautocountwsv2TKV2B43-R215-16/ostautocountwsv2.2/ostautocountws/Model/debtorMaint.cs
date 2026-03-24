using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
   public class debtorMaint
    {
        public string AccNo { get; set; }
        public string ControlAccount { get; set; }
        public string CompanyName { get; set; }
        public string RegisterNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostCode { get; set; }
        public string DeliverAddr1 { get; set; }
        public string DeliverAddr2 { get; set; }
        public string DeliverAddr3 { get; set; }
        public string DeliverAddr4 { get; set; }
        public string DeliverPostCode { get; set; }

        public string Attention { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax1 { get; set; }
        public string Fax2 { get; set; }
        public string AreaCode { get; set; }
        public string SalesAgent { get; set; }
        public string WebURL { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayTerm { get; set; }
        public decimal CreditLimit { get; set; }
        public string CurrencyCode { get; set; }
        public bool FOREIGNCURRENCY { get; set; }
        public string INSERTEDORUPDATED { get; set; }
        public string Note { get; set; }

        public string TaxType { get; set; }
    }
}
