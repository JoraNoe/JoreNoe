using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ok = new List<string>();
            ok.Add("3444");
            var ment = "1234,53,23";
            var CourseIgnores = ment.Contains(",") ? ment.Split(',').ToList()
                                : ment.Select(d => d.ToString()).ToList();

            // 增加免考课程
            CourseIgnores.AddRange(ok.Select(d => d));
        }
    }
}
