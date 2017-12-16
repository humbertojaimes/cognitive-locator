using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using CognitiveLocator.iOS.Classes;
using System;

namespace CognitiveLocator.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static MobileServiceClient MobileClient = null;

        public static NSData InstallationId { get; private set; } = null;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)PListCSLight.readPlist("Info.plist");

            Profile.EnableUpdatesOnAccessTokenChange(true);
            Facebook.CoreKit.Settings.AppID = dict.GetValueOrDefault("FacebookAppID").ToString();
            Facebook.CoreKit.Settings.DisplayName = dict.GetValueOrDefault("FacebookDisplayName").ToString();

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);

            var settings = UIUserNotificationSettings.GetSettingsForTypes(
               UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
               new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();


            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            AppEvents.ActivateApp();
        }


        #region Notification
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            AppDelegate.InstallationId = deviceToken;
        }

        public override void FailedToRegisterForRemoteNotifications(
            UIApplication application,
            NSError error)
        {
            // TODO: Show error
        }

        public override void DidReceiveRemoteNotification(
            UIApplication application,
            NSDictionary userInfo,
            Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            var messageKey = new NSString("alert");
            var silentMessageKey = new NSString("content-available");
            var actionKey = new NSString("action");

            string message = null;
            if (aps.ContainsKey(messageKey))
                message = (aps[messageKey] as NSString).ToString();

            // Show alert
            if (!string.IsNullOrEmpty(message))
            {
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    var alert = UIAlertController.Create(
                        "Notification",
                        message,
                        UIAlertControllerStyle.Alert);

                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));

                    var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
                    while (vc.PresentedViewController != null)
                    {
                        vc = vc.PresentedViewController;
                    }

                    vc.ShowDetailViewController(alert, vc);
                });
            }

            // If message template is silent message, parse action param
            if (aps.ContainsKey(silentMessageKey))
            {
                System.Diagnostics.Debug.WriteLine($"[PNS] Silent message received");
                var action = userInfo.ObjectForKey(new NSString("action")) as NSString;

                System.Diagnostics.Debug.WriteLine($"[PNS] Action required of type: {action}");
            }
        }
        #endregion
    }
}