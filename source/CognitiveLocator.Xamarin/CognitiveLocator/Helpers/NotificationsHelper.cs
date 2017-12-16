using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CognitiveLocator.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CognitiveLocator.Helpers
{
    public class NotificationsHelper
    {
        static HttpClient client = new HttpClient();
        const string url = "http://hjrcognitivelocatordev-fun.azurewebsites.net/api/devicenotificationsregistrations/";


        public static async Task<bool> RegisterDevice()
        {
            bool registrationResult = false;
            var deviceInstallation =
                DependencyService.Get<IDeviceInstallationService>();

            var jsonDeviceInfo =
                JsonConvert.SerializeObject(deviceInstallation);

            using (var response =
                  await client.PutAsync
                  (url, new StringContent(jsonDeviceInfo, Encoding.UTF8, "application/json")))
            {
                if (response.IsSuccessStatusCode)
                {
                    registrationResult = true;
                }

            }

            return registrationResult;
        }

    }
}
