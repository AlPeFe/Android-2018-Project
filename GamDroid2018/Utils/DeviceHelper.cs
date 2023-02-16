using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GamDroid2018.Utils
{
    public static class DeviceHelper
    {
        public static string RestUrl { get; set; }

        public static string ClientCode { get; set; }

        public static string PushNotificationServiceUrl { get; set; }

        public static string Vhi { get; set; }

        public static bool ActiveShift { get; set; }  
        
        public static bool ActiveBreak { get; set; }

        public static Android.Content.PM.ScreenOrientation Orientation { get; set; }
    }

    public class DeviceConfiguration
    {
        public string RestUrl { get; set; }

        public string ClientCode { get; set; }

        public string PushNotificationServiceUrl { get; set; }
    }


    public class DeviceConfigurationResponse
    {
        public bool Response { get; set; }

        public DeviceConfiguration DeviceConfiguration { get; set; }
    }
}