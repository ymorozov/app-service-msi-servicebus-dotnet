using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Amqp;
using WebAppServiceBus.Models;

namespace WebAppServiceBus.Controllers
{
    public class HomeController : Controller
    {
        public ServiceBusConfiguration Config { get; }

        public HomeController(IOptions<ServiceBusConfiguration> serviceBusConfig)
        {
            Config = serviceBusConfig.Value;
            ReceivedMessageStore.Initialize(Config);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> Send(ServiceBusMessageData messageInfo)
        {
            if (string.IsNullOrEmpty(messageInfo.MessageToSend))
            {
                return RedirectToAction("Index");
            }

            Address address = new Address(Config.Host, 5671, null, null, "/", "amqps");
            var connection = new Connection(address);
            TokenHelper.PutCsbToken(connection, Config, $"sb://{Config.Host}/{Config.Topic}");
            var session = new Session(connection);
            var sender = new SenderLink(session, Config.Subscription, Config.Topic);
            sender.Send(new Message(Encoding.UTF8.GetBytes(messageInfo.MessageToSend)));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Receive()
        {
            ServiceBusMessageData messageInfo = new ServiceBusMessageData();

            List<string> receivedMessages = ReceivedMessageStore.GetReceivedMessages();
            if (receivedMessages.Count > 0)
            {
                messageInfo.MessagesReceived = string.Join(Environment.NewLine, receivedMessages);
            }
            else
            {
                messageInfo.MessagesReceived = "No messages from queue received yet!";
            }

            return View("Index", messageInfo);
        }
    }
}
