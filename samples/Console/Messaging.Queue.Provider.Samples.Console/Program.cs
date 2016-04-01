using Messaging.Queue.Provider.Domain.Enum;
using Messaging.Queue.Provider.Domain.Service;
using Messaging.Queue.Provider.Domain.Service.Factories;
using Messaging.Queue.Provider.Infrastructure.Factories;
using Messaging.Queue.Provider.Samples.Console.Message;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Samples.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container();

            try
            {
                #region DI

                container.RegisterSingleton<IServiceProvider>(container);
                container.Register<IMessageQueueProviderFactory, MessageQueueProviderFactory>();

                #endregion

                var messageQueueProviderFactory = container.GetInstance<IMessageQueueProviderFactory>();

                PushMessage(messageQueueProviderFactory);
                ReceiveMessage(messageQueueProviderFactory);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unexpected error: " + ex.Message);
                System.Console.ReadLine();
            }

            System.Console.ReadLine();
        }

        #region Private Methods

        private static void PushMessage(IMessageQueueProviderFactory providerFactory)
        {
            using (var messageQueue = providerFactory.CreateMsmq<SampleMessage>(Serializer.Json, "message_queue_sample_console"))
            {
                for (int i = 0; i < 5; i++)
                {
                    var sampleMessage = new SampleMessage
                    {
                        SampleMessageId = i,
                        Name = "Sample Message",
                        Created = DateTime.UtcNow
                    };
                    
                    messageQueue.PushAsync(new QueueMessage<SampleMessage>(sampleMessage)).Wait();
                }
            }               
        }

        private static void ReceiveMessage(IMessageQueueProviderFactory providerFactory)
        {
            var isRunning = true;

            using (var messageQueue = providerFactory.CreateMsmq<SampleMessage>(Serializer.Json, "message_queue_sample_console"))
            {
                while (isRunning)
                {
                    QueueMessage<SampleMessage> queueMessage = null;

                    try
                    {
                        queueMessage = messageQueue.ReceiveAsync(new TimeSpan(0, 0, 0, 10, 0)).Result;

                        if (queueMessage != null)
                        {
                            System.Console.WriteLine(string.Format("Removed message from the queue. Body: {0}", queueMessage.Item));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Unexpected error: " + ex.Message);
                    }
                }
            }            
        }

        #endregion
    }
}
