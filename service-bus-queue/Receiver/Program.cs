using System;
using Newtonsoft.Json;
using Receiver.Entities;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var consumer = new Consumer();

            consumer.OnMessage += (object sender, string messageString) => {

                Message messageObject = JsonConvert.DeserializeObject<Message>(messageString);
                Console.WriteLine("Receiving Order with ID: " + messageObject.Payload.OrderId);

            };

            consumer.Start();

            Console.WriteLine("Press ENTER key to exit after you are done.");
            Console.Read();

            consumer.Close().GetAwaiter().GetResult();

            Console.WriteLine("Done.");
        }
    }
}
