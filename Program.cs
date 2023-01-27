using EasyNetQ;
using EasyNetQueueExample.ConsumerAPI.BackGroundServcies;
using Serilog;

try
{
	var builder = WebApplication.CreateBuilder(args);

    //Getting Configurations from appsettings.json
    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

	//initializing Serilog
	Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.Enrich.WithThreadId()
			.Enrich.FromLogContext()
			.CreateLogger();

	builder.Logging.ClearProviders();
	builder.Logging.AddSerilog(Log.Logger);


	Log.Information("Started Program.cs");

	//Register EasyNetQ IBus
	IBus bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMQConnectionString"));
	builder.Services.AddSingleton(bus);

	//Register EasyNetQ message event handler
    builder.Services.AddHostedService<EasyNetQEventHandler>();

    builder.Services.AddSingleton(configuration);



	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
	{
		app.UseSwagger();

		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (Exception ex)
{
    Log.Error("Exception ",ex);
    throw;
}

