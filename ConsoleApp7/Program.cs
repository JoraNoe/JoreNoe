using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ConsoleApp7
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var date1 = new DateTime(2024,4,01);
            //var datte2 = DateTime.Now;

            //var month = CalculateDifferenceInMonthsAndDays(date1,datte2);
            //var UseMonth = month.months;
            //UseMonth = UseMonth + (month.days >= 15 ? 1 : 0);
            //var returnMonth = UseMonth;

            for (int i = 0; i < 1001; i++) {
                await GetAsync("http://localhost:6631/api/File/IpAddress");

            }

        }

        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 发起 GET 请求
        /// </summary>
        /// <param name="url">请求的 URL</param>
        /// <returns>返回响应的字符串</returns>
        public static async Task<string> GetAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 抛出异常如果 HTTP 状态码不成功
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求失败: {ex.Message}");
                throw;
            }
        }


        private static string CalcYear(int year, int semester)
        {
            string[] chineseNumbers = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            if (semester % 2 != 0)
            {
                var strins = string.Concat("第", chineseNumbers[year == 0 ? 0 : year - 1], "学年", "；", "第", chineseNumbers[0], "学期");
                return strins;
            }
            else if (semester % 2 == 0)
            {
                var strins = string.Concat("第", chineseNumbers[year == 0 ? 0 : year - 1], "学年", "；", "第", chineseNumbers[1], "学期");
                return strins;
            }
            else
            {
                return string.Concat("第", chineseNumbers[year == 0 ? 0 : year - 1], "学年", "；", "第", chineseNumbers[semester - 1], "学期");
            }
        }

        static (int months, int days) CalculateDifferenceInMonthsAndDays(DateTime start, DateTime end)
        {
            if (start > end)
            {
                return (0, 0);
            }

            int months = (end.Year - start.Year) * 12 + end.Month - start.Month;

            // 如果结束日小于开始日，需要减少一个月
            if (end.Day < start.Day)
            {
                months--;
                // 计算天数差
                DateTime previousMonth = end.AddMonths(-1);
                int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
                int days = end.Day + (daysInPreviousMonth - start.Day);
                return (months, days);
            }
            else
            {
                // 天数为直接的日差
                int days = end.Day - start.Day;
                return (months, days);
            }
        }

    }
}
