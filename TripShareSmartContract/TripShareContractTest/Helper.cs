using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Neo.Cryptography;
using Neo.VM;
using Newtonsoft.Json;
using Neo;
using System.Collections;
using Neo.Core;
using System.Text;
using System.Linq;
using System.Numerics;

namespace TripShareContractTest
{
    static class Helper
    {
        public const string CONTRACT_ADDRESS = @"C:\Users\Tolya\source\repos\TripShareSmartContract\TripShareSmartContract\bin\Debug\TripShareSmartContract.avm";

        public const string NEO_ASSET_ID = "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b";

        public static IScriptContainer scriptContainer;
        public static CustomInteropService service;

        public static void Init()
        {
            service = new CustomInteropService();
            service.storageContext.data = new Hashtable();
        }

        public static string GetKey(string ownerKey)
        {
            return UInt160.Parse(ownerKey).ToArray().ToHexString();
        }

        public static string reverseHex(string s)
        {
            string m = "";
            for (int i = s.Length - 2; i >= 0; i -= 2)
            {
                m += s.Substring(i, 2);
            }
            return m;
        }

        public static void InitTransactionContext(string scriptHash, int value, ushort inputAmount = 1)
        {
            Transaction initialTransaction = new CustomTransaction(TransactionType.ContractTransaction);
            Transaction currentTransaction = new CustomTransaction(TransactionType.ContractTransaction);
            initialTransaction.Outputs = new TransactionOutput[inputAmount];
            currentTransaction.Inputs = new CoinReference[inputAmount];


            for (ushort i = 0; i < inputAmount; ++i)
            {
                /** CREATE FAKE PREVIOUS TRANSACTION */
                var transactionOutput = new TransactionOutput
                {
                    ScriptHash = UInt160.Parse(scriptHash),
                    Value = new Fixed8(value),
                    AssetId = UInt256.Parse(NEO_ASSET_ID)
                };

                initialTransaction.Outputs[i] = transactionOutput;
                /** CREATE FAKE CURRENT TRANSACTION */
                var coinRef = new CoinReference
                {
                    PrevHash = initialTransaction.Hash,
                    PrevIndex = i
                };

                currentTransaction.Outputs = new TransactionOutput[1];
                currentTransaction.Outputs[0] = new TransactionOutput
                {
                    ScriptHash = UInt160.Parse(scriptHash),
                    Value = new Fixed8(value),
                    AssetId = UInt256.Parse(NEO_ASSET_ID)
                };


                currentTransaction.Inputs[i] = coinRef;
            }


            /**INIT CONTEXT*/
            service.transactions[initialTransaction.Hash] = initialTransaction;
            scriptContainer = currentTransaction;
        }

        public static bool MintTokens()
        {
            ExecutionEngine engine = new ExecutionEngine(scriptContainer, Crypto.Default, null, service);
            engine.LoadScript(File.ReadAllBytes(Helper.CONTRACT_ADDRESS));

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(0);
                sb.Emit(OpCode.PACK);
                sb.EmitPush("mintTokens");
                engine.LoadScript(sb.ToArray());
            }

            engine.Execute();
            Assert.AreEqual(engine.State, VMState.HALT);
            return engine.EvaluationStack.Peek().GetBoolean();
        }


        public static bool TransferToken(string fromSH, string toSH, int value)
        {
            ExecutionEngine engine = new ExecutionEngine(scriptContainer, Crypto.Default, null, service);
            engine.LoadScript(File.ReadAllBytes(CONTRACT_ADDRESS));

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(value);
                sb.EmitPush(UInt160.Parse(toSH));
                sb.EmitPush(UInt160.Parse(fromSH));
                sb.EmitPush(3);
                sb.Emit(OpCode.PACK);
                sb.EmitPush("transfer");
                engine.LoadScript(sb.ToArray());
            }

            engine.Execute();
            Assert.AreEqual(engine.State, VMState.HALT);
            return engine.EvaluationStack.Peek().GetBoolean();
        }

        public static bool RegisterTrip(string id, string driver, string way, DateTime date,
            DateTime cancelDate, int seatsCount, int price,
            int depositDriver)
        {
            ExecutionEngine engine = new ExecutionEngine(scriptContainer, Crypto.Default, null, service);
            engine.LoadScript(File.ReadAllBytes(CONTRACT_ADDRESS));
            uint dateStart = date.ToTimestamp();
            uint dateCancel = cancelDate.ToTimestamp();

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(depositDriver);
                sb.EmitPush(price);
                sb.EmitPush(seatsCount);
                sb.EmitPush(dateCancel);
                sb.EmitPush(dateStart);
                sb.EmitPush(way);
                sb.EmitPush(UInt160.Parse(driver));
                sb.EmitPush(id);
                sb.EmitPush(8);
                sb.Emit(OpCode.PACK);
                sb.EmitPush("registerTrip");
                engine.LoadScript(sb.ToArray());
            }

            engine.Execute();
            Assert.AreEqual(engine.State, VMState.HALT);
            return engine.EvaluationStack.Peek().GetBoolean();
        }
    }
}
