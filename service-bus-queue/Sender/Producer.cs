using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Sender
{

    class Producer {

        private IQueueClient client;

        public Producer() {
            client = new QueueClient(Configuration.Instance.ConnectionString, 
                                                    Configuration.Instance.QueueName);
        }

        public async Task SendMessage(object messageObject) {
            string messageString = JsonConvert.SerializeObject(messageObject);
            await SendMessage(messageString);
        }

        public async Task SendMessage(string message) {
            Console.WriteLine("Message to send: " + message);
            byte[] messageArray = Encoding.UTF8.GetBytes(message);
            var messageObject = new Message(messageArray);

            await client.SendAsync(messageObject);

        }

        public async Task Close() {
            await client.CloseAsync();
        }

    }

}