using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.OneSignal;
using GamDroid2018.Helpers;
using GamDroid2018.Services;

namespace GamDroid2018.Utils
{
    public class ModifyVhiDialogFragment : AppCompatDialogFragment
    {
        private readonly ITransportLite _transportLite;
        private readonly IDeviceLite _deviceLite;
        private readonly IGamDroidService _gamDroidService;
        Button _button;
        TextView _txtViewVhi;
        


        public ModifyVhiDialogFragment() : this(new TransportLite(), new DeviceLite(), new GamDroidService()) { }

        internal ModifyVhiDialogFragment(ITransportLite transportLite, IDeviceLite deviceLite, IGamDroidService gamDroidService)
        {
            _transportLite = transportLite;
            _deviceLite = deviceLite;
            _gamDroidService = gamDroidService;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragmentDialog_ModifyVhi, container, false);
            _txtViewVhi = view.FindViewById<TextView>(Resource.Id.vhi);
            _button = view.FindViewById<Button>(Resource.Id.btnChangeVhi);
            _button.Click += ChangeVhi;
            _txtViewVhi.Text = DeviceHelper.Vhi;
            SetStyle(AppCompatDialogFragment.StyleNormal, Resource.Style.AlertDialogResponseServer);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        private async void ChangeVhi(Object sender, EventArgs e)
        {
            var vhiValue = _txtViewVhi.Text;
            bool vehicleChangeOk = true;
          
            if (string.IsNullOrWhiteSpace(vhiValue))
            {
                _txtViewVhi.SetError("El código de vehículo no es válido", null);
                vehicleChangeOk = false;
               
            }

            if (_transportLite.GetAllTransports().Count != 0)
            {
                _txtViewVhi.SetError("Tiene servicios pendientes, no puede modificar el vehículo", null);
                vehicleChangeOk = false;
            }

            if (DeviceHelper.ActiveShift)
            {
                _txtViewVhi.SetError("Finalice turno antes de cambiar de vehículo", null);
                vehicleChangeOk = false;
            }

            var response = await _gamDroidService.GetVhiAsync(vhiValue);

             // si el vehículo no existe

            if (!response)
            {
                _txtViewVhi.SetError("El vehículo no existe", null);
                vehicleChangeOk = false;
            }

            if (vehicleChangeOk)
            { 
                _deviceLite.ModifyVhiValue(vhiValue);
                
                Intent intent = new Intent(Context, typeof(DeviceActivity));
                Context.StartActivity(intent);

                
                //DeviceHelper.Vhi = vhiValue;
                //Activity.InvalidateOptionsMenu();
                //_deviceLite.ModifyVhiValue(vhiValue);
                //Com.OneSignal.Android.OneSignal.DeleteTag("Vhi");
                //Com.OneSignal.Android.OneSignal.SendTag("Vhi", DeviceHelper.Vhi);
                Dismiss();
            }
        }
    }
} 