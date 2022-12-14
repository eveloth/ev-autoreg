using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RedisTestController : ControllerBase
    {
        private readonly ITokenDb _tokenDb;

        public RedisTestController(ITokenDb tokenDb)
        {
            _tokenDb = tokenDb;
        }

        [HttpPost]
        [Route("test")]
        public async Task<IActionResult> SetAndGet([FromBody] string value)
        {
            var result = await _tokenDb.TestRedis("testkey", value);

            Console.WriteLine(result + "ГОТОВО");

            return Ok(result);
        }
    }
}