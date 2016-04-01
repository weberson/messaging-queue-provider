# Message Queue Provider

Message.Queue.Provider is a simple and generic implemetation for to peek, push and receive messaging.

## Providers

<a href="https://azure.microsoft.com/services/service-bus/" rel="NuGet">Service Bus</a>. </br>
<a href="https://msdn.microsoft.com/en-us/library/ms711472(v=vs.85).aspx" rel="NuGet">Microsoft Message Queuing</a>.

The message queue factory contains all providers create methods:

```c#
CreateMsmq<T>
CreateSbQueue<T>
```

## Getting Started

To get started with the Message.Queue.Provider, install <a href="https://www.nuget.org/packages/Messaging.Queue.Provider/" rel="NuGet">nuget package</a>.

### Sample with Msmq

```c#
// Dependency Injection (DI) using Simple Injector
var container = new Container();
container.RegisterSingleton<IServiceProvider>(container);
container.Register<IMessageQueueProviderFactory, MessageQueueProviderFactory>();
```

###### Push 

```c#
var messageQueueProviderFactory = container.GetInstance<IMessageQueueProviderFactory>();

var sampleMessage = new SampleMessage
{
  SampleMessageId = 1,
  Name = "Sample Message",
  Created = DateTime.UtcNow
};

using (var messageQueue = providerFactory.CreateMsmq<SampleMessage>(Serializer.Json, 
"message_queue_sample"))
{
  await messageQueue.PushAsync(new QueueMessage<SampleMessage>(sampleMessage)).Wait();
  System.Console.WriteLine($"Push message successfully. Body: {queueMessage.Item}");
}
```

###### Receive

```c#
var messageQueueProviderFactory = container.GetInstance<IMessageQueueProviderFactory>();

vvar isRunning = true;

using (var messageQueue = providerFactory.CreateMsmq<SampleMessage>(Serializer.Json, "message_queue_sample"))
{
  while (isRunning)
  {
    try
    {
      var queueMessage = await messageQueue.ReceiveAsync(new TimeSpan(0, 0, 0, 10, 0)).Result;
  
      if (queueMessage != null)
      {
        System.Console.WriteLine($"Removed message from the queue. Body: {queueMessage.Item}");
      }
    }
    catch (TimeoutException ex)
    {
      System.Console.WriteLine($"{ex.Message}");
    }
    catch (Exception e)
    {
      System.Console.WriteLine($"Unexpected error receive message. Error: {ex.Message}");
    }
  }
}
```

###### .config

if using service bus provider

```xml
 <appSettings>
    <!-- Service Bus specific app setings for messaging connections -->
    <add key="sbConnectionString"
      value="Endpoint=sb://[your namespace].servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;
        SharedAccessKey=[your secret]"/>
  </appSettings>
```
