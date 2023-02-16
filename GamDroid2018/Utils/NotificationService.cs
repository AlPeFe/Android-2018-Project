using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Com.OneSignal.Android;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using Java.Lang;
using Java.Util.Concurrent.Atomic;
using Newtonsoft.Json;
using Serilog;
using static GamDroid2018.MainActivity;
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Utils
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE", Exported = false)]
    [IntentFilter(actions: new[] { "com.onesignal.NotificationExtender" })]
    public class NotificationService : NotificationExtenderService 
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private IGamDroidService _gamDroidService;
        private ITransportLite _transportLite;
        private IMessageLite _messageLite;
        private string _notificationType;
        public NotificationService() : this(new GamDroidService(), new TransportLite(), new MessageLite()) { }
        internal NotificationService(IGamDroidService gamDroidService, ITransportLite transportLite, IMessageLite messageLite)
        {
            _gamDroidService = gamDroidService;
            _transportLite = transportLite;
            _messageLite = messageLite;
            Log.Information("Notification Service Creado");
        }
        protected override void OnHandleIntent(Intent intent)
        {
            
        }
        protected override bool OnNotificationProcessing(OSNotificationReceivedResult p0)
        {
            Log.Information("Notification received");

            // SI NO HAY UN TURNO INICIADO NO NOTIFICACIONES
           

            try
            {
                var result = p0;

                var data = p0.Payload.AdditionalData.ToString();

                var action = Newtonsoft.Json.JsonConvert.DeserializeObject<PushNotificationPayload>(data);
                _notificationType = action.ActionType;
                string title = "";

                Log.Information($"Notification type {_notificationType}");

                switch (action.ActionType)
                {
                    case "NewTransport":
                        TransportJob();
                        title = "Nuevo Servicio";
                        break;
                    case "NewMessage":
                        MessageJob();
                        title = "Nuevo Mensaje";
                        break;
                    case "CancelTransport":
                        if (!string.IsNullOrWhiteSpace(action?.TransportNumber))
                        {
                            CancelTransportJob(action.TransportNumber);
                            title = "Servicio Anulado";
                        }
                        
                        break;
                    default:
                        break;
                }

                DisplayLocalNotification(_notificationType, title);
            }
            catch(System.Exception ex) { Log.Error($"Error OnNotificationProcessing {ex.ToString()}"); }

            //StopSelf();

            return true;
        }
        public override void OnDestroy()
        {
            Log.Information("Destroyed notificationService");
            base.OnDestroy();
        }
        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }
        private void TransportJob()
        {
            try
            {
                List<Transport> transports = new List<Transport>();
                
                Log.Information("HandleTransport");

                var response = _httpClient.GetAsync($"{DeviceHelper.RestUrl}/api/Transport?vhi={DeviceHelper.Vhi}").Result;

                if (!response.IsSuccessStatusCode)
                    Log.Error("Error al solicitar los transportes");

                var responseString = response.Content.ReadAsStringAsync().Result;

                transports = JsonConvert.DeserializeObject<List<Transport>>(responseString);
                
                //nunca va a ser negativo pero mejor polla en mano que sida en el ano
                if (transports.Count <= 0)
                    return;

                var currentTransports = _transportLite.GetAllTransports();
                var insertedTransports = _transportLite.NewTransport(transports, currentTransports);

                foreach(var trp in insertedTransports)
                {

                    var trpStatus = new TransportStatus
                    {
                        Date = DateTime.Now,
                        Status = 740,
                        Vhi = DeviceHelper.Vhi,
                        TransportNumber = trp.ServiceNumber,
                        Latitude = "0",
                        Longitude = "0",
                    };

                    var statusDto = _transportLite.NewTransportStatus(trpStatus);

                    var statusJson = JsonConvert.SerializeObject(statusDto);

                    var content = new StringContent(JsonConvert.SerializeObject(statusDto), Encoding.UTF8, "application/json");

                    var uri = new Uri($"{DeviceHelper.RestUrl}/api/Transport");

                    var postResponse = _httpClient.PutAsync(uri, content).Result;

                    if (postResponse.IsSuccessStatusCode)
                        _transportLite.SetTransportStatusReceived(statusDto.Id);
              
                }

                if (insertedTransports.Count > 0)
                {
                    //DisplayNotification(new OverrideSettings { Extender = this });
                    Intent it = new Intent("update.ui.transport");
                    this.SendBroadcast(it);
                }  
            }
            catch(System.Exception ex)
            {
                Log.Error($"Error getting transports {ex.ToString()}");
            }
        }

        //private void DisplayLocalNotification(string intentExtra)
        //{
        //    var uri = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
           
        //    Intent intent = new Intent("navigate.to.fragment");
        //    intent.PutExtra("type", intentExtra);
        //    PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

        //    NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
        //    builder.SetPriority(NotificationCompat.PriorityHigh);
        //    builder.SetVibrate(new long[] { 100, 250 });
        //    builder.SetAutoCancel(true);
        //    builder.SetSound(uri);
        //    builder.SetSmallIcon(Resource.Drawable.gam);

        //    var notification = builder.Build();
        //    notification.Flags = NotificationFlags.Insistent;
        //    notification.ContentIntent = pendingIntent;
        //    NotificationManager nm = (NotificationManager)GetSystemService(Context.NotificationService);

        //    nm.Notify(2, not);

        //}
        private void DisplayLocalNotification(string intentExtra, string title)
        {
            if (!DeviceHelper.ActiveShift)
                return;

            // var settings = new OverrideSettings();
            //settings.Extender = this;
            var uri = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
            var id = NotificationId.GetNotificationId();
            Intent intent = new Intent("navigate.to.fragment");
            intent.PutExtra("type", intentExtra);
            intent.PutExtra("notId", id);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
            builder.SetPriority(NotificationCompat.PriorityHigh);
            builder.SetVibrate(new long[] { 100, 250 });
            builder.SetAutoCancel(true);
            builder.SetSound(uri);
            builder.SetContentTitle(title);

            NotificationManager nm = (NotificationManager)GetSystemService(Context.NotificationService);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel("channelId",
                    "channelName",
                    Android.App.NotificationImportance.High);
                channel.Description = "Default Channel";
                nm.CreateNotificationChannel(channel);
                builder.SetChannelId("channelId");
            }

            builder.SetSmallIcon(Resource.Drawable.gam);
            var not = builder.Build();
            not.Flags = NotificationFlags.Insistent;
            not.ContentIntent = pendingIntent;
            
           
            nm.Notify(id, not);
        }
        private void MessageJob()
        {
            Log.Information($"Message received");

            var url = new Uri($"{DeviceHelper.RestUrl}/api/Message?vhi={DeviceHelper.Vhi}");

            var response = _httpClient.GetAsync(url).Result;

            if(response.IsSuccessStatusCode)
            {
                var content = JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);

                if (content.Count > 0)
                {
                    content.RemoveAll(c => string.IsNullOrWhiteSpace(c.Content));
                    var messages = content.Select(c => { c.MessageReceived = true; return c; }).ToList();
                    _messageLite.InsertMessage(messages);

                    // var display =  DisplayNotification(new OverrideSettings { Extender = this });

                    Intent it = new Intent(this, typeof(Fragments.MessageFragment.MessageBroadcastReceiver));
                    this.SendBroadcast(it);
                }
            }
            
        }
        private void CancelTransportJob(string transportNumber)
        {
            try
            {
                Log.Information($"CancelTransport JOB {transportNumber}");
                string content = "";
                var values = _transportLite.GetAllTransports().Select(c => c.ServiceNumber).ToList();
                Log.Information($"CancelTransport JOB {transportNumber}");

                if (values != null)
                {
                    foreach (var item in values)
                    {
                        content += $"transportList={item}&";
                    }
                }
                var url = DeviceHelper.RestUrl + "/api/TransportCancellation?" + content;
                var response = _httpClient.GetAsync(DeviceHelper.RestUrl + "/api/TransportCancellation?" + content).Result;


                if (!response.IsSuccessStatusCode)
                    return;

                var responseString = response.Content.ReadAsStringAsync().Result;

                var listTransportsToCancel = JsonConvert.DeserializeObject<List<TransportCancellation>>(responseString);

                foreach (var item in listTransportsToCancel)
                {
                    Log.Information($"TransportToCancel -> {item.State} {item.SubState} {item.TransportNumber}");
                    if (item.State == "N" || (item.State == "P" && item.SubState == "A") || item.Vhi != DeviceHelper.Vhi)
                    {
                        _transportLite.CancelTransportFromGam(item.TransportNumber);
                        Log.Information($"CANCEL OK");
                    }
                }

                //DisplayNotification(new OverrideSettings { Extender = this });
                Intent it = new Intent("update.ui.transport");
                this.SendBroadcast(it);

               // DisplayNotification(new OverrideSettings { Extender = this });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }

    class PushNotificationPayload
    {
        public string TransportNumber { get; set; }
        public string ActionType { get; set; }
    }

    class NotificationId
    {
        private static readonly AtomicInteger c = new AtomicInteger(0);

        public static int GetNotificationId() => c.IncrementAndGet();
    }
}