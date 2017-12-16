using System;
using CognitiveLocator.Droid.Services;
using CognitiveLocator.Interfaces;
using Firebase.Iid;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInstallationService))]
namespace CognitiveLocator.Droid.Services
{
    public class DeviceInstallationService: IDeviceInstallationService
    {
        public string PushChannel => FirebaseInstanceId.Instance.Token;
        public string Platform => "gcm";
        public string InstallationId => Secure.GetString(Android.App.Application.Context.ContentResolver, Secure.AndroidId);
    }
}
