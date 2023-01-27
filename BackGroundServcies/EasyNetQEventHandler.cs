using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json;
using System.Text;
using EasyNetQueueExample.ConsumerAPI.Controllers;

namespace EasyNetQueueExample.ConsumerAPI.BackGroundServcies
{
    /// <summary>
    /// EasynetQ Message Event handler 
    /// </summary>
    public class EasyNetQEventHandler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IBus _bus;
        private readonly ILogger<TestServiceHealth> _logger;
        private readonly IConfiguration _configuration;


        /// <summary>
        /// Constror
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public EasyNetQEventHandler(IServiceProvider serviceProvider, ILogger<TestServiceHealth> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;


            logger.LogInformation("Constructor initiliazation started");
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync started");
            try
            {
                //Getting IBus Service from Injected Services
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    _bus = scope.ServiceProvider.GetRequiredService<IBus>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("StartAsync Exception",ex);
                throw;
            }

            return base.StartAsync(cancellationToken);

        }



        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("ExecuteAsync started");

                stoppingToken.ThrowIfCancellationRequested();


                var myQueue = new Queue(_configuration.GetValue<string>("RabbitMqQueueName"), false);

                _bus.Advanced.Consume(myQueue, (body, properties, info) => Task.Factory.StartNew(() =>
                {
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    _logger.LogInformation($"Queue Name:{myQueue.Name} Message Recieved:  {JsonConvert.SerializeObject(message, Formatting.Indented)}");

                    //Do something with the recieved Message here
                    var messageDto = JsonConvert.DeserializeObject<dynamic>(message);
                }));

             
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteAsync Exception", ex);
            }

            return Task.CompletedTask;
        }
    }
}
