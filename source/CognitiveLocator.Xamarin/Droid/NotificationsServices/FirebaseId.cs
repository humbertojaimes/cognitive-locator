using System;
using System.Threading.Tasks;
using Android.App;
using Firebase.Iid;

namespace CognitiveLocator.Droid.NotificationsServices
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
            Task.Run(async () =>
            {
                await Helpers.NotificationsHelper.RegisterDevice();
            });
        }

    }
}
