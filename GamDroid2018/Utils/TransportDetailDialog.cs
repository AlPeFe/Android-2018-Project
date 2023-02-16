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
using GamDroid2018.Helpers;
using GamDroid2018.Models;

namespace GamDroid2018.Utils
{
    public class TransportDetailDialog : AppCompatDialogFragment
    {
        readonly ITransportLite _transportLite;
        readonly IGamDroidService _gamDroidService;

        private TextView _name;
        private TextView _birthDate;
        private TextView _dni;
        private TextView _gender;
        private TextView _nass;
        private TextView _cip;
        private TextView _phoneNumber1;
        private TextView _phoneNumber2;
        private TextView _homeAdress;
        private TextView _obs;
        private TextView _serviceNumber;
        private TextView _destinationAdress;
        private TextView _originAddress;
        private TextView _oxigen;
        private TextView _oxigenConcentration;
        private TextView _silla;
        private TextView _camilla;
        private TextView _rampa;
        private TextView _companion;
        private TextView _initDate;
        private TextView _destinationDate;
        private TextView _txtReference;
        private TextView _txtTransportType;
        private TextView _txtTransportReason;
        private TextView _txtAssistant;
        private TextView _txtDue;
        private TransportDto _transport;

        public TransportDetailDialog(TransportDto transport)
        {
            _transport = transport;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.item_layoutDetail, container, false);

            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            _name = view.FindViewById<TextView>(Resource.Id.txtName);
            _birthDate = view.FindViewById<TextView>(Resource.Id.txtBirthDate);
            _dni = view.FindViewById<TextView>(Resource.Id.txtDni);
            _nass = view.FindViewById<TextView>(Resource.Id.txtNass);
            _cip = view.FindViewById<TextView>(Resource.Id.txtCip);
            _phoneNumber1 = view.FindViewById<TextView>(Resource.Id.txtPhoneNumber);
            _phoneNumber2 = view.FindViewById<TextView>(Resource.Id.txtPhoneNumber2);
            _homeAdress = view.FindViewById<TextView>(Resource.Id.txtOAdress);
            _gender = view.FindViewById<TextView>(Resource.Id.txtSexo);
            _obs = view.FindViewById<TextView>(Resource.Id.txtObs);
            _serviceNumber = view.FindViewById<TextView>(Resource.Id.txtNumSrv);
            _destinationAdress = view.FindViewById<TextView>(Resource.Id.txtDAdress);
            _originAddress = view.FindViewById<TextView>(Resource.Id.txtOAdress);
            _oxigen = view.FindViewById<TextView>(Resource.Id.txtOxigen);
            _oxigenConcentration = view.FindViewById<TextView>(Resource.Id.txtOxigenConcentracion);
            _silla = view.FindViewById<TextView>(Resource.Id.txtSilla);
            _camilla = view.FindViewById<TextView>(Resource.Id.txtCamilla);
            _rampa = view.FindViewById<TextView>(Resource.Id.txtRampa);
            _companion = view.FindViewById<TextView>(Resource.Id.txtCompanion);
            _initDate = view.FindViewById<TextView>(Resource.Id.initDate);
            _destinationDate = view.FindViewById<TextView>(Resource.Id.destinationDate);
            _txtReference = view.FindViewById<TextView>(Resource.Id.txtReference);
            _txtTransportType = view.FindViewById<TextView>(Resource.Id.txtTransportType);
            _txtTransportReason = view.FindViewById<TextView>(Resource.Id.txtTransportReason);
            _txtAssistant = view.FindViewById<TextView>(Resource.Id.txtAssistant);
            _txtDue = view.FindViewById<TextView>(Resource.Id.txtDue);

            BindData(_transport);

            return view;
        }

        private void BindData(TransportDto transport)
        {
            foreach (var prop in transport.GetType().GetProperties())
            {
                var value = prop.GetValue(transport, null);

                if (value is string s)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        prop.SetValue(transport, (string)s.ToString().Trim());
                    }
                }
            }

            _name.Text = "Nombre: " + transport.Name + " " + transport.Surname;
            _homeAdress.Text = "Recogida: " + transport.Street;
            _birthDate.Text = "F.Nacimiento: " + transport.BirthDate?.ToString();
            _dni.Text = "DNI: " + transport.Dni;
            _cip.Text = "CIP:" + transport.Cip;
            _nass.Text = "NASS: " + transport.Nass;
            _serviceNumber.Text = "N.Servicio: " + transport.ServiceNumber;
            _gender.Text = ("Sexo: ") + (transport.Gender == 0 ? "Mujer" : "Hombre");
            _obs.Text = transport.Observations;
            _originAddress.Text = "Origen: " + transport.Street + " ," + transport.City + " " + transport.Province;
            _destinationAdress.Text = "Destino: " + transport.DestinationStreet + " ," + transport.DestinationCity + " ," + transport.DestinationProvince;
            _oxigen.Text = "Oxigeno: " + (transport.Oxigen == true ? "SI" : "NO");
            _oxigenConcentration.Text = "O.Con: " + transport.OxigenConcentration;
            _rampa.Text = "Rampa: " + (transport.Ramp == true ? "SI" : "NO");
            _camilla.Text = "Camilla: " + (transport.Stretcher == true ? "SI" : "NO");
            _silla.Text = "Silla Ruedas: " + (transport.WheelChair == true ? "SI" : "NO");
            _phoneNumber1.Text = "Tel : " + transport.PhoneNumber + " " + transport.PhoneNumber2 + " " + transport.PhoneNumber3;
            _companion.Text = "Acomp. : " + (transport.Companion == true ? "SI" : "NO");
            _initDate.Text = $"H.Inicio: {transport.StartDate.Value.LocalDateTime.ToString("dd/MM/yyyy HH:mm")}";
            _destinationDate.Text = $"H.Destino: {transport.DestinationDate.Value.LocalDateTime.ToString("dd/MM/yyyy HH:mm")}";
            _txtTransportType.Text = $"Tipo Trasl: {transport.TransportType}";
            _txtReference.Text = $"Ref: {transport.Reference}";
            _txtTransportReason.Text = $"Mot. Trasl: {transport.TransportReason}";
            _txtAssistant.Text = " Ayud. " + (transport.Assistant == true ? "SI" : "NO");
            _txtDue.Text = "Due: " + (transport.Due == true ? "SI" : "NO");
        }
    }
}