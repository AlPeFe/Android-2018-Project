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
using Realms;

namespace GamDroid2018.Models
{
    public class Shift : RealmObject
    {
        [Realms.PrimaryKey]
        public string Id { get; set; }

        public string Km { get; set; }

        public string User { get; set; }

        public int ShiftType { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string Name { get; set; }

        public string Vhi { get; set; }

        public bool ShiftReceived { get; set; }

        public string Imei { get; set; }
    }

    public class ShiftDto
    {
        public string Id { get; set; }

        public string Km { get; set; }

        public string User { get; set; }

        public int ShiftType { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string Name { get; set; }

        public string Vhi { get; set; }

        public bool ShiftReceived { get; set; }

        public string Imei { get; set; }
    }

    public enum ShiftType
    {
        TES = 0, AYUDANTE= 1, Médico= 2, Enfermero = 3
    }

    public class ShiftResponse
    {
        public string Name { get; set; }

        public ShiftStatus ShiftStatus { get; set; }

    }

    public enum ShiftStatus
    {
        OK = 1, INTERNAL_ERROR = 2, MISSING = 3
    }
}