using Azure.Messaging.ServiceBus;

var connectionString =
    "Endpoint=sb://service-bus-topiczzz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=WB0RkE3RmVEx+J1d4S1QCqPw+BZLmrUI0JsymD/hJcA=";
var topicName = "my-topic";

ServiceBusClient client;
ServiceBusSender sender;

const int numOfMessages = 3;

client = new ServiceBusClient(connectionString);
sender = client.CreateSender(topicName);

// Create a batch
using var messageBatch = await sender.CreateMessageBatchAsync();

for(var i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send batch
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");
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
