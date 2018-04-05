using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Bitprim;
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class ChainController : Controller
    {
        private Chain chain_;
        private Executor nodeExecutor_;
        private readonly NodeConfig config_;
        private const string GET_BEST_BLOCK_HASH = "getBestBlockHash";
        private const string GET_LAST_BLOCK_HASH = "getLastBlockHash";
        private const string GET_DIFFICULTY = "getDifficulty";

        public ChainController(IOptions<NodeConfig> config, Executor executor)
        {
            config_ = config.Value;
            nodeExecutor_ = executor;
            chain_ = executor.Chain;
        }

        [HttpGet("/api/sync")]
        public ActionResult GetSyncStatus()
        {
            //TODO Try a more reliable way to know network max height (i.e. ask another node, or some service)
            ApiCallResult<UInt64> getLastHeightResult = chain_.GetLastHeight();
            Utils.CheckBitprimApiErrorCode(getLastHeightResult.ErrorCode, "GetLastHeight() failed");
            UInt64 currentHeight = getLastHeightResult.Result;
            bool synced = currentHeight >= config_.BlockchainHeight;
            dynamic syncStatus = new ExpandoObject();
            syncStatus.status = synced? "finished" : "synchronizing";
            syncStatus.blockChainHeight = config_.BlockchainHeight;
            syncStatus.syncPercentage = Math.Min((double)currentHeight / (double)config_.BlockchainHeight * 100.0, 100);
            syncStatus.error = null;
            syncStatus.type = config_.NodeType;
            return Json(syncStatus);   
        }

        [HttpGet("/api/status")]
        public ActionResult GetStatus(string method)
        {
            if(method == GET_DIFFICULTY)
            {
                return GetDifficulty();
            }
            else if(method == GET_BEST_BLOCK_HASH)
            {
                return GetBestBlockHash();
            }
            else if(method == GET_LAST_BLOCK_HASH)
            {
                return GetLastBlockHash();
            }
            else
            {
                return GetInfo();
            }   
        }

        [HttpGet("/api/utils/estimatefee")]
        public ActionResult GetEstimateFee([FromQuery] int? nbBlocks = 2)
        {
            var estimateFee = new ExpandoObject() as IDictionary<string, Object>;
            //TODO Check which algorithm to use (see bitcoin-abc's median, at src/policy/fees.cpp for an example)
            estimateFee.Add(nbBlocks.ToString(), 1.0);
            return Json(estimateFee);   
        }

        [HttpGet("/api/currency")]
        public ActionResult GetCurrency()
        {
            //TODO Implement in node-cint? Or here? Ask
            return Json(new{
                status = 200,
                data = new
                {
                    bistamp = 8025.3f
                }
            });
        }

        private ActionResult GetDifficulty()
        {
            Tuple<Block, UInt64> topBlock = GetLastBlock();
            return Json
            (
                new
                {
                    difficulty = Utils.BitsToDifficulty(topBlock.Item1.Header.Bits)
                }
            );
        }

        private ActionResult GetBestBlockHash()
        {
            Tuple<Block, UInt64> topBlock = GetLastBlock();
            return Json
            (
                new
                {
                    bestblockhash = Binary.ByteArrayToHexString(topBlock.Item1.Hash)
                }
            );
        }

        private ActionResult GetLastBlockHash()
        {
            Tuple<Block, UInt64> topBlock = GetLastBlock();
            string hashHexString = Binary.ByteArrayToHexString(topBlock.Item1.Hash); 
            return Json
            (
                new
                {
                    syncTipHash = hashHexString,
                    lastblockhash = hashHexString
                }
            );
        }

        private ActionResult GetInfo()
        {
            Tuple<Block, UInt64> block = GetLastBlock();
            return Json
            (
                new
                {
                    info = new 
                    {
                        //version = 120100, //TODO
                        //protocolversion = 70012, //TODO
                        blocks = block.Item2,
                        //timeoffset = 0, //TODO
                        //connections = 8, //TODO
                        //proxy = "", //TODO
                        difficulty = Utils.BitsToDifficulty(block.Item1.Header.Bits),
                        testnet = nodeExecutor_.UseTestnetRules,
                        //relayfee = 0.00001, //TODO
                        //errors = "Warning: unknown new rules activated (versionbit 28)", //TODO
                        network = nodeExecutor_.NetworkType.ToString()
                    }
                }
            );
        }

        private Tuple<Block, UInt64> GetLastBlock()
        {
            ApiCallResult<UInt64> getLastHeightResult = chain_.GetLastHeight();
            Utils.CheckBitprimApiErrorCode(getLastHeightResult.ErrorCode, "GetLastHeight() failed");
            UInt64 currentHeight = getLastHeightResult.Result;
            DisposableApiCallResult<GetBlockDataResult<Block>> getBlockResult = chain_.GetBlockByHeight(currentHeight);
            Utils.CheckBitprimApiErrorCode(getBlockResult.ErrorCode, "GetBlockByHeight(" + currentHeight + ") failed");
            Block topBlock = getBlockResult.Result.BlockData;
            return new Tuple<Block, UInt64>(topBlock, currentHeight);
        }

    }
}