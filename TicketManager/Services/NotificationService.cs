using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.Services
{
	public class NotificationService : INotificationService
	{
		private IChannel _rabbitMqChannel;
		private IRepository<Notification, Guid> _notificationRepository;

		private const string _notificationQueueName = "ticketNotification";

		public NotificationService(IChannel rabbitMqChannel, IRepository<Notification, Guid> notificationRepository)
		{
			_rabbitMqChannel = rabbitMqChannel;
			_notificationRepository = notificationRepository;
		}

		public async Task AddNotification(Notification payload)
		{
			await _rabbitMqChannel.QueueDeclareAsync(queue: _notificationQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
			await _rabbitMqChannel.BasicPublishAsync(exchange: string.Empty, routingKey: _notificationQueueName, body: body);
		}

		public async Task CreateNotification(string operationName, string customerEmail, string customerName, float orderAmount, List<string> orderSummary)
		{
			var notificationContent = new
			{
				OrderAmount = orderAmount,
				OrderSummary = orderSummary
			};

			var notificationParameters = new 
			{
				CustomerName = customerName,
				CustomerEmail = customerEmail
			};

			var notification = new Notification
			{
				Id = Guid.NewGuid(),
				OperationName = operationName,
				Timestamp = DateTime.UtcNow,
				NotificationContent = JsonConvert.SerializeObject(notificationContent),
				NotificationParameters = JsonConvert.SerializeObject(notificationParameters),
				RequestStatus = NotificationRequestStatus.None
			};

			await _notificationRepository.CreateAsync(notification);
		}
	}
}
