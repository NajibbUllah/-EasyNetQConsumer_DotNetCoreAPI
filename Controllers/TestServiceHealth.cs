using Microsoft.AspNetCore.Mvc;

namespace EasyNetQueueExample.ConsumerAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/servicehealth")]
    [ApiController]
    public class TestServiceHealth : ControllerBase
    {
        private readonly ILogger<TestServiceHealth> logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public TestServiceHealth(ILogger<TestServiceHealth> logger)
        {
            this.logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("healthcheck")]
        public string Index()
        {
            logger.LogInformation($"The response for the get weather forecast is logger.LogInformation");
            logger.LogDebug($"The response for the get weather forecast is  logger.LogDebug");
            logger.LogWarning($"The response for the get weather forecast is logger.LogWarning");
            logger.LogError($"The response for the get weather forecast is logger.LogError");
            return "Service is healthy and running.";
        }
    }
}
