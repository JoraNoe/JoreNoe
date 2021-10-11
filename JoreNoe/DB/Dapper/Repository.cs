using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;

namespace JoreNoe.DB.Dapper
{
    public class Repository<T>
    {
        public readonly IDbConnection DBConnection;

        public Repository()
        {
            this.DBConnection = Registory._Connection;
        }

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        public IList<T> All()
        {
            this.DBConnection.Open();
            var TableName = typeof(T).Name;
            var AllResult = this.DBConnection.Query<T>("Select * from "+TableName);
            return AllResult.ToList();
        }

        /// <summary>
        /// 单个数据
        /// </summary>
        /// <returns></returns>
        public T Single(string Id)
        {
            var TableName = typeof(T).Name;
            var Single = this.DBConnection.Query<T>("Select * from " + TableName + " where Id == '"+Id+"'");
            return (T)Single;
        }
    }
}
