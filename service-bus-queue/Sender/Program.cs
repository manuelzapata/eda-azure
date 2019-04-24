using System;
using Newtonsoft.Json;
using Sender.Entities;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {

            Producer producer = new Producer();

            int command = 1;
            while(command == 1) {

                var message = new Message {
                    MessageId = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Payload = new Order {
                        OrderId = Guid.NewGuid(),
                        ProductName = "Scooter",
                        Price = 299
                    }
                };

                Console.WriteLine("Sending order with ID " + message.Payload.OrderId);

                producer.SendMessage(message).GetAwaiter().GetResult();
                
                Console.WriteLine("Message sent");

                Console.WriteLine("Do you want to send a new order? \n1. Yes \n2. No");
                command = Convert.ToInt32(Console.ReadLine());
            }

            producer.Close().GetAwaiter().GetResult();

            Console.WriteLine("Done.");

        }
    }
}
