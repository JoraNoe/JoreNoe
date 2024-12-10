using JoreNoe;
using JoreNoe.Cache.Redis;
using JoreNoe.Queue.RBMQ;
using Microsoft.AspNetCore.Mvc;

namespace TestNET6Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRedisManager _redisMan22ager;
        // private readonly IQueueManger _queueManger;
        //IRedisManager _redisManager
        private readonly IJoreNoeRedisBaseService ss;
        private readonly TestUse x;
        private readonly IServiceProvider f;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IJoreNoeRedisBaseService ss,TestUse x,IServiceProvider f)
        {
            _logger = logger;
            //_redisMan22ager = _redisManager;
            //_queueManger = ii;
            this.ss = ss;
            this.x= x;
            this.f = f;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public APIReturnInfo<string> Get()
        {
            //var xx = int.Parse("x");

            //this._queueManger.SendPublish("我是张三", "test");

            var x = f.GetRequiredService<TestUse>();
            x.get();

            return APIReturnInfo<string>.Success("测试");

        }

        [HttpPost("asdf")]
        public ActionResult uploadtest (IFormFile s)
        {

            return Ok(s);
        }

        [HttpGet("MENT")]
        public async Task<ActionResult> Ge1t()
        {
            ss.RedisDataBase.StringSet("sadf", false.ToString());
            var v = ss.RedisDataBase.StringGet("sadf");

            var l = bool.Parse(v);
            return Ok();
        }

        [HttpPost("fff")]
        public ActionResult sfasd(s ss)
        {
            return Ok("asdf");
        }

    }

    public class s
    {
        public int MyProperty { get; set; }
    }

}
