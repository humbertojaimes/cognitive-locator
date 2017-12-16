using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using CognitiveLocator.Domain.Documents;
using CognitiveLocator.Functions.Helpers;
using System;

namespace CognitiveLocator.Functions.Functions
{
    public static class DeviceNotificationsRegistration
    {
        [FunctionName("DeviceNotificationsRegistration")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "devicenotificationsregistrations/")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("New device registration incoming");
                var content = await req.Content.ReadAsStringAsync();
                DeviceInstallation deviceUpdate = await req.Content.ReadAsAsync<DeviceInstallation>();
                await NotificationsHelper.RegisterDevice(deviceUpdate);
                await NotificationsHelper.SendBroadcastNotification("Nuevo dispositivo");
                log.Info("New device registered");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                log.Info($"Error during device registration: {ex.Message}");
            }
            return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error during device registration");
        }
    }
}
