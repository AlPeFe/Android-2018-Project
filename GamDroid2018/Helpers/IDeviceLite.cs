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
using GamDroid2018.Models;

namespace GamDroid2018.Helpers
{
    public interface IDeviceLite
    {
        void SetDeviceConfiguration(Device device);

        DeviceDto GetDeviceConfiguration();

        void ModifyVhiValue(string vhi);
    }
}