using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core.Models.Config.Server;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Kms.gRPC.Services.Cache
{
    /// <summary>
    /// Cache service with StackExcahnge.Redis
    /// </summary>
    public class RedisService
    {
        private readonly string server = string.Empty;
        private readonly ConnectionMultiplexer connection = null;
        private readonly AppSettings appSettings = null;
        private readonly IServer redisServer = null;
        private readonly IDatabase redisDb = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public RedisService(
            IOptions<AppSettings> configuration)
        {
            this.appSettings = configuration.Value;
            var redisOptions = this.appSettings.Redis;
            if (redisOptions == null)
            {
                throw new NullReferenceException("AppSettings.Redis");
            }

            this.server = redisOptions.Host;
            ConfigurationOptions options = ConfigurationOptions.Parse(this.server);

            // options.AbortOnConnectFail = false;
            options.ConnectTimeout = redisOptions.Timeout;
            options.SyncTimeout = redisOptions.Timeout;
            options.ConnectRetry = redisOptions.ConnectRetry;

            this.connection = ConnectionMultiplexer.Connect(options);
            this.redisServer = this.connection.GetServer(options.EndPoints.First());
            this.redisDb = this.connection.GetDatabase();
        }

        /// <summary>
        /// Redis Database
        /// </summary>
        public IDatabase Database
        {
            get => this.redisDb;
        }

        /// <summary>
        /// Search Redis keys with pattern
        /// </summary>
        /// <param name="pattern">Pattern, such as "xxxx*"</param>
        /// <returns>Keys collection</returns>
        public IEnumerable<string> SearchKeys(string pattern = "")
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return this.redisServer.Keys().Select(x => x.ToString());
            }
            else
            {
                var redisVal = (RedisValue)pattern;
                return this.redisServer.Keys(pattern: redisVal).Select(x => x.ToString());
            }
        }

        /// <summary>
        /// Save cache
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">The data which will be cached</param>
        public void SaveCache<T>(string key, T data)
        {
            var value = JsonConvert.SerializeObject(data);
            this.redisDb.StringSet(key, value);
        }

        /// <summary>
        /// Save cache with expire time
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="expire">Expire timespan</param>
        /// <param name="data">The data which will be cached</param>
        public void SaveCache<T>(string key, TimeSpan? expire, T data)
        {
            var value = JsonConvert.SerializeObject(data);
            this.redisDb.StringSet(key, value, expiry: expire);

            // this.redisDb.KeyExpire(key, expire);
        }

        /// <summary>
        /// Save cache (async)
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">The data which will be cached</param>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        public async Task SaveCacheAsync<T>(string key, T data)
        {
            var value = JsonConvert.SerializeObject(data);
            await this.redisDb.StringSetAsync(key, value);
        }

        /// <summary>
        /// Save cache with expire time (async)
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="expire">Expire timespan</param>
        /// <param name="data">The data which will be cached</param>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        public async Task SaveCacheAsync<T>(string key, TimeSpan? expire, T data)
        {
            var value = JsonConvert.SerializeObject(data);
            await this.redisDb.StringSetAsync(key, value, expiry: expire);
            this.redisDb.KeyExpire(key, expire);
        }

        /// <summary>
        /// Increase the key's value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="by">Increment</param>
        public async Task IncreaseAsyc(string key, TimeSpan? expire = null, long by = 1)
        {
            if (!this.redisDb.KeyExists(key))
            {
                await this.SaveCacheAsync(key, expire, 0);
            }
            else
            {
                await this.redisDb.KeyExpireAsync(key, expire);
            }

            await this.redisDb.StringIncrementAsync(key, by);
        }

        /// <summary>
        /// Decrease the key's value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="by">Decrement</param>
        public async Task DecreaseAsyc(string key, long by = 1)
        {
            await this.redisDb.StringDecrementAsync(key, by);
        }

        /// <summary>
        /// Get cache by key
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="rtn">T</param>
        /// <returns>Boolean</returns>
        public bool GetCache<T>(string key, out T rtn)
            where T : new()
        {
            if (this.redisDb.KeyExists(key))
            {
                var value = this.redisDb.StringGet(key);
                rtn = JsonConvert.DeserializeObject<T>(value);
                return true;
            }
            else
            {
                rtn = default(T);
                return false;
            }
        }

        /// <summary>
        /// Get Cache (async)
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Tuple for T and boolean</returns>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        public async Task<Tuple<T, bool>> GetCacheAsync<T>(string key)
            where T : new()
        {
            if (await this.redisDb.KeyExistsAsync(key))
            {
                var value = await this.redisDb.StringGetAsync(key);
                return new Tuple<T, bool>(JsonConvert.DeserializeObject<T>(value), true);
            }
            else
            {
                return new Tuple<T, bool>(default(T), false);
            }
        }

        /// <summary>
        /// Clear cache with key
        /// </summary>
        /// <param name="key">Key</param>
        public void ClearCache(string key)
        {
            if (this.redisDb.KeyExists(key))
            {
                this.redisDb.KeyDelete(key);
            }
        }

        /// <summary>
        /// Clear cache with key
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        public async Task ClearCacheAsync(string key)
        {
            if (this.redisDb.KeyExists(key))
            {
                await this.redisDb.KeyDeleteAsync(key);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.connection.Close();
        }
    }

}
