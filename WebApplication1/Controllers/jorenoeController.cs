using JoreNoe.JoreHttpClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class jorenoeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<jorenoeController> _logger;

        public jorenoeController(ILogger<jorenoeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            // 在程序启动时设置 SSL 回调
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            string appid = "wx5ef5c85e918372b6";
            string scrty = "48f617e850b54e4c7d7a28318052f553";
            var mm = jhk.GetSync($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={scrty}");
            var x = Newtonsoft.Json.JsonConvert.DeserializeObject<data>(mm);
            var mms = jhk.GetSync($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={x.access_token}&type=jsapi");
            var xx = Newtonsoft.Json.JsonConvert.DeserializeObject<data1>(mms);

            ServicePointManager.ServerCertificateValidationCallback -= (sender, certificate, chain, sslPolicyErrors) => true;
            return Ok(new { ticket=xx.ticket }) ;
        }
    }

    public class data
    {
        public string access_token { get; set; }
    }

    public class data1
    {
        public string ticket { get; set; }
    }
}
