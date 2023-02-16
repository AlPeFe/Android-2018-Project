using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using GamDroid2018.Adapter;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using GamDroid2018.Utils;
using Fragment = Android.Support.V4.App.Fragment;

namespace GamDroid2018.Fragments
{
    /// <summary>
    ///  ESTA ES LA NUEVA OPCION DESARROLLADA A PARTE 
    /// </summary>
    public class CheckListFragment : Fragment, ICheckListAdapterListeners
    {
        RecyclerView _recyclerView;
        Button _button;
        EditText _editText;
        Android.Widget.ListPopupWindow _popup;
        private ICheckListLite _checkListLite;
        private IGamDroidService _gamDroidService;
        private IShiftLite _shiftLite;
        private List<CheckListDto> _adapterItems;
        AdapterCheckList _adapter;
        private string[] _types;

        public CheckListFragment() : this(new CheckListLite(), new GamDroidService(), new ShiftLite())
        {

        }

        public CheckListFragment(CheckListLite checkListLite, GamDroidService gamDroidService, ShiftLite shiftLite)
        {
            this._checkListLite = checkListLite;
            this._gamDroidService = gamDroidService;
            this._shiftLite = shiftLite;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.checkListFragment_layout, container, false);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.checkListRecyclerView);
            _button = view.FindViewById<Button>(Resource.Id.btnSend);
            _editText = view.FindViewById<EditText>(Resource.Id.checkListTypeSelector);

            var charSeq = _types = _checkListLite.GetCheckListType().GroupBy(x => x.Code)
                  .Select(x =>
                      x.Select(c => c.CheckListName)
                      .First())
                      .Distinct().ToArray();

            _popup = new Android.Widget.ListPopupWindow(Activity);
            var popupAdapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, charSeq);
            _popup.SetAdapter(popupAdapter);

            _popup.AnchorView = _editText;

            _editText.Text = "SELECCIONAR TIPO";

            _editText.Click += (sender, e) =>
            {
                _popup.Show();
            };

            _button.Click -= OnButtonSendClick;
            _button.Click += OnButtonSendClick;

            _popup.ItemClick -= PopupItemSelected;
            _popup.ItemClick += PopupItemSelected;

            _recyclerView.HasFixedSize = true;

            return view;
        }

        public void OnButtonSendClick(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Enviar");
            builder.SetMessage("¿Seguro que quiere enviar los datos del checklist seleccionado?");
            builder.SetPositiveButton("Enviar", async delegate
            {
                var activeShifts = _shiftLite.GetActiveShifts();
                var km =
                    activeShifts.Where(x => x.ShiftType == 0).Select(c => c.Km).FirstOrDefault() == null
                        ? 0
                        : Convert.ToInt32(activeShifts.Where(x => x.ShiftType == 0).Select(c => c.Km)
                            .FirstOrDefault());

                await _gamDroidService.PostCheckList(
                    new CheckListCompleted
                    {
                        CheckList = _adapterItems,
                        Km = km,
                        VhiCode = DeviceHelper.Vhi,
                        User = activeShifts.Where(x => x.ShiftType == 0).Select(c => c.User).FirstOrDefault() ?? "",
                        SecondaryUser = activeShifts.Where(x => x.ShiftType == 1).Select(c => c.User).FirstOrDefault() ?? "",
                    });

                Toast.MakeText(Activity, "El checklist está preparado para su envío", ToastLength.Long).Show();

                //CERRAR EL CHECKLIST 
                var fm = Activity.SupportFragmentManager;
                var transaction = fm.BeginTransaction();
                transaction.Replace(Resource.Id.content_frame, new FragmentTransport());
                transaction.Commit();
            });

            builder.Show();
        }

        public void OnItemSelected(string id)
        {
            var item = _adapterItems.Where(c => c.GamId == id).FirstOrDefault();

            if(item == null)
               return;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Seleccionar");
            var charSeq = Enumerable.Range(0, Convert.ToInt32(item.MinValue) + 1).Select(c => c.ToString()).ToArray();

            builder.SetItems(charSeq, (sender, args) =>
            {
                item.Value = args.Which;

                _adapter.NotifyItemChanged(_adapterItems.IndexOf(item));
            });

            builder.Show();  
        }

        public void OnObsButtonClick(string id)
        {
            var item = _adapterItems.Where(c => c.GamId == id).FirstOrDefault();

            if (item == null)
                return;

            var builder = new Android.App.AlertDialog.Builder(Activity);
            builder.SetTitle("Observaciones");
            EditText txt = new EditText(Activity);
            txt.Text = item.Observations;
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.MatchParent);

            txt.LayoutParameters = lp;
            builder.SetView(txt);
            builder.SetNegativeButton("Cancelar", delegate { });
            builder.SetPositiveButton("Añadir", delegate
            {
                item.Observations = txt.Text;
                var index = _adapterItems.IndexOf(item);
                _adapter.NotifyItemChanged(index);
            });

            builder.Show();
        }

        public void OnChange(string id, bool value)
        {
            var item = _adapterItems.Where(c => c.GamId == id).FirstOrDefault();

            if (item == null)
                return;

            item.IsRequired = value;

            _adapter.NotifyItemChanged(_adapterItems.IndexOf(item));
        }

        private void PopupItemSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            var type = _types[e.Position];
            var code = _checkListLite.GetCheckListType().Where(c => c.CheckListName == type).Select(c => c.Code).First();
            _adapterItems = _checkListLite.GetCheckListIndex(code);

            _recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            _recyclerView.HasFixedSize = true;

            _adapter = new AdapterCheckList(_adapterItems, this, Activity);
            _recyclerView.SetAdapter(_adapter);
            _popup.Dismiss();
            _editText.Text = type;
        }
    }
}