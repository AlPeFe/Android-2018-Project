using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Message = GamDroid2018.Models.Message;
using GamDroid2018.Helpers;

namespace GamDroid2018.Adapter
{
    class MessageAdapter : RecyclerView.Adapter
    {
        List<Message> _listMessage;
        IMessageLite _messageLite;
        private static int MESSAGE_SENT => 1;
        private static int MESSAGE_RECIEVED => 2;

        public event EventHandler<string> ItemLongClick;

        public MessageAdapter() : this(new MessageLite()) { }

        internal MessageAdapter(IMessageLite messageLite)
        {
            _messageLite = messageLite;
            _listMessage = _messageLite.GetAllMessages();             
        }

        public override int GetItemViewType(int position)
        {
            var message = _listMessage[position];

            if(message.Origin == 1)
            {
                return MESSAGE_RECIEVED;
            }
            else
            {
                return MESSAGE_SENT;
            }
            
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {           
            if(viewType == MESSAGE_RECIEVED)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.in_message, parent, false);

                MessageAdapterInViewHolder vh = new MessageAdapterInViewHolder(itemView, OnLongClick);
                return vh;

            }
            else if(viewType == MESSAGE_SENT)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.out_message, parent, false);               
                MessageAdapterOutViewHolder vh = new MessageAdapterOutViewHolder(itemView, OnLongClick);
                return vh;
            }

            return null;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = _listMessage[position];

            if (item.Origin == 1)
            {
                ((MessageAdapterInViewHolder)viewHolder).Bind(item);
            }
            else
            {
                ((MessageAdapterOutViewHolder)viewHolder).Bind(item);
            }
        }

        void OnLongClick(int position)
        {
            ItemLongClick?.Invoke(this, _listMessage[position].Id);
        }

      

        public override int ItemCount => _listMessage.Count;
    }

    public class MessageAdapterInViewHolder : RecyclerView.ViewHolder
    {
        public RelativeLayout LayoutRelative { get; set; }

        public TextView Sender { get; set; }

        public TextView Title { get; set; }

        public TextView MessageBody { get; set; }

        public TextView MessageTime { get; set; }

        public MessageAdapterInViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            LayoutRelative = ItemView.FindViewById<RelativeLayout>(Resource.Id.layout);
            Sender = itemView.FindViewById<TextView>(Resource.Id.tvEmailSender);
            MessageBody = itemView.FindViewById<TextView>(Resource.Id.tvEmailDetails);
            MessageTime = ItemView.FindViewById<TextView>(Resource.Id.tvEmailTime);
            Title = itemView.FindViewById<TextView>(Resource.Id.tvEmailTitle);
            itemView.LongClick += (sender, e) => listener(base.LayoutPosition);
        }

        public void Bind(Message message)
        {
            Sender.Text = "Cordinación";
            Title.Text = "Nuevo Mensaje";
            MessageBody.Text = message.Content;
            MessageTime.Text = message.Time.ToString("dd/MM/yyyy HH:mm");  //DateTime.ToString("dd/MM/yyyy HH:mm");
            LayoutRelative.SetBackgroundColor(Android.Graphics.Color.LightGray);
        }
    }

    public class MessageAdapterOutViewHolder : RecyclerView.ViewHolder
    {
        public RelativeLayout LayoutRelative { get; set; }

        public TextView Sender { get; set; }

        public TextView Title { get; set; }

        public TextView MessageBody { get; set; }

        public TextView MessageTime { get; set; }

        public MessageAdapterOutViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Sender = itemView.FindViewById<TextView>(Resource.Id.tvEmailSender);
            LayoutRelative = ItemView.FindViewById<RelativeLayout>(Resource.Id.layout);
            MessageBody = itemView.FindViewById<TextView>(Resource.Id.tvEmailDetails);
            MessageTime = ItemView.FindViewById<TextView>(Resource.Id.tvEmailTime);
            Title = itemView.FindViewById<TextView>(Resource.Id.tvEmailTitle);
            itemView.LongClick += (sender, e) => listener(base.LayoutPosition);
        }

        public void Bind(Message message)
        {
            Sender.Text = "Tu Mensaje";           
            MessageBody.Text = message.Content;
            MessageTime.Text = message.Time.ToLocalTime().DateTime.ToString("dd/MM/yyyy HH:mm");
            Title.Text = "";
            LayoutRelative.SetBackgroundColor(Android.Graphics.Color.LightGreen);
        }
    }

    public class MessageClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

}