# Bitprim Insight <a target="_blank" href="http://semver.org">![Version][badge.version]</a> <a target="_blank" href="https://travis-ci.org/bitprim/bitprim-insight">![Travis status][badge.Travis]</a> [![Appveyor Status](https://ci.appveyor.com/api/projects/status/github/bitprim/bitprim-insight?svg=true&branch=master)](https://ci.appveyor.com/project/bitprim/bitprim-insight) <a target="_blank" href="https://gitter.im/bitprim/Lobby">![Gitter Chat][badge.Gitter]</a>

> Multi-Cryptocurrency _Rest_ API.

*Bitprim Insight* is a REST API written in _C#_ with .NET Core 2.x which exposes methods matching the insight API interface

Bitprim Insight supports the following cryptocurrencies:
  * [Bitcoin Cash](https://www.bitcoincash.org/)
  * [Bitcoin](https://bitcoin.org/)
  * [Litecoin](https://litecoin.org/) (coming soon)

## Installation Requirements

- 64-bit machine.
- [Conan](https://www.conan.io/) package manager, version 1.1.0 or newer. See [Conan Installation](http://docs.conan.io/en/latest/installation.html#install-with-pip-recommended).
- [.NET Core 2.0 SDK](https://www.microsoft.com/net/download/)


In case there are no pre-built binaries for your platform, conan will automatically try to build from source code. In such a scenario, the following requirements must be added to the previous ones:

- C++11 Conforming Compiler.
- [CMake](https://cmake.org/) building tool, version 3.4 or newer.


## Building Procedure

The *Bitprim* libraries can be installed using conan (see below) on Linux, macOS, FreeBSD, Windows and others. These binaries are pre-built for the most usual operating system/compiler combinations and are downloaded from an online repository. If there are no pre-built binaries for your platform, conan will attempt to build from source during the installation.

1. Build 

In the project folder run:

For Bitcoin Cash

```
dotnet build /property:Platform=x64 /p:BCH=true -c Release -f netcoreapp2.0 -v normal
```

For Bitcoin

```
dotnet build /property:Platform=x64 /p:BTC=true -c Release -f netcoreapp2.0 -v normal
```

2. Run

```
dotnet bin/x64/Release/netcoreapp2.0/bitprim.insight.dll --server.port=3000 --server.address=0.0.0.0
```

or you can publish the app and run over the published folder 

```
dotnet publish /property:Platform=x64 /p:BTC=true -c Release -f netcoreapp2.0 -v normal -o published
```

```
dotnet bin/x64/Release/netcoreapp2.0/published/bitprim.insight.dll --server.port=3000 --server.address=0.0.0.0
```

### Command line arguments

**--server.port**: Defines the listening TCP port. 

*Default value:1549*

**--server.address**: Defines the listening IP address.

*Default value:localhost*

## Configuration Options


You need to create an appsettings.json file in the build directory to run the application. You can use appsettings.example.json as a starting point.

Eg.

```
{
  "ApiPrefix" : "api",
  "AcceptStaleRequests" : true,
  "AllowedOrigins": "http://localhost:1549",
  "Connections": 8,
  "DateInputFormat": "yyyy-MM-dd",
  "EstimateFeeDefault": "0.00001000",
  "ForwardUrl" : "http://localhost:1234",
  "InitializeNode" : true,
  "LongResponseCacheDurationInSeconds": 86400,
  "MaxBlockSummarySize": 500,
  "MaxCacheSize": 50000,
  "MaxSocketPublishRetries": 3,
  "NodeConfigFile": "config.cfg",
  "NodeType": "bitprim node",
  "PoolsFile":  "pools.json", 
  "ProtocolVersion": "70015",
  "Proxy": "",
  "RelayFee": "0.00001",
  "ShortResponseCacheDurationInSeconds": 30,
  "SocketPublishRetryIntervalInSeconds": 1,
  "TimeOffset": "0",
  "TransactionsByAddressPageSize": 10,
  "Version": "170000",
  "HttpClientTimeoutInSeconds" : 5,
  "WebsocketForwarderClientRetryDelay": 10,
  "Serilog":
  {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel":
    {
      "Default": "Information",
      "Override":
      {
        "Microsoft": "Warning"
      }
    },
    "WriteTo":
    [
      {
        "Name": "Console",
        "Args":
        {
          "outputTemplate" : "[{Timestamp:yyyy-MM-dd HH:mm:ss} {TimeZone}] {Level:u3} {SourceIP} {RequestId} {HttpMethod} {RequestPath} {HttpProtocol} {HttpResponseStatusCode} {HttpResponseLength} {ElapsedMs} {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args":
        {
           "path": "log-.txt",
           "rollingInterval": "Day",
           "fileSizeLimitBytes": null,
           "retainedFileCountLimit" : 5, 
           "outputTemplate" : "[{Timestamp:yyyy-MM-dd HH:mm:ss} {TimeZone}] {Level:u3} {SourceIP} {RequestId} {HttpMethod} {RequestPath} {HttpProtocol} {HttpResponseStatusCode} {HttpResponseLength} {ElapsedMs} {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": ["FromLogContext"]
  }
}
```

All the settings can be passed as command line arguments prefixing the name with '--'

Eg.

```
dotnet bin/x64/Release/netcoreapp2.0/published/bitprim.insight.dll --server.port=3000 --server.address=0.0.0.0 --MaxBlockSummarySize=1000
```


The application has two different operation modes. As a **Full Node** or a **Forwarder**.

In **Full Node** mode, the application starts a full Bitprim node, generating a copy of the blockchain.

In **Forwarder** mode, the application only relays the request to a **Full Node** application.

### Settings

**ApiPrefix**: Defines the name of the url segment where you expose the api methods.
```
http://blockdozer.com/[ApiPrefix]/blocks/
```
*Default value:api*

**AcceptStaleRequests**: Allows the API to respond to requests even if the chain is stale (the local copy of the blockchain isn't fully synchronized with the network). 
*Default value:true*

**AllowedOrigins**: Configure the allowed CORS origins. For multiple origins, separate them with semicolon (;).
*Default value:**

**Connections**: Configures the value returned in the *connection* element of the /status request. 
*Default value:8*

**DateInputFormat**: Defines the date format used by /blocks and other requests that require dates.
*Default value:yyyy-MM-dd*

**EstimateFeeDefault**: Sets the value returned by /utils/estimatefee.
*Default value:0.00001000*

**ForwardUrl**: When you use the application in **Forwarder** mode, this settings defines the Full Node's URL. 
*Default value:""*

**InitializeNode**: This setting defines the node's working mode: *True* for Full Node, *False* for Forwarder Node.
*Default value:true*

**LongResponseCacheDurationInSeconds**: Duration of the long cache responses. Used to cache results for the following requests: 
* /rawblock 
* /rawtx
*Default value:86400* 

**MaxBlockSummarySize**: Defines the max limit of the /blocks method.
*Default value:500* 

**MaxCacheSize**: Configures the cache size limit; this is an adimensional value, because measuring object size is not trivial. The size for each cache entry is also adimensional and arbitrarily set by the user. The total size sum will never exceed this value.
*Default value:50000*

**MaxSocketPublishRetries**: Defines how many times the server retries when publishing websocket messages before throwing an exception.  
*Default value:3*

**NodeConfigFile**: Node config file path; can be absolute, or relative to the project directory. Only use in **Full Node** mode.
*Default value:""*

**NodeType**: The value returned in *type* element by the /sync method.
*Default value:bitprim node*

**PoolsFile**: Path to the json file with the mining pool information.
*Default value:pools.json*

**ProtocolVersion**: The value returned in *protocolversion* element by the /status method.
*Default value:70015*

**Proxy**: The value returned in *proxy* element by the /status method.
*Default value:""*

**RelayFee**: The value returned in *relayfee* element by the /status method.
*Default value:0.00001*

**ShortResponseCacheDurationInSeconds**: Duration of the short cache responses. Used to cache results for the following requests:
* /txs
* /addrs/{paymentAddresses}/txs
* /addrs/txs
* /tx/{hash}
* /txs
* /rawblock-index/{height}
* /blocks
* /block/{hash}
* /block-index/{height}
* /sync
* /status
* /addr/{paymentAddress}/balance
* /addr/{paymentAddress}/totalReceived
* /addr/{paymentAddress}/totalSent
* /addr/{paymentAddress}/unconfirmedBalance
* /addr/{paymentAddress}/utxo
* /addrs/{paymentAddresses}/utxo
* /addrs/utxo
* /addr/{paymentAddress}
* /peer
* /version
*Default value:30*

**SocketPublishRetryIntervalInSeconds**: Delay in seconds between retries for "Publish" websocket messages.
*Default value:1*

**MaxSocketPublishRetries**: Maximum number of retries for a "Publish" websocket message
*Default value:3*

**TimeOffset**: The value returned in *timeoffset* element by the /status method.
*Default value:0*

**TransactionsByAddressPageSize**: The max page limit used by the /txs method. 
*Default value:10*

**Version**: The value returned in *version* element by the /status method. 
*Default value:""*

**HttpClientTimeoutInSeconds**: Defines HttpClient timeout. Used in forwarder mode. 
*Default value:5*

**WebsocketForwarderClientRetryDelay**: The delay in seconds beetween retries when the websocket connection to the fullnode fails.
*Default value:10*

**Serilog**: The Serilog configuration. For more detailed documentation, check https://github.com/serilog/serilog/wiki/Getting-Started


## API HTTP Endpoints

### Block

    /api/block/[:hash]
    /api/block/00000000a967199a2fad0877433c93df785a8d8ce062e5f9b451cd1397bdbf62

### Block Index

Get block hash by height

    /api/block-index/[:height]
    /api/block-index/0

This would return:

    {
        "blockHash":"000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f"
    }

which is the hash of the Genesis block (0 height)

### Raw Block

    /api/rawblock/[:blockHash]
    /api/rawblock/[:blockHeight]

This would return:

    {
        "rawblock":"blockhexstring..."
    }    

### Block Summaries

Get block summaries by date:

    /api/blocks?limit=3&blockDate=2016-04-22

Example response:

    {
        "blocks": [
            {
                "height": 408495,
                "size": 989237,
                "hash": "00000000000000000108a1f4d4db839702d72f16561b1154600a26c453ecb378",
                "time": 1461360083,
                "txlength": 1695,
                "poolInfo": {
                    "poolName": "BTCC Pool",
                    "url": "https://pool.btcc.com/"
                }
            }
        ],
        "length": 1,
        "pagination": {
            "next": "2016-04-23",
            "prev": "2016-04-21",
            "currentTs": 1461369599,
            "current": "2016-04-22",
            "isToday": true,
            "more": true,
            "moreTs": 1461369600
        }
    }

### Transaction

    /api/tx/[:txid]
    /api/tx/525de308971eabd941b139f46c7198b5af9479325c2395db7f2fb5ae8562556c
    /api/rawtx/[:rawid]
    /api/rawtx/525de308971eabd941b139f46c7198b5af9479325c2395db7f2fb5ae8562556c

### Address

    /api/addr/[:addr][?noTxList=1][&from=&to=]
    /api/addr/mmvP3mTe53qxHdPqXEvdu8WdC7GfQ2vmx5?noTxList=1
    /api/addr/mmvP3mTe53qxHdPqXEvdu8WdC7GfQ2vmx5?from=1000&to=2000

* noTxList: 1 to include transactions, 0 otherwise.    

### Address Properties

    /api/addr/[:addr]/balance
    /api/addr/[:addr]/totalReceived
    /api/addr/[:addr]/totalSent
    /api/addr/[:addr]/unconfirmedBalance

The response contains the value in Satoshis.

### Unspent Outputs

            /api/addr/[:addr]/utxo

Sample return:

    [
        {
            "address":"mo9ncXisMeAoXwqcV5EWuyncbmCcQN4rVs",
            "txid":"d5f8a96faccf79d4c087fa217627bb1120e83f8ea1a7d84b1de4277ead9bbac1",
            "vout":0,
            "scriptPubKey":"76a91453c0307d6851aa0ce7825ba883c6bd9ad242b48688ac",
            "amount":0.000006,
            "satoshis":600,
            "confirmations":0,
            "ts":1461349425
        },
        {
            "address": "mo9ncXisMeAoXwqcV5EWuyncbmCcQN4rVs",
            "txid": "bc9df3b92120feaee4edc80963d8ed59d6a78ea0defef3ec3cb374f2015bfc6e",
            "vout": 1,
            "scriptPubKey": "76a91453c0307d6851aa0ce7825ba883c6bd9ad242b48688ac",
            "amount": 0.12345678,
            "satoshis: 12345678,
            "confirmations": 1,
            "height": 300001
        }
    ]

### Unspent Outputs for Multiple Addresses

GET method:

    /api/addrs/[:addrs]/utxo
    /api/addrs/2NF2baYuJAkCKo5onjUKEPdARQkZ6SYyKd5,2NAre8sX2povnjy4aeiHKeEh97Qhn97tB1f/utxo

POST method:

    /api/addrs/utxo

POST params:

    addrs: 2NF2baYuJAkCKo5onjUKEPdARQkZ6SYyKd5,2NAre8sX2povnjy4aeiHKeEh97Qhn97tB1f

### Transactions by Block

    /api/txs/?block=HASH
    /api/txs/?block=00000000fa6cf7367e50ad14eb0ca4737131f256fc4c5841fd3c3f140140e6b6

### Transactions by Address

    /api/txs/?address=ADDR
    /api/txs/?address=mmhmMNfBiZZ37g1tgg2t8DDbNoEdqKVxAL

### Transactions for Multiple Addresses

GET method:

    /api/addrs/[:addrs]/txs[?from=&to=]
    /api/addrs/2NF2baYuJAkCKo5onjUKEPdARQkZ6SYyKd5,2NAre8sX2povnjy4aeiHKeEh97Qhn97tB1f/txs?from=0&to=20

POST method:

    /api/addrs/txs

POST params:

    addrs: 2NF2baYuJAkCKo5onjUKEPdARQkZ6SYyKd5,2NAre8sX2povnjy4aeiHKeEh97Qhn97tB1f
    from (optional): 0
    to (optional): 20
    noAsm (optional): 1 (will omit script asm from results)
    noScriptSig (optional): 1 (will omit the scriptSig from all inputs)
    noSpent (option): 1 (will omit spent information per output)

Sample output:

    {
        totalItems: 100,
        from: 0,
        to: 20,
        items: [
            {
                txid: '3e81723d069b12983b2ef694c9782d32fca26cc978de744acbc32c3d3496e915',
               version: 1,
               locktime: 0,
               vin: [Object],
               vout: [Object],
               blockhash: '00000000011a135e5277f5493c52c66829792392632b8b65429cf07ad3c47a6c',
               confirmations: 109367,
               time: 1393659685,
               blocktime: 1393659685,
               valueOut: 0.3453,
               size: 225,
               firstSeenTs: undefined,
               valueIn: 0.3454,
               fees: 0.0001
            },
            { ... },
            { ... },
              ...
            { ... }
        ]
    }

Note: if pagination params are not specified, the result is an array of transactions.

### Transaction Broadcasting

POST method:

    /api/tx/send

POST params:

    {
        "rawtx": "signed transaction as hex string"
    }

    eg
    
    {
        "rawtx": "01000000017b1eabe0209b1fe794124575ef807057c77ada2138ae4fa8d6c4de0398a14f3f00000000494830450221008949f0cb400094ad2b5eb399d59d01c14d73d8fe6e96df1a7150deb388ab8935022079656090d7f6bac4c9a94e0aad311a4268e082a725f8aeae0573fb12ff866a5f01ffffffff01f0ca052a010000001976a914cbc20a7664f2f69e5355aa427045bc15e7c6c77288ac00000000"
    }

POST response:

    {
        txid: [:txid]
    }
    
    eg
    
    {
        txid: "c7736a0a0046d5a8cc61c8c3c2821d4d7517f5de2bc66a966011aaa79965ffba"
    }

### Historic Blockchain Data Sync Status

    /api/sync

### Live Network P2P Data Sync Status

    /api/peer

### Status of the Bitcoin Network

    /api/status?q=method

Where "method" can be:

*   getInfo
*   getDifficulty
*   getBestBlockHash
*   getLastBlockHash

### Utility Methods

    /api/utils/estimatefee[?nbBlocks=2]


### Healthcheck Methods

    * /api/healthcheck[?minimumSync]
    * /forwarderhealth (only for Forwarder mode)
    
## Web Socket API

The web socket API is served using [standard, pure web sockets](https://developer.mozilla.org/en-US/docs/Web/API/WebSocket). The first step is connecting to `_domain_/wss`; once connection is established, specific messages need to be sent to the server in order to subscribe to the different events (see each event entry). To simplify event subscription, the `ScopedPureWebSocket` class can be used.

The following are the events published by insight:

`tx`: new transaction received from network. To receive this event, after connecting to the websocket endpoint, send the `SubscribeToTxs` plain text message.

Sample output:

    {
        "eventname": 'tx',
        "txid":"00c1b1acb310b87085c7deaaeba478cef5dc9519fab87a4d943ecbb39bd5b053",
        "valueOut: "0.564BCH",
        "addresses": ["17orHVW3pF86VQqraegS6PCjk579EasXYg", "12vJYnCm5QgY4vntutTG95SkiLfXhgbiAc"]
        ...
    }

Output fields: `txid` is the transaction hash, `valueOut` is the sum of all the transaction outputs, and `addresses` contains a list of the addresses involved in the transaction, considering inputs and outputs.

`block`: new block received from network. After connecting to the webscoket endpoint, send the `SubscribeToBlocks` plain text message to begin receiving these notifications.

Sample output:

    {
        "eventname":"block"
    }

`<addresstx>`: new transaction received on a specific address. To subscribe to a specific address, send a message with that address in legacy format in plain text.

Sample output:

    {
        eventname: 'addresstx',
        txid: "00c1b1acb310b87085c7deaaeba478cef5dc9519fab87a4d943ecbb39bd5b053"
    }

### Example Usage

The following html page connects to the web socket insight API and listens for new transactions.

    <html>
        <body>
            <script>
                var socket = new WebSocket('http://domain.com/ws');
                socket.onopen = function() {
                    socket.send("SubscribeToTxs"); 
                };
                socket.onmessage = function(msg) {
                    var messageData = JSON.parse(msg.data);
                    if(messageData.eventname != undefined && messageData.eventname == 'tx')
                    {
                        console.log("Transaction received! txid: " + messageData.txid);
                    }
                };
            </script>
        </body>
    </html>


<!-- Links -->
[badge.Appveyor]: https://ci.appveyor.com/api/projects/status/github/bitprim/bitprim-insight?svg=true&branch=master
[badge.Gitter]: https://img.shields.io/badge/gitter-join%20chat-blue.svg
[badge.Travis]: https://travis-ci.org/bitprim/bitprim-insight.svg?branch=master
[badge.version]: https://badge.fury.io/gh/bitprim%2Fbitprim-insight.svg
