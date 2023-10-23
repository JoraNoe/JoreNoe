using Dapper;
using JoreNoe.Extend;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using static Dapper.SqlMapper;

namespace JoreNoe.DB.Dapper
{
    public class Repository<T> : IRepository<T>
    {
        public readonly IDbConnection DBConnection;

        public Repository()
        {
            this.DBConnection = Registory._Connection;
        }

        /// <summary>
        /// 尽量不要使用,全表检索
        /// </summary>
        /// <param name="ParamsColumns">输出列</param>
        /// <returns></returns>
        public IEnumerable<T> All(string[] ParamsColumns = null)
        {
            return this.DBConnection.Query<T>($"Select {(ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns))} from " + typeof(T).Name);
        }

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="ParamsValue">一般是主键ID </param>
        /// <param name="ParamsKeyName">主键名称 默认为 Id </param>
        /// <param name="ParamsColumns">输出的列名,默认为 * 全部</param>
        /// <returns></returns>
        public T Single(string ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null)
        {
            if (string.IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            // 组装SQL 
            string QuerySQL = string.Concat("select ", (ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns)), " From ", typeof(T).Name,
                " where ", ParamsKeyName, " = ", ParamsValue);

            return this.DBConnection.QueryFirstOrDefault<T>(QuerySQL);
        }

        /// <summary>
        /// 删除单条数据，物理删除
        /// </summary>
        /// <param name="ParamsValue">要删除的数据值</param>
        /// <param name="ParamsKeyName">匹配的健</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public T Remove(string ParamsValue, string ParamsKeyName = "Id")
        {
            if (string.IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            //验证数据是否存在
            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
            if (ExistsValueInfo == null)
                return default;

            Dictionary<string, object> parameters = new Dictionary<string, object> { { ParamsKeyName, ParamsValue } };
            string DeleteSQL = $"DELETE FROM {typeof(T).Name} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.DBConnection.Execute(DeleteSQL, parameters);

            return ExistsValueInfo;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="ParamsValue"></param>
        /// <param name="Entity"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public T Update(string ParamsValue, T Entity, string ParamsKeyName = "Id")
        {
            if (string.IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");
            if (Entity == null)
                throw new System.Exception("实体为空,请传递参数。");

            //验证数据是否存在
            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
            if (ExistsValueInfo == null)
                return default;

            // 实体转换为字典
            var ConvertToDictionary = EntityToDictionaryExtend.EntityToDictionary(Entity, new string[] { ParamsKeyName });
            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
            string DeleteSQL = $"UPDATE {typeof(T).Name} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.DBConnection.Execute(DeleteSQL, ConvertToDictionary);

            return ExistsValueInfo;
        }
    }
}
