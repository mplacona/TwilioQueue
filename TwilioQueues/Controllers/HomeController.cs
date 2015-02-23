using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Libs;
using RabbitMQ.Client;

namespace TwilioQueues.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return Content("I'm running");
        }

        [HttpPost]
        public ActionResult SayHello(Message incomingMessage)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("messages", false, false, false, null);
                    var message = new JavaScriptSerializer().Serialize(incomingMessage);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", "messages", null, body);
                }
            }
            return Content(incomingMessage.Body);
        }

    }
}
