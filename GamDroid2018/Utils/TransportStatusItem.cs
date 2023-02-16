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
    public class TransportStatusItem
    {
        public string Description { get; set; }

        public int Status { get; set; }

        public Android.Graphics.Color Colour { get; set; }
    }

}