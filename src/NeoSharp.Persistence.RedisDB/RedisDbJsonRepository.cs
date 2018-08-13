using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.Persistence.RedisDB.Helpers;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbJsonRepository : IRepository
    {
        #region Private Fields

        private readonly IRedisDbContext _redisDbContext;

        #endregion

        #region Constructor

        public RedisDbJsonRepository(
            IRedisDbContext redisDbContext)
        {
            _redisDbContext = redisDbContext ?? throw new ArgumentNullException(nameof(redisDbContext));
        }

        #endregion

        #region IRepository System Members

        public Task SetTotalBlockHeight(uint height)
        {
            throw new NotImplementedException();
            // TODO: redis logic
            //_redis.Database.AddToIndex(RedisIndex.BlockHeight, height);
        }

        public Task<uint> GetTotalBlockHeight()
        {
            //Use the block height secondary index to tell us what our block height is
            //return _redis.Database.GetIndexLength(RedisIndex.BlockHeight);

            // TODO: redis logic
            throw new NotImplementedException();
        }

        public Task<uint> GetTotalBlockHeaderHeight()
        {
            throw new NotImplementedException();
        }

        public Task SetTotalBlockHeaderHeight(uint height)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetVersion()
        {
            throw new NotImplementedException();
        }

        public Task SetVersion(string version)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRepository Data Members

        public async Task AddBlockHeader(BlockHeader blockHeader)
        {
            // TODO [AboimPinto]: This serialization cannot be mocked, therefore cannot be tested properly.
            var blockHeaderJson = JsonConvert.SerializeObject(blockHeader);
            await _redisDbContext.Set(blockHeader.Hash.BuildDataBlockKey(), blockHeaderJson);

            await _redisDbContext.AddToIndex(RedisIndex.BlockTimestamp, blockHeader.Hash, blockHeader.Timestamp);
            await _redisDbContext.AddToIndex(RedisIndex.BlockHeight, blockHeader.Hash, blockHeader.Index);
        }

        public async Task AddTransaction(Transaction transaction)
        {
            var transactionJson = JsonConvert.SerializeObject(transaction);
            await _redisDbContext.Set(transaction.Hash.BuildDataTransactionKey(), transactionJson);
        }

        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            return await _redisDbContext.GetFromHashIndex(RedisIndex.BlockHeight, height);
        }

        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var blockHeaderRedisValue = await _redisDbContext.Get(hash.BuildDataBlockKey());

            return JsonConvert.DeserializeObject<BlockHeader>(blockHeaderRedisValue);

        }

        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            var transactionRedisValue = await _redisDbContext.Get(hash.BuildDataTransactionKey());

            return JsonConvert.DeserializeObject<Transaction>(transactionRedisValue);
        }

        #endregion

        #region IRepository State Members

        public Task<Account> GetAccount(UInt160 hash)
        {
            throw new NotImplementedException();
        }

        public Task AddAccount(Account acct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAccount(UInt160 hash)
        {
            throw new NotImplementedException();
        }

        public Task<CoinState[]> GetCoinStates(UInt256 txHash)
        {
            throw new NotImplementedException();
        }

        public Task AddCoinStates(UInt256 txHash, CoinState[] coinStates)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCoinStates(UInt256 txHash)
        {
            throw new NotImplementedException();
        }

        public Task<Validator> GetValidator(ECPoint publicKey)
        {
            throw new NotImplementedException();
        }

        public Task AddValidator(Validator validator)
        {
            throw new NotImplementedException();
        }

        public Task DeleteValidator(ECPoint point)
        {
            throw new NotImplementedException();
        }

        public Task<Asset> GetAsset(UInt256 assetId)
        {
            throw new NotImplementedException();
        }

        public Task AddAsset(Asset asset)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsset(UInt256 assetId)
        {
            throw new NotImplementedException();
        }

        public Task<Contract> GetContract(UInt160 contractHash)
        {
            throw new NotImplementedException();
        }

        public Task AddContract(Contract contract)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContract(UInt160 contractHash)
        {
            throw new NotImplementedException();
        }

        public Task<StorageValue> GetStorage(StorageKey key)
        {
            throw new NotImplementedException();
        }

        public Task AddStorage(StorageKey key, StorageValue val)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStorage(StorageKey key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRepository Index Members

        public async Task<uint> GetIndexHeight()
        {
            var raw = await _redisDbContext.Get(DataEntryPrefix.IxIndexHeight.ToString());
            return raw == RedisValue.Null ? uint.MinValue : (uint) raw;
        }

        public async Task SetIndexHeight(uint height)
        {
            await _redisDbContext.Set(DataEntryPrefix.IxIndexHeight.ToString(), height);
        }

        public async Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 scriptHash)
        {
            var redisVal = await _redisDbContext.Get(scriptHash.BuildIxConfirmedKey());
            if (redisVal == RedisValue.Null) return new HashSet<CoinReference>();
            return JsonConvert.DeserializeObject<HashSet<CoinReference>>(redisVal);
        }

        public Task SetIndexConfirmed(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            throw new NotImplementedException();
        }

        public async Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 scriptHash)
        {
            var redisVal = await _redisDbContext.Get(scriptHash.BuildIxClaimableKey());
            if (redisVal == RedisValue.Null) return new HashSet<CoinReference>();
            return JsonConvert.DeserializeObject<HashSet<CoinReference>>(redisVal);
        }

        public Task SetIndexClaimable(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}