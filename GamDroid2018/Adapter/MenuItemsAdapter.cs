using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using GamDroid2018.Utils;
using Android.Graphics.Drawables;

namespace GamDroid2018.Adapter
{
    class MenuItemsAdapter : RecyclerView.Adapter
    {
        List<MenuItem> _listItems;

        public event EventHandler<int> ItemClick;

        public MenuItemsAdapter(List<MenuItem> items)
        {
            _listItems = items;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_menuItem, parent, false);
            MenuItemsAdapterViewHolder vh = new MenuItemsAdapterViewHolder(itemView, OnItemClick);         
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = _listItems[position];
            var holder = viewHolder as MenuItemsAdapterViewHolder;
            holder.Icon.SetImageResource(item.Icon);
        }

        public override int ItemCount => _listItems.Count;

        void OnItemClick(int position)
        {
            ItemClick(this, _listItems[position].ItemId);
        }
     
    }

    public class MenuItemsAdapterViewHolder : RecyclerView.ViewHolder
    {
       
        public CardView Item { get; set; }

        public ImageView Icon { get; set; }

        public MenuItemsAdapterViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Icon = ItemView.FindViewById<ImageView>(Resource.Id.imgIcon);
            Item = itemView.FindViewById<CardView>(Resource.Id.cardViewMenuItem);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
           
        }
    }  
}