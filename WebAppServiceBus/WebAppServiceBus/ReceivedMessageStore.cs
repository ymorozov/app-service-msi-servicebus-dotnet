using System.Collections.Generic;
using System.Text;
using Amqp;
using WebAppServiceBus.Models;

namespace WebAppServiceBus
{
    public static class ReceivedMessageStore
    {
        private static readonly IReceiverLink _receiverLink = null;
        private static readonly List<string> _receivedMessages = new List<string>();

        public static void Initialize(ServiceBusConfiguration config)
        {
            if (_receiverLink != null)
            {
                return;
            }

            Address address = new Address(config.Host, 5671, null, null, "/", "amqps");
            var connection = new Connection(address);
            TokenHelper.PutCsbToken(connection, config, $"sb://{config.Host}/{config.Topic}");
            var session = new Session(connection);
            var receiver = new ReceiverLink(session, config.Subscription, config.Topic);
            receiver.Start(1, ProcessMessage);
        }

        private static void ProcessMessage(IReceiverLink receiver, Message message)
        {
            var msg = Encoding.UTF8.GetString((byte[])message.Body);
            _receivedMessages.Add(msg);
            receiver.Accept(message);
        }

        public static List<string> GetReceivedMessages()
        {
            List<string> messages = new List<string>(_receivedMessages);
            _receivedMessages.Clear();
            return messages;
        }
    }
}
