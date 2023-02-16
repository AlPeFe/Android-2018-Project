using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GamDroid2018.Models;
using GamDroid2018.Utils;

namespace GamDroid2018.Adapter
{
    public class AdapterTransport : RecyclerView.Adapter
    {
        private List<TransportDto> _items;
        private ITransportListeners _listeners;
        private Context _context;
        private long _lastButtonClick;

        public AdapterTransport(List<TransportDto> items, ITransportListeners listeners, Context context)
        {
            _context = context;
            _items = items;
            _listeners = listeners;

        }
        public override int ItemCount => _items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _items[position];

            var transportHolder = holder as TransportAdapterViewHolder;

            transportHolder.PatientInfo.Text = $"{item.GetPatientName()}";
            transportHolder.StartDate.Text = $"{item.GetTransportDatesAndBasicInfo()}";
            transportHolder.ExtraInfo.Text = $"{item.GetTransportExtraInfo()}";
            transportHolder.Address.Text = (item.Status < 715 || item.Status == 740) ? item.GetOriginAddress() : item.GetDestinationAddress();

            transportHolder.Status.Tag = item.ServiceNumber;
            transportHolder.BtnCancel.Tag = item.ServiceNumber;
            transportHolder.BtnDetail.Tag = item.ServiceNumber;
            transportHolder.BtnNavigate.Tag = item.ServiceNumber;

                          
            var status = TransportUtils.GetStatus(item.Status, DeviceHelper.ClientCode);


            transportHolder.Status.Text = status.Description;
            transportHolder.Status.SetBackgroundColor(status.Colour);

            transportHolder.Status.Click -= OnStatusClick;
            transportHolder.Status.Click += OnStatusClick;

            transportHolder.BtnCancel.Click -= OnCancelClick;
            transportHolder.BtnCancel.Click += OnCancelClick;

            transportHolder.BtnNavigate.Click -= OnNavigateClick;
            transportHolder.BtnNavigate.Click += OnNavigateClick;

            transportHolder.BtnDetail.Click -= OnDetailClick;
            transportHolder.BtnDetail.Click += OnDetailClick;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_transport, parent, false);
            return  new TransportAdapterViewHolder(itemView);
        }

        public void OnDetailClick(object sender, EventArgs e)
        {
            var itemId = (sender as AppCompatImageView).Tag.ToString();
            _listeners.OnDetailClick(itemId);
        }

        public void OnNavigateClick(object sender, EventArgs e)
        {
            var itemId = (sender as AppCompatImageView).Tag.ToString();
            _listeners.OnNavigateClick(itemId);
        }

        public void OnCancelClick(object sender, EventArgs e)
        {
            var itemId = (sender as AppCompatImageView).Tag.ToString();
             _listeners.OnCancelTransportClick(itemId);
        }

        public async void OnStatusClick(object sender, EventArgs e)
        {
            if (SystemClock.ElapsedRealtime() - _lastButtonClick < 500) return;

            _lastButtonClick = SystemClock.ElapsedRealtime();

            var itemId = (sender as TextView).Tag.ToString();
            await _listeners.OnTransportStatusClick(itemId);
        }
    }

    public interface ITransportListeners
    {
        Task OnTransportStatusClick(string id);

        void OnNavigateClick(string id);

        void OnDetailClick(string id);

        void OnCancelTransportClick(string id);
    }

    public class TransportAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Status { get; set; }
        public TextView ExtraInfo { get; set; }
        public TextView StartDate { get; set; }
        public TextView PatientInfo { get; set; }
        public TextView Address { get; set; }
        public AppCompatImageView BtnDetail { get; set; }
        public AppCompatImageView BtnCancel { get; set; }
        public AppCompatImageView BtnNavigate { get; set; }

        public TransportAdapterViewHolder(View itemView) : base(itemView)
        {
            Status = itemView.FindViewById<TextView>(Resource.Id.transStatus);
            StartDate = itemView.FindViewById<TextView>(Resource.Id.transDate);
            PatientInfo = itemView.FindViewById<TextView>(Resource.Id.transContentPatient);
            Address = itemView.FindViewById<TextView>(Resource.Id.transContentDireccion);
            BtnDetail = itemView.FindViewById<AppCompatImageView>(Resource.Id.transBtnDescription);
            BtnCancel = itemView.FindViewById<AppCompatImageView>(Resource.Id.transBtnCancel);
            BtnNavigate = itemView.FindViewById<AppCompatImageView>(Resource.Id.transBtnMaps);
            ExtraInfo = itemView.FindViewById<TextView>(Resource.Id.extraInfo);
        }
    }
}