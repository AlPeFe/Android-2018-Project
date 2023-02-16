using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using GamDroid2018.Fragments;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Fragment = Android.Support.V4.App.Fragment;
using GamDroid2018.Utils;
using GamDroid2018.Services;
using Android.Support.V7.Widget;
using GamDroid2018.Adapter;
using Plugin.Connectivity;
using Android.Widget;
using System.Threading.Tasks;
using Android.Content;
using Android.Text;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using Com.OneSignal;
using Com.OneSignal.Android;

using Uri = Android.Net.Uri;
using System.Threading;
using static Android.OS.PowerManager;
using Com.OneSignal.Abstractions;
using Serilog;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace GamDroid2018
{
    [Activity(LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        DrawerLayout _drawer;
        Fragment _fragment;
        private readonly Fragment _fragmentToolbar;
        RecyclerView _recyclerView;
        FragmentManager _ft;
        private PowerManager.WakeLock _wakeLock;
        private PowerManager _powerManager;
        private GamSyncData _gamSyncData;
        private readonly IAuxTableLite _auxTable;
        private IGamDroidService _gamDroidService;
        private IShiftLite _shiftLite;
        private ICheckListLite _checkListLite;
        public static Context context;
        private NavigationBroadcastReceiver _navigationReceiver;
        //NavigationTESTBroadcastReceiver _TEST;

        public MainActivity() : this(new AuxTableLite(), new GamDroidService(), new ShiftLite(), new CheckListLite()) { }

        internal MainActivity(IAuxTableLite auxTable, IGamDroidService gamDroidService, IShiftLite shiftLite, ICheckListLite checkListLite)
        {
            _auxTable = auxTable;
            _gamDroidService = gamDroidService;
            _shiftLite = shiftLite;
            context = this;
            _checkListLite = checkListLite;

            _gamSyncData = new GamSyncData();
        }

        protected override void OnResume()
        {
            //_ft.BeginTransaction().Replace(Resource.Id.content_frame, new TransportFragment()).Commit();
            base.OnResume();
            _drawer.OpenDrawer(GravityCompat.Start, true);
        }

        private void Navigate(string action)
        {
            bool shouldNavigate = true;

            switch(action)
            {
                case "NewMessage":
                    DisplaySelectedFragment(null, 3);
                    break;
                case "NewTransport":
                    DisplaySelectedFragment(null, 1);
                    break;
                default:
                    shouldNavigate = false;
                    break;
            }

            if (shouldNavigate)
                _ft.BeginTransaction().Replace(Resource.Id.content_frame, _fragment).CommitAllowingStateLoss();//.Commit();
        }
     
        protected override void OnCreate(Bundle savedInstanceState)
        {  
            base.OnCreate(savedInstanceState);

            RequestedOrientation = DeviceHelper.Orientation;

            StartService(new Android.Content.Intent(this, typeof(LocationService)));
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            _powerManager = (PowerManager)GetSystemService(PowerService);
            SetUpPushNotificationService();
            _ft = SupportFragmentManager;
            SyncData();
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            _navigationReceiver = new NavigationBroadcastReceiver();
            _navigationReceiver.Navigate -= Navigate;
            _navigationReceiver.Navigate += Navigate;
            this.RegisterReceiver(_navigationReceiver,
                new IntentFilter("navigate.to.fragment"));

            //_TEST = new NavigationTESTBroadcastReceiver();
            //this.RegisterReceiver(_TEST, new IntentFilter("com.xamarin.example.TEST"));

            if (_wakeLock == null)
            {
                _wakeLock = _powerManager.NewWakeLock(WakeLockFlags.Partial, "KeepAwake");
                _wakeLock.Acquire();//
            }

            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, _drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            _drawer.AddDrawerListener(toggle);
            _drawer.DrawerClosed += delegate
            {
                if (_fragment != null)
                {
                    _ft.BeginTransaction().Replace(Resource.Id.content_frame, _fragment).Commit();
                }
            };
            
            toggle.SyncState();
  
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                Log.Information($"Connectivity Changed IsConnected {args.IsConnected}");

                if (args.IsConnected)
                {
                    SyncData();
                }
            };

            MenuItemsAdapter adapter = new MenuItemsAdapter(MenuItem.GetMenuItems());
            GridLayoutManager manager = new GridLayoutManager(this, MenuItem.CalculateNoOfColumns(this));
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.nav_drawer_recycler_view);
            _recyclerView.SetAdapter(adapter);
            _recyclerView.SetLayoutManager(manager);
            adapter.ItemClick += DisplaySelectedFragment;

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);       
        }

        // se genera el botón de descanso y sus elementos que se recogen de GAM
        //private void InitializeBuilder()
        //{
        //    List<Breaks> breakList = _auxTable.GetBreaksList();
        //    _arrayItems = new string[breakList.Count];
        //    var l = _arrayItems.Length;

        //    for (var i = 0; i < breakList.Count; i++)
        //    {
        //        var item = breakList[i];

        //        if (item != null)
        //            _arrayItems.SetValue(item.Code + " - " + item.Description, i);
        //    }

        //    _builder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.AlertDialogResponseServer);
        //    _builder.SetTitle("Iniciar Descanso");
        //    _builder.SetSingleChoiceItems(_arrayItems, -1, BreakCodeSelected);
        //    _builder.SetNegativeButton("Cancelar", (Android.Content.IDialogInterfaceOnClickListener)null);
        //    _builder.SetPositiveButton("Iniciar", async (sender, args) =>
        //    {          
        //        var breakCode = _arrayItems[_selectedItem].Split('-')[0].Trim();

        //        var vhiBreak = new Break
        //        {
        //            StartDate = DateTime.Now,
        //            Code = breakCode,
        //            Vhi = DeviceHelper.Vhi
        //        };

        //        _auxTable.StartBreak(vhiBreak);

        //        await _gamDroidService.PostBreak(vhiBreak);

        //    });
        //}
        //private void BreakCodeSelected(object sender, DialogClickEventArgs eventArgs)
        //{
        //    _selectedItem = eventArgs.Which;
        //}

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var menuItem = menu.FindItem(Resource.Id.action_vhi);

            if (DeviceHelper.ClientCode == "VITA" || DeviceHelper.ClientCode == "VITMA")
                menuItem.SetVisible(false);

            menuItem.SetTitle(DeviceHelper.Vhi);

            var menuItemReset = menu.FindItem(Resource.Id.action_close);

            if (DeviceHelper.ClientCode != "SAGAL")
                menuItemReset.SetVisible(false);

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch (id)
            {
                case Resource.Id.action_refresh:

                    Task.Factory.StartNew(async () =>
                    {
                        await ManualSyncTransport();
                    });

                    break;
                case Resource.Id.action_close: //TODO: SAGALES, RESETEAR GAMDROID
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Confirmación");
                    alert.SetMessage("Se borraran los servicios y el turno");
                    alert.SetPositiveButton("Aceptar", (senderAlert, args) => {
                        _auxTable.ClearData();
                        DeviceHelper.ActiveShift = false;
                        Intent it = new Intent("update.ui.transport");
                        this.SendBroadcast(it);
                    });
                    alert.Show();
                    break;
                case Resource.Id.action_vhi:
                    ModifyVhiDialogFragment dialogVhi = new ModifyVhiDialogFragment();
                    dialogVhi.Show(_ft, "");
                    break;
                case Resource.Id.action_expenses:
                    DisplayDialogExpenses();
                    break;
                default:
                    break;
            }

            if (_fragmentToolbar != null)
            {
                _ft.BeginTransaction().Replace(Resource.Id.content_frame, _fragmentToolbar).Commit();
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {        
            return true; //devuelve true siempre porque es necesaria para la implementacion de la interfaz
        }

        private void DisplaySelectedFragment(object sender, int itemId)
        {
            Fragment fragment = null;

            switch (itemId) {

                case 1:
                    fragment = new FragmentTransport();//new TransportFragment();
                    break;
                case 2:

                    if (DeviceHelper.ClientCode == "CATA")
                    {
                        fragment = new ShiftFragment(() =>
                        {
                            _fragment = new CheckListFragment();
                            _ft.BeginTransaction().Replace(Resource.Id.content_frame, _fragment).Commit();
                        });
                    }
                    else
                    {
                        fragment = new ShiftFragment();
                    }
                    break;
                case 3:
                    fragment = new MessageFragment();
                    break;
                case 4:
                    fragment = new CheckListFragment();
                    break;
                case 5:
                    fragment = new TransportApplicationWebView();
                    break;
            }

            if(fragment != null)
            {         
                _fragment = fragment;
            }

            _drawer.CloseDrawer(GravityCompat.Start);
        }

        private void DisplayDialogExpenses()
        {
            ExpensesDialogFragment dia = new ExpensesDialogFragment();
            dia.Show(_ft, "");
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        //TURNO = FALSE NO SINCRONIA PUSH NOTIFICATION SERVICE
        private void SetUpPushNotificationService()
        {
        //    if (!DeviceHelper.ActiveShift) // SI NO HAY TURNO ACTIVO EL SERVICIO NO SE EJECUTA
        //        return;

            Log.Information("INIT OneSignalServiceInit");
            Com.OneSignal.Android.OneSignal.Init(this, "", "867a818b-60b0-434a-b71e-f50317ba00fe", null, null);
            Com.OneSignal.Android.OneSignal.SetInFocusDisplaying(Com.OneSignal.Android.OneSignal.OSInFocusDisplayOption.Notification);
            Com.OneSignal.Android.OneSignal.PromptLocation();
            Com.OneSignal.Android.OneSignal.SetLocationShared(false);
            Com.OneSignal.Android.OneSignal.SendTag("Vhi", DeviceHelper.Vhi);
            Com.OneSignal.Android.OneSignal.SendTag("Client", DeviceHelper.ClientCode);
            Log.Information($"One Signal TAGS [{DeviceHelper.Vhi} {DeviceHelper.ClientCode}]"); 
        }

        private async Task ManualSyncTransport()
        {
            try
            {
                await _gamSyncData.SyncTransportsGamDroid();
                Intent it = new Intent("update.ui.transport");
                this.SendBroadcast(it);
            }
            catch
            {

            }
        }

        //metodo para sincronizar los datos de GAM en GamDroid, creo que está fatal pero no se ya nada de esta vida
        private void SyncData()
        {
            Log.Information("Sync Data Triggered");

            try
            {
                Task.Factory.StartNew(async () =>
                {
                    await _gamSyncData.SyncAuxDataGamDroid();
                    await _gamSyncData.SyncTransportsGamDroid();
                    await _gamSyncData.SyncMessages();
                });
            }
            catch
            {

            }
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Log.Information("Connectivity Changed");
        }

        //private void Navigate(string action)
        //{
        //    DisplaySelectedFragment(null, 2);
        //}

        public class NavigationBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                var notType = intent.GetStringExtra("type");
                var notId = intent.GetIntExtra("notId", 0);
                Navigate(notType);
                Android.Support.V4.App.NotificationManagerCompat.From(context).Cancel(notId);
            }

            public delegate void NavigateToFragmentEventHandler(string action);
            public event NavigateToFragmentEventHandler Navigate;
        }
    }


    //public class NavigationTESTBroadcastReceiver : BroadcastReceiver
    //{
    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        //intent get extra string
    //        Navigate("");
    //    }

    //    public delegate void NavigateToFragmentEventHandler(string action);
    //    public event NavigateToFragmentEventHandler Navigate;
    //}
}

