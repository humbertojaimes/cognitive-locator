using System;
using CognitiveLocator.Interfaces;
using CognitiveLocator.iOS.Services;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(DeviceInstallationService))]
namespace CognitiveLocator.iOS.Services
{
    public class DeviceInstallationService : IDeviceInstallationService
    {
        public string InstallationId => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

        public string PushChannel => AppDelegate.InstallationId.Description
                .Trim('<', '>').Replace(" ", string.Empty).ToUpperInvariant();

        public string Template => "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

        public string Platform => "apns";
    }
}
