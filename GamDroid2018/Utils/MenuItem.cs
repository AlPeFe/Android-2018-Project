using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace GamDroid2018.Utils
{
    public class MenuItem
    {
        public int ItemId { get; set; }

        public int Icon { get; set; }
        
        public string Text { get; set; }     

        public ItemType Type { get; set; } 

        //aqui se añaden los iconos nuevos y los filtros con referencia a los clientes
        public static List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            menuItems.Add(
                new MenuItem
                {
                    ItemId = 1,
                    Icon = Resource.Drawable.ic_transport,
                    Text = "Servicios",
                    Type = ItemType.Transport
                });

            menuItems.Add(
                new MenuItem
                {
                    ItemId = 2,
                    Icon = Resource.Drawable.ic_turno,
                    Text = "Turno",
                    Type = ItemType.Message
                });

            menuItems.Add(
               new MenuItem
               {
                   ItemId = 3,
                   Icon = Resource.Drawable.ic_message,
                   Text = "Mensaje",
                   Type = ItemType.Shift
               });

            menuItems.Add(
             new MenuItem
             {
                 ItemId = 4,
                 Icon = Resource.Drawable.ic_check_box_24dp,
                 Text = "CheckList",
                 Type = ItemType.Shift
             });

            if (DeviceHelper.ClientCode == "OSOFT" || DeviceHelper.ClientCode == "UTECO")
            {
                menuItems.Add(
                   new MenuItem
                   {
                       ItemId = 5,
                       Icon = Resource.Drawable.ic_transport_application_24dp,
                       Text = "Alta Servicio",
                       Type = ItemType.Shift
                   });
            }


            return menuItems;
        }

        public static int CalculateNoOfColumns(Context context)
        {
            DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
            float dpWidth = displayMetrics.WidthPixels / displayMetrics.Density;
            int noOfColumns = (int)(dpWidth / 180);
            return noOfColumns;
        }
    }

    public enum ItemType
    {
        Message, Transport, Shift, CheckBox
    }
}