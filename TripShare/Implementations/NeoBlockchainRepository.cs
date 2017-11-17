using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using TripShare.Models;
using TripShare.Abstract;
using Newtonsoft.Json;

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

        /*RegisterTrip(byte[] id, byte[] driver, string way, BigInteger date,
          BigInteger cancelDate, BigInteger seatsCount, BigInteger price,
          BigInteger depositDriver)

          public static bool CancelTrip(byte[] id)

          public static bool ReserveSeat(byte[] id, byte[] passenger)
          public static bool CancelSeat(byte[] id, byte[] passenger)
          public static bool PayForTrip(byte[] id, byte[] passenger)
          public static bool RequestRefund(byte[] user, BigInteger value)
          public static bool CleanUp(byte[] id)*/


        public async Task<bool> InvokeContractRegisterTrip(NETWORK_TYPE net, string wif, Trip trip, int gasCost)
        {
            try
            {
                string revSh = Helper.ReverseHex(trip.DriverId);
                uint a = trip.Date.ToTimestamp();
                uint b = trip.CancelDate.ToTimestamp();
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractRegisterTrip", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.Str2Hex(trip.Id), revSh, Helper.Str2Hex(trip.From + trip.To),
                    a, b, trip.SeatsCount, trip.Price, trip.Deposit);
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractCancelTrip(NETWORK_TYPE net, string wif, string id, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractCancelTrip", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.Str2Hex(id));
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractTransfer(NETWORK_TYPE net, string wif, string from, string to, int amount, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractTransfer", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.ReverseHex(from), Helper.ReverseHex(to), amount);
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractCancelSeat(NETWORK_TYPE net, string wif, string id, string passengerId, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractCancelSeat", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.Str2Hex(id), Helper.ReverseHex(passengerId));
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractReserveSeat(NETWORK_TYPE net, string wif, string id, string passengerId, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractReserveSeat", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.Str2Hex(id), Helper.ReverseHex(passengerId));
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractPayForTrip(NETWORK_TYPE net, string wif, string id, string passengerId, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractPayForTrip", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.Str2Hex(id), Helper.ReverseHex(passengerId));
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractRequestRefund(NETWORK_TYPE net, string wif, string userId, int value, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractRequestRefund", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.ReverseHex(userId), value);
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> InvokeContractCleanUp(NETWORK_TYPE net, string wif, string id, int gasCost)
        {
            try
            {
                var res = await nodeServices.InvokeExportAsync<bool>(GetScriptLocation(),
                    "InvokeContractCleanUp", GetNetwork(net), wif, CONTRACT_HASH, gasCost, Helper.Str2Hex(id));
                return res;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

