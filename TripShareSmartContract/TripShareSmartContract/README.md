# TripShares
# Introduction
I assume everybody had a situation when someone cancels his trip just before it was
upposed to start. Or someone who reserved a seat cancels it at the last moment.
In any case it costs a huge amount of time to find a new trip and the one who broke
your plans done lose anything.
So how confident can you be in your future plans?

In this smartcontract for the trip registration you must make a TRS deposit which 
is viewable for any user. You also enter the date when trip is planned to start and the cancel time
before which a trip can be canceled for free. Since its blockchain, this data
is viewable for everyone. SC won't allow you to take your deposit back when the trip
is not done and the cancel date has passed. Trip is marked DONE only when 
every passenger pays for it (confirms that its done, should be a real-life meating
like a simple checkout) Then you get your deposit back. The same scheme works for seat reservation. 
Passenger makes deposit which equal to drivers divided by seats count. Once trip is paid, 
the deposit gets back to the passenger.
Blockchain provides confirmation of deposits, time limits and etc. Everything becomes transparent.
If the driver is confident in his plans he should make a bigger deposit, so it will be 
bigger for passengers too and then he probably can rely on them. Also a passenger 
which is confident in his plans will choose a driver with a bigger deposit, because 
there are less chances to fail with a driver who made a huge deposit.
However, nobody is interested in not confirmation the trip, because not confirming means 
losing deposit. (Anyway these confirmations should be done face to face since the trip
allows it).

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

Then at the top you can register a trip (will be commited to db and blockchain)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegRegTrip.png)

Then you can watch it in your trips section (won't watch in blockchain)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegYourTrips.png)

You can click view on any trip then you can cancel a trip
![alt text](https://github.com/xtolya/TripShares/blob/master/images/TripCancel.png)

By searching trips on the main page (currently only through db) you will get to
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegAfterSearchReserve.png)

There you can reserve a seat
![alt text](https://github.com/xtolya/TripShares/blob/master/images/TripReservedDb.png)

Theres a section at the top where you can watch your seat (search only in db)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/RegYourSeatsView.png)

Again you can click on it and cancel the seat.

You can request a refund (transaction on blockchain for method RequestRefund)
![alt text](https://github.com/xtolya/TripShares/blob/master/images/NoRegRefund.png)

Without registration:
You can search on the main page of the database
