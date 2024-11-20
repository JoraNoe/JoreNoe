using JoreNoe.Extend;
using JoreNoe.Limit;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using JoreNoe.Extend;
using static JoreNoe.Extend.BooleanExtend;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Web;
using Newtonsoft.Json;
namespace ConsoleApp5
{
    public class SmallProgramPhotoGather
    {
        /// <summary>
        /// 进行MD5计算
        /// </summary>
        /// <param name="Value">计算数据</param>
        /// <returns></returns>
        public static string ComputeMD5Hash(string Value)
        {
            using (MD5 md5 = MD5.Create())
            {
                // 将字符串转换为字节数组
                byte[] dataBytes = Encoding.UTF8.GetBytes(Value);

                // 计算哈希值
                byte[] hashBytes = md5.ComputeHash(dataBytes);

                // 将哈希值转换为字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // 将每个字节转换为两位十六进制数
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 进行Md5 验证
        /// </summary>
        /// <param name="Value">计算数据</param>
        /// <param name="hashToVerify">待验证的数据</param>
        /// <returns></returns>
        public static bool VerifyMD5Hash(string Value, string hashToVerify)
        {
            // 计算传入数据的哈希值
            string computedHash = ComputeMD5Hash(Value);

            // 将计算得到的哈希值与传入的哈希值进行比较
            return computedHash.Equals(hashToVerify, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// 将Model拼接成字符串&
        /// "Prop1=PropValue1&Prop2=42"
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string FormatModelProperties<EnType>(EnType model)
        {
            // 使用反射获取模型的属性，并按照 "Prop1=PropValue1&Prop2=42" 的格式拼接 奥利给 
            PropertyInfo[] properties = model.GetType().GetProperties().Where(d => d.Name != "Signature").OrderBy(prop => prop.Name, StringComparer.Ordinal).ToArray();
            string queryString = string.Join("&", properties.Where(d => !string.IsNullOrEmpty(d.GetValue(model) + string.Empty))
                .Select(prop => $"{prop.Name}={prop.GetValue(model)}"));
            return queryString;
        }

        /// <summary>
        /// 通用MD5验签 OKOK
        /// </summary>
        /// <typeparam name="EnType">实体类</typeparam>
        /// <param name="Model">数据</param>
        /// <param name="UnverifiValue">待验签数据</param>
        /// <returns></returns>
        public static Tuple<bool, string> VerificationCommon<EnType>(EnType Model, string UnverifiValue)
        {
            if (Model == null || string.IsNullOrEmpty(UnverifiValue))
                return new Tuple<bool, string>(false, "数据为空，验签无法进行");

            var HasKey = "00011469004a4d5f8f0f71ce628ddb11";
            if (HasKey == null)
                return new Tuple<bool, string>(false, "验签秘钥，获取为空，请检查配置文件！");

            // 进行自己验证 拼接起来 哦了哦了
            var GetMD5Value = string.Concat(FormatModelProperties(Model), "&Key=", HasKey);

            // 好了开始验证了
            var ProgressAuthenticity = VerifyMD5Hash(GetMD5Value, UnverifiValue);

            return new Tuple<bool, string>(ProgressAuthenticity, !ProgressAuthenticity ? "验签失败" : "验签成功");
        }
    }
    internal class Program
    {
        private static readonly string url = "https://jorenoe.top/dogegg/api/File/IpAddress";
        private static readonly int concurrentRequests = 100; // 设置并发数
        private static readonly int totalRequests = 1000; // 设置总请求数
        private static int successCount = 0;
        private static int failureCount = 0;

        private static async Task RunLoadTest()
        {
            using HttpClient client = new HttpClient();
            SemaphoreSlim semaphore = new SemaphoreSlim(concurrentRequests);

            Task[] tasks = new Task[totalRequests];
            for (int i = 0; i < totalRequests; i++)
            {
                await semaphore.WaitAsync();
                tasks[i] = Task.Run(async () =>
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            Interlocked.Increment(ref successCount);
                        }
                        else
                        {
                            Interlocked.Increment(ref failureCount);
                        }
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref failureCount);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            await Task.WhenAll(tasks);
        }

        static void vo(long s)
        {
            Task.Run(async () =>
            {
                while (true)
                {

                    DateTime a = DateTime.Now;
                    if (a.Minute == 01)
                        break;

                    //Console.WriteLine("进程：" + s + "_" + DateTime.Now);
                    await System.Threading.Tasks.Task.Delay(3000);
                    Console.WriteLine("执行完一次");
                }
            });
        }

        static async Task Main(string[] args)
        {
            //Console.WriteLine("开始并发压力测试...");
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            //await RunLoadTest();

            //stopwatch.Stop();
            //Console.WriteLine($"并发测试完成，共耗时: {stopwatch.Elapsed.TotalSeconds} 秒");
            //Console.WriteLine($"成功请求数: {successCount}, 失败请求数: {failureCount}");


            //var xx = SmallProgramPhotoGather.ComputeMD5Hash("Id=" + 123 + "&Key=" + "00011469004a4d5f8f0f71ce628ddb11");

            //Parallel.For(0, 900000000000, e =>
            //{
            //    vo( e);
            //});



            //// 主线程继续运行，避免立即退出
            //Console.WriteLine("主线程继续运行...");
            //Console.ReadLine(); // 等待用户输入，保持主线程活动

            // CalcSemesterYear(241,220);

            //var xx1 = SmallProgramPhotoGather.ComputeMD5Hash("CourseId=" + "04383" + "CourseName=" + "穿出你的优雅——服饰搭配" + "Phone=" + "99999999999" + "&Key=" + "00011469004a4d5f8f0f71ce628ddb11");


            //var xx12 = SmallProgramPhotoGather.ComputeMD5Hash("PhoneNumber=" + "18583857276" + "&Key=" + "00011469004a4d5f8f0f71ce628ddb11");



            //var xx112 = SmallProgramPhotoGather.ComputeMD5Hash("Id=" + "7152b154c8b54fa4aea56188f0d0cb" + "&Key=" + "00011469004a4d5f8f0f71ce628ddb11");

            string json = @"[
  {
    ""CreatePreExamReviewModel"": {
      ""ExamName"": ""string"",
      ""ExamCode"": ""string"",
      ""StudentNumber"": ""string"",
      ""StudentName"": ""string"",
      ""SpecialtyName"": ""string"",
      ""SpecialtyCode"": ""string"",
      ""CourseName"": ""string"",
      ""CourseCode"": ""string"",
      ""IsExam"": true,
      ""InSemesterId"": ""string"",
      ""InSemesterName"": ""string"",
      ""SpecialtyRuleId"": ""string"",
      ""LiveCourseCountTotal"": 0,
      ""LiveCourseCountSuccess"": 0,
      ""XkHomeWorkCountTotal"": 0,
      ""XkHomeWorkCountSuccess"": 0,
      ""StudyPress"": 0,
      ""OrganizationCode"": ""string"",
      ""Id"": ""string""
    },
    ""CreatePreExamReviewResultModel"": {
      ""PreExamReviewId"": ""string"",
      ""CourseName"": ""string"",
      ""CourseCode"": ""string"",
      ""LiveCourseName"": ""string"",
      ""LiveCourseCode"": ""string"",
      ""LookLong"": ""string"",
      ""LookGoBackLong"": ""string"",
      ""LookTotalLong"": ""string"",
      ""Is100Minture"": true,
      ""Id"": ""string""
    },
    ""CreatePreExamReviewXkHomeWorkModel"": {
      ""PreExamReviewId"": ""string"",
      ""XkTaskName"": ""string"",
      ""XkScore"": 0,
      ""IsCommint"": true,
      ""Id"": ""123444424""
    }
  }
]";

            var xxx = JsonConvert.SerializeObject(json, formatting: Newtonsoft.Json.Formatting.None);
            var xx = MD5q(xxx + "152EB4973567D7FD873955435D172A7E");
        }

        public static string MD5q(string data)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static Tuple<string, string> CalcSemesterYear(int SemesterId, int InSemesterId)
        {
            var SemesterIds = new List<string>();
            for (int i = 0; i < SemesterId - InSemesterId+1; i++)
            {
                var AddSemester = InSemesterId;
                AddSemester = AddSemester - 1 + i;
                AddSemester++;
                if (AddSemester.ToString()[^1].ToString() == "0" || AddSemester.ToString()[^1].ToString() == "1")
                {
                    SemesterIds.Add(AddSemester.ToString());
                }
            }

            var Year = int.Parse( SemesterId.ToString()[..2]) - int.Parse( InSemesterId.ToString()[..2]);
            return new Tuple<string,string>(Year.ToString(),string.Join(",",SemesterIds));
        }
        public void test()
        {

            Console.WriteLine("测试看看行不");
        }

        public static IEnumerable<int> GenerateFibonacci()
        {
            int prev = 0, current = 1;
            yield return prev; // F(0)
            yield return current; // F(1)

            for (int i = 2; i < 10; i++)
            {
                int next = prev + current;
                yield return next;
                prev = current;
                current = next;
            }
        }


        static IEnumerable<string> Contin(IList<string> Strings)
        {
            foreach (var item in Strings)
            {
                if (item.Length > 3) yield return item;
            }
        }

        static IEnumerable<int> ONumber()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                    yield return i;
            }
        }

        static IEnumerable<int> Numbers()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return i;
            }
        }


        static IEnumerator<int> mm()
        {
            for (int i = 0; i < 100; i++)
            {
                yield return i;
            }
        }

        static IEnumerable<IList<int>> ints()
        {
            var cc = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                cc.Add(i);
                if (cc.Count == 5)
                {
                    yield return cc;
                    cc.Clear();
                }
            }
        }
    }

    public class TreeNode
    {
        public int value { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        public IEnumerable<TreeNode> traverse()
        {
            yield return this;
            foreach (var item in Children)
            {
                foreach (var child in item.traverse())
                {
                    yield return child;
                }
            }
        }
    }

}
