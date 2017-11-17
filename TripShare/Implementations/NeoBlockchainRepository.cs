using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using TripShare.Models;
using TripShare.Abstract;

namespace TripShare.Implementations
{
    public class NeoBlockchainRepository : IBlockchainRepository
    {
        private readonly string CONTRACT_HASH = "95276724df6b95f745e77b6061d7c158c168fa25";
        private readonly INodeServices nodeServices;

        public NeoBlockchainRepository(
            INodeServices _nodeServices)
        {
            nodeServices = _nodeServices;
        }

        private string GetScriptLocation() => "./Node/blockchain.js";

        public async Task<Wallet> GenerateWallet()
        {
            return await nodeServices.InvokeExportAsync<Wallet>(GetScriptLocation(), "GenerateWallet");
        }

        public async Task<string> GetScriptHashFromAddress(string address)
        {
            var res = await nodeServices.InvokeExportAsync<string>(GetScriptLocation(), "GetScriptHashFromAddress", address);
            return res;
        }

        public async Task<Balance> GetBalance(NETWORK_TYPE type, string addr)
        {
            return await nodeServices.InvokeExportAsync<Balance>(GetScriptLocation(), "GetBalance", GetNetwork(type), addr);
        }

        public async Task<string> GetStorage(NETWORK_TYPE type, string key)
        {
            try
            {
                var res = await nodeServices
                    .InvokeExportAsync<string>(GetScriptLocation(), "GetStorage", GetNetwork(type), CONTRACT_HASH, key);
                return res;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<IEnumerable<TransactionHistoryItem>> GetTransactionHistory(
            NETWORK_TYPE type, string addr)
        {
            return await nodeServices.InvokeExportAsync<
                IEnumerable<TransactionHistoryItem>>(GetScriptLocation(), "GetTransactionHistory", GetNetwork(type), addr);
        }

        public async Task<bool> SendAsset(NETWORK_TYPE type, string wif, ASSET_NAME assetName, string addr, int amount)
        {
            var asset = (assetName == ASSET_NAME.NEO) ? "NEO" : "GAS";
            var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                "SendAsset", GetNetwork(type), wif, asset, addr, amount);
            return res;
        }

        public async Task<bool> VerifyAddress(string addr)
        {
            return await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(), "VerifyAddress", addr);
        }

        private string GetNetwork(NETWORK_TYPE type)
        {
            return (type == NETWORK_TYPE.MAINNET) ? "MainNet" : "TestNet";
        }

        public async Task<bool> InvokeContractMintToken(NETWORK_TYPE net, string wif, int neoAmount, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractMintToken", GetNetwork(net), wif, CONTRACT_HASH, gasCost, neoAmount);
                return res;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<int> GetTokenBalance(NETWORK_TYPE net, string revSh)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<string>(GetScriptLocation(),
                    "GetTokenBalance", GetNetwork(net), CONTRACT_HASH, revSh);
                var a = Int32.Parse(res);
                return a;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

