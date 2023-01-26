// See https://aka.ms/new-console-template for more information

using Azure.Messaging.ServiceBus;

var connectionString =
    "Endpoint=sb://service-bus-topiczzz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=WB0RkE3RmVEx+J1d4S1QCqPw+BZLmrUI0JsymD/hJcA=";
var subscriptionName = "my-subscription";
var topicName = "my-topic";

ServiceBusClient client;

ServiceBusProcessor processor;

async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body} from subscription: {subscriptionName}");

    // complete the message. messages is deleted from the subscription. 
    await args.CompleteMessageAsync(args.Message);  
}

// handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}


client = new ServiceBusClient(connectionString);
processor = client.CreateProcessor(topicName,subscriptionName,new ServiceBusProcessorOptions());

try
{
    // add handler to process messages
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;
    // start processing
    await processor.StartProcessingAsync();
    
    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    // stop processing 
    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.DisposeAsync();
    await client.DisposeAsync();
}