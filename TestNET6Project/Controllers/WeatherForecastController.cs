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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IJoreNoeRedisBaseService ss)
        {
            _logger = logger;
            //_redisMan22ager = _redisManager;
            //_queueManger = ii;
            this.ss = ss;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public APIReturnInfo<string> Get()
        {
            //var xx = int.Parse("x");

            //this._queueManger.SendPublish("我是张三", "test");


            return APIReturnInfo<string>.Success("测试");

        }

        [HttpGet("MENT")]
        public async Task<ActionResult> Ge1t()
        {
            ss.RedisDataBase.StringSet("sadf", false.ToString());
            var v = ss.RedisDataBase.StringGet("sadf");

            var l = bool.Parse(v);
            return Ok();
        }

    }

    public class s
    {
        public int MyProperty { get; set; }
    }

}
