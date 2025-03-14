﻿using Dapper;
using JoreNoe.Extend;
using JoreNoe.JoreNoeLog;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static Dapper.SqlMapper;
using static JoreNoe.DB.Dapper.DapperExtend;

namespace JoreNoe.DB.Dapper
{
    /// <summary>
    /// 通用方法扩展
    /// </summary>
    static class JoreNoeDapperDBConnectionExtensions
    {
        /// <summary>
        /// 是否启用多数据库操作
        /// </summary>
        private static AsyncLocal<bool> IsMulitConnection = new AsyncLocal<bool>();
        /// <summary>
        /// 扩展执行方法异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="DBAction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<T> ExcuteWithConnectionAsync<T>(this IDbConnection Connection, Func<IDbConnection, Task<T>> DBActionAsync)
        {
            if (IsMulitConnection.Value)
            {
                using (Connection)
                {

                    return await DBActionAsync(Connection).ConfigureAwait(false);

                }
            }
            else
            {
                return await DBActionAsync(Connection).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 扩展执行方法同步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="DBActionAsync"></param>
        /// <returns></returns>
        public static T ExcuteWithConnection<T>(this IDbConnection Connection, Func<IDbConnection, T> DBAction)
        {
            if (IsMulitConnection.Value)
            {
                using (Connection)
                {
                    return DBAction(Connection);
                }
            }
            else
            {
                return DBAction(Connection);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Value"></param>
        public static void DapperSettingIsEnabledMulitConnection(this IDbConnection Connection, bool Value)
        {
            IsMulitConnection.Value = Value;
        }
    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        /// <summary>
        /// 数据库
        /// </summary>
        private readonly IDatabaseService databaseService;

        /// <summary>
        /// 是否启用多数据库链接
        /// </summary>
        private readonly bool IsMulitEnabledConnection;

        /// <summary>
        /// 插入批次数量
        /// </summary>
        private readonly long mulitInsertBatchcount;

        /// <summary>
        /// 当前表名
        /// </summary>
        private readonly string CurrentTableName;
        public Repository(IDatabaseService dataBaseService)
        {
            this.databaseService = dataBaseService;

            var SettingKey = this.databaseService.MulitDatabaseSettings.IsMulit ? typeof(T).Name.ToLower() : "Default";
            if (!databaseService.DataBaseSettingsLookUp.TryGetValue(SettingKey, out var SingleSettings))
                throw new ArgumentException($"No database settings found for {SettingKey}.", nameof(SettingKey));

            this.IsMulitEnabledConnection = SingleSettings.IsEnabledMulitConnection;
            this.mulitInsertBatchcount = SingleSettings.mulitInsertBatchcount;

            this.CurrentTableName = this.GetTableName<T>();
        }

        /// <summary>
        /// 判断是否允许多条链接，不允许只用一条，允许就使用多条
        /// </summary>
        private IDbConnection GetDBConnection => this.GetConnectionNewMethod();
        private IDbConnection GetConnectionNewMethod()
        {
            var GetClassName = typeof(T).Name;
            var NewConnection = this.databaseService.GetConnection(GetClassName);
            NewConnection.DapperSettingIsEnabledMulitConnection(this.IsMulitEnabledConnection);
            return NewConnection;
        }

        #region 单个查询

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="ParamsValue">一般是主键ID </param>
        /// <param name="ParamsKeyName">主键名称 默认为 Id </param>
        /// <param name="ParamsColumns">输出的列名,默认为 * 全部</param>
        /// <returns></returns>
        public T Single<TKey>(TKey ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null)
        {
            if (DapperExtend.IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            // 组装SQL 
            string QuerySQL = string.Concat("select ", (ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns)), " From ", this.CurrentTableName,
                " where ", ParamsKeyName, " = ", "'", ParamsValue, "'");

            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefault<T>(QuerySQL));
        }

        /// <summary>
        /// 查询单个 
        /// </summary>
        /// <param name="ExPression"></param>
        /// <returns></returns>
        public T Single(Expression<Func<T, bool>> ExPression)
        {
            var Convert = ExpressionToSqlConverter.ConvertSingle(ExPression);
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefault<T>(Convert));
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="ExPression"></param>
        /// <returns></returns>
        public async Task<T> SingleAsync(Expression<Func<T, bool>> ExPression)
        {
            var Convert = ExpressionToSqlConverter.ConvertSingle(ExPression);
            return await this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefaultAsync<T>(Convert));
        }

        /// <summary>
        /// 异步查询单个数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ParamsValue"></param>
        /// <param name="ParamsKeyName"></param>
        /// <param name="ParamsColumns"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<T> SingleAsync<TKey>(TKey ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null)
        {
            if (DapperExtend.IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            // 组装SQL 
            string QuerySQL = string.Concat("select ", (ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns)), " From ", this.CurrentTableName,
                " where ", ParamsKeyName, " = ", "'", ParamsValue, "'");

            return await this.GetDBConnection.ExcuteWithConnectionAsync(async DbCon => await DbCon.QueryFirstOrDefaultAsync<T>(QuerySQL).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <summary>
        /// 通过SQL查询单个数据
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public T SingleSQL(string SQL)
        {
            if (string.IsNullOrEmpty(SQL))
                throw new System.Exception("SQL 为空");
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefault<T>(SQL));
        }

        public async Task<T> SingleSQLAsync(string SQL)
        {
            if (string.IsNullOrEmpty(SQL))
                throw new System.Exception("SQL 为空");
            return await this.GetDBConnection.ExcuteWithConnectionAsync(async DbCon => await DbCon.QueryFirstOrDefaultAsync<T>(SQL).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public T SingleSQL(string SQL, object Params)
        {
            if (string.IsNullOrEmpty(SQL))
                throw new System.Exception("SQL 为空");
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefault<T>(SQL, Params));
        }

        public async Task<T> SingleSQLAsync(string SQL, object Params)
        {
            if (string.IsNullOrEmpty(SQL))
                throw new System.Exception("SQL 为空");
            return await this.GetDBConnection.ExcuteWithConnectionAsync(async DbCon => await DbCon.QueryFirstOrDefaultAsync<T>(SQL, Params).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region 删除数据

        /// <summary>
        /// 删除单条数据，物理删除
        /// </summary>
        /// <param name="ParamsValue">要删除的数据值</param>
        /// <param name="ParamsKeyName">匹配的健</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public T Remove<TKey>(TKey ParamsValue, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            //验证数据是否存在
            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
            if (ExistsValueInfo == null)
                return default;

            Dictionary<string, object> parameters = new Dictionary<string, object> { { ParamsKeyName, ParamsValue } };
            string DeleteSQL = $"DELETE FROM {typeof(T).Name} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(DeleteSQL, parameters));

            return ExistsValueInfo;
        }

        public T Remove(Expression<Func<T, bool>> ExPression)
        {
            var Convert = ExpressionToSqlConverter.ConvertDelete(ExPression);
            //验证数据是否存在
            var ExistsValueInfo = this.Single(ExPression);
            if (ExistsValueInfo == null)
                return default;

            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(Convert));
            return ExistsValueInfo;
        }

        public async Task<T> RemoveAsync(Expression<Func<T, bool>> ExPression)
        {
            var Convert = ExpressionToSqlConverter.ConvertDelete(ExPression);
            //验证数据是否存在
            var ExistsValueInfo = await this.SingleAsync(ExPression).ConfigureAwait(false);
            if (ExistsValueInfo == null)
                return default;

            await this.GetDBConnection.ExcuteWithConnection(async DbCon => await DbCon.ExecuteAsync(Convert)).ConfigureAwait(false);
            return ExistsValueInfo;
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="ParamsValues"></param>
        /// <param name="ParamsKeyName"></param>
        /// <exception cref="System.Exception"></exception>
        public void Removes<Tkey>(Tkey[] ParamsValues, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValues))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (ParamsValues == null || ParamsValues.Count() == 0)
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            var GetTableName = this.CurrentTableName; //typeof(T).Name.ToLower();
            //var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            this.DeleteBatch<Tkey>(ParamsValues, GetTableName, ParamsKeyName);
        }

        /// <summary>
        /// 软删除 数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ParamsValues">软删除Key</param>
        /// <param name="ParamsKeyName">软删除Keyname</param>
        /// <param name="SoftKeyName">软删除字段</param>
        /// <param name="SoftKeyValue">软删除数据</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public T SoftRemove<TKey>(TKey ParamsValues, string ParamsKeyName = "Id", string SoftKeyName = "IsDelete", bool? SoftKeyValue = null)
        {
            if (IsNullOrEmpty(ParamsValues))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");
            if (string.IsNullOrEmpty(SoftKeyName))
                throw new System.Exception("SoftKeyName为空,请传递参数。");

            //验证数据是否存在
            var Single = this.Single(ParamsValues, ParamsKeyName);
            if (Single == null)
                return default;

            var _SoftKeyValue = SoftKeyValue == null ? true : false;
            var SoftRemoveSQL = $"Update {this.CurrentTableName} SET {SoftKeyName} = @{SoftKeyName} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            var parameters = new Dictionary<string, object>
            {
                { ParamsKeyName, ParamsValues },
                { SoftKeyName, _SoftKeyValue }
            };

            return this.Excute(SoftRemoveSQL, parameters) != 0 ? Single : default;
        }

        #endregion

        #region 查询是否存在

        /// <summary>
        ///  查询是否存在数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ParamsValues"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool IsExists<TKey>(TKey ParamsValues, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValues))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            var ExistsSQL = $"SELECT COUNT(*) FROM {this.CurrentTableName} WHERE {ParamsKeyName}='{ParamsValues}'";
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefault<bool>(ExistsSQL, ParamsKeyName));
        }

        public async Task<bool> IsExistsAsync<TKey>(TKey ParamsValues, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValues))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");

            var ExistsSQL = $"SELECT COUNT(*) FROM {this.CurrentTableName} WHERE {ParamsKeyName}='{ParamsValues}'";
            return await this.GetDBConnection.ExcuteWithConnection(async DbCon => await DbCon.QueryFirstOrDefaultAsync<bool>(ExistsSQL, ParamsKeyName).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public bool IsExists(Expression<Func<T, bool>> ExPression)
        {
            var Convert = ExpressionToSqlConverter.ConvertCount(ExPression);
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QueryFirstOrDefault<bool>(Convert));
        }

        public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> ExPression)
        {
            var Convert = ExpressionToSqlConverter.ConvertCount(ExPression);
            return await this.GetDBConnection.ExcuteWithConnection(async DbCon => await DbCon.QueryFirstOrDefaultAsync<bool>(Convert).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region 修改数据
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="ParamsValue"></param>
        /// <param name="Entity"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public T Update<TKey>(TKey ParamsValue, object Entity, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValue))
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
            var ConvertToDictionary = EntityToDictionaryExtend.ObjectToDictionary(Entity);
            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
            string DeleteSQL = $"UPDATE {this.CurrentTableName} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(DeleteSQL, ConvertToDictionary));

            return ExistsValueInfo;
        }


        public T Update<TKey>(TKey ParamsValue, T Entity, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValue))
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
            var ConvertToDictionary = EntityToDictionaryExtend.ObjectToDictionary(Entity);
            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
            string DeleteSQL = $"UPDATE {this.CurrentTableName} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(DeleteSQL, ConvertToDictionary));

            return ExistsValueInfo;
        }


        public T Update<TKey>(TKey ParamsValue, Func<T, T> Entity, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");
            if (Entity == null)
                throw new System.Exception("实体为空,请传递参数。");

            //验证数据是否存在
            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
            if (ExistsValueInfo == null)
                return default;

            var temp = Entity(ExistsValueInfo);

            // 实体转换为字典
            var ConvertToDictionary = EntityToDictionaryExtend.EntityToDictionary(temp, new string[] { ParamsKeyName });
            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
            string DeleteSQL = $"UPDATE {this.CurrentTableName} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(DeleteSQL, ConvertToDictionary));


            return ExistsValueInfo;
        }


        public T Update<TKey>(TKey ParamsValue, Action<T> Entity, string ParamsKeyName = "Id")
        {
            if (IsNullOrEmpty(ParamsValue))
                throw new System.Exception("ParamsValue为空,请传递参数。");
            if (string.IsNullOrEmpty(ParamsKeyName))
                throw new System.Exception("ParamsKeyName为空,请传递参数。");
            if (Entity == null)
                throw new System.Exception("实体为空,请传递参数。");

            //验证数据是否存在
            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
            if (ExistsValueInfo == null)
                return default;

            Entity(ExistsValueInfo);

            // 实体转换为字典
            var ConvertToDictionary = EntityToDictionaryExtend.EntityToDictionary(ExistsValueInfo, new string[] { ParamsKeyName });
            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
            string DeleteSQL = $"UPDATE {this.CurrentTableName} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(DeleteSQL, ConvertToDictionary));


            return ExistsValueInfo;
        }

        #endregion

        #region 批量插入

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="System.Exception"></exception>
        public void BulkInsert(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
                throw new System.Exception("数据为空,请传递参数。");
            var GetTableName = this.CurrentTableName;//typeof(T).Name.ToLower();
            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();

            var BatchData = data.GetBatchData(this.mulitInsertBatchcount);
            if (this.IsMulitEnabledConnection)
                Parallel.ForEach(BatchData, batch => { this.InsertBatchNew(batch, GetTableName, GetColumns.Item1); });
            else
                foreach (var batch in BatchData) this.InsertBatchNew(batch, GetTableName, GetColumns.Item1);
            //Parallel.ForEach(batches, batch => { this.InsertBatch(batch, GetTableName, GetColumns.Item1, GetColumns.Item2); });
        }

        /// <summary>
        /// 异步批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task BulkInsertAsync(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
                throw new System.Exception("数据为空,请传递参数。");
            var GetTableName = this.CurrentTableName; //typeof(T).Name;
            //var batches = data.Batch(this.BatchCount); // 自定义批量扩展方法
            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            var BatchData = data.GetBatchData(this.mulitInsertBatchcount);
            foreach (var batch in BatchData) await this.InsertBatchAsync(batch, GetTableName, GetColumns.Item1).ConfigureAwait(false);


            //await Task.WhenAll(batches.Select(batch => this.InsertBatchAsync(batch, GetTableName, GetColumns.Item1, GetColumns.Item2)));
        }

        /// <summary>
        /// 批量插入
        /// 如果失败全部失败
        /// 如果成功全部成功
        /// 事务
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="System.Exception"></exception>
        public void BulkInsertTransaction(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
                throw new System.Exception("数据为空,请传递参数。");
            var GetTableName = this.CurrentTableName; //typeof(T).Name;
            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            var BatchData = data.GetBatchData(this.mulitInsertBatchcount);
            foreach (var batch in BatchData) this.InsertBatchTransaction(batch, GetTableName, GetColumns.Item1);
        }

        #endregion

        #region 插入单个数据

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<T> AddAsync(T entity, string[] IgnoreFailds = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>(IgnoreFailds);
            string insertQuery = $"INSERT INTO {typeof(T).Name} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
            await this.GetDBConnection.ExcuteWithConnectionAsync(async (DbCon) => await DbCon.ExecuteAsync(insertQuery, entity).ConfigureAwait(false));
            return entity;
        }

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T Add(T entity, string[] IgnoreFailds = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>(IgnoreFailds);
            string insertQuery = $"INSERT INTO {this.CurrentTableName} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(insertQuery, entity));
            return entity;
        }

        #endregion

        #region Find 查询数据

        /// <summary>
        /// 尽量不要使用,全表检索
        /// </summary>
        /// <param name="ParamsColumns">输出列</param>
        /// <returns></returns>
        public IEnumerable<T> All(string[] ParamsColumns = null)
        {
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Query<T>($"Select {(ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns))} from " + this.CurrentTableName));
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="SQLExcute">SQL 语句</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<T> Find(string SQLExcute)
        {
            if (string.IsNullOrEmpty(SQLExcute))
                throw new ArgumentNullException(nameof(SQLExcute));

            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Query<T>(SQLExcute));
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="SQLExcute">执行SQL</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<T> Find(string SQLExcute, object Params)
        {
            if (string.IsNullOrEmpty(SQLExcute))
                throw new ArgumentNullException(nameof(SQLExcute));

            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Query<T>(SQLExcute, Params));
        }

        public async Task<IEnumerable<T>> FindAsync(string SQLExcute, object Params)
        {
            if (string.IsNullOrEmpty(SQLExcute))
                throw new ArgumentNullException(nameof(SQLExcute));

            return await this.GetDBConnection.ExcuteWithConnectionAsync(async DbCon => await DbCon.QueryAsync<T>(SQLExcute, Params).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<T> Find(Expression<Func<T, bool>> Predicate)
        {
            if (Predicate == null)
                throw new ArgumentNullException(nameof(Predicate));

            var sql = ExpressionToSqlConverter.Convert(Predicate);
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Query<T>(sql));
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> Predicate)
        {
            if (Predicate == null)
                throw new ArgumentNullException(nameof(Predicate));

            var sql = ExpressionToSqlConverter.Convert(Predicate);
            return await this.GetDBConnection.ExcuteWithConnectionAsync(async DbCon => await DbCon.QueryAsync<T>(sql).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region 执行，创建表方法

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="SQLExcute"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Excute(string SQLExcute, object Params)
        {
            if (string.IsNullOrEmpty(SQLExcute))
                throw new ArgumentNullException(nameof(SQLExcute));

            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(SQLExcute, Params));
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="SQLExcute">执行SQL</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Excute(string SQLExcute)
        {
            if (string.IsNullOrEmpty(SQLExcute))
                throw new ArgumentNullException(nameof(SQLExcute));

            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(SQLExcute));
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool CreateTable(T Entity)
        {
            if (Entity == null) throw new ArgumentNullException("参数为空");

            StringBuilder sqlBuilder = new StringBuilder($"CREATE TABLE IF NOT EXISTS {this.CurrentTableName} (");

            foreach (var property in typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var columnName = GetColumnName(property);
                var columnType = GetColumnType(property);
                var isNullable = IsNullable(property);

                sqlBuilder.Append($"{columnName} {columnType}");

                if (!isNullable)
                {
                    sqlBuilder.Append(" NOT NULL");
                }
                sqlBuilder.Append(", ");
            }

            sqlBuilder.Length -= 2;
            sqlBuilder.Append(")");

            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(sqlBuilder.ToString()));

            return true;
        }
        #endregion

        #region 公用方法


        private void PrivateCerateTable(BaseHistory e, string TableName = "BaseHistory")
        {
            string GetTableName = "BaseHistory";
            if (TableName != "BaseHistory")
                GetTableName = TableName;

            StringBuilder sqlBuilder = new StringBuilder($"CREATE TABLE IF NOT EXISTS {GetTableName} (");

            foreach (var property in typeof(BaseHistory).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var columnName = GetColumnName(property);
                var columnType = GetColumnType(property);
                var isNullable = IsNullable(property);

                sqlBuilder.Append($"{columnName} {columnType}");

                if (!isNullable)
                {
                    sqlBuilder.Append(" NOT NULL");
                }
                sqlBuilder.Append(", ");
            }

            sqlBuilder.Length -= 2;
            sqlBuilder.Append(")");

            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(sqlBuilder.ToString()));
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="tableName">表名</param>
        /// <param name="InsertColumns">要插入的列名</param>
        /// <param name="InsertColumnValues">要插入的列名数据</param>
        /// <returns></returns>
        private async Task InsertBatchAsync<TData>(IEnumerable<TData> data, string tableName, string InsertColumns, string InsertColumnValues)
        {
            using (IDbConnection dbConnection = this.GetDBConnection)
            {
                dbConnection.Open();
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var insertQuery = $"INSERT INTO {tableName} " +
                                 $"({InsertColumns}) " +
                                 $"VALUES ({InsertColumnValues})";
                    await dbConnection.ExecuteAsync(insertQuery, data).ConfigureAwait(false);
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="InsertColumns"></param>
        /// <param name="InsertColumnValues"></param>
        /// <returns></returns>
        private async Task InsertBatchAsync<TData>(IEnumerable<TData> data, string tableName, string InsertColumns)
        {
            // 顺序处理队列中的数据并插入
            StringBuilder InsertSQL = new StringBuilder($"INSERT INTO {tableName} ({InsertColumns}) VALUES ");
            var insertQueue = new ConcurrentQueue<string>();

            // 使用并行循环将数据插入队列
            Parallel.ForEach(data, item =>
            {
                string insertValues = GetEntityFiledParams(item);
                insertQueue.Enqueue($"({insertValues}),");
            });

            InsertSQL.Append(string.Join("", insertQueue));

            // 判断是否启用多连接插入 
            if (this.IsMulitEnabledConnection)
            {
                using (var connection = new MySqlConnection(this.GetDBConnection.ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    await connection.ExecuteAsync(InsertSQL.ToString().TrimEnd(',')).ConfigureAwait(false);
                }
            }
            else
            {
                await this.GetDBConnection.ExecuteAsync(InsertSQL.ToString().TrimEnd(',')).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// 批量插入数据同步
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="tableName">表名</param>
        /// <param name="InsertColumns">要插入的列名</param>
        /// <param name="InsertColumnValues">要插入的列名数据</param>
        /// <returns></returns>
        private void InsertBatch<TData>(IEnumerable<TData> data, string tableName, string InsertColumns, string InsertColumnValues)
        {
            using (IDbConnection dbConnection = this.GetDBConnection)
            {
                dbConnection.Open();
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var insertQuery = $"INSERT INTO {tableName} " +
                                 $"({InsertColumns}) " +
                                 $"VALUES ({InsertColumnValues})";
                    dbConnection.Execute(insertQuery, data);
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// 批量插入 高性能
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="InsertColumns"></param>
        /// <param name="InsertColumnValues"></param>
        private void InsertBatchNew<TData>(IEnumerable<TData> data, string tableName, string InsertColumns)
        {
            // 顺序处理队列中的数据并插入
            StringBuilder InsertSQL = new StringBuilder($"INSERT INTO {tableName} ({InsertColumns}) VALUES ");
            var insertQueue = new ConcurrentQueue<string>();

            // 使用并行循环将数据插入队列
            Parallel.ForEach(data, item =>
            {
                string insertValues = GetEntityFiledParams(item);
                insertQueue.Enqueue($"({insertValues}),");
            });

            InsertSQL.Append(string.Join("", insertQueue));

            // 判断是否启用多连接插入 
            if (this.IsMulitEnabledConnection)
            {
                using (var connection = new MySqlConnection(this.GetDBConnection.ConnectionString))
                {
                    //connection.Open();
                    connection.Execute(InsertSQL.ToString().TrimEnd(','));
                }
            }
            else
            {
                this.GetDBConnection.Execute(InsertSQL.ToString().TrimEnd(','));
            }
        }

        /// <summary>
        /// 批量插入事务
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="InsertColumns"></param>
        private void InsertBatchTransaction<TData>(IEnumerable<TData> data, string tableName, string InsertColumns)
        {
            using (IDbConnection dbConnection = this.GetDBConnection)
            {
                dbConnection.Open();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var insertQueue = new ConcurrentQueue<string>();

                    // 使用并行循环将数据插入队列
                    Parallel.ForEach(data, item =>
                    {
                        string insertValues = GetEntityFiledParams(item);
                        insertQueue.Enqueue(insertValues);
                    });

                    // 顺序处理队列中的数据并插入
                    StringBuilder InsertSQL = new StringBuilder($"INSERT INTO {tableName} ({InsertColumns}) VALUES ");
                    while (insertQueue.TryDequeue(out string insertValues))
                    {
                        InsertSQL.Append($"({insertValues}),");
                    }

                    dbConnection.Execute(InsertSQL.ToString().TrimEnd(','));
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="Value"></param>
        /// <param name="tableName"></param>
        private void DeleteBatch<TKey>(TKey[] Value, string tableName, string ParamsKeyName)
        {
            string inClause = string.Join(",", Value.Select(param => $"{param}"));
            string sql = $"DELETE FROM {tableName} WHERE {ParamsKeyName} IN ({inClause})";
            this.GetDBConnection.Execute(sql);
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ment"></param>
        /// <returns></returns>
        private string GetTableName<TData>()
        {
            return typeof(TData).Name.ToLower();
        }

        #endregion

        #region 调试方法

        /// <summary>
        /// 创建日志
        /// 自动创建表
        /// </summary>
        /// <param name="LogContext"></param>
        /// <param name="ResultContext"></param>
        /// <param name="ContextType"></param>
        /// <param name="UserName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void PublishHistory(string LogContext, string ResultContext, string ContextType, string UserName = "SystemCreate", string TableName = "BaseHistory")
        {
            if (string.IsNullOrEmpty(LogContext) ||
                string.IsNullOrEmpty(ContextType)) throw new ArgumentNullException("数据为空");

            //if(this.DBConnection.QuerySingle<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{this.DBConnection.Database}' AND TABLE_NAME = '{TableName}'") != 1)
            //    this.PrivateCerateTable(new BaseHistory());

            this.PrivateCerateTable(new BaseHistory());
            var Entity = new BaseHistory { Context = LogContext, HistoryType = ContextType, ResultContext = ResultContext, CreateUser = UserName, CreateTime = DateTime.Now };

            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<BaseHistory>();
            string insertQuery = $"INSERT INTO {typeof(BaseHistory).Name} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
            this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.Execute(insertQuery, Entity));
        }


        public async Task TestMUlit(IEnumerable<T> D)
        {
            string connectionString = this.GetDBConnection.ConnectionString; // 替换为你的 MySQL 连接字符串
            int totalDataCount = 10000;
            int batchSize = 500;

            List<Task> insertTasks = new List<Task>();
            for (int i = 0; i < totalDataCount; i += batchSize)
            {
                insertTasks.Add(InsertDataAsync(connectionString, i, batchSize));
            }

            await Task.WhenAll(insertTasks);

            Console.WriteLine("All data inserted successfully.");
        }

        public async Task InsertDataAsync(string connectionString, int startIndex, int batchSize)
        {
            Parallel.For(startIndex, startIndex + batchSize, async i =>
            {
                //await semaphore.WaitAsync(); // 请求一个许可，如果已达到并发连接数上限，会阻塞直到有许可可用

                try
                {
                    string innerConnectionString = connectionString; // 创建一个本地副本，避免闭包引用问题

                    using (var connection = new MySqlConnection(innerConnectionString))
                    {
                        await connection.OpenAsync();

                        // 构造插入数据的 SQL 语句，假设你的表名是 test，列名为 id、Name、email、Flg
                        string sql = "INSERT INTO test (Name, email, Flg) VALUES (@Name, @Email, @Flg)";

                        // 构造参数
                        var parameters = new
                        {
                            Name = $"Name{i}",
                            Email = $"email{i}@example.com",
                            Flg = false // 假设 Flg 交替为 true 和 false
                        };

                        // 执行插入操作
                        await connection.ExecuteAsync(sql, parameters);
                    }
                }
                finally
                {
                    // semaphore.Release(); // 释放许可，允许其他线程进入临界区
                }
            });
        }


        #endregion

        #region 查询数量

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QuerySingle<int>($"select Count(*) from {this.CurrentTableName}"));
        }

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Count(Expression<Func<T, bool>> Predicate)
        {
            if (Predicate == null)
                throw new ArgumentNullException(nameof(Predicate));

            var sql = ExpressionToSqlConverter.ConvertCount(Predicate);
            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QuerySingle<int>(sql));
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> Predicate)
        {
            if (Predicate == null)
                throw new ArgumentNullException(nameof(Predicate));

            var sql = ExpressionToSqlConverter.ConvertCount(Predicate);
            return await this.GetDBConnection.ExcuteWithConnection(async DbCon => await DbCon.QuerySingleAsync<int>(sql).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <summary>
        /// 通过SQL 查询数量
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Count(string SQL)
        {
            if (string.IsNullOrEmpty(SQL))
                throw new ArgumentNullException(nameof(SQL));

            return this.GetDBConnection.ExcuteWithConnection(DbCon => DbCon.QuerySingle<int>(SQL));
        }

        #endregion
    }
}
