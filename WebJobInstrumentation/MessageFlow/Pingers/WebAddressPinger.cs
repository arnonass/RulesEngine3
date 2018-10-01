using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace WebJobInstrumentation.MessageFlow.Pingers
{
    public class WebAddressPinger : IPinger
    {
        private readonly ILogger logger;

        public WebAddressPinger(ILogger logger)
        {
            this.logger = logger;
        }

        public bool Ping(string endpoint)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(endpoint);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                // Log exception and continue.
                logger.LogCritical(ex, $"404 - Web endpoint {endpoint} is unavailable.");
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
    }
}
