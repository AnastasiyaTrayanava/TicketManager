using Newtonsoft.Json;
using NotificationHandler.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace NotificationHandler
{
	public class Worker(ILogger<Worker> logger, IChannel channel, INotificationProvider notificationProvider, IRepository<Notification, Guid> notificationRepository) : BackgroundService
	{
		private const string _notificationQueueName = "ticketNotification";

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await channel.QueueDeclareAsync(queue: _notificationQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += async (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($" [x] Received {message}");
				logger.LogInformation(message);

				var notification = JsonConvert.DeserializeObject<Notification>(message);
				await ProcessNotification(notification);
			};

			await channel.BasicConsumeAsync(_notificationQueueName, autoAck: true, consumer: consumer);
		}

		private async Task ProcessNotification(Notification notification)
		{
			if (notification == null) return;

			await notificationProvider.SendNotification(notification);
			await SetNotificationInProgress(notification);
		}

		private async Task SetNotificationInProgress(Notification notification)
		{
			notification.RequestStatus = TicketManager.Common.Enums.NotificationRequestStatus.InProgress;
			await notificationRepository.UpdateAsync(notification);
		}
	}
}
