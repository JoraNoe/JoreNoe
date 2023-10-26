using Dapper;
using JoreNoe.Extend;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Dapper.SqlMapper;
using static JoreNoe.DB.Dapper.DapperExtend;

namespace JoreNoe.DB.Dapper
{
    public class Repository<T> : IRepository<T>
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
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
        public T Single<TKey>(TKey ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null)
        {
            if (DapperExtend.IsNullOrEmpty(ParamsValue))
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
            this.DBConnection.Execute(DeleteSQL, parameters);

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

            var GetTableName = typeof(T).Name;
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
            var SoftRemoveSQL = $"Update {typeof(T).Name} SET {SoftKeyName} = @{SoftKeyName} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            var parameters = new Dictionary<string, object>
            {
                { ParamsKeyName, ParamsValues },
                { SoftKeyName, _SoftKeyValue }
            };

            return this.Excute(SoftRemoveSQL, parameters) != 0 ? Single : default;
        }


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

            var ExistsSQL = $"SELECT COUNT(*) FROM {typeof(T).Name} WHERE {ParamsKeyName}={ParamsValues}";
            return this.DBConnection.QueryFirstOrDefault<bool>(ExistsSQL, ParamsKeyName);
        }



        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="ParamsValue"></param>
        /// <param name="Entity"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
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
            var ConvertToDictionary = EntityToDictionaryExtend.EntityToDictionary(Entity, new string[] { ParamsKeyName });
            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
            string DeleteSQL = $"UPDATE {typeof(T).Name} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
            this.DBConnection.Execute(DeleteSQL, ConvertToDictionary);

            return ExistsValueInfo;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="System.Exception"></exception>
        public void BulkInsert(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
                throw new System.Exception("数据为空,请传递参数。");
            var GetTableName = typeof(T).Name;
            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();

            var BatchData = data.GetBatchData(Registory.BatchCount);
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
            var GetTableName = typeof(T).Name;
            //var batches = data.Batch(this.BatchCount); // 自定义批量扩展方法
            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            var BatchData = data.GetBatchData(Registory.BatchCount);
            foreach (var batch in BatchData) await this.InsertBatchAsyncNew(batch, GetTableName, GetColumns.Item1).ConfigureAwait(false);

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
            var GetTableName = typeof(T).Name;
            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            var BatchData = data.GetBatchData(Registory.BatchCount);
            foreach (var batch in BatchData) this.InsertBatchTransaction(batch, GetTableName, GetColumns.Item1);
        }

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            string insertQuery = $"INSERT INTO {typeof(T).Name} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
            await this.DBConnection.ExecuteAsync(insertQuery, entity);
            return entity;
        }

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // 获取列
            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
            string insertQuery = $"INSERT INTO {typeof(T).Name} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
            this.DBConnection.Execute(insertQuery, entity);
            return entity;
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

            return this.DBConnection.Query<T>(SQLExcute);
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

            return this.DBConnection.Query<T>(SQLExcute, Params);
        }

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

            return this.DBConnection.Execute(SQLExcute, Params);
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

            return this.DBConnection.Execute(SQLExcute);
        }



        #region 公用方法

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
            using (IDbConnection dbConnection = this.DBConnection)
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
        private async Task InsertBatchAsyncNew<TData>(IEnumerable<TData> data, string tableName, string InsertColumns)
        {
            //using (IDbConnection dbConnection = this.DBConnection)
            //{
            //dbConnection.Open();
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
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

            await this.DBConnection.ExecuteAsync(InsertSQL.ToString().TrimEnd(',')).ConfigureAwait(false);
            //scope.Complete();
            //}
            //}
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
            using (IDbConnection dbConnection = this.DBConnection)
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
            //using (IDbConnection dbConnection = this.DBConnection)
            //{
            //dbConnection.Open();
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
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

            this.DBConnection.Execute(InsertSQL.ToString().TrimEnd(','));
            //scope.Complete();
            //}
            //}
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
            using (IDbConnection dbConnection = this.DBConnection)
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
        /// <param name="InsertColumns"></param>
        /// <param name="InsertColumnValues"></param>
        private void DeleteBatch<TKey>(TKey[] Value, string tableName, string ParamsKeyName)
        {
            //using (IDbConnection dbConnection = this.DBConnection)
            //using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            //dbConnection.Open();
            // 使用 IN 子句
            string inClause = string.Join(",", Value.Select(param => $"{param}"));
            string sql = $"DELETE FROM {tableName} WHERE {ParamsKeyName} IN ({inClause})";

            this.DBConnection.Execute(sql);
            //transactionScope.Complete();
            //}
        }


        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <param name="connectionDbType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private IDbConnection CreateDbConnection(IDBType connectionDbType, string connectionString)
        {
            return connectionDbType switch
            {
                IDBType.MySql => new MySqlConnection(connectionString),
                IDBType.SqlServer => new SqlConnection(connectionString),
                _ => throw new NotSupportedException("Unsupported database type"),
            };
        }

        /// <summary>
        /// 获取一个新的数据库链接上下文
        /// </summary>
        /// <param name="connectionDbType">链接类型</param>
        /// <param name="connectionString">链接字符串</param>
        /// <returns></returns>
        public IDbConnection GetDbConnection(IDBType connectionDbType, string connectionString)
        {
            return this.CreateDbConnection(connectionDbType, connectionString);
        }

        #endregion
    }
}
