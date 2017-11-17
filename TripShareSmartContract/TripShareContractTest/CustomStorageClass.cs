using Neo;
using Neo.VM;
using System;
using System.Collections;
using System.Numerics;

namespace TripShareContractTest
{
    class CustomStorageContext : IInteropInterface
    {
        public UInt160 ScriptHash;

        public Hashtable data;

        public CustomStorageContext()
        {
            data = new Hashtable();
        }

        public byte[] ToArray()
        {
            return ScriptHash.ToArray();
        }
    }
}
