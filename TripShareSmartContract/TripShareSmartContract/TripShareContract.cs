using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;
using Neo.SmartContract.Framework.Services.System;

namespace TripShareSmartContract
{
    /*
MIT License

Copyright (c) 2017 xtolya

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
    
    /// <summary>
    /// TRS - token name
    /// TripShare Contract represents itself an autonomous platform for people who
    /// want to make their trip more captivating through joining blockchain interested friends!
    /// Trip-host-passenger model is just an abstract thing to find out what is the dApp is made for.
    /// We call a trip any sort of cruise, journey, ride, travel, excursion and etc. (But mostly
    /// it is expected to be a simple road trip)
    /// We call a driver the one and those who actually organize the trip, so he can get paid for it if 
    /// he has chosen that way.
    /// We call a passenger the one who wants to join a trip which is organized by a driver, so he can
    /// reserve a seat on a trip!
    /// Whenever the trip is done a passenger can pay for this trip and simply proof it to the host
    /// via any blockchain tracker or our web app
    /// 
    /// Why blockchain?
    /// I assume everybody had a situation when someone cancels his trip just before it was
    /// supposed to start. Or someone who reserved a seat cancels it at the last moment.
    /// In any case it costs a huge amount of time to find a new trip and the one who broke
    /// your plans done lose anything.
    /// So how confident can you be in your future plans?
    /// 
    /// In this smartcontract for the trip registration you must make a TRS deposit which 
    /// is viewable for any user. You also enter the date when trip is planned to start and the cancel time
    /// before which a trip can be canceled for free. Since its blockchain, this data
    /// is viewable for everyone. SC won't allow you to take your deposit back when the trip
    /// is not done and the cancel date has passed. Trip is marked DONE only when 
    /// every passenger pays for it (confirms that its done, should be a real-life meating
    /// like a simple checkout) Then you get your deposit back. The same scheme works for seat reservation. 
    /// Passenger makes deposit which equal to drivers divided by seats count. Once trip is paid, 
    /// the deposit gets back to the passenger.
    /// Blockchain provides confirmation of deposits, time limits and etc. Everything becomes transparent.
    /// If the driver is confident in his plans he should make a bigger deposit, so it will be 
    /// bigger for passengers too and then he probably can rely on them. Also a passenger 
    /// which is confident in his plans will choose a driver with a bigger deposit, because 
    /// there are less chances to fail with a driver who made a huge deposit.
    /// However, nobody is interested in not confirmation the trip, because not confirming means 
    /// losing deposit. (Anyway these confirmations should be done face to face since the trip
    /// allows it).
    /// 
    /// Contract includes its own token which is equal to NEO because it has 1 to 100 swap rate
    /// You can buy a token whenever you want
    /// You can request a refund whenever you want and there isn't any time limitations such as ico
    /// usually has.
    /// 
    /// Overall, this dApp is made not for speculative purposes as it is usually done in many projects
    /// somehow associated with blockchain technology itself!
    /// </summary>
    public class TripShareContract : SmartContract
    {
        //NEP-5 interface things
        private static string Name() => "TripShare";
        private static string Symbol() => "TRS";
        public static byte Decimals() => 6;
        private const ulong neo_decimals = 1000000;

        //troubles with compiling events
        /*[DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        [DisplayName("refund")]
        public static event Action<byte[], BigInteger> Refund;*/

        private static void Transferred(byte[] from, byte[] to, BigInteger value)
        {
            Runtime.Notify("TRANSFER", from, to, value);
        }

        private static void Refund(byte[] to, BigInteger value)
        {
            Runtime.Notify("REFUND", to, value);
        }

        /*Neo asset id - NEO
        Owner - creators scripthash*/
        private static readonly byte[] owner = { 134, 145, 12, 9, 163, 135, 211, 239, 30, 52, 183, 75, 246, 135, 108, 158, 9, 124, 124, 59 };
        private static readonly byte[] neo_asset_id = { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147,
            229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 };

        /* Postfix as a helper to store data
        all the info about trip stores on it's ID+POSTFIX
        For example date key of a trip is id + postfix_date
        postfix's name displays the data it related to */
        public static readonly char POSTFIX_DATE = 'A';
        public static readonly char POSTFIX_DRIVER = 'B';
        public static readonly char POSTFIX_WAY = 'C';
        public static readonly char POSTFIX_SEAT_NUMBER = 'D';
        public static readonly char POSTFIX_PRICE = 'E';
        public static readonly char POSTFIX_SEAT_AVAILABLE = 'F';
        public static readonly char POSTFIX_DEPOSIT = 'G';
        public static readonly char POSTFIX_CANCEL_DATE = 'H';

        /* Const byte arrays to mark used keys which shouldnt be used again
        such as trip id, because when trip is cancelled, passenger will still have a seat
        on its id */
        public static readonly byte[] REGISTERED = { 31, 32 };
        public static readonly byte[] CANCELLED = { 32, 33 };
        public static readonly byte[] DONE = { 33, 34 };

        //Required length of id, so it cannot intersect with user's scripthash
        public const int ID_LENGTH = 14;

        /// <summary>
        /// Main method of a contract
        /// </summary>
        /// <param name="operation">Method to invoke</param>
        /// <param name="args">Method parameters</param>
        /// <returns>Method's return value or false if operation is invalid</returns>
        public static object Main(string operation, params object[] args)
        {
            //dApp methods
            if (operation == "registerTrip")
                return RegisterTrip((byte[])args[0], (byte[])args[1], (string)args[2], (BigInteger)args[3],
                    (BigInteger)args[4], (BigInteger)args[5], (BigInteger)args[6], (BigInteger)args[7]);

            if (operation == "reserveSeat")
                return ReserveSeat((byte[])args[0], (byte[])args[1]);

            if (operation == "cancelSeat")
                return CancelSeat((byte[])args[0], (byte[])args[1]);

            if (operation == "cancelTrip")
                return CancelTrip((byte[])args[0]);

            if (operation == "payForTrip")
                return PayForTrip((byte[])args[0], (byte[])args[1]);

            if (operation == "cleanUp")
                return CleanUp((byte[])args[0]);

            //NEP-5 related methods
            if (operation == "mintTokens")
                return MintTokens();

            if (operation == "transfer")
                return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);

            if (operation == "balanceOf")
                return BalanceOf((byte[])args[0]);

            if (operation == "requestRefund")
                return RequestRefund((byte[])args[0], (BigInteger)args[1]);

            //Notifying that the user made a mistake on calling mint tokens operation name
            byte[] sender = GetSender();
            if (sender.Length != 0)
            {
                BigInteger contribute_value = GetContributeValue();
                if (contribute_value > 0)
                {
                    Refund(sender, contribute_value);
                }
            }
            return false;
        }

        /// <summary>
        /// Method is just for the owner
        /// In our web App we are going to reward most active users
        /// typical transfer from owner's balance (no free mint for owner)
        /// </summary>
        /// <param name="user">scripthash of the user to ward</param>
        /// <param name="value">value in tokens to reward</param>
        private static bool Reward(byte[] user, BigInteger value)
        {
            //only owner can reward
            if (!Runtime.CheckWitness(owner))
                return false;

            //transfering reward and notifying
            Transfer(owner, user, value);
            Runtime.Notify("REWARD", user, value);
            return true;
        }

        /// <summary>
        /// Puts data value to the storage on a key + postfix
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="postfix">postfix</param>
        private static void PutOnPostfix(byte[] key, byte[] value, char postfix)
        {
            string k = key.AsString() + postfix;
            Storage.Put(Storage.CurrentContext, k, value);
            Runtime.Notify("PUT", value);
        }

        /// <summary>
        /// Gets data from the storage on key + postfix
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="postfix">postfix</param>
        /// <returns></returns>
        private static byte[] GetOnPostfix(byte[] key, char postfix)
        {
            string k = key.AsString() + postfix;
            return Storage.Get(Storage.CurrentContext, k);
        }

        /// <summary>
        /// Deletes data on key + postfix
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="postfix">postfix</param>
        private static void DeleteOnPostfix(byte[] key, char postfix)
        {
            string k = key.AsString() + postfix;
            Storage.Delete(Storage.CurrentContext, k);
            Runtime.Notify("DELETE", key);
        }

        /// <summary>
        /// Gets the current time
        /// </summary>
        /// <returns>current uint format date</returns>
        private static BigInteger Now()
        {
            uint height = Blockchain.GetHeight();
            Header header = Blockchain.GetHeader(height);
            uint res = header.Timestamp + 10;
            Runtime.Notify("NOW", res);
            return header.Timestamp + 10;
        }

        /// <summary>
        /// Registers a trip with parameters attached and puts it into storage
        /// All the information about the trip puts with on id + its postfix
        /// Once trip is registered, its id will never be able to register again on
        /// All the parameters are required
        /// works with deposits on its rules
        /// METHOD USES A LOT OF STORAGE / SHOULD BE SPREAD BY DIVIDED
        /// IN NEON ATTACH 2 GAS (FOR SOME REASON IT 
        /// DONT ACCEPT LESS THAN 1 AND NON INTEGER VALUES.)
        /// </summary>
        /// <param name="id">id of a trip, 21 length byte array</param>
        /// <param name="date">date in uint representation</param>
        /// <param name="driver">Drivers script hash (owner of a trip)</param>
        /// <param name="way">way</param>
        /// <param name="seatsCount">seats in a trip</param>
        /// <param name="price">ticket cost for a trip (may be 0)</param>
        /// <param name="depositDriver">amount of token which driver is to deposit</param>
        /// <param name="cancelDate">Date after which any cancel will lose deposit</param>
        /// <returns></returns>
        public static bool RegisterTrip(byte[] id, byte[] driver, string way, BigInteger date,
            BigInteger cancelDate, BigInteger seatsCount, BigInteger price,
            BigInteger depositDriver)
        {
            //only driver can register a trip
            if (!Runtime.CheckWitness(driver))
                return false;
            Runtime.Notify("passed check for driver");

            //Validating the input
            if (id.Length != ID_LENGTH || date < Now() ||
                way.Length < 3 || seatsCount < 1 || cancelDate > date)
                return false;
            Runtime.Notify("TripValidated");
            //Check if the trip id is not user and was not used before, so 
            //passengers can't get a free seat by mistake
            if (Storage.Get(Storage.CurrentContext, id).Length != 0)
                return false;

            string depositLoc = id.AsString() + POSTFIX_DEPOSIT;
            //try to transfer deposit
            if (!Transfer(driver, depositLoc.AsByteArray(), depositDriver))
                return false;
            Runtime.Notify("DEPOSIT", depositLoc, depositDriver);
            //Put info about the trip in storage
            //data sets on its postfix
            //marks trip as true as it is valid
            Runtime.Notify("PUT", id, REGISTERED);
            Storage.Put(Storage.CurrentContext, id, REGISTERED);
            PutOnPostfix(id, date.AsByteArray(), POSTFIX_DATE);
            PutOnPostfix(id, driver, POSTFIX_DRIVER);
            PutOnPostfix(id, way.AsByteArray(), POSTFIX_WAY);
            BigInteger b = seatsCount;
            PutOnPostfix(id, b.AsByteArray(), POSTFIX_SEAT_NUMBER);
            PutOnPostfix(id, b.AsByteArray(), POSTFIX_SEAT_AVAILABLE);
            PutOnPostfix(id, price.AsByteArray(), POSTFIX_PRICE);
            PutOnPostfix(id, cancelDate.AsByteArray(), POSTFIX_CANCEL_DATE);

            //Notify clients (for not web app users)
            Runtime.Notify("REG", driver, id, date, way, seatsCount, price, depositDriver, cancelDate);
            return true;
        }

        /// <summary>
        /// Cancel a trip with all deposit rules
        /// passengers will be notified about cancellation
        /// works with deposits on its rules
        /// </summary>
        /// <param name="id">id of the trip</param>
        /// <returns>true if trip cancelled, false if not</returns>
        public static bool CancelTrip(byte[] id)
        {
            //Check if the driver tries to cancel a trip
            byte[] driver = GetOnPostfix(id, POSTFIX_DRIVER);
            if (!Runtime.CheckWitness(driver))
                return false;
            Runtime.Notify("CancelTripDriver");

            //Check the status
            if (Storage.Get(Storage.CurrentContext, id) != REGISTERED)
                return false;
            Runtime.Notify("CancelTripRegistered");

            BigInteger cancelDate = GetOnPostfix(id, POSTFIX_CANCEL_DATE).AsBigInteger();
            //return deposit if cancel data hasnt come
            if (Now() < cancelDate)
            {
                GetDepositBack(id, driver);
                Runtime.Notify("BACKDEP", driver, id);
            }

            //Mark trip as cancelled, so passengers may now take deposit back
            //Also nobody will take this id again
            Storage.Put(Storage.CurrentContext, id, CANCELLED);

            //Cleaning up only if no passenger seats reserved
            CleanUp(id);
            Runtime.Notify("CANCELLED", id);
            return true;
        }

        /// <summary>
        /// Reserving a seat on a trip chosen for passengers scripthash
        /// Decreasing trip's seats available
        /// Making deposit
        /// </summary>
        /// <param name="id">id of a trip to reserve a seat on</param>
        /// <param name="passenger">scripthash of a passenger to own the seat</param>
        /// <returns>true if the seat was reserved, false in other case</returns>
        public static bool ReserveSeat(byte[] id, byte[] passenger)
        {
            //Check if the passenger tries to reserve seat
            if (!Runtime.CheckWitness(passenger))
                return false;
            Runtime.Notify("ReserveSeatWit");

            //Check if the trip on this id is registered (not done or cancelled)
            if (Storage.Get(Storage.CurrentContext, id) != REGISTERED)
                return false;
            Runtime.Notify("ReserveSeatReg");

            //Check the date
            if (GetOnPostfix(id, POSTFIX_DATE).AsBigInteger() < Now())
                return false;
            Runtime.Notify("ReserveDate");

            //Check if the passenger hasn't bought seat on this trip already
            string key = id.AsString() + passenger.AsString();
            if (Storage.Get(Storage.CurrentContext, key) == REGISTERED)
                return false;
            Runtime.Notify("Reserve2");

            //Check if there are seats available on a trip
            BigInteger seats = GetOnPostfix(id, POSTFIX_SEAT_AVAILABLE).AsBigInteger();
            if (seats < 1)
                return false;
            Runtime.Notify("NoSeats");

            //Calculating passenger's deposit
            BigInteger deposit = GetOnPostfix(id, POSTFIX_DEPOSIT).AsBigInteger();
            BigInteger seatNumber = GetOnPostfix(id, POSTFIX_SEAT_NUMBER).AsBigInteger();
            deposit = deposit / seatNumber;
            string keyDeposit = key + POSTFIX_DEPOSIT;
            //Trying to transfer deposit
            if (!Transfer(passenger, key.AsByteArray(), deposit))
                return false;

            //Put info that the passenger reserved a seat into storage
            Storage.Put(Storage.CurrentContext, key, REGISTERED);

            //decreasing available seat count on a trip
            seats = seats - 1;
            PutOnPostfix(id, seats.AsByteArray(), POSTFIX_SEAT_AVAILABLE);

            Runtime.Notify("SEAT", passenger, id);
            return true;
        }

        /// <summary>
        /// Cancels trip reservation
        /// Can only be called by the passenger
        /// works with deposits on its rules
        /// </summary>
        /// <param name="id">id of a trip</param>
        /// <param name="passenger">passengers scripthash</param>
        /// <returns>true if the seat was cancelled, false in any other case</returns>
        public static bool CancelSeat(byte[] id, byte[] passenger)
        {
            //Check if the passenger cancels the seat
            if (!Runtime.CheckWitness(passenger))
                return false;
            Runtime.Notify("CancelSeatWit");

            //Check if the trip exists
            if (Storage.Get(Storage.CurrentContext, id).Length == 0)
                return false;
            Runtime.Notify("TripExists");

            //Check if the passenger has the seat
            string key = id.AsString() + passenger.AsString();
            if (Storage.Get(Storage.CurrentContext, key) != REGISTERED)
                return false;
            Runtime.Notify("HasSeat");

            //Check if the trip is cancelled
            if (Storage.Get(Storage.CurrentContext, id) == CANCELLED)
            {
                //send passengers deposit
                GetDepositBack(key.AsByteArray(), passenger);
                //Check if drivers deposit not taken
                //It means that trip cancelled by the driver after the cancel date
                BigInteger deposit = GetOnPostfix(id, POSTFIX_DEPOSIT).AsBigInteger();
                if (deposit > 0)
                {
                    //calculate passenger part of deposit
                    BigInteger seatCount = GetOnPostfix(id, POSTFIX_SEAT_NUMBER).AsBigInteger();
                    BigInteger withdraw = deposit / seatCount;
                    //withdraw
                    deposit = deposit - withdraw;
                    PutOnPostfix(id, withdraw.AsByteArray(), POSTFIX_DEPOSIT);
                    //withdraw deposits part
                    BigInteger balance = Storage.Get(Storage.CurrentContext, passenger).AsBigInteger();
                    Storage.Put(Storage.CurrentContext, passenger, balance + withdraw);
                }
            }
            else
            {
                BigInteger cancelDate = GetOnPostfix(id, POSTFIX_CANCEL_DATE).AsBigInteger();
                //return deposit if cancel data hasnt come
                if (Now() < cancelDate)
                {
                    GetDepositBack(key.AsByteArray(), passenger);
                }
                //else sends deposit to driver
                else
                {
                    byte[] driver = GetOnPostfix(id, POSTFIX_DRIVER);
                    GetDepositBack(key.AsByteArray(), driver);
                }
            }

            //Delete reservation from storage
            Storage.Delete(Storage.CurrentContext, key);

            //Adding a new seat available
            IncreaseSeatCount(id);

            Runtime.Notify("SEATC", passenger, id);
            return true;
        }

        /// <summary>
        /// Gets deposit stored on a key back to user
        /// </summary>
        /// <param name="key">key of deposit (tripId or passenger + tripId)</param>
        /// <param name="user">passengersId</param>
        private static void GetDepositBack(byte[] key, byte[] user)
        {
            BigInteger deposit = GetOnPostfix(key, POSTFIX_DEPOSIT).AsBigInteger();
            BigInteger balance = Storage.Get(Storage.CurrentContext, user).AsBigInteger();
            Storage.Put(Storage.CurrentContext, user, balance + deposit);
            DeleteOnPostfix(key, POSTFIX_DEPOSIT);
            Runtime.Notify("DEPBACK", user, balance);
        }


        /// <summary>
        /// Sends token amount for a trip to the driver
        /// true if trip was paid, false in any other case
        /// </summary>
        /// <param name="id">id of a trip</param>
        /// <param name="passenger">passengers scripthash</param>
        /// <returns>true if the transfer was done, else in any other case</returns>
        public static bool PayForTrip(byte[] id, byte[] passenger)
        {
            //Check if the passenger calls the method
            if (!Runtime.CheckWitness(passenger))
                return false;
            Runtime.Notify("PassPay");

            //Check if the trip on this id is active
            if (Storage.Get(Storage.CurrentContext, id) == REGISTERED)
                return false;
            Runtime.Notify("PayREG");

            //Check if the passenger has the seat reserved
            string key = id.AsString() + passenger.AsString();
            if (Storage.Get(Storage.CurrentContext, key) != REGISTERED)
                return false;
            Runtime.Notify("PayReserved");

            //Getting the driver and price
            byte[] driver = GetOnPostfix(id, POSTFIX_DRIVER);
            BigInteger price = GetOnPostfix(id, POSTFIX_PRICE).AsBigInteger();

            //Trying to transfer tokens 
            if (!Transfer(passenger, driver, price))
                return false;

            GetDepositBack(key.AsByteArray(), passenger);
            Runtime.Notify("PAID", id, passenger);
            Storage.Delete(Storage.CurrentContext, key);

            //increasing seats count
            IncreaseSeatCount(id);
            CleanUp(id);
            return true;
        }

        /// <summary>
        /// Help function to increase available seat of a trip
        /// </summary>
        /// <param name="id">id of a trip</param>
        private static void IncreaseSeatCount(byte[] id)
        {
            BigInteger seatCount = GetOnPostfix(id, POSTFIX_SEAT_AVAILABLE).AsBigInteger();
            seatCount = seatCount + 1;
            PutOnPostfix(id, seatCount.AsByteArray(), POSTFIX_SEAT_AVAILABLE);
        }

        /// <summary>
        /// Creates a notification that user wants to refund tokens to NEO
        /// At this time refund can only be done manually by the contract owner
        /// </summary>
        /// <param name="user">refund requster's scripthash</param>
        /// <param name="value">value to refund</param>
        /// <returns></returns>
        public static bool RequestRefund(byte[] user, BigInteger value)
        {
            //Check if the user requested the refund
            if (!Runtime.CheckWitness(user))
                return false;
            Runtime.Notify("ReqWit");
            //Check if the user has amount he wants to refund
            BigInteger balance = BalanceOf(user);
            if (balance < value)
                return false;

            byte[] empty = { 35, 36 };
            if (!Transfer(user, empty, value))
                return false;

            DecSupply(value);
            Refund(user, value);
            return true;
        }

        /// <summary>
        /// Decreasing Supply
        /// </summary>
        /// <param name="value"></param>
        private static void DecSupply(BigInteger value)
        {
            BigInteger current = TotalSupply();
            BigInteger total = current - value;
            Storage.Put(Storage.CurrentContext, "totalSupply", total);
            Runtime.Notify("DECTOTAL", total);
        }

        /// <summary>
        /// Cleaning up storage that is used for canceled/done trips
        /// Everybody can call the method since it only suppose to 
        /// work on trips which marked done or cancelled
        /// Cleans only if no passenger has seats
        /// and return deposit back to driver (if it was cancelled by him out of time
        /// every passenger would already had their part of deposit so it would be 0.
        /// Also prevents an error when there are no passengers on a trip and driver cant
        /// get deposit back)
        /// </summary>
        /// <param name="id">id of a trip</param>
        /// <returns>true if info was deleted, false if there isn't done trip on this id</returns>
        public static bool CleanUp(byte[] id)
        {
            //Can only try to clean done or cancelled trips
            byte[] status = Storage.Get(Storage.CurrentContext, id);
            if (status != CANCELLED && status != DONE)
                return false;

            BigInteger seatsNumber = GetOnPostfix(id, POSTFIX_SEAT_NUMBER).AsBigInteger();
            BigInteger seatsAvailable = GetOnPostfix(id, POSTFIX_SEAT_AVAILABLE).AsBigInteger();
            //only when all seats are gone
            if (seatsAvailable != seatsNumber)
                return false;

            //return deposit to driver (0 seats bought)
            byte[] driver = GetOnPostfix(id, POSTFIX_DRIVER);
            GetDepositBack(id, driver);

            //Cleaning up
            DeleteOnPostfix(id, POSTFIX_DATE);
            DeleteOnPostfix(id, POSTFIX_DRIVER);
            DeleteOnPostfix(id, POSTFIX_WAY);
            DeleteOnPostfix(id, POSTFIX_SEAT_NUMBER);
            DeleteOnPostfix(id, POSTFIX_SEAT_AVAILABLE);
            DeleteOnPostfix(id, POSTFIX_PRICE);

            Storage.Put(Storage.CurrentContext, id, DONE);
            Runtime.Notify("DONE", id);
            return true;
        }

        /*NEP-5 INTERFACE IMPLEMENTATION
        TOTAL supply is equal to NEO's total supply and swap rate is always 1 to 1
        So it will never happen to get over the total supply*/
        /// <summary>
        /// Total supply of token minted
        /// </summary>
        /// <returns></returns>
        public static BigInteger TotalSupply()
        {
            return Storage.Get(Storage.CurrentContext, "totalSupply").AsBigInteger();
        }

        /// <summary>
        /// Awarding tokens to the senders scripthash
        /// 1 token for 1 NEO
        /// </summary>
        /// <returns>true if neo was attached, else false</returns>
        public static bool MintTokens()
        {
            //getting script hash who sent tokens
            byte[] id = GetSender();

            //check for references (if no outputs in invocation transaction's inputs)
            if (id.Length == 0)
                return false;

            //getting amount contributed in neo and set the balance
            BigInteger value = GetContributeValue();
            if (value == 0)
                return false;
            value = value / neo_decimals;
            BigInteger balance = BalanceOf(id);
            Storage.Put(Storage.CurrentContext, id, value + balance);
            AddSupply(value);
            return true;
        }

        /// <summary>
        /// Trying to transfer token from one scripthash to another
        /// </summary>
        /// <param name="from">scripthash to transfer from</param>
        /// <param name="to">scripthash to transfer to</param>
        /// <param name="value">amount of tokens to transfer</param>
        /// <returns>true if the transfer was successful, false if its failed</returns>
        public static bool Transfer(byte[] from, byte[] to, BigInteger value)
        {
            if (value <= 0) return false;
            //check scripthash
            if (!Runtime.CheckWitness(from))
                return false;
            if (from == to)
                return true;
            //check if the balance is enough to make a transfer
            BigInteger balance = BalanceOf(from);
            if (balance < value || balance.AsByteArray().Length == 0)
                return false;

            Runtime.Notify("Transfer validated");

            //decreasing balance 
            Storage.Put(Storage.CurrentContext, from, balance - value);
            balance = 0;
            balance = BalanceOf(to);
            //increasing balance and notify
            Storage.Put(Storage.CurrentContext, to, balance + value);
            Transferred(from, to, value);
            return true;
        }

        /// <summary>
        /// Getting balance of a user
        /// </summary>
        /// <param name="address">scripthash of a user</param>
        /// <returns>balance stored in token value</returns>
        public static BigInteger BalanceOf(byte[] address)
        {
            BigInteger currentBalance = Storage.Get(Storage.CurrentContext, address).AsBigInteger();
            Runtime.Notify("BALANCE", address, currentBalance);
            return currentBalance;
        }

        /// <summary>
        /// Inreasing the total supply to call when someone mint tokens
        /// </summary>
        /// <param name="amount">amount to increase on</param>
        private static void AddSupply(BigInteger amount)
        {
            BigInteger current = TotalSupply();
            BigInteger add = current + amount;
            Storage.Put(Storage.CurrentContext, "totalSupply", add);
            Runtime.Notify("ADDTOTAL", add);
        }

        /// <summary>
        /// Gets the amount of NEO attached to the contract with invocation
        /// </summary>
        /// <returns>the amount of neo</returns>
        private static BigInteger GetContributeValue()
        {
            //getting the contributing amount
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput[] outputs = tx.GetOutputs();
            BigInteger value = 0;
            foreach (TransactionOutput output in outputs)
            {
                Runtime.Notify("Contribution", value);
                if (output.ScriptHash == GetReceiver() && output.AssetId == neo_asset_id)
                {
                    value += (BigInteger)output.Value;
                }
            }
            return value;
        }

        /// <summary>
        /// contracts scripthash
        /// </summary>
        /// <returns></returns>
        private static byte[] GetReceiver()
        {
            return ExecutionEngine.ExecutingScriptHash;
        }

        /// <summary>
        /// Gets the scripthash of sender (only with NEO attached)
        /// YOUR FIRST INVOCATION'S TRANSACTION INPUT MUST HAVE OUTPUT 
        /// WITH NEO ATTACH TO THE CONTRACTS SCRIPT HASH
        /// IF YOU SEND GAS YOU WILL LOSE YOUR FUNDS
        /// </summary>
        /// <returns>script hash of sender if neo was attached else empty byte array</returns>
        private static byte[] GetSender()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput[] reference = tx.GetReferences();
            foreach (TransactionOutput output in reference)
            {
                if (output.AssetId == neo_asset_id)
                    return output.ScriptHash;
            }
            return new byte[0];
        }
    }
}
