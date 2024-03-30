using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JoreNoe.JoreNoeLog
{
    public class BaseHistory
    {
        /// <summary>
        /// ID
        /// </summary>
        [InsertIgnoreAutoIncrement]
        [ColumnPrimaryKey]
        [ColumnAutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        [Required]
        public string Context { get; set; }

        /// <summary>
        /// 结果内容
        /// </summary>
        public string ResultContext { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public string HistoryType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUser { get; set; }
    }
}
