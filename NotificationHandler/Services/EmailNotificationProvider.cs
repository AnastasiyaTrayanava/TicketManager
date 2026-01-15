using NotificationHandler.Interfaces;
using Polly;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Text;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace NotificationHandler.Services
{
	public class EmailNotificationProvider : INotificationProvider
	{
		private string _sendGridApiKey;
		private const string _adminEmail = "a@a.com";
		private const string _adminUserName = "Admin";

		private IRepository<Notification, Guid> _notificationRepository;

		public EmailNotificationProvider(IRepository<Notification, Guid> notificationRepository)
		{
			_sendGridApiKey = Environment.GetEnvironmentVariable("SEND_GRID_API_KEY") ?? string.Empty;
			_notificationRepository = notificationRepository;
		}

		public async Task SendNotification(Notification notification)
		{
			var client = new SendGridClient(_sendGridApiKey);

			var from = new EmailAddress(_adminEmail, _adminUserName);
			var subject = "New Order Placed Notification";
			var to = new EmailAddress(_adminEmail, _adminUserName);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, BuildEmailBody(notification), null);

			var response = await Policy
				.HandleResult<Response>(r => !r.IsSuccessStatusCode)
				.Or<HttpRequestException>()
				.RetryAsync(3)
				.ExecuteAsync(async () => await client.SendEmailAsync(msg));

			if (response != null && !response.IsSuccessStatusCode)
			{
				notification.RequestStatus = TicketManager.Common.Enums.NotificationRequestStatus.Error;
				await _notificationRepository.UpdateAsync(notification);
				throw new HttpRequestException("Email wasn't delivered.");
			}

			notification.RequestStatus = TicketManager.Common.Enums.NotificationRequestStatus.Sent;
			await _notificationRepository.UpdateAsync(notification);
		}

		private string BuildEmailBody(Notification notification)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"New operation has been performed: {notification.OperationName}");
			stringBuilder.AppendLine($"Notification GUID: {notification.Id}");
			stringBuilder.AppendLine($"Notification parameters: {notification.NotificationParameters}");
			stringBuilder.AppendLine($"Notification content: {notification.NotificationContent}");

			return stringBuilder.ToString();
		}
	}
}
