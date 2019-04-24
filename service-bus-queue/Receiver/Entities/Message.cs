using System;

namespace Receiver.Entities {

    class Message {

        public Guid MessageId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Order Payload { get; set; }

    }

}