using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;

namespace SBSender.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration _configuration;

        public QueueService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
        {
            var queueClient = new QueueClient(_configuration.GetConnectionString("AzureServiceBus"), queueName);
            string messageBody = JsonSerializer.Serialize(serviceBusMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);

        }
    }
}
