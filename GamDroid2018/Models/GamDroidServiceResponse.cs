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
    public class GamDroidServiceResponse
    {
        public string ServiceName { get; set; }

        public ResponseStatus Response { get; set; }

        public string Observations { get; set; }
    }

    public enum ResponseStatus
    {
        OK = 0, MISSING_INFORMATION = 1, SERVER_ERROR = 2
    }
}