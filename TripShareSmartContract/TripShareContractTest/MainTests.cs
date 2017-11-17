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

/// <summary>
/// TESTS SHOULD BE RUN WITH HUGE RESTICTIONS AND
/// WITH MANY CODE COMMENTED ON SC
/// </summary>
namespace TripShareContractTest
{/*
    [TestClass]
    public class MainTest
    {

        public string[] HASHES = new string[] {
            "0x22a4d553282d7eaf53538eb8ccb27e842d0d90b6",
            "0xbc89c04256bd0a5b9d53a0d239d615a8734bc459",
            "0x1e66cccfed7a0a9f4bc9bf6c92b286acef65fc77",
            "0xa42abb913fa551de74fd4626ad4a789a2987e52e"
        };


        [TestInitialize]
        public void InitInteropService()
        {
            Helper.Init();
        }

        [TestMethod]
        public void CanMintTokens()
        {
            var data = Helper.service.storageContext.data;
            string key;
            byte[] ba;

            Helper.InitTransactionContext(HASHES[0], 5);
            Assert.AreEqual(Helper.MintTokens(), true);
            key = Helper.GetKey(HASHES[0]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 5);

            Helper.InitTransactionContext(HASHES[1], 40);
            Assert.AreEqual(Helper.MintTokens(), true);
            key = Helper.GetKey(HASHES[1]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 40);

            Helper.InitTransactionContext(HASHES[0], 100);
            Assert.AreEqual(Helper.MintTokens(), true);
            key = Helper.GetKey(HASHES[0]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 100 + 5);

            Helper.InitTransactionContext(HASHES[0], 5);
            Assert.AreEqual(Helper.MintTokens(), true);
            key = Helper.GetKey(HASHES[0]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 105 + 5);
        }

        [TestMethod]
        public void ReturnsFalseWhenContributedValueIsZero()
        {
            Helper.InitTransactionContext(HASHES[0], 0);
            var data = Helper.service.storageContext.data;
            Assert.AreEqual(Helper.MintTokens(), false);
        }


        [TestMethod]
        public void CanTransferToken()
        {
            var data = Helper.service.storageContext.data;
            string key;
            byte[] ba;

            Helper.InitTransactionContext(HASHES[1], 10);
            Assert.AreEqual(Helper.MintTokens(), true);

            Assert.AreEqual(Helper.TransferToken(HASHES[1], HASHES[0], 3), true);

            key = Helper.GetKey(HASHES[1]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 10 - 3);
            key = Helper.GetKey(HASHES[0]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 0 + 3);

            /SENDING TO YOURSELF
            Assert.AreEqual(Helper.TransferToken(HASHES[1], HASHES[1], 2), true);
            key = Helper.GetKey(HASHES[1]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 7);

            Helper.InitTransactionContext(HASHES[2], 10);
            Assert.AreEqual(Helper.MintTokens(), true);
            Assert.AreEqual(Helper.TransferToken(HASHES[2], HASHES[1], 4), true);
            key = Helper.GetKey(HASHES[2]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 10 - 4);
            key = Helper.GetKey(HASHES[1]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 7 + 4);
        }

        [TestMethod]
        public void CanTransferTokenWhenInsufficient()
        {
            Helper.InitTransactionContext(HASHES[1], 10);
            Assert.AreEqual(Helper.MintTokens(), true);
            Assert.AreEqual(Helper.TransferToken(HASHES[1], HASHES[0], 14), false);
        }

        [TestMethod]
        public void CanSendAllTokens()
        {
            var data = Helper.service.storageContext.data;
            string key;
            byte[] ba;

            Helper.InitTransactionContext(HASHES[1], 10);
            Assert.AreEqual(Helper.MintTokens(), true);
            Assert.AreEqual(Helper.TransferToken(HASHES[1], HASHES[0], 10), true);

            key = Helper.GetKey(HASHES[1]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 10 - 10);

            key = Helper.GetKey(HASHES[0]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 0 + 10);
        }

        [TestMethod]
        public void ReturnsFalseWhenInvalidOperation()
        {
            Helper.InitTransactionContext(HASHES[0], 10);
            ExecutionEngine engine = new ExecutionEngine(Helper.scriptContainer, Crypto.Default, null, Helper.service);
            engine.LoadScript(File.ReadAllBytes(Helper.CONTRACT_ADDRESS));


            //Assert.AreEqual(engine.CurrentContext.ScriptHash.Length, 0);
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush("arg1");
                sb.EmitPush("arg2");
                sb.EmitPush(2);
                sb.Emit(OpCode.PACK);
                sb.EmitPush("invalidoperation");
                engine.LoadScript(sb.ToArray());
            }

            engine.Execute();
            Assert.AreEqual(engine.State, VMState.HALT);
            var result = engine.EvaluationStack.Peek().GetBoolean();
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void TestMethodString()
        {
            Assert.AreEqual(true, true);
            Assert.AreEqual(true, false);
        }

        [TestMethod]
        public void CanRegisterTrip()
        {
            var data = Helper.service.storageContext.data;
            string key;
            byte[] ba;

            Helper.InitTransactionContext(HASHES[0], 120);
            Assert.AreEqual(Helper.MintTokens(), true);
            Assert.AreEqual(data.Count, 2);
            key = Helper.GetKey(HASHES[0]);
            ba = (byte[])data[key];
            Assert.AreEqual((int)ba[0], 120);
            
            Assert.AreEqual(Helper.RegisterTrip("CfD8cAeDefC1Dd5Bd7af7", HASHES[0], "blablabla", 
                DateTime.Now, new DateTime(1999, 12, 22), 10, 100, 50), true);
                
            Assert.AreEqual(data.Count, 11);

        }
    }*/
}
