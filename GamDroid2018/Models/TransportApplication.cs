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
    public class TransportApplication
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public string Nass { get; set; }

        public DateTimeOffset? BirthDate { get; set; }

        public string OAddressCode { get; set; }

        public string OProvinceCode { get; set; }

        public string OProvince { get; set; }

        public string OCity { get; set; }

        public string OCityCode { get; set; }

        public string OAddress { get; set; }

        public string DProvinceCode { get; set; }

        public string DProvince { get; set; }

        public string DCity { get; set; }

        public string DCityCode { get; set; }

        public string DAddress { get; set; }

        public string DAddressCode { get; set; }

        public string Lot { get; set; }

        public string Company { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? DateAtDestination { get; set; }

        public DateTimeOffset? ReturnDate { get; set; }

        public string Observations { get; set; }

        public string ServiceType { get; set; }

        public string VhiType { get; set; }

        public string TransferType { get; set; }

        public string TransportType { get; set; }

        public bool Oxigen { get; set; }

        public bool Accompanied { get; set; }

        public bool Ramp { get; set; }

        public bool RoundTrip { get; set; }
    }

    public class TransportApplicationForm
    { 

        public Dictionary<string, string> Provinces { get; set; } //

        public List<Tuple<string, string, string>> Cities { get; set; }//

        public List<Tuple<string, string, string, string, string>> Places { get; set; }//

        public Dictionary<string, string> TransportReasons { get; set; }//

        public Dictionary<string, string> TransportType { get; set; }//

        public Dictionary<string, string> VhiType { get; set; }//

        public Dictionary<string, string> Companies { get; set; }//

        public Dictionary<string, string> TransferType { get; set; }

        public Dictionary<string, string> LotList { get; set; }//
    }
}