using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripShare.Models;

namespace TripShare.Abstract
{
    public enum NETWORK_TYPE { TESTNET = 0, MAINNET = 1 };
    public enum ASSET_NAME { NEO = 0, GAS = 1 };

    public interface IBlockchainRepository
    {
        Task<Balance> GetBalance(NETWORK_TYPE type, string addr);
        Task<IEnumerable<TransactionHistoryItem>> GetTransactionHistory(
            NETWORK_TYPE type, string addr);
        Task<bool> SendAsset(NETWORK_TYPE type, string wif, ASSET_NAME assetName, string addr, int amount);
        Task<Wallet> GenerateWallet();
        Task<string> GetScriptHashFromAddress(string address);
        Task<string> GetStorage(NETWORK_TYPE type, string key);
        Task<bool> VerifyAddress(string addr);
        Task<bool> InvokeContractMintToken(NETWORK_TYPE net, string wif, int neoAmount, int gasCost);
        Task<int> GetTokenBalance(NETWORK_TYPE net, string revSh);
    }
}
