using System.Text.Json;
using Azure.Storage.Queues;

namespace demo
{
    public class DemoBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;

        public DemoBackgroundService(IConfiguration configuration) => _configuration = configuration;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = _configuration.GetValue<string>("ConnectionString");
            var queueName = _configuration.GetValue<string>("QueueName");
            var queueClient = new QueueClient(connectionString, queueName);
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await queueClient.ReceiveMessageAsync();
                if (message.Value is not null)
                {
                    var body = JsonSerializer.Deserialize<WeatherForecast>(message.Value.Body);
                    await queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
                }
            }
        }
    }
}