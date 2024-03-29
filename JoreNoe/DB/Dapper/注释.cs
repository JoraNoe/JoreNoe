﻿//using Dapper;
//using Dapper.Contrib.Extensions;
//using JoreNoe.Extend;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Microsoft.Extensions.FileSystemGlobbing;
//using MySql.Data.MySqlClient;
//using NPOI.POIFS.FileSystem;
//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Data;
//using System.Dynamic;
//using System.Linq;
//using System.Reflection.Metadata.Ecma335;
//using System.Threading.Tasks;
//using System.Transactions;
//using System.Windows.Media.TextFormatting;
//using static Dapper.SqlMapper;
//using static JoreNoe.DB.Dapper.DapperExtend;

//namespace JoreNoe.DB.Dapper
//{
//    public class Repository<T> : IRepository<T>
//    {
//        /// <summary>
//        /// 数据库上下文
//        /// </summary>
//        public readonly IDbConnection DBConnection;

//        /// <summary>
//        /// 批次数量
//        /// </summary>
//        public int BatchCount { get; set; } = 2000;

//        public Repository()
//        {
//            this.DBConnection = Registory._Connection;
//        }

//        /// <summary>
//        /// 尽量不要使用,全表检索
//        /// </summary>
//        /// <param name="ParamsColumns">输出列</param>
//        /// <returns></returns>
//        public IEnumerable<T> All(string[] ParamsColumns = null)
//        {
//            return this.DBConnection.Query<T>($"Select {(ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns))} from " + typeof(T).Name);
//        }

//        /// <summary>
//        /// 查询单个数据
//        /// </summary>
//        /// <param name="ParamsValue">一般是主键ID </param>
//        /// <param name="ParamsKeyName">主键名称 默认为 Id </param>
//        /// <param name="ParamsColumns">输出的列名,默认为 * 全部</param>
//        /// <returns></returns>
//        public T Single<TKey>(TKey ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null)
//        {
//            if (DapperExtend.IsNullOrEmpty(ParamsValue))
//                throw new System.Exception("ParamsValue为空,请传递参数。");
//            if (string.IsNullOrEmpty(ParamsKeyName))
//                throw new System.Exception("ParamsKeyName为空,请传递参数。");

//            // 组装SQL 
//            string QuerySQL = string.Concat("select ", (ParamsColumns == null ? "*" : string.Join(", ", ParamsColumns)), " From ", typeof(T).Name,
//                " where ", ParamsKeyName, " = ", ParamsValue);

//            return this.DBConnection.QueryFirstOrDefault<T>(QuerySQL);
//        }

//        /// <summary>
//        /// 删除单条数据，物理删除
//        /// </summary>
//        /// <param name="ParamsValue">要删除的数据值</param>
//        /// <param name="ParamsKeyName">匹配的健</param>
//        /// <returns></returns>
//        /// <exception cref="System.Exception"></exception>
//        public T Remove<TKey>(TKey ParamsValue, string ParamsKeyName = "Id")
//        {
//            if (IsNullOrEmpty(ParamsValue))
//                throw new System.Exception("ParamsValue为空,请传递参数。");
//            if (string.IsNullOrEmpty(ParamsKeyName))
//                throw new System.Exception("ParamsKeyName为空,请传递参数。");

//            //验证数据是否存在
//            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
//            if (ExistsValueInfo == null)
//                return default;

//            Dictionary<string, object> parameters = new Dictionary<string, object> { { ParamsKeyName, ParamsValue } };
//            string DeleteSQL = $"DELETE FROM {typeof(T).Name} WHERE {ParamsKeyName} = @{ParamsKeyName}";
//            this.DBConnection.Execute(DeleteSQL, parameters);

//            return ExistsValueInfo;
//        }

//        /// <summary>
//        /// 批量删除数据
//        /// </summary>
//        /// <param name="ParamsValues"></param>
//        /// <param name="ParamsKeyName"></param>
//        /// <exception cref="System.Exception"></exception>
//        public void Removes<Tkey>(Tkey[] ParamsValues, string ParamsKeyName = "Id")
//        {
//            if (IsNullOrEmpty(ParamsValues))
//                throw new System.Exception("ParamsValue为空,请传递参数。");
//            if (ParamsValues == null || ParamsValues.Count() == 0)
//                throw new System.Exception("ParamsValue为空,请传递参数。");
//            if (string.IsNullOrEmpty(ParamsKeyName))
//                throw new System.Exception("ParamsKeyName为空,请传递参数。");

//            var GetTableName = typeof(T).Name;
//            //var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
//            this.DeleteBatch<Tkey>(ParamsValues, GetTableName, ParamsKeyName);
//        }

//        /// <summary>
//        /// 修改数据
//        /// </summary>
//        /// <param name="ParamsValue"></param>
//        /// <param name="Entity"></param>
//        /// <param name="ParamsKeyName"></param>
//        /// <returns></returns>
//        /// <exception cref="System.Exception"></exception>
//        public T Update<TKey>(TKey ParamsValue, T Entity, string ParamsKeyName = "Id")
//        {
//            if (IsNullOrEmpty(ParamsValue))
//                throw new System.Exception("ParamsValue为空,请传递参数。");
//            if (string.IsNullOrEmpty(ParamsKeyName))
//                throw new System.Exception("ParamsKeyName为空,请传递参数。");
//            if (Entity == null)
//                throw new System.Exception("实体为空,请传递参数。");

//            //验证数据是否存在
//            var ExistsValueInfo = this.Single(ParamsValue, ParamsKeyName);
//            if (ExistsValueInfo == null)
//                return default;

//            // 实体转换为字典
//            var ConvertToDictionary = EntityToDictionaryExtend.EntityToDictionary(Entity, new string[] { ParamsKeyName });
//            var GetSQLParams = DictionaryToFormattedExtend.DictionaryToFormattedSQL(ConvertToDictionary);
//            ConvertToDictionary.Add(ParamsKeyName, ParamsValue);
//            string DeleteSQL = $"UPDATE {typeof(T).Name} SET {GetSQLParams} WHERE {ParamsKeyName} = @{ParamsKeyName}";
//            this.DBConnection.Execute(DeleteSQL, ConvertToDictionary);

//            return ExistsValueInfo;
//        }

//        /// <summary>
//        /// 批量插入数据
//        /// </summary>
//        /// <param name="data"></param>
//        /// <exception cref="System.Exception"></exception>
//        public void BulkInsert(IEnumerable<T> data)
//        {
//            if (data == null || !data.Any())
//                throw new System.Exception("数据为空,请传递参数。");
//            var GetTableName = typeof(T).Name;
//            //var batches = data.Batch(this.BatchCount); // 自定义批量扩展方法
//            // 获取列
//            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();

//            this.InsertBatch(data, GetTableName, GetColumns.Item1, GetColumns.Item2);
//            //Parallel.ForEach(batches, batch => { this.InsertBatch(batch, GetTableName, GetColumns.Item1, GetColumns.Item2); });
//        }

//        public void BulkInsert1(IEnumerable<T> data)
//        {
//            if (data == null || !data.Any())
//                throw new System.Exception("数据为空,请传递参数。");
//            var GetTableName = typeof(T).Name;
//            //var batches = data.Batch(this.BatchCount); // 自定义批量扩展方法
//            // 获取列
//            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();

//            using (IDbConnection dbConnection = this.DBConnection)
//            {
//                dbConnection.Open();

//                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    string insertSql = $"INSERT INTO {GetTableName} ({GetColumns.Item1}) VALUES ";
//                    foreach (var item in data)
//                    {
//                        insertSql += $"({GetEntityParams(item)}),";
//                    }
//                    insertSql = insertSql.TrimEnd(',');
//                    dbConnection.Execute(insertSql);
//                    scope.Complete();
//                }

//                //using (var transaction = dbConnection.dataBeginTransaction())
//                //{
//                //    try
//                //    {
//                //        // 创建 SQL 查询，使用参数化查询来插入数据
//                //        string insertSql = $"INSERT INTO {GetTableName} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";

//                //        // 执行批量插入操作
//                //        dbConnection.Execute(insertSql, data, transaction);

//                //        transaction.Commit();
//                //    }
//                //    catch (Exception ex)
//                //    {
//                //        transaction.Rollback();
//                //        // 处理错误
//                //    }
//                //}
//            }


//            //using (IDbConnection dbConnection = this.DBConnection)
//            //{
//            //    dbConnection.Open();
//            //    using (var transaction = dbConnection.BeginTransaction())
//            //    {
//            //        try
//            //        {
//            //            // 使用Dapper.Contrib的扩展方法Insert来执行批量插入
//            //            dbConnection.Insert(data, transaction);

//            //            transaction.Commit();
//            //        }
//            //        catch (Exception ex)
//            //        {
//            //            transaction.Rollback();
//            //            // 处理错误
//            //        }
//            //    }
//            //}

//            //this.InsertBatch(data, GetTableName, GetColumns.Item1, GetColumns.Item2);
//            //Parallel.ForEach(batches, batch => { this.InsertBatch(batch, GetTableName, GetColumns.Item1, GetColumns.Item2); });
//        }


//        /// <summary>
//        /// 异步批量插入数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        /// <exception cref="System.Exception"></exception>
//        public async Task BulkInsertAsync(IEnumerable<T> data)
//        {
//            if (data == null || !data.Any())
//                throw new System.Exception("数据为空,请传递参数。");
//            var GetTableName = typeof(T).Name;
//            //var batches = data.Batch(this.BatchCount); // 自定义批量扩展方法
//            // 获取列
//            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
//            await this.InsertBatchAsync(data, GetTableName, GetColumns.Item1, GetColumns.Item2).ConfigureAwait(false);
//            //await Task.WhenAll(batches.Select(batch => this.InsertBatchAsync(batch, GetTableName, GetColumns.Item1, GetColumns.Item2)));
//        }

//        /// <summary>
//        /// 添加单条数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        /// <exception cref="ArgumentNullException"></exception>
//        public async Task<T> AddAsync(T entity)
//        {
//            if (entity == null)
//                throw new ArgumentNullException(nameof(entity));

//            // 获取列
//            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
//            string insertQuery = $"INSERT INTO {typeof(T).Name} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
//            await this.DBConnection.ExecuteAsync(insertQuery, entity);
//            return entity;
//        }

//        /// <summary>
//        /// 添加单条数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        /// <exception cref="ArgumentNullException"></exception>
//        public T Add(T entity)
//        {
//            if (entity == null)
//                throw new ArgumentNullException(nameof(entity));

//            // 获取列
//            var GetColumns = EntityToDictionaryExtend.EntityToSQLParams<T>();
//            string insertQuery = $"INSERT INTO {typeof(T).Name} ({GetColumns.Item1}) VALUES ({GetColumns.Item2})";
//            this.DBConnection.Execute(insertQuery, entity);
//            return entity;
//        }


//        #region 公用方法

//        /// <summary>
//        /// 批量插入数据
//        /// </summary>
//        /// <typeparam name="TData"></typeparam>
//        /// <param name="data">数据源</param>
//        /// <param name="tableName">表名</param>
//        /// <param name="InsertColumns">要插入的列名</param>
//        /// <param name="InsertColumnValues">要插入的列名数据</param>
//        /// <returns></returns>
//        private async Task InsertBatchAsync<TData>(IEnumerable<TData> data, string tableName, string InsertColumns, string InsertColumnValues)
//        {
//            using (IDbConnection dbConnection = this.DBConnection)
//            {
//                dbConnection.Open();
//                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    var insertQuery = $"INSERT INTO {tableName} " +
//                                 $"({InsertColumns}) " +
//                                 $"VALUES ({InsertColumnValues})";
//                    await dbConnection.ExecuteAsync(insertQuery, data).ConfigureAwait(false);
//                    transactionScope.Complete();
//                }
//            }

//            //using (IDbConnection dbConnection = this.CreateDbConnection(Registory.ConnectionDbType, Registory.ConnectionString))
//            //{
//            //    dbConnection.Open();
//            //    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            //    {
//            //        await dbConnection.ExecuteAsync(
//            //            $"INSERT INTO {tableName} ({InsertColumns}) VALUES ({InsertColumnValues})",
//            //            data
//            //        ).ConfigureAwait(false);
//            //    }
//            //}
//        }

//        /// <summary>
//        /// 批量插入数据同步
//        /// </summary>
//        /// <typeparam name="TData"></typeparam>
//        /// <param name="data">数据源</param>
//        /// <param name="tableName">表名</param>
//        /// <param name="InsertColumns">要插入的列名</param>
//        /// <param name="InsertColumnValues">要插入的列名数据</param>
//        /// <returns></returns>
//        private void InsertBatch<TData>(IEnumerable<TData> data, string tableName, string InsertColumns, string InsertColumnValues)
//        {

//            //using (IDbConnection dbConnection = this.DBConnection)
//            //{
//            //    dbConnection.Open();
//            //    using (var transaction = dbConnection.BeginTransaction())
//            //    {
//            //        try
//            //        {
//            //            // 使用Dapper.Contrib的扩展方法Insert来执行批量插入
//            //            dbConnection.Insert(data, transaction);

//            //            transaction.Commit();
//            //        }
//            //        catch (Exception ex)
//            //        {
//            //            transaction.Rollback();
//            //            // 处理错误
//            //        }
//            //    }
//            //}

//            using (IDbConnection dbConnection = this.DBConnection)
//            {
//                dbConnection.Open();
//                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    var insertQuery = $"INSERT INTO {tableName} " +
//                                 $"({InsertColumns}) " +
//                                 $"VALUES ({InsertColumnValues})";
//                    dbConnection.Execute(insertQuery, data);
//                    transactionScope.Complete();
//                }
//            }

//            //using (IDbConnection dbConnection = this.CreateDbConnection(Registory.ConnectionDbType, Registory.ConnectionString))
//            //{
//            //    dbConnection.Open();
//            //    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            //    {
//            //        dbConnection.Execute(
//            //            $"INSERT INTO {tableName} ({InsertColumns}) VALUES ({InsertColumnValues})",
//            //            data
//            //        );
//            //    }
//            //}
//        }

//        /// <summary>
//        /// 批量删除
//        /// </summary>
//        /// <typeparam name="TKey"></typeparam>
//        /// <param name="Value"></param>
//        /// <param name="tableName"></param>
//        /// <param name="InsertColumns"></param>
//        /// <param name="InsertColumnValues"></param>
//        private void DeleteBatch<TKey>(TKey[] Value, string tableName, string ParamsKeyName)
//        {
//            using (IDbConnection dbConnection = this.DBConnection)
//            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            {
//                dbConnection.Open();
//                // 使用 IN 子句
//                string inClause = string.Join(",", Value.Select(param => $"{param}"));
//                string sql = $"DELETE FROM {tableName} WHERE {ParamsKeyName} IN ({inClause})";

//                dbConnection.Execute(sql);
//                transactionScope.Complete();
//            }




//            //using (IDbConnection dbConnection = this.CreateDbConnection(Registory.ConnectionDbType, Registory.ConnectionString))
//            //{
//            //    dbConnection.Open();
//            //    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            //    {
//            //        var Params = new Dictionary<string, object>
//            //        {
//            //            { ParamsKeyName, Value }
//            //        };
//            //        dbConnection.Execute(
//            //            $"DELETE FROM {tableName} WHERE {ParamsKeyName} = @{ParamsKeyName}",
//            //            Params
//            //        );
//            //    }
//            //}
//        }


//        /// <summary>
//        /// 创建数据库链接
//        /// </summary>
//        /// <param name="connectionDbType"></param>
//        /// <param name="connectionString"></param>
//        /// <returns></returns>
//        /// <exception cref="NotSupportedException"></exception>
//        private IDbConnection CreateDbConnection(IDBType connectionDbType, string connectionString)
//        {
//            return connectionDbType switch
//            {
//                IDBType.MySql => new MySqlConnection(connectionString),
//                IDBType.SqlServer => new SqlConnection(connectionString),
//                _ => throw new NotSupportedException("Unsupported database type"),
//            };
//        }

//        /// <summary>
//        /// 获取一个新的数据库链接上下文
//        /// </summary>
//        /// <param name="connectionDbType">链接类型</param>
//        /// <param name="connectionString">链接字符串</param>
//        /// <returns></returns>
//        public IDbConnection GetDbConnection(IDBType connectionDbType, string connectionString)
//        {
//            return this.CreateDbConnection(connectionDbType, connectionString);
//        }

//        #endregion
//    }
//}
