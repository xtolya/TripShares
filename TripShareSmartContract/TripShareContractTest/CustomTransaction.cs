using Neo.Core;

namespace TripShareContractTest
{
    class CustomTransaction : Transaction
    {
        public CustomTransaction(TransactionType type) : base(type)
        {
            Version = 1;
            Inputs = new CoinReference[0];
            Outputs = new TransactionOutput[0];
            Attributes = new TransactionAttribute[0];
        }
    }
}
