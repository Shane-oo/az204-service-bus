//Endpoint=sb://service-bus-queuezzz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KEMACAVfNGDTlp68di1+ThKqWp847l603Qd6w+X2Hsk=

using Azure.Messaging.ServiceBus;

var queueName = "my-queue";
var connectionString =
    "Endpoint=sb://service-bus-queuezzz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KEMACAVfNGDTlp68di1+ThKqWp847l603Qd6w+X2Hsk=";
ServiceBusClient client;

ServiceBusSender sender;

const int numOfMessages = 3;

var clientOptions = new ServiceBusClientOptions
                    {
                        TransportType = ServiceBusTransportType.AmqpWebSockets
                    };
client = new ServiceBusClient(connectionString,clientOptions);
sender = client.CreateSender(queueName);

// create a batch
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for(var i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if its too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");

    }
}

try
{
    // Use the producer client to send the batch of messages
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");

}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();