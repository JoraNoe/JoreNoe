﻿using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace JoreNoe.DB.Dapper
{
    public static class DapperExtend
    {

        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var keyValue = part.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    parameters[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            return parameters;
        }

        public static string GetDatabaseName(string connectionString)
        {
            var parameters = ParseConnectionString(connectionString);
            if (parameters.ContainsKey("Database"))
            {
                return parameters["Database"];
            }

            throw new ArgumentException("Database not found in SQL Server connection string.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return YieldBatchElements(enumerator, size - 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int size)
        {

            yield return source.Current;
            for (int i = 0; i < size && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }

        /// <summary>
        /// 判断泛型是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(T value)
        {
            //
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        /// <summary>
        /// 获取数据字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string GetEntityFiledParams<T>(T data)
        {
            Type type = data.GetType();
            PropertyInfo[] properties = type.GetProperties();

            var resultBag = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                // 跳过带有特性标记的属性
                if (Attribute.IsDefined(property, typeof(InsertIgnoreAutoIncrementAttribute)))
                    continue;

                var propertyValue = property.GetValue(data, null); // 获取属性值

                // 处理 null 或 DBNull 值
                if (propertyValue == null || propertyValue == DBNull.Value)
                {
                    resultBag.Add("NULL");
                }
                else if (propertyValue is string)
                {
                    // 对于字符串类型，加单引号并处理单引号字符
                    var stringValue = propertyValue.ToString().Replace("'", "''");
                    resultBag.Add($"'{stringValue}'");
                }
                else if (propertyValue is DateTime dateTime)
                {
                    // 对于 DateTime 类型，格式化为 'yyyy-MM-dd HH:mm:ss'
                    resultBag.Add($"'{dateTime:yyyy-MM-dd HH:mm:ss}'");
                }
                else if (propertyValue is bool boolean)
                {
                    // 布尔类型转换为 '1' 或 '0'
                    resultBag.Add(boolean ? "1" : "0");
                }
                else if (propertyValue is Guid guid)
                {
                    // 对于 GUID 类型，加单引号
                    resultBag.Add($"'{guid}'");
                }
                else if (propertyValue is decimal decimalValue || propertyValue is float floatValue || propertyValue is double doubleValue)
                {
                    // 对于数值类型（decimal, float, double），保持数值格式
                    resultBag.Add(propertyValue.ToString());
                }
                else
                {
                    // 对于其他类型（如整数、长整型），直接转为字符串
                    resultBag.Add(propertyValue.ToString());
                }
            }

            // 合并结果并返回
            return string.Join(",", resultBag);
        }




        /// <summary>
        /// 获取批次数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetBatchData<T>(this IEnumerable<T> data, long BatchCount)
        {

            return data
            .Select((value, index) => new { Value = value, Index = index })
            .GroupBy(x => x.Index / BatchCount)
            .Select(g => g.Select(x => x.Value).ToList());
        }

        public static string GetColumnName(PropertyInfo property)
        {

            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            return columnAttr != null ? columnAttr.Name : property.Name;
        }

        public static string GetColumnType(PropertyInfo property)
        {

            var type = property.PropertyType;

            if (type == typeof(int))
            {
                StringBuilder Str = new StringBuilder();
                var cloumnAttr = property.GetCustomAttribute<ColumnAutoIncrementAttribute>();
                if (cloumnAttr == null)
                    Str.Append("INT ");
                else Str.Append("INT auto_increment ");
                var cloumnAttr1 = property.GetCustomAttribute<ColumnPrimaryKeyAttribute>();
                if (cloumnAttr1 != null)
                    Str.Append(" Primary Key ");
                return Str.ToString();
            }
            else if (type == typeof(string))
            {
                var columnAttr = property.GetCustomAttribute<ColumnLengthAttribute>();
                if (columnAttr != null)
                    return $"VARCHAR({columnAttr.Length})";
                else
                    return $"VARCHAR(255)";
            }
            else if (type == typeof(bool))
            {
                return $" TINYINT(1) ";
            }
            else if (type == typeof(DateTime))
            {
                return $" DATETIME ";
            }
            else if (type == typeof(Guid))
            {
                return $" UNIQUEIDENTIFIER ";
            }

            throw new ArgumentException($"Unsupported type: {type.Name}");
        }

        public static bool IsNullable(PropertyInfo property)
        {

            return Nullable.GetUnderlyingType(property.PropertyType) != null ||
                   property.GetCustomAttribute<RequiredAttribute>() == null;
        }


        public static string Convert<T>(Expression<Func<T, bool>> expression)
        {

            // Basic implementation: only handles simple expressions
            var body = expression.Body as BinaryExpression;
            if (body == null)
                throw new NotSupportedException("Only simple binary expressions are supported.");

            var left = body.Left as MemberExpression;
            var right = body.Right as ConstantExpression;

            if (left == null || right == null)
                throw new NotSupportedException("Only simple member and constant expressions are supported.");

            // Here you should handle different types of expressions and operators
            string columnName = left.Member.Name;
            object value = right.Value;

            // Create a simple SQL query
            return $"SELECT * FROM {typeof(T).Name} WHERE {columnName} = '{value}'";
        }

    }

    public static class ExpressionToSqlConverter
    {
        public static string Convert<T>(Expression<Func<T, bool>> expression)
        {
            return "SELECT * FROM " + typeof(T).Name + " WHERE " + ProcessExpression(expression.Body);
        }

        public static string ConvertCount<T>(Expression<Func<T, bool>> expression)
        {
            return "SELECT COUNT(*) FROM " + typeof(T).Name + " WHERE " + ProcessExpression(expression.Body);
        }

        public static string ConvertSingle<T>(Expression<Func<T, bool>> expression)
        {
            return "SELECT * FROM " + typeof(T).Name + " WHERE " + ProcessExpression(expression.Body);
        }

        public static string ConvertDelete<T>(Expression<Func<T, bool>> expression)
        {
            return "DELETE FROM " + typeof(T).Name + " WHERE " + ProcessExpression(expression.Body);
        }

        private static string ProcessExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return ProcessBinaryExpression((BinaryExpression)expression);

                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return ProcessLogicalExpression((BinaryExpression)expression);

                case ExpressionType.MemberAccess:
                    return ProcessMemberExpression((MemberExpression)expression);

                case ExpressionType.Constant:
                    return ProcessConstantExpression((ConstantExpression)expression);

                case ExpressionType.Call:
                    return ProcessMethodCallExpression((MethodCallExpression)expression);

                default:
                    throw new NotSupportedException($"The expression type '{expression.NodeType}' is not supported.");
            }
        }

        private static string ProcessBinaryExpression(BinaryExpression expression)
        {
            string left, right;
            var operatorSymbol = GetOperatorSymbol(expression.NodeType);
            if (IsParameterMemberExpression(expression.Left))
            {
                left = ProcessExpression(expression.Left);
                right = GetConstantValue(expression.Right)?.ToString() ?? "NULL";
                return $"{left} {operatorSymbol} {right}";
            }
            else if (IsParameterMemberExpression(expression.Right))
            {
                left = GetConstantValue(expression.Left)?.ToString() ?? "NULL";
                right = ProcessExpression(expression.Right);
                return $"{right} {operatorSymbol} {left}";
            }
            else
            {
                left = ProcessExpression(expression.Left);
                right = ProcessExpression(expression.Right);
                return $"{right}   {operatorSymbol}   {left}";
            }
        }

        private static string ProcessMethodCallExpression(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Contains")
            {
                var memberExpression = expression.Object as MemberExpression;
                var argumentExpression = expression.Arguments[0];

                if (memberExpression != null && IsParameterMemberExpression(memberExpression))
                {
                    var columnName = ProcessMemberExpression(memberExpression);
                    var value = GetConstantValue(argumentExpression)?.ToString() ?? "NULL";
                    return $"{columnName} LIKE '%{value}%'";
                }
            }

            throw new NotSupportedException($"The method '{expression.Method.Name}' is not supported.");
        }

        private static bool IsParameterMemberExpression(Expression expression)
        {
            return expression is MemberExpression memberExpression &&
                   memberExpression.Expression is ParameterExpression;
        }

        private static object GetConstantValue(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
            {
                return string.Concat("'", constantExpression.Value, "'");
            }
            else if (expression is MemberExpression memberExpression)
            {
                var objectMember = Expression.Convert(memberExpression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                return string.Concat("'", getter(), "'");
            }
            return null;
        }

        private static string ProcessLogicalExpression(BinaryExpression expression)
        {
            string left, right;
            var operatorSymbol = GetOperatorSymbol(expression.NodeType);
            if (IsParameterMemberExpression(expression.Left))
            {
                left = ProcessExpression(expression.Left);
                right = GetConstantValue(expression.Right)?.ToString() ?? "NULL";
                return $"{left} {operatorSymbol} {right}";
            }
            else if (IsParameterMemberExpression(expression.Right))
            {
                left = GetConstantValue(expression.Left)?.ToString() ?? "NULL";
                right = ProcessExpression(expression.Right);
                return $"{right} {operatorSymbol} {left}";
            }
            else
            {
                left = ProcessExpression(expression.Left);
                right = ProcessExpression(expression.Right);
                return $"{left} {operatorSymbol} {right}";
            }
        }

        private static string ProcessMemberExpression(MemberExpression expression)
        {
            return expression.Member.Name;
        }

        private static string ProcessConstantExpression(ConstantExpression expression)
        {
            if (expression.Type == typeof(string))
            {
                return $"'{expression.Value}'";
            }
            return expression.Value.ToString();
        }

        private static string GetOperatorSymbol(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"The expression type '{type}' is not supported."),
            };
        }

        //public static void CheckAndUpdateDatabaseSchema(string connectionString, Assembly modelsAssembly)
        //{
        //    using (IDbConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        // 获取所有数据库表
        //        var tables = connection.Query<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'").ToList();

        //        // 获取所有 Model（实体类）
        //        var modelTypes = modelsAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract).ToList();

        //        foreach (var modelType in modelTypes)
        //        {
        //            string tableName = modelType.Name; // 假设表名与类名一致

        //            if (!tables.Contains(tableName))
        //            {
        //                Console.WriteLine($"⚠️  数据库中不存在表 `{tableName}`，可以考虑创建该表！");
        //                continue;
        //            }

        //            // 获取数据库表字段
        //            var dbColumns = connection.Query<string>(
        //                "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName",
        //                new { TableName = tableName }).ToHashSet();

        //            // 获取 Model 字段
        //            var modelProperties = modelType.GetProperties().Select(p => p.Name).ToHashSet();

        //            // 找出 Model 中但数据库缺失的字段
        //            var missingInDb = modelProperties.Except(dbColumns).ToList();

        //            // 找出数据库中但 Model 中没有的字段（谨慎删除）
        //            var extraInDb = dbColumns.Except(modelProperties).ToList();

        //            // 自动添加缺失字段
        //            foreach (var missingColumn in missingInDb)
        //            {
        //                var propertyInfo = modelType.GetProperty(missingColumn);
        //                string columnType = GetSqlType(propertyInfo.PropertyType);

        //                string alterTableSql = $"ALTER TABLE {tableName} ADD {missingColumn} {columnType}";
        //                connection.Execute(alterTableSql);
        //                Console.WriteLine($"✅ 添加字段 `{missingColumn}` 到表 `{tableName}`");
        //            }

        //            // 这里不自动删除 extraInDb 的字段，避免误删
        //        }
        //    }
        //}

        //// 获取 SQL 数据类型
        //private static string GetSqlType(Type type)
        //{
        //    if (type == typeof(int) || type == typeof(int?)) return "INT";
        //    if (type == typeof(long) || type == typeof(long?)) return "BIGINT";
        //    if (type == typeof(float) || type == typeof(float?)) return "FLOAT";
        //    if (type == typeof(double) || type == typeof(double?)) return "DOUBLE PRECISION";
        //    if (type == typeof(decimal) || type == typeof(decimal?)) return "DECIMAL(18,2)";
        //    if (type == typeof(bool) || type == typeof(bool?)) return "BIT";
        //    if (type == typeof(DateTime) || type == typeof(DateTime?)) return "DATETIME";
        //    if (type == typeof(string)) return "NVARCHAR(255)"; // 这里可以调整长度
        //    return "NVARCHAR(MAX)"; // 其他默认处理
        //}

    }
}
