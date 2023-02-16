using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AutoMapper;
using GamDroid2018.Helpers;
using GamDroid2018.Models;
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Utils
{
    public class NewMessageDialogFragment : AppCompatDialogFragment
    {
        IMessageLite _messageLite;
        IShiftLite _shiftLite;
        
        
        EditText _body;
        Spinner _codeSelector;
        Button _buttonSend;
        IGamDroidService _gamdroidService;
        LoadingProgressDialog _dialogProgress;

        public NewMessageDialogFragment
            () : this(new MessageLite(), new GamDroidService()) { }

        internal NewMessageDialogFragment(IMessageLite messageLite, IGamDroidService gamDroidService)
        {
            _messageLite = messageLite;
            _gamdroidService = gamDroidService;
            _shiftLite = new ShiftLite();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragmentDialog_newMessage, container, false);

            _body = view.FindViewById<EditText>(Resource.Id.bodyContent);
            _codeSelector = view.FindViewById<Spinner>(Resource.Id.messageCodeSelector);
            _buttonSend = view.FindViewById<Button>(Resource.Id.sendMessage);
            _buttonSend.Click += SendMessage;
            
            return view;
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

        private async void SendMessage(Object sender, EventArgs e)
        {
            _dialogProgress = new LoadingProgressDialog();
            _dialogProgress.Cancelable = false;
            _dialogProgress.Show(FragmentManager, "");

            var listShifts = _shiftLite.GetActiveShifts();

            var msg = new MessageDto
            {
                Content = _body.Text,
                MessageCode = _codeSelector.SelectedItem != null ? _codeSelector.SelectedItem.ToString() : "",
                Vhi = DeviceHelper.Vhi,
                Origin = 2,
                TesCode = listShifts.Where(c=> c.ShiftType == 0).Select(c=> c.User).FirstOrDefault() ?? "",
                AssistantCode = listShifts.Where(c => c.ShiftType == 1).Select(c => c.User).FirstOrDefault() ?? "",
                Time = DateTime.Now             
            };


            //msgDto y msg es lo mismo pero con el ID updateado, devuelvo el objeto entero igualmente
            var msgDto = _messageLite.InsertMessage(msg);

            var result = await  _gamdroidService.PostMessageAsync(msgDto);

             _messageLite.SetMessageReceivedStatus(msg.Id, result == MessageReceivedStatus.OK ? true : false);
                            
            Intent it = new Intent(Activity, typeof(Fragments.MessageFragment.MessageBroadcastReceiver));
            Activity.SendBroadcast(it);

            _dialogProgress.Dismiss();

            Dismiss();
        }
    }
}