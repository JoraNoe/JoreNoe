using JoreNoe.DB.ODBC.Core.Access.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Threading.Tasks;

namespace JoreNoe.DB.ODBC.Core.Access
{
    public class Repository : IRepository
    {
        /// <summary>
        /// 基类
        /// </summary>
        private OdbcConnection OdbcConnection { get; set; }

        private OdbcCommand OdbcCommand { get; set; }

        public Repository()
        {
            this.OdbcConnection = Register._Connection;
        }

        /// <summary>
        /// 查询全部表名包括系统表
        /// </summary>
        /// <returns></returns>
        public List<AccessTable> AllTables()
        {
            using OdbcConnection Connection = OdbcConnection;

            DataTable Tables = Connection.GetSchema("tables", null);

            List<AccessTable> ReturnTable = new List<AccessTable>();
            foreach (DataRow item in Tables.Rows)
            {
                ReturnTable.Add(new AccessTable
                {
                    TableType = item[3] + "",
                    TableName = item[2] + ""
                });
            }

            return ReturnTable;

        }

        /// <summary>
        /// 查询全部表名 忽略 系统表
        /// </summary>
        /// <returns></returns>
        public List<AccessTable> AllTablesIngoreSystemTable()
        {
            using OdbcConnection Connection = OdbcConnection;

            DataTable Tables = Connection.GetSchema("tables", null);

            List<AccessTable> ReturnTable = new List<AccessTable>();
            foreach (DataRow item in Tables.Rows)
            {
                if (item[3].ToString() == "TABLE")
                {
                    ReturnTable.Add(new AccessTable
                    {
                        TableType = item[3] + "",
                        TableName = item[2] + ""
                    });
                }
            }

            return ReturnTable;

        }


        public object ExecuteScalar(string SQL)
        {
            using OdbcConnection Connection = OdbcConnection;
            OdbcCommand = Connection.CreateCommand();
            OdbcCommand.CommandType = CommandType.Text;
            OdbcCommand.CommandText = SQL;

            return OdbcCommand.ExecuteScalar();
        }

        /// <summary>
        /// 查询单个表数据
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataTable FindSingleTable(string TableName)
        {
            DataTable dataTable = new DataTable();

            using OdbcConnection Connection = OdbcConnection;

            OdbcCommand cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + TableName;

            OdbcDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            dataTable.Load(reader);

            return dataTable;
        }

        /// <summary>
        /// 执行SQL 异步操作
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public async Task<object> ExcuteScalarAsync(string SQL)
        {
            using OdbcConnection Connection = OdbcConnection;
            OdbcCommand = Connection.CreateCommand();
            OdbcCommand.CommandType = CommandType.Text;
            OdbcCommand.CommandText = SQL;

            return await OdbcCommand.ExecuteScalarAsync().ConfigureAwait(false);
        }
    }
}
