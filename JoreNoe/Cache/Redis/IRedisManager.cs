using System.Collections.Generic;

namespace JoreNoe.Cache.Redis
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public interface IRedisManager
    {
        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        bool Add(string KeyName, string Context, int Expire = 180);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        bool Remove(string KeyName);

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        T Add<T>(string KeyName, T Context, int Expire = 180);

        /// <summary>
        /// 添加或者获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        T AddOrGet<T>(string KeyName, T Context, int Expire = 180);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        bool Exists(string KeyName);

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        IList<T> Find<T>(string KeyName);

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        T Single<T>(string KeyName);

        /// <summary>
        /// 文件夹存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="FolderName"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        IList<T> AddMulitToFolder<T>(string KeyName, IList<T> Context, string FolderName, int Expire = 180);

        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();

        /// <summary>
        /// 添加获取或者获取
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        string AddOrGet(string KeyName,string Context,int Expire = 180);

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        string Get(string KeyName);
    }
}
