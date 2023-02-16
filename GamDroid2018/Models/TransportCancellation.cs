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

namespace GamDroid2018.Models
{
    public class TransportCancellation
    {
        public string TransportNumber { get; set; }

        public string State { get; set; }

        public string SubState { get; set; }

        public string Vhi { get; set; }
    }
}