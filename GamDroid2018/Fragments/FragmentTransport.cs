using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AutoMapper;
using GamDroid2018.Adapter;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using GamDroid2018.Services;
using GamDroid2018.Utils;
using Serilog;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace GamDroid2018.Fragments
{
    public class FragmentTransport : Fragment, ITransportListeners
    {
        private RecyclerView _recyclerView;
        private ITransportLite _transportLite;
        private IGamDroidService _gamDroidService;
        private IAuxTableLite _auxTableLite;
        private AdapterTransport _adapter;
        private List<TransportDto> _transports;
        private FragmentManager _fm;
        private TransportBroadcastReceiver updateUiReceiver;

        public FragmentTransport()
        {
            _transportLite = new TransportLite();
            _gamDroidService = new GamDroidService();
            _auxTableLite = new AuxTableLite();
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_transport, container, false);
            _recyclerView = _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.transportRecyclerView);
            _fm = FragmentManager;

            _transports = DeviceHelper.ActiveShift  // SI NO HAY TURNO INICIADO NO MUESTRA SRV
                ?  _transportLite.GetAllTransports() 
                : new List<TransportDto>();

            _adapter = new AdapterTransport(_transports, this, Activity);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            _recyclerView.SetAdapter(_adapter);
            updateUiReceiver = new TransportBroadcastReceiver();
            updateUiReceiver.UpdateUi -= UpdateUi;
            updateUiReceiver.UpdateUi += UpdateUi;
            Activity.RegisterReceiver(updateUiReceiver, 
                new IntentFilter("update.ui.transport"));

            return view;
        }

        private void UpdateUi()
        {
            UpdateDataSet();
        }

        public override void OnPause()
        {
            try
            {
                Activity.UnregisterReceiver(updateUiReceiver);
            }
            catch { }

            base.OnPause();
        }

        public override void OnResume()
        {
            //LocalBroadcastManager.GetInstance(this.Activity).UnregisterReceiver(updateUiReceiver);
            base.OnResume();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public void OnCancelTransportClick(string id)
        {
            var trp = _transports.Where(c => c.ServiceNumber == id).FirstOrDefault();
            var cancellationList = _auxTableLite.GetCancellationList();

            if (DeviceHelper.ClientCode != "CUEN")
            {
                cancellationList.Add(new CancellationDto { CancellationCode = "AEO", CancellationReaseon = "FINALIZAR EN ORIGEN", Id = Guid.NewGuid().ToString() }); // SE AÑADE FINALIZAR EN ORIGEN
            }

            var builder = new Android.App.AlertDialog.Builder(Activity);
            builder.SetTitle("Anulación");
            var charSeq = cancellationList.Select(c => $"{c.CancellationCode} - {c.CancellationReaseon}").ToArray();

            builder.SetItems(charSeq, async (sender, args) => 
            {
                var selectedItem = args.Which;

                var item = cancellationList[selectedItem];

                await _gamDroidService.PostCancellationAsync(
                   new CancellationToken
                   {
                       ServiceNumber = trp.ServiceNumber,
                       Vhi = DeviceHelper.Vhi,
                       CancellationDate = DateTime.Now,
                       CancellationCode = item.CancellationCode,
                   });

                _transportLite.CancelTransportFromGamDroid(trp.Id);
                var indexToRemove = _transports.IndexOf(trp);
                _transports.RemoveAt(indexToRemove);
                _adapter.NotifyItemRemoved(indexToRemove);
                _transportLite.DeleteTransport(trp.Id);

            });

            builder.Show();
        }

        private async Task PostAndUpdateTransportStatus(List<TransportDto> items, int status)
        {
            foreach (var item in items)
            {
                try
                {
                    var transportStatus = new TransportStatus
                    {
                        Date = DateTime.Now,
                        Status = status,
                        Vhi = DeviceHelper.Vhi,
                        TransportNumber = item.ServiceNumber,
                        Latitude = LocationService.LastLocation == null ? "0" : Convert.ToDecimal(LocationService.LastLocation.Latitude).ToString(),
                        Longitude = LocationService.LastLocation == null ? "0" : Convert.ToDecimal(LocationService.LastLocation.Longitude).ToString(),
                        Received = false
                    };

                    var result = await _gamDroidService.PostTransportStatus(Mapper.Map<TransportStatusDto>(transportStatus));

                    if (result == TransportResponse.ERROR)
                        _transportLite.NewTransportStatus(transportStatus);
                }
                catch
                {
                    Log.Error($"Error al enviar el status {item.Status}");
                }
            }
        }

        private List<TransportDto> MultiStatus(TransportDto transport)
        {
            List<TransportDto> transportToUpdate = new List<TransportDto>();

            var currentStatus = transport.Status;

            if (DeviceHelper.ClientCode == "CUEN" && currentStatus == 705) //CUENCA
            {
                transportToUpdate.Add(transport);
                return transportToUpdate;
            }

            if (currentStatus == 740)
                return _transports.Where(c => c.Status == 740).ToList();

            if ((currentStatus == 705 || currentStatus == 710))
            {
                transportToUpdate = _transports
                        .Where(c => c.Street == transport.Street
                        && c.Status == currentStatus
                        && c.City == transport.City
                       && c.ServiceNumber != transport.ServiceNumber).ToList();

            }
            else if (currentStatus == 715 || currentStatus == 720 || currentStatus == 725 || currentStatus == 730)
            {
                transportToUpdate = _transports
                        .Where(c => c.DestinationStreet == transport.DestinationStreet
                        && c.Status == currentStatus
                        && c.DestinationCity == transport.DestinationCity && c.ServiceNumber != transport.ServiceNumber).ToList();
            }

            transportToUpdate.Add(transport);

            return transportToUpdate;
        }

        public void OnDetailClick(string id)
        {
            var trp = _transports.Where(c => c.ServiceNumber == id).FirstOrDefault();

            if (trp == null)
                return;

            new TransportDetailDialog(trp)
                .Show(_fm, "");
        }

        public void OnNavigateClick(string id)
        {
            var item = _transports.Where(c => c.ServiceNumber == id).FirstOrDefault();

            if (item == null)
                return;

            var address = (item.Status < 715 || item.Status == 740) ? item.GetOriginAdressNavigation() : item.GetDestinationAddress();

            string uri = $"http://maps.google.com/maps?daddr={address}&";

            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uri));

            Activity.StartActivity(intent);
        }

        public async Task OnTransportStatusClick(string id)
        {
            List<string> transportListToDelete = new List<string>();
            List<int> indexUpdateItems = new List<int>();

            var trp = _transports.Where(c => c.ServiceNumber == id).FirstOrDefault();

            var listTransportsToUpdate = MultiStatus(trp);

            var nextStatus = TransportUtils.GetNextStatusByClientCode(DeviceHelper.ClientCode, trp.Status);

            //update en DB
            foreach (var transport in listTransportsToUpdate)
            {
                _transportLite.UpdateTransportStatus(transport.Id, nextStatus);
                transport.Status = nextStatus;
                indexUpdateItems.Add(_transports.IndexOf(transport));

                if (nextStatus == 735) // si el status es el final se añade para eliminar 
                    transportListToDelete.Add(transport.Id);
            }

            if (nextStatus == 735) // si algun servicio se finaliza mejor updatear el dataSet
            {
                UpdateDataSet();

                foreach (var item in transportListToDelete)
                {
                    _transportLite.DeleteTransport(item); //borrar servicios 735 previamente añadidos
                }
            }
               
            else
                UpdateItems(indexUpdateItems);

            await PostAndUpdateTransportStatus(listTransportsToUpdate, nextStatus);

           
        }

        private void UpdateItems(List<int> itemsIndex)
        {
            foreach (var index in itemsIndex)
                _adapter.NotifyItemChanged(index);
        }

        private void UpdateDataSet()
        {
            _transports.Clear();
            _transports = _transportLite.GetAllTransports();
            _adapter = new AdapterTransport(_transports, this, Activity);
            _recyclerView.SetAdapter(_adapter);
            _adapter.NotifyDataSetChanged(); // para que no se menee el adapter
            // reseteo adapter
        }

        public class TransportBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {

                UpdateUi();
            }

            public delegate void UpdateTransportListEventHandler();
            public event UpdateTransportListEventHandler UpdateUi;
        }
    }
}