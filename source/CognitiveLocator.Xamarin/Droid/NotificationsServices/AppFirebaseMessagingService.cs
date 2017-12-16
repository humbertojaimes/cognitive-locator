using System;
using Android.App;
using Android.Content;
using Firebase.Messaging;

namespace CognitiveLocator.Droid.NotificationsServices
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class AppFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "NHubFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            var messageBody = message.Data["message"];
            if (string.IsNullOrWhiteSpace(messageBody))
                return;
            SendNotification(messageBody);
        }

      

        void SendNotification(string messageBody)
        {
             //Display notification however necessary
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentTitle("FCM Message")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

    }
}
