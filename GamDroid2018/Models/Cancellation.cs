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
    public class Cancellation : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string CancellationReaseon { get; set; }

        public string CancellationCode { get; set; }
    }

    public class CancellationDto
    {
        public string Id { get; set; }

        public string CancellationReaseon { get; set; }

        public string CancellationCode { get; set; }
    }

    public class CancellationToken 
    {
        public int Id { get; set; }

        public string Vhi { get; set; }

        public string AssistantCode { get; set; }

        public string TesCode { get; set; }

        public string CancellationCode { get; set; }

        public string ServiceNumber { get; set; }

        public DateTime CancellationDate { get; set; }

        public bool Recieved { get; set; }


    }

    public class CancelTransport
    {
        public string ServiceNumber { get; set; }

        public bool IsCanceled { get; set; }
    }
}