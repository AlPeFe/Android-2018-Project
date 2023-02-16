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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GamDroid2018.Helpers;

namespace GamDroid2018.Utils
{
    public class MessageDialogFragment : AppCompatDialogFragment
    {
        private IMessageLite _messageLite;

        public MessageDialogFragment(): this(new MessageLite()) { }

        internal MessageDialogFragment(IMessageLite messageLite)
        {
            _messageLite = messageLite;
        }

        //implemento un constructor diferente para poder pasarle los datos que necesito
        public static MessageDialogFragment Instance(string idMessage)
        {
            MessageDialogFragment fragment = new MessageDialogFragment();

            Bundle args = new Bundle();
            args.PutString("idMessage", idMessage);
            fragment.Arguments = args;

            return fragment;
        }
    
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.dialogFragment_message, container, false);
            TextView textTitle =  view.FindViewById<TextView>(Resource.Id.custom_title);
            TextView textContent = view.FindViewById<TextView>(Resource.Id.custom_message);
            Button btn = view.FindViewById<Button>(Resource.Id.btnDelete);
            var idMessage = Arguments.GetString("idMessage");

            var message = _messageLite.GetMessage(idMessage);

            if(message != null)
            {
                textTitle.Text = (message.Origin == 1 ? "Cordinación" : "GamDroid");
                textContent.Text = message.Content.ToUpper();
            }

            btn.Click += delegate
            {
                _messageLite.DeleteMessage(idMessage);
                this.Dismiss();
                Intent it = new Intent(Activity, typeof(Fragments.MessageFragment.MessageBroadcastReceiver));
                Activity.SendBroadcast(it);
            };

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
    }
}