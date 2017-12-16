using CognitiveLocator.Domain.Documents;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveLocator.Functions.Helpers
{
    class NotificationsHelper
    {
        
        public static async Task RegisterDevice(DeviceInstallation deviceUpdate)
        {
            Installation installation = new Installation();
            installation.InstallationId = deviceUpdate.InstallationId;
            installation.PushChannel = deviceUpdate.PushChannel;
            switch (deviceUpdate.Platform)
            {
                case "apns":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                case "gcm":
                    installation.Platform = NotificationPlatform.Gcm;
                    break;
                default:
                    throw new Exception("Invalid Channel");
            }
            installation.Tags = new List<string>();
            //installation.Tags.Add($"requestid:{requestId}");
            //Dictionary<string, InstallationTemplate> templates = new Dictionary<string, InstallationTemplate>();
            //templates.Add("template", new InstallationTemplate { Body = deviceUpdate.Template });
            await Client.NotificationClient.Instance.Hub.CreateOrUpdateInstallationAsync(installation);
        }


        public static async Task AddToGroup(string installationId, string newGroup)
        {
            await AddTag(installationId, $"groupid:{newGroup}");
        }

        public static async Task AddToRequest(string installationId, string requestId)
        {
            await AddTag(installationId, $"requestId:{requestId}");
        }

        public static async Task RemoveFromGroup(string installationId, string newGroup)
        {
            await RemoveTag(installationId, $"groupid:{newGroup}");
        }

        public static async Task RemoveFromRequest(string installationId, string requestId)
        {
            await RemoveTag(installationId, $"requestId:{requestId}");
        }

        private static async Task RemoveTag(string installationId, string tag)
        {
            try
            {
                Installation installation = await Client.NotificationClient.Instance.Hub.GetInstallationAsync(installationId);
                if (installation.Tags == null)
                {
                    if (installation.Tags.Contains(tag))
                        installation.Tags.Remove(tag);
                    await Client.NotificationClient.Instance.Hub.CreateOrUpdateInstallationAsync(installation);
                }
            }
            catch (Exception ex)
            {
#warning No error handling during exception
            }
        }

        private static async Task AddTag(string installationId, string newTag)
        {
            try
            {
                Installation installation = await Client.NotificationClient.Instance.Hub.GetInstallationAsync(installationId);
                if (installation.Tags == null)
                    installation.Tags = new List<string>();
                installation.Tags.Add(newTag);
                await Client.NotificationClient.Instance.Hub.CreateOrUpdateInstallationAsync(installation);
            }
            catch (Exception ex)
            {
#warning No error handling during exception
            }
        }

        public static async Task RemoveDevice(string installationId)
        {
            await Client.NotificationClient.Instance.Hub.DeleteInstallationAsync(installationId);
        }


        public static async Task SendNotification(bool isErrorNotification, string text, string requestId, string installationId)
        {
            try
            {
                Installation installation = await Client.NotificationClient.Instance.Hub.GetInstallationAsync(installationId);

                if (installation.Platform == NotificationPlatform.Gcm)
                {
                    var json = string.Format("{{\"data\":{{\"message\":\"{0}\"}}}}", text);
                    await Client.NotificationClient.Instance.Hub.SendGcmNativeNotificationAsync(json, $"requestid:{requestId}");
                }
                else
                {
                    var json = string.Format("{{\"aps\":{{\"alert\":\" {0}\"}}}}", text);
                    await Client.NotificationClient.Instance.Hub.SendAppleNativeNotificationAsync(json, $"requestid:{requestId}");
                }
            }
            catch (Exception ex)
            {
#warning No error handling during exception
            }
        }

        public static async Task SendBroadcastNotification(string text)
        {
            try
            {
                var json = string.Format("{{\"data\":{{\"message\":\"{0}\"}}}}", text);
                await Client.NotificationClient.Instance.Hub.SendGcmNativeNotificationAsync(json);
                json = string.Format("{{\"aps\":{{\"alert\":\" {0}\"}}}}", text);
                await Client.NotificationClient.Instance.Hub.SendAppleNativeNotificationAsync(json);
            }
            catch (Exception ex)
            {
#warning No error handling during exception
            }
        }
    }
}
