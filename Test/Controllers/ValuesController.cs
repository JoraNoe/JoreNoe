using JoreNoe.Cache.Redis;
using JoreNoe.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IJoreNoeRedisBaseService redisManager;
        public ValuesController(IJoreNoeRedisBaseService s) {
            this.redisManager = s;
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
            var x = JoreNoeRequestCommonTools.ApiControllerEndpoints();
            return Ok(x);
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
