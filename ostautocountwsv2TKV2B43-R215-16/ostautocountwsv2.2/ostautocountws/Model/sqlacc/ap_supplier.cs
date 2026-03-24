using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model.sqlacc
{
    public class ap_supplier
    {

        public string CODE { get; set; }
        public string CONTROLACCOUNT { get; set; }
        public string COMPANYNAME { get; set; }
        public string COMPANYNAME2 { get; set; }
        public string COMPANYCATEGORY { get; set; }
        public string AREA { get; set; }
        public string AGENT { get; set; }
        public string BIZNATURE { get; set; }
        public string CREDITTERM { get; set; }
        public decimal CREDITLIMIT { get; set; }
        public decimal OVERDUELIMIT { get; set; }
        public string STATEMENTTYPE { get; set; }
        public string CURRENCYCODE { get; set; }
        public decimal OUTSTANDING { get; set; }
        public string ALLOWEXCEEDCREDITLIMIT { get; set; }
        public string ADDPDCTOCRLIMIT { get; set; }
        public string AGINGON { get; set; }
        public string STATUS { get; set; }
        public string PRICETAG { get; set; }
        public DateTime CREATIONDATE { get; set; }
        public string TAX { get; set; }
        public string TAXEXEMPTNO { get; set; }
        public DateTime TAXEXPDATE { get; set; }
        public string REGISTERNO { get; set; }
        public string GSTNO { get; set; }
        public string TAXAREA { get; set; }
        public byte[] ATTACHMENTS { get; set; }
        public string REMARK { get; set; }
        public byte[] NOTE { get; set; }
     
    
       
    }
}
