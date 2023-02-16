using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using GamDroid2018.Adapter;
using GamDroid2018.Helpers;
using GamDroid2018.Utils;
using Fragment = Android.Support.V4.App.Fragment;
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Fragments
{
    public class MessageFragment : Fragment
    {
        RecyclerView _recyclerView;
        readonly IMessageLite _messageLite;
        private FloatingActionButton _newMessageButton;
        public static MessageFragment Instance;
        private  MessageBroadcastReceiver _bReceiver;
        TextView _emptyMessageList;

        public MessageFragment() : this(new MessageLite()) { }

        internal MessageFragment(IMessageLite messageLite)
        {
            _messageLite = messageLite;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MessageFragment.Instance = this;
            _bReceiver = new MessageBroadcastReceiver();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.layout_messaging, container, false);
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.reyclerview_message_list);
            _newMessageButton = view.FindViewById<FloatingActionButton>(Resource.Id.fabNewMessage);
            _emptyMessageList = view.FindViewById<TextView>(Resource.Id.empty_view_message);

            var messageListCount = _messageLite.GetAllMessages().Count;

            _newMessageButton.Click += OnNewMessageClick;

            MessageAdapter adapter = new MessageAdapter();
            adapter.ItemLongClick += OnMessageItemLongClick;
            LinearLayoutManager lym = new LinearLayoutManager(Activity)
            {
                StackFromEnd = true
            };

            if (!DeviceHelper.ActiveShift)
                _newMessageButton.Visibility = ViewStates.Gone;

            _recyclerView.SetLayoutManager(lym); 
            _recyclerView.SetAdapter(adapter);
            _recyclerView.ScrollToPosition(messageListCount - 1);

            if(messageListCount == 0)
            {
                _emptyMessageList.Visibility = ViewStates.Visible;
            }

            return view;
        }

        private void OnMessageItemLongClick(object sender, string messageId)
        {
            var mng = FragmentManager;
            MessageDialogFragment fragment = MessageDialogFragment.Instance(messageId);
            fragment.Show(mng, "");
        }

        private void UpdateMessageFragment()
        {
            MessageAdapter adapter = new MessageAdapter();
            adapter.ItemLongClick += OnMessageItemLongClick;
            _recyclerView.SetAdapter(adapter);       
            _recyclerView.ScrollToPosition(_messageLite.GetAllMessages().Count - 1);
            _emptyMessageList.Visibility = ViewStates.Gone;

        }
        
        private void OnNewMessageClick(object sender, EventArgs e)
        {
            var mng = FragmentManager;
            NewMessageDialogFragment fragment = new NewMessageDialogFragment();
            fragment.Show(mng, "");          
        }

   
        [BroadcastReceiver]
        [IntentFilter(new[] { "com.message.update.ui" })]
        public class MessageBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                if (Instance != null)
                {
                    MessageFragment.Instance.UpdateMessageFragment();
                }
            }
        }
    }
}