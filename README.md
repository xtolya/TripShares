# TripShares
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
