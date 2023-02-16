using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GamDroid2018.Models
{
    public class Breaks 
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }
    }


    public class Break : Breaks
    {
        public string Vhi { get; set; }

        public bool Received { get; set; }

        public DateTime? StartDate { get; set; } 

        public DateTime? EndDate { get; set; }
    }
}