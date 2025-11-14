using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.Services
{
	public class NotificationService : INotificationService
	{
		public async Task AddNotification(NotificationPayload payload)
		{
			var factory = new ConnectionFactory { HostName = "localhost" };
			await using var connection = await factory.CreateConnectionAsync();
			await using var channel = await connection.CreateChannelAsync();

			await channel.QueueDeclareAsync(queue: "ticketNotification", durable: false, exclusive: false, autoDelete: false, arguments: null);

			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
			await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "ticketNotification", body: body);

			await connection.CloseAsync();
		}
	}
}
