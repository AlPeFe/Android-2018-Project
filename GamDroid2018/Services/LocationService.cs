using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using GamDroid2018.Utils;
using Serilog;

namespace GamDroid2018.Services
{
    [Service]
    [IntentFilter(new string[] { "location.service" })]
    public class LocationService : Service, ILocationListener
    {
       
        private static readonly int LOCATION_INTERVAL = 30000;
        private LocationManager _locationManager;
        public static Location LastLocation;
        public GpsLocation _gpsLocation;
        readonly IGamDroidService _gamdroidService;
        private ITransportLite _transportLite;
        private IMessageLite _messageLite;
        private IShiftLite _shiftLite;
        private IAuxTableLite _auxTableLite;

        public LocationService() : this(new GamDroidService(),
            new TransportLite(), 
            new MessageLite(), 
            new ShiftLite(),
            new AuxTableLite()) { }

        internal LocationService(IGamDroidService gamdroidService,
            ITransportLite transportLite,
            IMessageLite messageLite, 
            IShiftLite shiftLite, 
            IAuxTableLite auxTableLite)
        {
            _shiftLite = shiftLite;
            _gamdroidService = gamdroidService;
            _transportLite = transportLite;
            _messageLite = messageLite;
            _auxTableLite = auxTableLite;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);

            var thread = new Thread(new ThreadStart(LocationManagerSetUp));
            thread.Start();
          
            return StartCommandResult.NotSticky; // modificado de Sticky-> Not Sticky
        }

        public override void OnCreate()
        {
            base.OnCreate();
            LastLocation = null;
            LocationManagerSetUp();        
        }

        private void LocationManagerSetUp()
        {
            if(_locationManager == null)
            {
                _locationManager = (LocationManager)GetSystemService(Context.LocationService);
            }

            var bestProvider = _locationManager.GetBestProvider(new Criteria()
            {
                Accuracy = Accuracy.Fine,
                AltitudeRequired = false,
                BearingRequired = true,
                CostAllowed = true,
                PowerRequirement = Power.High,
                SpeedRequired = true,
                SpeedAccuracy = Accuracy.Fine,
                HorizontalAccuracy = Accuracy.Fine

            }, false);

            _locationManager.RequestLocationUpdates(bestProvider, LOCATION_INTERVAL, 1f, this);           
        }


        //si hay algo pendiente enviar todo, a parte de lo que ya está haciendo 
        public void OnLocationChanged(Location location)
        {

            

            LastLocation = location;
            _gamdroidService.PostLocationAsync(location);
            SendPendingNotifications();
         }

        private void SendPendingNotifications()
        {
            try
            {
                _transportLite.GetNotSentTransportStatus()
                    .ForEach(async c =>
                    {
                        var result = await _gamdroidService.PostTransportStatus(c);

                        if (result == TransportResponse.OK)
                            _transportLite.SetTransportStatusResponse(c.Id, true);
                    });

                _messageLite.GetNotSentMessages()
                    .ForEach(async c =>
                    {
                        var result = await _gamdroidService.PostMessageAsync(c);

                        if (result == MessageReceivedStatus.OK)
                            _messageLite.SetMessageReceivedStatus(c.Id, true);

                    });

                _shiftLite.GetNotSentShift().ForEach(async c =>
                {
                    var result = await _gamdroidService.PostShiftAsync(c);

                    if (result.ShiftStatus == ShiftStatus.OK)
                    {
                        _shiftLite.UpdateShiftName(c.Id, result.Name);
                        _shiftLite.UpdateShiftStatus(c.Id, true);
                    }
                });

                _auxTableLite.GetPendingExpense().ForEach(async c =>
                {
                    var result = await _gamdroidService.PostReportExpenseAsync(c);

                    if (result.Response == ResponseStatus.OK)
                        _auxTableLite.DeletePendingExpense(c.Id);
                });
            }
            catch(Exception ex) { Log.Error($"SendPendingItems ERROR IN LOCATIONLISTENER {ex.Message}_{ex?.InnerException}"); }
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Information($"{DeviceHelper.Vhi} GPS DISABLED");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Information($"{DeviceHelper.Vhi} GPS ENABLED");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
           
        }
    }
}