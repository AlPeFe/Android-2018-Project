using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GamDroid2018.Models;
using ListPopupWindow = Android.Widget.ListPopupWindow;

namespace GamDroid2018.Adapter
{
    /// <summary>
    /// EL NUEVO ADAPTER A PARTE
    /// </summary>
    public class AdapterCheckList : RecyclerView.Adapter
    {
        private List<CheckListDto> _items;
        public ICheckListAdapterListeners listeners;
        Context _context;

        public AdapterCheckList(List<CheckListDto> items, ICheckListAdapterListeners listener, Context context)
        {
            _items = items;
            listeners = listener;
            _context = context;
        }

        public override int GetItemViewType(int position)
        {
            return _items[position].Type == "1" ? 1 : 2;
        }

        public override int ItemCount => _items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _items[position];

            switch (holder.ItemViewType)
            {
                case 1:
                    CheckListViewHolderSelector holderSelector = holder as CheckListViewHolderSelector;
                    holderSelector.checkListDescription.Text = $"{item.Description} ({item.MinValue})";

                
                    holderSelector.popupWindow.Tag = item.GamId;
                    holderSelector.popupWindow.Text = item.Value.ToString();

                    holderSelector.obsButton.Tag = item.GamId;

                    //popup.AnchorView = holderSelector.popupWindow;

                    holderSelector.popupWindow.Click -= OnSelectorClick;
                    holderSelector.popupWindow.Click += OnSelectorClick;

                    //popup.ItemClick -= OnItemSelected;
                    //popup.ItemClick += OnItemSelected;

                    holderSelector.obsButton.Click -= ObsAddClick;
                    holderSelector.obsButton.Click += ObsAddClick;

                    break;
                case 2:

                    CheckListViewHolderSwitch holderSwitch = holder as CheckListViewHolderSwitch;

                    holderSwitch.checkListDescription.Text = $"{item.Description}";
                    holderSwitch.switchValue.Checked = item.IsRequired;

                    holderSwitch.switchValue.Tag = item.GamId;
                    holderSwitch.obsButton.Tag = item.GamId;

                    holderSwitch.switchValue.CheckedChange -= OnSwitchChanged;
                    holderSwitch.switchValue.CheckedChange += OnSwitchChanged;

                    holderSwitch.obsButton.Click -= ObsAddClick;
                    holderSwitch.obsButton.Click += ObsAddClick;

                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;

            switch (viewType)
            {
                case 1:
                    itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.checkListItem_valueSelector, parent, false);
                    return new CheckListViewHolderSelector(itemView);
                case 2:
                    itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.checkListItem_switch, parent, false);
                    return new CheckListViewHolderSwitch(itemView);
            }

            return null;
        }


        private void OnSelectorClick(object sender, EventArgs e)
        {
            var id = (sender as EditText).Tag.ToString();

            listeners.OnItemSelected(id);
        }

        //private void OnItemSelected(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    var listPopup = (sender as Android.Widget.ListPopupWindow);
        //    var id = listPopup.AnchorView.Tag.ToString();
        //    listeners.OnItemSelected(id, e.Position);
        //    listPopup.Dismiss();
        //}

        private void OnSwitchChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if ((sender as Switch).Pressed)
            {
                var id = (sender as Switch).Tag.ToString();

                listeners.OnChange(id, e.IsChecked);
            }
        }

        private void ObsAddClick(object sender, EventArgs e)
        {
            var id = (sender as Button).Tag.ToString();

            listeners.OnObsButtonClick(id);
        }
    }

    public interface ICheckListAdapterListeners
    {
        void OnObsButtonClick(string id);

        void OnItemSelected(string id);

        void OnChange(string id, bool value);
    }

    public class CheckListViewHolderSwitch : RecyclerView.ViewHolder
    {
        public TextView checkListDescription;
        public Button obsButton;
        public Switch switchValue;

        public CheckListViewHolderSwitch(View itemView) : base(itemView)
        {
            obsButton = (Button)itemView.FindViewById(Resource.Id.btnObs);
            checkListDescription = (TextView)itemView.FindViewById(Resource.Id.itemDescription);
            switchValue = (Switch)ItemView.FindViewById(Resource.Id.switchValue);
        }
    }

    public class CheckListViewHolderSelector : RecyclerView.ViewHolder
    {
        public TextView checkListDescription;
        public Button obsButton;
        public EditText popupWindow;
        public CheckListViewHolderSelector(View itemView) : base(itemView)
        {
            obsButton = (Button)itemView.FindViewById(Resource.Id.btnObs);
            checkListDescription = (TextView)itemView.FindViewById(Resource.Id.itemDescription);
            popupWindow = (EditText)ItemView.FindViewById(Resource.Id.valueSelector);
        }
    }
}