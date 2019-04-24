using System;

namespace Sender.Entities {

    class Order {

        public Guid OrderId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

    }

}