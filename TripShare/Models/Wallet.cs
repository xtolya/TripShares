using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;

namespace TripShare.Models
{
    public class Wallet
    {
        public string privateKey { get; set; }
        public string address { get; set; }
        public string wif { get; set; }
        public string scriptHash { get; set; }
    }
}
