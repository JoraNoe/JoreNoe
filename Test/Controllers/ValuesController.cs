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
        private readonly IJoreNoeRedisBaseService redisManager;
        private readonly IRepository<User> User;
        private readonly IRepository<Test> Test;
        public ValuesController(IRepository<User> User, IRepository<Test> Test) {
            this.User = User;
            this.Test = Test;
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
        public ActionResult test(testss testok)
        {
            //Console.WriteLine(s);
            //this.redisManager.ConnectionMultiplexer.GetSubscriber();
            //var x = JoreNoeRequestCommonTools.ApiControllerEndpoints();
            //return Ok(x);
            var x = this.User.Single("7d8453db-6ef6-4730-bc3c-e6668a02c87f");
            var xx = this.Test.Single("99541111");
            return null;
        }

        [HttpPost("sdf")]
        public ActionResult cao()
        {
            return null;
        }
    }
    public class testss
    {
        public string asdf { get; set; }
        public string asffds { get; set; }
    }
}
