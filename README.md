# TripShares
# Introduction
I assume everybody had a situation when someone cancels his trip just before it was
upposed to start. Or someone who reserved a seat cancels it at the last moment.
In any case it costs a huge amount of time to find a new trip and the one who broke
your plans done lose anything.
So how confident can you be in your future plans?

Current version works with wallets generated there and takes some test-GAS for transactions. (Will be fixed after I figure out neon-js GAS calculations)
To communicate with neo blockchain I used neon-js and node.js. Web app is on asp.net core 2.0 (mvc)
The main function of dApp is to provide some sort of insurance in planning of people's trips with transparency of SC. Web app is just an interface to make searching through SC and use its functions to register trip and etc. Logic is very simple: 
1) a driver registers a trip with a deposit (locked is SC) and a cancel date
2) a passenger buys a seat on this trip making a deposit of a driver divided by seats count (also locked)
3) if driver/passenger cancels a trip/seat before cancel date it is done with returns of deposit
4) if a passenger cancels his seat after cancel date his deposit goes to the driver
5) if a driver cancels the trip after cancel date his deposit spreads for the passengers
6) when the trip is done everyone gets his deposit back (after passengers pay for seats)
To summarize, this dApp is made for people which are confident in their plans. Drivers can make a higher deposit to be sure a trip will happen, passengers can search for higher deposit trips. So everyone will be happy in any case. With SC you don't need a reliable middleman to hold deposit and solve disputes.

Contract includes its own token which is equal to NEO because it has 1 to 100 swap rate
You can buy a token whenever you want
You can request a refund whenever you want and there isn't any time limitations such as ico
sually has.

Overall, this dApp is made not for speculative purposes as it is usually done in many projects
somehow associated with blockchain technology itself!

# Usage

SC_USAGE  -  DOCS TO USE WITH NEO-GUI
DOCS  - simple docs for web App

INDEX.JS HAS A FUNCTION TO CALL CONTRACT (CHANGE PARAMETERS, CURRENTLY IT FOR THE TOKEN MINTING)

Interface of interacting with blockchain is already implemented, but some things like Search By Id via get_storage are still to do.

Through web abb you can mint tokens, register trip, cancel, add and buy seat

For registered users (no confirmations, BETTER GENERATE WALLET, SEE INSTRUCTIONS BELOW):
Once you log in you can see your balance at the right top corner (link to wallet)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegBalance.png)

By clicking on it you will come to wallet generation field
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegGenerate.png)

Once your wallet generated you can see amounts you have there and use withdraw and mint tokens
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegWalletGenerated.png)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegWithdraw.png)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegMint.png)

Then at the top you can register a trip (TO GO ON BC REQUIRED 3 GAS AND TRS MORE THAN DEPOSIT ON THE BALANCE)
(will be commited to db and blockchain)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegRegTrip.png)

Then you can watch it in your trips section (won't watch in blockchain)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegYourTrips.png)

You can click view on any trip then you can cancel a trip
![alt text](https://github.com/xtolya/TripShares/blob/master/images/TripCancel.png)

By searching trips on the main page (currently only through db) you will get to
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegAfterSearchReserve.png)

There you can reserve a seat (required TSR to deposit and gas to commit on bc)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/TripReservedDb.png)

Theres a section at the top where you can watch your seat (search only in db)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegYourSeatsView.png)

Again you can click on it and cancel the seat.

You can request a refund (transaction on blockchain for method RequestRefund, some gas required)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/NoRegRefund.png)

Without registration:
You can search on the main page of the database
