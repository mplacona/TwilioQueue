using System;
using System.Text;
using System.Web.Script.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Twilio;

namespace QueueReader
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var client = new TwilioRestClient(Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"), Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"));
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("messages", false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("messages", true, consumer);

                    Console.WriteLine(" [*] Waiting for messages." +
                                             "To exit press CTRL+C");
                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        var message = new JavaScriptSerializer().Deserialize<Libs.Message>(Encoding.UTF8.GetString(ea.Body));
                        Console.WriteLine(" [x] Received {0}", message.Body);
                        client.SendSmsMessage(message.To, message.From, "Thanks for seeing me @ IDLConf. Wanna know more about Twilio? Here's $20 to get you started, just use the code {{code}}. Reach me up @marcos_placona");
                    }
                }
            }
        }
    }
}
