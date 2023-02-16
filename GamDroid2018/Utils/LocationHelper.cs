
using Android.Locations;

using GamDroid2018.Models;
using System;

namespace GamDroid2018.Utils
{
    public static class LocationHelper
    {
        public static GpsLocation GpsLocation(Location location)
        {
            return new GpsLocation
            {
                Date = DateTime.Now,//(new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(location.Time.ToString())),
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Speed = location.Speed,
                Vhi = DeviceHelper.Vhi
            };
        }
    }
}