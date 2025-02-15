using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.Cache.Redis
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public interface IRedisManager
    {
        bool Add(string KeyName, string Context, TimeSpan? Expire = null);
        bool Remove(string KeyName);
        T Add<T>(string KeyName, T Context, TimeSpan? Expire = null);
        T AddOrGet<T>(string KeyName, T Context, TimeSpan? Expire = null);
        T AddOrGet<T>(string keyName, Func<T> contentProvider, TimeSpan? Expire = null);
        bool Exists(string KeyName);
        IList<T> Find<T>(string KeyName);
        T Single<T>(string KeyName);
        string Single(string KeyName);
        IList<T> AddMulitToFolder<T>(string KeyName, IList<T> Context, string FolderName, TimeSpan? Expire = null);
        string AddOrGet(string KeyName, string Context, TimeSpan? Expire = null);
        string Get(string KeyName);
        string Update(string KeyName, string Context);
        T Update<T>(string KeyName, T Context);
        IList<T> Update<T>(string KeyName, IList<T> Contexts);


        Task<bool> AddAsync(string KeyName, string Context, TimeSpan? Expire = null);
        Task<bool> RemoveAsync(string KeyName);
        Task<T> AddAsync<T>(string KeyName, T Context, TimeSpan? Expire = null);
        Task<T> AddOrGetAsync<T>(string KeyName, T Context, TimeSpan? Expire = null);
        Task<T> AddOrGetAsync<T>(string keyName, Func<Task<T>> contentProvider, TimeSpan? Expire = null);
        Task<bool> ExistsAsync(string KeyName);
        Task<IList<T>> FindAsync<T>(string KeyName);
        Task<T> SingleAsync<T>(string KeyName);
        Task<string> SingleAsync(string KeyName);
        Task<IList<T>> AddMulitToFolderAsync<T>(string KeyName, IList<T> Context, string FolderName, TimeSpan? Expire = null);
        Task<string> AddOrGetAsync(string KeyName, string Context, TimeSpan? Expire = null);
        Task<string> GetAsync(string KeyName);
        Task<string> UpdateAsync(string KeyName, string Context);
        Task<T> UpdateAsync<T>(string KeyName, T Context);
        Task<IList<T>> UpdateAsync<T>(string KeyName, IList<T> Contexts);
        Task<bool> SetContainsAsync(string KeyName, string Vlaue);

        Task<bool> SetAddAsync(string KeyName, string Value);


    }
}
