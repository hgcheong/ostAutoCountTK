using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws.Model
{
    public class licenseMaint
    {
        public string ProductKey { get; set; }
        public string EncryptedData { get; set; }
        public string DecryptedData { get; set; }
    }
}
