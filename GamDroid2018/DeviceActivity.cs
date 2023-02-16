using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using GamDroid2018.Utils;
using Newtonsoft.Json;
using Realms;
using Serilog;
using Xamarin.Essentials;

namespace GamDroid2018
{
    [Activity(MainLauncher = true, LaunchMode =Android.Content.PM.LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class DeviceActivity : AppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        EditText _pwd;
        EditText _client;
        Button _btnConfig;
        ProgressBar _loadingbar;
        RadioGroup _groupOrientation;
        IDeviceLite _deviceLite;
        LinearLayout _layout;
        EditText _vhi;
        IShiftLite _shiftLite;
        private static DeviceActivity DeviceActivityInstanceCreated;
        public DeviceActivity() : this(new DeviceLite(), new ShiftLite()) { }

        public DeviceActivity(IDeviceLite deviceLite, IShiftLite shiftLite)
        {
            if (DeviceActivityInstanceCreated == null)
            {
                AutoMaperConfig.RegisterMapping();
                _deviceLite = deviceLite;
                _shiftLite = shiftLite;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // REALM DATABASE MIGRATION
            var config = RealmConfiguration.DefaultConfiguration; //gets realm default configuration
            config.SchemaVersion = 4; // increment the schema version each time your Realm models changes its a cutre(no se como se escribe en ingles) workarround but its the easiest way to do it
            //if needed you can also delete the previous database and create a new one with the current schema 

            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath + "/logs.txt")
               .MinimumLevel.Information()
               .CreateLogger();

            GetDeviceConfiguration();

            Log.Information(DateTime.Now.ToShortDateString() + " DeviceActivity OnCreate");

            SetContentView(Resource.Layout.activity_device);
            _pwd = FindViewById<EditText>(Resource.Id.pwd);
            _client = FindViewById<EditText>(Resource.Id.user_client);
            _btnConfig = FindViewById<Button>(Resource.Id.btnConfiguration);
            _loadingbar = FindViewById<ProgressBar>(Resource.Id.loading_bar);
            _vhi = FindViewById<EditText>(Resource.Id.vhi);
            _layout = FindViewById<LinearLayout>(Resource.Id.linearMain);
            _groupOrientation = FindViewById<RadioGroup>(Resource.Id.groupOrientation);
            _loadingbar.Visibility = ViewStates.Gone;

            _btnConfig.Click += async delegate
            {
                if(string.IsNullOrWhiteSpace(_pwd.Text) 
                    || string.IsNullOrWhiteSpace(_client.Text) 
                        || string.IsNullOrWhiteSpace(_vhi.Text))
                {
                    Toast.MakeText(this, "Faltan datos por rellenar", ToastLength.Long).Show();
                    _loadingbar.Visibility = ViewStates.Gone;
                    _layout.Visibility = ViewStates.Visible;
                    return;
                }

                _loadingbar.Visibility = ViewStates.Visible;
                _layout.Visibility = ViewStates.Gone;
                var result = await GetDeviceConfiguration(_client.Text, _pwd.Text);

                if (!result.Response)
                {
                    Toast.MakeText(this, "Los datos introducidos no son correctos", ToastLength.Long).Show();
                    _loadingbar.Visibility = ViewStates.Gone;
                    _layout.Visibility = ViewStates.Visible;
                    return;
                }
                
                SetDeviceConfiguration(result.DeviceConfiguration);          
            };
        }

        protected override void OnStart()
        {
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.AccessCoarseLocation,
                    Manifest.Permission.AccessFineLocation,
                    Manifest.Permission.ReadPhoneState,
                }, 1);
            }

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                PowerManager powerManager = (PowerManager)this.GetSystemService(PowerService);
                String packageName = this.ApplicationContext.ApplicationInfo.PackageName;
                bool ignoringOptimizations = powerManager.IsIgnoringBatteryOptimizations(packageName);

                if (!ignoringOptimizations)
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                    intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                    this.StartActivity(intent);
                }      //
            }

            base.OnStart();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private Android.Content.PM.ScreenOrientation GetSelectedOrientation()
        {
            var selectedItem = _groupOrientation.CheckedRadioButtonId;
            View btn = _groupOrientation.FindViewById(selectedItem);
            var index = _groupOrientation.IndexOfChild(btn);
            var radioBtn =(RadioButton) _groupOrientation.GetChildAt(index);
            var selectedValue = radioBtn.Text.ToString();

            return selectedValue == "Vertical" ? Android.Content.PM.ScreenOrientation.Portrait : Android.Content.PM.ScreenOrientation.Landscape;
        }

        private void SetDeviceConfiguration(DeviceConfiguration configuration)
        {
            DeviceHelper.ClientCode = configuration.ClientCode;
            DeviceHelper.PushNotificationServiceUrl = configuration.PushNotificationServiceUrl;
            DeviceHelper.RestUrl = configuration.RestUrl;
            DeviceHelper.Vhi = _vhi.Text;
            DeviceHelper.ClientCode = _client.Text;
            DeviceHelper.Orientation = GetSelectedOrientation();
            _deviceLite.SetDeviceConfiguration(
                new Models.Device {
                    BaseUrl = configuration.RestUrl,
                    PushNotificationServiceUrl = configuration.PushNotificationServiceUrl,
                    RestServiceUrl = configuration.RestUrl,
                    ClientCode = configuration.ClientCode,
                    Vhi = _vhi.Text,
                    Orientation = GetSelectedOrientation() == Android.Content.PM.ScreenOrientation.Portrait ? true : false
                });

            _loadingbar.Visibility = ViewStates.Gone;
            //_layout.Visibility = ViewStates.Visible;

            GoToMain();
        }

        private void SetConfig(DeviceDto config)
        {
            WriteDeviceInformation();

            DeviceHelper.ClientCode = config.ClientCode;
            DeviceHelper.PushNotificationServiceUrl = config.PushNotificationServiceUrl;
            DeviceHelper.RestUrl = config.RestServiceUrl;
            DeviceHelper.Vhi = config.Vhi;
            DeviceHelper.ClientCode = config.ClientCode;
            DeviceHelper.ActiveShift = _shiftLite.GetAllShifts().Count > 0 ? true : false;
            DeviceHelper.Orientation = config.Orientation == true ? Android.Content.PM.ScreenOrientation.Portrait : Android.Content.PM.ScreenOrientation.Landscape;

            GoToMain();
        }

        private void GetDeviceConfiguration()
        {
            var configuration = _deviceLite.GetDeviceConfiguration();

            if(configuration != null)
            {
                SetConfig(configuration);
            }          
        }

        private async Task<DeviceConfigurationResponse> GetDeviceConfiguration(string clientCode, string password)
        {
            DeviceConfigurationResponse deviceConfiguration = new DeviceConfigurationResponse();
            var client = new HttpClient
            {
                MaxResponseContentBufferSize = int.MaxValue
            };

            //egamdroid.original-soft.com DNS A UTILIZAR QUE APUNTA AL IIS DONDE SE ENCUENTRA EL REST DE CONFIGURACIÓN

            //var uri = new Uri("http://egamdroid.original-soft.com/GamDroidClientDeviceConfiguration/api/values?cliente=" + clientCode + "&password=" + (!string.IsNullOrWhiteSpace(password) ? password : "osoft")); //DNS OSOFT

            var uri = new Uri($"https://egamdroid.original-soft.com/GamDroidClientDeviceConfiguration/api/values?clientCode={clientCode}&password={password}");

            //var uri = new Uri("http://82.223.10.68/GamDroidConfiguracion/api/values?cliente=" + clientCode + "&password=" + password); //ARSYS IP

            //var uri = new Uri("http://192.168.1.247:49915/api/values?cliente=" + clientCode + "&password=" + password); //SOLO LOCALHOST

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                deviceConfiguration = JsonConvert.DeserializeObject<DeviceConfigurationResponse>(content);
            }

            return deviceConfiguration;
        }

        //private string GetPasswordHash(string password)
        //{
        //    byte[] salt;
        //    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        //    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        //    byte[] hash = pbkdf2.GetBytes(20);

        //    byte[] hashBytes = new byte[36];
        //    Array.Copy(salt, 0, hashBytes, 0, 16);
        //    Array.Copy(hash, 0, hashBytes, 16, 20);

        //    return Convert.ToBase64String(hashBytes);
        //}
      
        private void GoToMain()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);         
            Finish();       
        }

        private void WriteDeviceInformation()
        {
            try
            {
                var model = DeviceInfo.Model;
                var manufacturer = DeviceInfo.Manufacturer;
                var version = DeviceInfo.VersionString;

                Log.Information($"Model {model} Manufacturer {manufacturer} AndroidVersion {version}");
            }
            catch { }
        }
    }
}