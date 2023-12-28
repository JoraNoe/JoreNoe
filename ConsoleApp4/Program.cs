using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ment = new List<string>();
            ment.Add("asdf");
            ment.Add("123222");
            ment.Add("fawwef");
           var xx =  GetRedisExistsDataList(ment);
        }

        public static IList<string> GetRedisExistsDataList(List<string> ChannelIds)
        {
            var channelIdsHashSet = new HashSet<string>(ChannelIds);
            var mm = new List<string>();
            mm.Add("asdf");
            mm.Add("jjhh");

            if(mm.Count != 0)
            {
                return mm.Where(d => channelIdsHashSet.Contains(d)).ToList();
            }
            return null;
        }

    }
}
