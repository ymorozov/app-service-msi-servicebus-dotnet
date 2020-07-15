using System;
using System.Net;
using Amqp;
using Amqp.Framing;
using WebAppServiceBus.Models;
using WebAppServiceBus.MSI;

namespace WebAppServiceBus
{
    public class TokenHelper
    {
        public static void PutCsbToken(Connection connection, ServiceBusConfiguration config, string audience)
        {
            var tokenProvider = new ManagedIdentityTokenProvider();
            var token = tokenProvider.GetTokenAsync($"sb://{config.Host}/");

            var session = new Session(connection);

            var cbsClientAddress = "cbs-client-reply-to";
            var cbsSender = new SenderLink(session, config.Subscription, "$cbs");
            var cbsReceiver = new ReceiverLink(session, cbsClientAddress, "$cbs");

            // construct the put-token message
            var request = new Message(token.TokenValue)
            {
                Properties = new Properties
                {
                    MessageId = Guid.NewGuid().ToString(), 
                    ReplyTo = cbsClientAddress
                },
                ApplicationProperties = new ApplicationProperties
                {
                    ["operation"] = "put-token",
                    ["type"] = token.TokenType,
                    ["expiration"] = token.ExpiresAtUtc,
                    ["name"] = audience
                }
            };

            cbsSender.SendAsync(request).Wait();
            Trace.WriteLine(TraceLevel.Information, " request: {0}", request.Properties);
            Trace.WriteLine(TraceLevel.Information, " request: {0}", request.ApplicationProperties);

            // receive the response
            var response = cbsReceiver.ReceiveAsync().Result;
            if (response?.Properties == null || response.ApplicationProperties == null)
            {
                throw new Exception("invalid response received");
            }

            // validate message properties and status code.
            Trace.WriteLine(TraceLevel.Information, " response: {0}", response.Properties);
            Trace.WriteLine(TraceLevel.Information, " response: {0}", response.ApplicationProperties);
            int statusCode = (int)response.ApplicationProperties["status-code"];
            if (statusCode != (int)HttpStatusCode.Accepted && statusCode != (int)HttpStatusCode.OK)
            {
                throw new Exception("put-token message was not accepted. Error code: " + statusCode);
            }

            // the sender/receiver may be kept open for refreshing tokens
            cbsSender.Close();
            cbsReceiver.Close();
            session.Close();
        }
    }
}
