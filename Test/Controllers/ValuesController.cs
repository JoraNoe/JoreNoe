using Dogegg.Abstractions.Domains;
using JoreNoe.Cache.Redis;
using JoreNoe.DB.Dapper;
using JoreNoe.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace Test.Controllers
{
    public class User { public Guid Id { get; set; } }
    public class Test { public int Id { get; set; } }
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRedisManager redisManager;
        //private readonly IRepository<User> User;
        private readonly IRepository<GlobalRunningHistory> Test;
        //public ValuesController(IRepository<User> User, IRepository<Test> Test) {
        //    this.User = User;
        //    this.Test = Test;
        //}
        public ValuesController(IRedisManager redis, IRepository<GlobalRunningHistory> Test)
        {
            this.Test = Test;
            redisManager = redis;
        }
        
        /// <summary>
        /// 测试1
        /// </summary>
        /// <returns></returns>
        //[HttpGet()]
        //public ActionResult ok1()
        //{
        //    this.redisManager.ConnectionMultiplexer.GetSubscriber();
        //    return Ok("23");
        //}

        [HttpPost("create")]
        public async Task<ActionResult> test(testss testok)
        {
            //Console.WriteLine(s);
            //this.redisManager.ConnectionMultiplexer.GetSubscriber();
            //var x = JoreNoeRequestCommonTools.ApiControllerEndpoints();
            //return Ok(x);
            //var x = this.User.Single("7d8453db-6ef6-4730-bc3c-e6668a02c87f");
            // var xx = this.Test.Single("99541111");
            //var x = new List<GlobalRunningHistory>();
            //x.Add(new GlobalRunningHistory { 
            //    FullPathUrl= "http://jorenoe.top/api/File/IpAddress",
            //    CreateTime= DateTime.Now,
            //    Duration="23",
            //    Headers= "{\"Connection\":[\"close\"],\"Host\":[\"jorenoe.top\"],\"User-Agent\":[\"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0\"],\"X-Real-IP\":[\"104.234.0.22\"],\"X-Forwarded-For\":[\"14.87.126.25, 104.234.0.22\"],\"X-Forwarded-Proto\":[\"https\"]}",
            //    Id = Guid.NewGuid(),
            //});
            //this.Test.BulkInsert(x);
            this.redisManager.Add("s","sdf",TimeSpan.FromDays(1));
            await this.redisManager.UpdateAsync<string>("s", "sdf1");
            return null;
        }

        [HttpPost("sdf")]
        public ActionResult cao()
        {
            this.redisManager.Add("test",2,TimeSpan.FromSeconds(60));

            this.redisManager.Update("test", 3);
            var x = "ok1";
            var mm = "";
            //Parallel.For(1, 10, e =>
            //{
            //    var ax = APIGlobalLimitIntefaceAccessMiddleware.test(x);
            //    Console.WriteLine(ax);
            //});
            return Content(mm);
        }
    }
    public class testss
    {
        public string asdf { get; set; }
        public string asffds { get; set; }
    }
}
