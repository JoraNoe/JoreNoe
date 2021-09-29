using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace JoreNoe.DB.ODBC.Core.Access
{
    /// <summary>
    /// 注册服务
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// EntityFrameCore 上下文
        /// </summary>
        public static OdbcConnection _Connection { get; set; }

        /// <summary>
        /// 设置上下文方法
        /// </summary>
        /// <param name="DB"></param>
        public static void SetInitOdbc(string ConnectionString)
        {
            _Connection = new OdbcConnection();
            _Connection.ConnectionString = ConnectionString;
            if (_Connection.State == System.Data.ConnectionState.Open)
            {
                _Connection.Close();
            }
            else if (_Connection.State == System.Data.ConnectionState.Closed)
            {
                _Connection.Open();
            }
            else if (_Connection.State == System.Data.ConnectionState.Broken)
            {
                _Connection.Close();
                _Connection.Open();
            }
        }
    }
}
