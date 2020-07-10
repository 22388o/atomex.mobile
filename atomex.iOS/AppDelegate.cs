﻿using System;
using System.Runtime.InteropServices;
using Atomex.Common;
using Foundation;
using UIKit;
using UserNotifications;

using atomex.Common.FileSystem;

namespace atomex.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public string DeviceToken { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            FileSystem.UseFileSystem(new IosFileSystem());


            global::Xamarin.Forms.Forms.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            //after iOS 10
            //if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            //{
            //    UNUserNotificationCenter center = UNUserNotificationCenter.Current;

            //    center.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge, (bool arg1, NSError arg2) =>
            //    {

            //    });

            //    center.Delegate = new iOSNotificationReceiver();
            //}

            //else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //{

            //    var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());

            //    UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

            //}

            UNUserNotificationCenter.Current.Delegate = new iOSNotificationReceiver();
            
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //DeviceToken = Regex.Replace(deviceToken.ToString(), "[^0-9a-zA-Z]+", "");

            byte[] result = new byte[deviceToken.Length];
            Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
            DeviceToken = BitConverter.ToString(result).Replace("-", "");
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            new UIAlertViewDelegate();
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            
        }
    }
}
