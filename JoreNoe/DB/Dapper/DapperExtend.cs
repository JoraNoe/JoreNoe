using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public static string GetEntityFiledParams<T>(T Data)
        {

            Type type = Data.GetType();
            PropertyInfo[] properties = type.GetProperties();
            // 使用线程安全的集合来存储处理结果
            var resultBag = new List<object>();

            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(InsertIgnoreAutoIncrementAttribute)))
                    continue;
                //string propertyName = property.Name;
                var propertyValue = property.GetValue(Data, null); // 获取属性值
                // 根据属性的类型判断是否需要添加单引号
                var processedValue = (propertyValue is string || propertyValue is DateTime) ? $"'{propertyValue}'" : propertyValue;
                resultBag.Add(processedValue);
            }

            // 合并结果
            object[] results = resultBag.ToArray();
            string result = string.Join(",", results);
            return result;
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

                default:
                    throw new NotSupportedException($"The expression type '{expression.NodeType}' is not supported.");
            }
        }

        private static string ProcessBinaryExpression(BinaryExpression expression)
        {
            var left = ProcessExpression(expression.Left);
            var right = ProcessExpression(expression.Right);
            var operatorSymbol = GetOperatorSymbol(expression.NodeType);

            return $"{left} {operatorSymbol} {right}";
        }

        private static string ProcessLogicalExpression(BinaryExpression expression)
        {
            var left = ProcessExpression(expression.Left);
            var right = ProcessExpression(expression.Right);
            var operatorSymbol = GetOperatorSymbol(expression.NodeType);

            return $"({left} {operatorSymbol} {right})";
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
    }

}
