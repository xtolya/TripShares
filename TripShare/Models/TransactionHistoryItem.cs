using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TripShare.Models
{
    public class TransactionHistoryItem
    {
        public double GAS { get; set; }
        public double NEO { get; set; }
        public Int64 block_index { get; set; }
        public bool gas_sent { get; set; }
        public bool neo_sent { get; set; }
        public string txid { get; set; }
    }
}
