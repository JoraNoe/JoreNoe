using JoreNoe.Cache.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet()]
        public ActionResult ok1()
        {
            this.redisManager.ConnectionMultiplexer.GetSubscriber();
            return Ok("23");
        }

        [HttpPost("test")]
        public ActionResult ok1(testss s)
        {
            Console.WriteLine(s);
            //this.redisManager.ConnectionMultiplexer.GetSubscriber();
            return Ok("23");
        }
    }
    public class testss
    {
        public string asdf { get; set; }
        public string asffds { get; set; }
    }
}
