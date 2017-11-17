using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TripShare.Models.ManageViewModels
{
    public class WalletViewModel

    {
        public bool HasGeneratedWallet { get; set; }
        public Wallet WalletInfo { get; set; }
        public int AmountToMint { get; set; }
    }

}
