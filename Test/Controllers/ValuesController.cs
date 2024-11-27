using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// 测试1
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult ok1()
        {
            return Ok("23");
        }
    }
}
