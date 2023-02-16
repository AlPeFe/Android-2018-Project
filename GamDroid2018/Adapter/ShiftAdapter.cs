using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Content;
using GamDroid2018.Models;
using System.Collections.Generic;
using GamDroid2018.Helpers;
using GamDroid2018.Utils;
using Android.App;
using AutoMapper;

namespace GamDroid2018.Adapter
{
    class ShiftAdapter : RecyclerView.Adapter
    {      
        Context _context;
        List<ShiftDto> _listShift;
        IShiftLite _shiftLite;
        IGamDroidService _gamDroidService;
    
        public ShiftAdapter() : this(new ShiftLite(), new GamDroidService()) { }

        internal ShiftAdapter(IShiftLite shiftLite, IGamDroidService gamDroidService)
        {
            _shiftLite = shiftLite;
            _gamDroidService = gamDroidService;
            _listShift = _shiftLite.GetAllShifts();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_shift, parent, false);          
            var vh = new ShiftAdapterViewHolder(itemView);
            _context = parent.Context;
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = _listShift[position];
            if (viewHolder is ShiftAdapterViewHolder holder)
            {
                holder.Name.Text = item.Name;
                holder.ShiftType.Text = item.ShiftType == 0 ? "TES" : item.ShiftType == 1 ? "AYUDANTE" : item.ShiftType == 2 ? "DUE" : item.ShiftType == 3 ? "MÉDICO" : "";

                if (!holder.ButtonEndShift.HasOnClickListeners)
                {
                    holder.ButtonEndShift.Click += delegate
                    {
                        holder.ButtonEndShift.Enabled = false;
                        EndShiftClick(item.Id, viewHolder.AdapterPosition);
                        holder.ButtonEndShift.Enabled = true;
                    };
                }
            }
        }

        private void EndShiftClick(string id, int position)
        {
            var builder = new AlertDialog.Builder(_context);
            builder.SetTitle("Introduzca los KM actuales");
            EditText txt = new EditText(_context);
            txt.InputType = Android.Text.InputTypes.ClassNumber;
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.MatchParent);

            txt.LayoutParameters = lp;
            builder.SetView(txt);
            builder.SetNegativeButton("Cancelar", delegate { });
            builder.SetPositiveButton("Enviar", async delegate
            {
                var shift = _shiftLite.EndShift(id);
                shift.Km = txt.Text;

                if (shift != null)
                {
                    var response = await _gamDroidService.PostShiftAsync(shift);
                    _shiftLite.UpdateShiftStatus(id, response.ShiftStatus == ShiftStatus.OK ? true : false); //marco si se ha recibido OK el status en el servidor
                }

                _listShift.RemoveAt(position);

                this.NotifyItemRemoved(position);

                var shiftActive = _shiftLite.GetAllShifts();

                if (shiftActive.Count == 0)
                {
                    DeviceHelper.ActiveShift = false;
                    Com.OneSignal.Android.OneSignal.DeleteTag("ActiveShift");
                }
            });

            builder.Show();       
        }

        public override int ItemCount => _listShift.Count;
       
    }

    public class ShiftAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public TextView ShiftType { get; set; }
        public Button ButtonEndShift { get; set; }

        public ShiftAdapterViewHolder(View itemView) : base(itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.shiftContentName);
            ShiftType = itemView.FindViewById<TextView>(Resource.Id.shiftType);
            ButtonEndShift = itemView.FindViewById<Button>(Resource.Id.btnEndshift);
        }
    }  
}