using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using CognitiveLocator.iOS.Classes;
using System;
using UserNotifications;
using CognitiveLocator.iOS.NotificationServices;

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

           
            //Create accept action
            var replyId = "reply";
            var replyTitle = "Reply";
            var replyAction = UNTextInputNotificationAction.FromIdentifier(replyId, replyTitle, UNNotificationActionOptions.None,"Reply","Message");

           


            //Create category
            var categoryID = "general";
            var actions = new UNNotificationAction[] { replyAction };
            var intentIDs = new string[] { };
            var categoryOptions = new UNNotificationCategoryOptions[] { };
            var category = UNNotificationCategory.FromIdentifier(categoryID, actions, intentIDs, UNNotificationCategoryOptions.None);

            //Register category
            var categories = new UNNotificationCategory[] { category };
            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));



            UNUserNotificationCenter.Current.Delegate = new CustomUNUserNotificationCenterDelegate();


            if (options!=null && options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
            {
                appIsStarting = true;
            }

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
            appIsStarting = false;
        }


        #region Notification

        bool appIsStarting = false;

        [Export("applicationDidEnterBackground:")]
        public void DidEnterBackground(UIApplication application)
        {
            appIsStarting = false;
        }


        public override void WillEnterForeground(UIApplication uiApplication)
        {
            appIsStarting = true;
        }


        [Export("applicationWillResignActive:")]
        public void OnResignActivation(UIApplication application)
        {
            appIsStarting = false;
        }


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
            UIApplicationState state = application.ApplicationState;
            if (state == UIApplicationState.Background ||
                (state == UIApplicationState.Inactive &&
                !appIsStarting))
            {
                NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

                var messageKey = new NSString("alert");
                string message = null;
                if (aps.ContainsKey(messageKey))
                    message = (aps[messageKey] as NSString).ToString();

                ShowAlert(message);

            }
            else if (state == UIApplicationState.Inactive && appIsStarting)
            {
                NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

                var messageKey = new NSString("alert");
                string message = null;
                if (aps.ContainsKey(messageKey))
                    message = (aps[messageKey] as NSString).ToString();

                ShowAlert(message);
                // user tapped notification
                //completionHandler(UIBackgroundFetchResult.NewData);

            }
            else
            {
                NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

                var messageKey = new NSString("alert");
                string message = null;
                if (aps.ContainsKey(messageKey))
                    message = (aps[messageKey] as NSString).ToString();

                ShowAlert(message);
                // app is active             
                //completionHandler(UIBackgroundFetchResult.NoData);
            }



            void ShowAlert(string message)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        var alert = UIAlertController.Create(
                            "Notificación",
                            $"{message} {appIsStarting}",
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
            }



        }
        #endregion
    }
}