using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GamDroid2018.Helpers;
using GamDroid2018.Models;

namespace GamDroid2018.Utils
{
    public class ExpensesDialogFragment : AppCompatDialogFragment
    {
        private readonly IGamDroidService _gamDroidService;
        private readonly IAuxTableLite _auxTableLite;
        private readonly IShiftLite _shiftLite;
        private List<ExpenseDto> _listExpenses;

        private EditText _description;
        private EditText _km;
        private EditText _amount;
        private EditText _card;
        private Spinner _selectorExpense;
        private Button _buttonSend;

        
        public ExpensesDialogFragment() : this(new GamDroidService(), new AuxTableLite(), new ShiftLite()) { }

        internal ExpensesDialogFragment(IGamDroidService gamDroidService, IAuxTableLite auxTableLite, IShiftLite shiftLite)
        {
            _gamDroidService = gamDroidService;
            _auxTableLite = auxTableLite;
            _shiftLite = shiftLite;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.dialogFragmentExpenses, container, false);

            _description = view.FindViewById<EditText>(Resource.Id.descriptionValue); // definido en GAM el valor ( litros, pepinos, tupacs etc..)
            _selectorExpense = view.FindViewById<Spinner>(Resource.Id.expense_selector);
            _km = view.FindViewById<EditText>(Resource.Id.km_value);
            _card = view.FindViewById<EditText>(Resource.Id.card);
            _amount = view.FindViewById<EditText>(Resource.Id.inputAmount);
            _km.Visibility = ViewStates.Gone;
            _buttonSend = view.FindViewById<Button>(Resource.Id.btnSendExpense);

            _listExpenses = _auxTableLite.GetListExpenses();

            if (_listExpenses.Count == 0)
            {
                return null;
            }

            var arraySpinner = _listExpenses.Select(x => x.ExpenseCode + " | " + x.Description).ToArray();
            var arrayAdapter = new ArrayAdapter<string>(view.Context,
                   Android.Resource.Layout.SimpleSpinnerDropDownItem, arraySpinner);

            _selectorExpense.Adapter = arrayAdapter;

            _selectorExpense.ItemSelected += delegate { ItemSelected(_selectorExpense.SelectedItem?.ToString().Split('|')[0]); };
            _buttonSend.Click += async delegate { await SendExpense(); };

            return view;
        }

        private async Task SendExpense()
        {
            var listShifts = _shiftLite.GetActiveShifts();
            string code = "";
            string assistantCode = "";

            if(listShifts.Count > 0)
            {
                var tes = listShifts.Where(x => x.ShiftType == 0)
                    .Select(x => x.User)
                    .FirstOrDefault();

                if (tes != null)
                {
                    code = tes;
                }

                var assistant = listShifts.Where(x => x.ShiftType == 1)
                    .Select(x => x.User)
                    .FirstOrDefault();

                if(assistant != null)
                {
                    assistantCode = assistant;
                }
            }

            var reportExpense = 
                new ReportExpense
                    {
                        Amount = _amount.Text,
                        Km = _km.Text,
                        Date = DateTime.Now,
                        Code = _selectorExpense.SelectedItem
                            .ToString().Split('|').ElementAt(0).Trim(),
                        VhiCode = DeviceHelper.Vhi,
                        Card = _card.Text,
                        Tes = code,
                        AssistantCode = assistantCode,
                        InputValue = _description.Text
                    };

            var response = await _gamDroidService.PostReportExpenseAsync(reportExpense);

            if (response.Response != Models.ResponseStatus.OK)
                _auxTableLite.SaveReportExpense(reportExpense);

            Dismiss();    
        }

        private void ItemSelected(string itemCode)
        {
            _km.Visibility = ViewStates.Visible;
            _card.Visibility = ViewStates.Visible;

            var selectedItem = _listExpenses.Where(x => x.ExpenseCode == itemCode.Trim()).First();

            _description.Hint = selectedItem.DescriptionInputValue;

            if (!selectedItem.HasKm)
                _km.Visibility = ViewStates.Gone;

            //códigos genericos de GAM para gastos en el momento que se creen nuevos se tendrán que añadir
            if (!(selectedItem.ExpenseCode == "999"
                || selectedItem.ExpenseCode == "99G"
                 || selectedItem.ExpenseCode == "99A"
                  || selectedItem.ExpenseCode == "99S"
                   || selectedItem.ExpenseCode == "99B"))
                _card.Visibility = ViewStates.Gone;

        }

        public override void OnResume()
        {
            Window window = Dialog.Window;
            Point size = new Point();
            // Store dimensions of the screen in `size`
            Display display = window.WindowManager.DefaultDisplay;
            display.GetSize(size);
            // Set the width of the dialog proportional to 75% of the screen width
            window.SetLayout((int)(size.X * 0.75), WindowManagerLayoutParams.WrapContent);
            window.SetGravity(GravityFlags.Center);
            base.OnResume();

        }
    }
}