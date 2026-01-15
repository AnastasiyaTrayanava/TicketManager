using NotificationHandler;
using NotificationHandler.Interfaces;
using NotificationHandler.Services;
using RabbitMQ.Client;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.DAL.Repositories;

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

builder.Services.AddScoped<IRepository<Notification, Guid>, NotificationRepository>();

builder.Services.AddScoped<INotificationProvider, EmailNotificationProvider>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();