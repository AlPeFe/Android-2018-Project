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
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using Android.Support.V7.Widget;
using GamDroid2018.Adapter;
using GamDroid2018.Utils;
using Android.Telephony;
using Com.OneSignal;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using AutoMapper;

namespace GamDroid2018.Fragments
{
    public class ShiftFragment : Fragment
    {
        private Button _shiftButton;
        private Spinner _spinner;
        private EditText _user;
        private EditText _km;
        private readonly IShiftLite _shiftLite;
        private RecyclerView _recyclerView;
        private ShiftAdapter _shiftAdapter;
        private readonly IGamDroidService _gamDroidService;
        LoadingProgressDialog _dialogProgress;
        private Dictionary<int, string> _shiftDictionary;
        private Action _onNavigate;

        //
        //
        //
        // SHIFT TYPES TES = 0, AYUDANTE = 1 DUE = 2, MEDICO = 3
        //
        //
        //

        public ShiftFragment() : this(new ShiftLite(), new GamDroidService()) { }

        internal ShiftFragment(Action onNavigationAction) : this(new ShiftLite(), new GamDroidService())
        {
            _onNavigate = onNavigationAction;
        }

        internal ShiftFragment(IShiftLite shiftLite, IGamDroidService gamDroidService)
        {
            _shiftLite = shiftLite;
            _gamDroidService = gamDroidService;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _shiftDictionary = new Dictionary<int, string>();
            string[] values = { "TES", "AYUDANTE", "DUE", "MÉDICO"};
            _shiftDictionary.Add(0, "TES");
            _shiftDictionary.Add(1, "AYUDANTE");
            _shiftDictionary.Add(2, "DUE");
            _shiftDictionary.Add(3, "MÉDICO");

            View view = inflater.Inflate(Resource.Layout.shift_fragment, container, false);
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.shiftRecyclerView);
            LinearLayoutManager layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);

            _shiftAdapter = new ShiftAdapter();
            _recyclerView.SetLayoutManager(layoutManager);
            _recyclerView.HasFixedSize = true;
            _recyclerView.SetAdapter(_shiftAdapter);
           
          
            _shiftButton = view.FindViewById<Button>(Resource.Id.login);
            _user = view.FindViewById<EditText>(Resource.Id.user_code);
            _km = view.FindViewById<EditText>(Resource.Id.km);
            _spinner = (Spinner)view.FindViewById(Resource.Id.type_selector);

            //SHIFT TYPES TES = 0, AYUDANTE = 1 DUE = 2, MEDICO = 3
          


            _shiftButton.Click += BtnShiftOnClick;
            ArrayAdapter adapter = new ArrayAdapter(view.Context, Android.Resource.Layout.SimpleSpinnerItem, values);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
            _spinner.Adapter = adapter;
           
            return view;
        }

        private async void BtnShiftOnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_km.Text) || string.IsNullOrWhiteSpace(_user.Text) ||
                string.IsNullOrWhiteSpace(_spinner.SelectedItem?.ToString())) return;

            _dialogProgress = new LoadingProgressDialog
            {
                Cancelable = false
            };

            _dialogProgress.Show(FragmentManager, "");

            TelephonyManager manager = (TelephonyManager)this.Activity.GetSystemService(Context.TelephonyService);

            var shift = new ShiftDto
            {
                Km = _km.Text,
                ShiftType = _shiftDictionary.Where(x => 
                                    x.Value == _spinner.SelectedItem.ToString())
                                    .Select(x => x.Key)
                                    .First(),
                User = _user.Text,
                Date = DateTime.Now,
                Vhi = DeviceHelper.Vhi,
                Imei = manager.Imei,
                ShiftReceived = true
                
            
            };

            AlertDialog.Builder builder = new AlertDialog.Builder(Context, Resource.Style.AlertDialogResponseServer);

            if (_shiftLite.GetShift(shift))
            {
                _dialogProgress.Dismiss();

                builder.SetTitle("Inicio Turno");
                builder.SetMessage("Ya existe un turno con estas características, cierre el turno anterior");
                builder.SetPositiveButton("OK", (Android.Content.IDialogInterfaceOnClickListener)null);
                builder.Show();

                return;
            }
           
            var response = await _gamDroidService.PostShiftAsync(shift);

            if (response.ShiftStatus == ShiftStatus.MISSING /*|| response.ShiftStatus == ShiftStatus.INTERNAL_ERROR*/)
            {
                _dialogProgress.Dismiss();

                builder.SetTitle("Inicio Turno");
                builder.SetMessage("El código de trabajador no es válido");
                builder.SetPositiveButton("OK", (Android.Content.IDialogInterfaceOnClickListener)null);
                builder.Show();

                return;
            }

            if (response.ShiftStatus == ShiftStatus.INTERNAL_ERROR)
            {
                shift.ShiftReceived = false;
                shift.Name = "PENDIENTE";
            }

            var shiftDto = _shiftLite.AddShift(shift);

            shift.Id = shiftDto.Id;

            _shiftLite.UpdateShiftName(shift.Id, response.Name);

            _recyclerView.SetAdapter(new ShiftAdapter());

            _dialogProgress.Dismiss();

            DeviceHelper.ActiveShift = true;

            _onNavigate?.Invoke();
        }     
    }
}