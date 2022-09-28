// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.ServiceBus;
using SBShared.Models;
using System.Text;
using System.Text.Json;

class Program
{
    const string connectionString = "ServiceBusConnection";
    const string queueName = "personqueue";

    static IQueueClient _queueClient;

    static async Task Main(string[] args)
    {
        _queueClient = new QueueClient(connectionString, queueName);

        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

        Console.ReadLine();

        await _queueClient.CloseAsync();
    }

    private static async Task ProcessMessageAsync(Message message, CancellationToken token)
    {
        var jsonString = Encoding.UTF8.GetString(message.Body);

        Person person = JsonSerializer.Deserialize<Person>(jsonString);
        Console.WriteLine($"Person Received: {person.FirstName} {person.LastName}");
        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);         
    }

    private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
    {
        Console.WriteLine($"Message handler exception:{ arg.Exception }");
        return Task.CompletedTask;
    }
}

