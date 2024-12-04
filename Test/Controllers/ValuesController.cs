using JoreNoe.Cache.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [Route("api/v2/[controller]")]
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
    }
}
