using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using GamDroid2018.Helpers;
using GamDroid2018.Utils;
using Fragment = Android.Support.V4.App.Fragment;

namespace GamDroid2018.Fragments
{
    public class TransportApplicationWebView : Fragment
    {
        private readonly IShiftLite _shiftLite;

        public TransportApplicationWebView() : this(new ShiftLite()) { }

        internal TransportApplicationWebView(IShiftLite shiftLite)
        {
            _shiftLite = shiftLite;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.TransportApplicationWebView, container, false);

            var listActiveShift = _shiftLite.GetActiveShifts();

            var tes = listActiveShift.Where(x => x.ShiftType == 0)
                .Select(x=> x.User)
                .FirstOrDefault() ?? "";

            var assistant = listActiveShift.Where(x => x.ShiftType == 1)
                .Select(x => x.User)
                .FirstOrDefault() ?? "";


            var splitUrl = DeviceHelper.RestUrl.Split(':');
            var ip = $"{splitUrl[0]}:{splitUrl[1]}";
            var url = $"{ip}/GamDroidAltaSrv/Home/ListSRV?data={DeviceHelper.Vhi}-{tes}-{assistant}";

            var webView = view.FindViewById<WebView>(Resource.Id.webviewTransportApplication);
            webView.SetWebViewClient(new WebViewClient());
          
            webView.SetWebChromeClient(new WebChromeClient());
            var webSettings = webView.Settings;

            webSettings.JavaScriptEnabled = true;
            webSettings.DomStorageEnabled = true;

            webView.LoadUrl(url);

            return view;
        }
    }
}