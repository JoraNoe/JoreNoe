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
        private readonly IQueueManger _queueManger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRedisManager _redisManager,IQueueManger ii)
        {
            _logger = logger;
            _redisMan22ager = _redisManager;
            _queueManger = ii;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public APIReturnInfo<string> Get()
        {
            //var xx = int.Parse("x");

            this._queueManger.SendPublish("我是张三","test");


            return APIReturnInfo<string>.Success("测试");
         
        }

        [HttpGet("MENT")]
        public async Task<ActionResult> Ge1t()
        {
            //var  v = await this._redisMan22ager.GetAsync("1").ConfigureAwait(false);

            return Ok();
        }

    }

    public class s
    {
        public int MyProperty { get; set; }
    }

}
