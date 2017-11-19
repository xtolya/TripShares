import {
    getAccountFromWIFKey, generatePrivateKey,
    getStorage, getTransactionHistory, doSendAsset,
    getWIFFromPrivateKey, getBalance, verifyAddress,
    queryRPC, serializeTransaction, create, signTransaction,
    getScriptHashFromAddress, ASSETS
} from 'neon-js'

const MAIN_ADDRESS = "AXWWMKZaL7BYxSonrdTfAjpY3wnH1F2g4Z"
const TEST_ADDRESS = "AYRBCHPqusVjP37cU2EUfEUrzW83NFWwqj"
const CONTRACT_HASH = "95276724df6b95f745e77b6061d7c158c168fa25"

const MAIN_NET = "MainNet"
const TEST_NET = "TestNet"

const ASSET_NEO = "NEO"
const ASSET_GAS = "GAS"



function doInvoke(net, wif, scriptHash, gasCost, ...args) {
            const account = getAccountFromWIFKey(wif)
    
            getBalance(net, account.address)
                .then((balances) => {
                    const intents = [
                        { assetId: ASSETS['NEO'], value: 2, scriptHash }
                    ]
                    
                    const invoke = { operation: "mintTokens", scriptHash }
                    const unsignedTx = create.invocation(account.publicKeyEncoded, balances, intents, invoke, gasCost, { version: 1 })
                    const signedTx = signTransaction(unsignedTx, account.privateKey)

                    const hexTx = serializeTransaction(signedTx)
                    queryRPC(net, 'sendrawtransaction', [hexTx])
                        .then(res => console.log(res))
                        .catch(err => console.log(err))
                })
                .catch(err => console.log(err))
        }

doInvoke(TEST_NET, "YOUR WIF", CONTRACT_HASH, 3)
