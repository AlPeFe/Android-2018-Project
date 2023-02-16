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
using Realms;


namespace GamDroid2018.Models
{
    public class Device : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Vhi { get; set; }

        public string BaseUrl { get; set; }
        
        public string PushNotificationServiceUrl { get; set; }

        public string RestServiceUrl { get; set; }

        public string ClientCode { get; set; }

        public bool Orientation { get; set; }


    }


    public class DeviceDto
    {
        public string Id { get; set; }

        public string Vhi { get; set; }

        public string BaseUrl { get; set; }

        public string PushNotificationServiceUrl { get; set; }

        public string RestServiceUrl { get; set; }

        public string ClientCode { get; set; }

        public bool Orientation { get; set; }

    }
}