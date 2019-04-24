using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Receiver
{

    class Consumer {

        public delegate void OnMessageHandler(object sender, string message);
        public event OnMessageHandler OnMessage;

        private IQueueClient client;

        public Consumer() {
            client = new QueueClient(Configuration.Instance.ConnectionString, 
                                                    Configuration.Instance.QueueName);
        }

        public void Start() {

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler);
            messageHandlerOptions.MaxConcurrentCalls = 1;
            messageHandlerOptions.AutoComplete = false;
            client.RegisterMessageHandler(ProcessMessages, messageHandlerOptions);

        }

        private async Task ProcessMessages(Message message, CancellationToken token)
        {
            long messageNumber = message.SystemProperties.SequenceNumber;
            string messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received message: SequenceNumber:{messageNumber} Body:{messageBody}");

            if(OnMessage != null) {
                OnMessage(this, messageBody);
            }

            //Remove message from queue
            await client.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler exception {exceptionReceivedEventArgs.Exception.Message}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        public async Task Close() {
            await client.CloseAsync();
        }

    }

}