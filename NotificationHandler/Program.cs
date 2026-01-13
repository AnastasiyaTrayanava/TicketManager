using NotificationHandler;
using NotificationHandler.Interfaces;
using NotificationHandler.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

// RabbitMQ
builder.Services.AddSingleton<IConnection>(x =>
{
	var factory = new ConnectionFactory { HostName = "localhost" };
	return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddScoped<IChannel>(x =>
{
	var connection = x.GetRequiredService<IConnection>();
	return connection.CreateChannelAsync().GetAwaiter().GetResult();
});

builder.Services.AddScoped<INotificationProvider, EmailNotificationProvider>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();