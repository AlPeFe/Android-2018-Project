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
    public class TransportStatus : RealmObject
    {
        [Realms.PrimaryKey]
        public string Id { get; set; }

        public string TransportNumber { get; set; }

        public int Status { get; set; }

        public string Vhi { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Latitude { get; set; }
        
        public string Longitude { get; set; }

        public bool Received { get; set; }
    }

    public class TransportStatusDto
    {
        [Realms.PrimaryKey]
        public string Id { get; set; }

        public string TransportNumber { get; set; }

        public int Status { get; set; }

        public string Vhi { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public bool Received { get; set; }
    }
}