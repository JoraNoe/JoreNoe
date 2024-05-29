using JoreNoe.Cache.Redis;
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRedisManager _redisManager)
        {
            _logger = logger;
            _redisMan22ager = _redisManager;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {


            var a = int.Parse("s");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("MENT")]
        public ActionResult Get(s OK)
        {
            this._redisMan22ager.Get("1");

            return Ok(OK);
        }

    }

    public class s
    {
        public int MyProperty { get; set; }
    }

}
