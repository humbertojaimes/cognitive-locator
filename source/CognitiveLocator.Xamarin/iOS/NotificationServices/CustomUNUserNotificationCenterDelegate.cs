using System;
using Foundation;
using UserNotifications;
using UserNotificationsUI;

namespace CognitiveLocator.iOS.NotificationServices
{
    public class CustomUNUserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
       
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

      
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var res = response.Notification;
            completionHandler();
        }

    }
}
