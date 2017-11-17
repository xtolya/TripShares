using Neo;
using Neo.Core;
using Neo.VM;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace TripShareContractTest
{
    class CustomInteropService : InteropService
    {
        public const int NEO_FACTOR = 100000000;
        public CustomStorageContext storageContext;
        public Hashtable transactions;
        public const uint DATE = 1000;
        byte[] exSH = { };

        public CustomInteropService()
        {
            Register("Neo.Storage.GetContext", Storage_GetContext);
            Register("Neo.Storage.Get", Storage_Get);
            Register("Neo.Storage.Put", Storage_Put);
            Register("Neo.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("Neo.Transaction.GetInputs", Transaction_GetInputs);
            Register("Neo.Transaction.GetOutputs", Transaction_GetOutputs);
            Register("Neo.Blockchain.GetTransaction", Blockchain_GetTransaction);
            Register("Neo.Transaction.GetReferences", Transaction_GetReferences);
            Register("Neo.Input.GetHash", Input_GetHash);
            Register("Neo.Input.GetIndex", Input_GetIndex);
            Register("Neo.Output.GetScriptHash", Output_GetScriptHash);
            Register("Neo.Output.GetValue", Output_GetValue);
            Register("Neo.Output.GetAssetId", Output_GetAssetId);
            Register("Neo.Runtime.Notify", Runtime_Notify);
            Register("Neo.Blockchain.GetHeader", Blockchain_GetHeader);
            Register("Neo.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("Neo.Header.GetTimestamp", Header_GetTimestamp);
            Register("Neo.Runtime.Notify", Runtime_Notify);
            Register("System.ExecutionEngine.GetExecutingScriptHash", ExecutionEngine_GetExecutingScriptHash);

            storageContext = new CustomStorageContext();
            transactions = new Hashtable();
        }


        private static bool ExecutionEngine_GetExecutingScriptHash(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(engine.CurrentContext.ScriptHash);
            return true;
        }

        protected virtual bool Output_GetAssetId(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null)
                return false;

            engine.EvaluationStack.Push(output.AssetId.ToArray());
            return true;
        }

        protected virtual bool Runtime_CheckWitness(ExecutionEngine engine)
        {
            StackItem si = engine.EvaluationStack.Pop();
            engine.EvaluationStack.Push(true);
            return true;
        }

        public bool Storage_GetContext(ExecutionEngine engine)
        {
            this.storageContext.ScriptHash = new UInt160(engine.CurrentContext.ScriptHash);
            engine.EvaluationStack.Push(StackItem.FromInterface(storageContext));
            return true;
        }

        protected bool Storage_Get(ExecutionEngine engine)
        {
            CustomStorageContext context = engine.EvaluationStack.Pop().GetInterface<CustomStorageContext>();
            var key = engine.EvaluationStack.Pop().GetByteArray().ToHexString();
            StorageItem item = new StorageItem
            {
                Value = (byte[])context.data[key]
            };
            engine.EvaluationStack.Push(item?.Value ?? new byte[0]);
            return true;
        }


        protected bool Storage_Put(ExecutionEngine engine)
        {
            CustomStorageContext context = engine.EvaluationStack.Pop().GetInterface<CustomStorageContext>();
            var top = engine.EvaluationStack.Pop();
            var key = top.GetByteArray().ToHexString();
            if (key.Length > 1024)
                return false;
            byte[] value = engine.EvaluationStack.Pop().GetByteArray();

            context.data[key] = value;
            return true;
        }

        protected bool Transaction_GetInputs(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Inputs.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected bool Blockchain_GetTransaction(ExecutionEngine engine)
        {
            byte[] hash = engine.EvaluationStack.Pop().GetByteArray();
            Transaction tx = (Transaction)this.transactions[hash];
            engine.EvaluationStack.Push(StackItem.FromInterface(tx));
            return true;
        }

        protected bool Blockchain_GetHeight(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(DATE);
            return true;
        }
        

        protected bool Blockchain_GetHeader(ExecutionEngine engine)
        {
            BigInteger header = engine.EvaluationStack.Pop().GetBigInteger();
            engine.EvaluationStack.Push(header);
            return true;
        }

        protected bool Header_GetTimestamp(ExecutionEngine engine)
        {
            BigInteger header = engine.EvaluationStack.Pop().GetBigInteger();
            engine.EvaluationStack.Push(header);
            return true;
        }

        protected virtual bool Input_GetHash(ExecutionEngine engine)
        {
            CoinReference input = engine.EvaluationStack.Pop().GetInterface<CoinReference>();
            if (input == null) return false;
            engine.EvaluationStack.Push(input.PrevHash.ToArray());
            return true;
        }

        protected virtual bool Output_GetValue(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null)
                return false;
            engine.EvaluationStack.Push(output.Value.GetData());
            return true;
        }

        protected virtual bool Output_GetScriptHash(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null)
                return false;
            engine.EvaluationStack.Push(output.ScriptHash.ToArray());
            return true;
        }

        protected virtual bool Input_GetIndex(ExecutionEngine engine)
        {
            CoinReference input = engine.EvaluationStack.Pop().GetInterface<CoinReference>();
            if (input == null) return false;
            engine.EvaluationStack.Push((int)input.PrevIndex);
            return true;
        }

        protected virtual bool Transaction_GetOutputs(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null)
                return false;
            engine.EvaluationStack.Push(tx.Outputs.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected virtual bool Transaction_GetReferences(ExecutionEngine engine)
        {
            Transaction current_tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();

            if (current_tx == null)
                return false;

            Dictionary<CoinReference, TransactionOutput> dictionary = new Dictionary<CoinReference, TransactionOutput>();
            foreach (var group in current_tx.Inputs.GroupBy(p => p.PrevHash))
            {
                Transaction prev_tx = (Transaction)transactions[group.Key];

                if (prev_tx == null)
                    return false;

                var inReferences = group.Select(p => new
                {
                    Input = p,
                    Output = prev_tx.Outputs[p.PrevIndex]
                });

                foreach (var reference in inReferences)
                {
                    dictionary.Add(reference.Input, reference.Output);
                }
            }

            engine.EvaluationStack.Push(dictionary.Select(v => StackItem.FromInterface(v.Value)).ToArray());

            return true;
        }

        protected virtual bool Runtime_Notify(ExecutionEngine engine)
        {
            StackItem state = engine.EvaluationStack.Pop();
            return true;
        }

    }

}
