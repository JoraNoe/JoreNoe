using System.Collections.Generic;

namespace JoreNoe.Cache.Redis
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public interface IRedisManager
    {
        bool Add(string KeyName, string Context, int Expire = 180);
        bool Remove(string KeyName);

        T Add<T>(string KeyName, T Context, int Expire = 180);

        T AddOrGet<T>(string KeyName, T Context, int Expire = 180);

        bool Exists(string KeyName);

        IList<T> Find<T>(string KeyName);

        T Single<T>(string KeyName);
        string Single(string KeyName);

        IList<T> AddMulitToFolder<T>(string KeyName, IList<T> Context, string FolderName, int Expire = 180);

        void Dispose();

        string AddOrGet(string KeyName, string Context, int Expire = 180);

        string Get(string KeyName);
        string Update(string KeyName, string Context);

        T Update<T>(string KeyName, T Context);

        IList<T> Update<T>(string KeyName, IList<T> Contexts);

    }
}
