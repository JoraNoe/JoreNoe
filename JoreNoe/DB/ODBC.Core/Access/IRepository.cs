using JoreNoe.DB.ODBC.Core.Access.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace JoreNoe.DB.ODBC.Core.Access
{
    public interface IRepository
    {
        #region 读取数据
        /// <summary>
        /// 获取全部表名
        /// </summary>
        /// <returns></returns>
        List<AccessTable> AllTables();

        /// <summary>
        /// 读取单个表数据 
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        DataTable FindSingleTable(string TableName);
        #endregion

        #region 执行Sql
        /// <summary>
        /// 执行SQl语句
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        object ExecuteScalar(string SQL);

        /// <summary>
        /// 执行SQL 异步
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        Task<object> ExcuteScalarAsync(string SQL);
        #endregion
    }
}
