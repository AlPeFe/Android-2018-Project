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
using AutoMapper;
using GamDroid2018.Models;
using GamDroid2018.Utils;
using Realms;

namespace GamDroid2018.Helpers
{
    public class DeviceLite : IDeviceLite
    {
        public void SetDeviceConfiguration(Device device)
        {
            using(var realm = Realms.Realm.GetInstance())
            {
                realm.Write(() =>
                {
                    device.Id = Guid.NewGuid().ToString();
                    realm.Add(device);
                });
              
            }
        }

        public DeviceDto GetDeviceConfiguration()
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<DeviceDto>(realm.All<Device>().FirstOrDefault());
            }
        }

        public void ModifyVhiValue(string vhi)
        {
            using(var realm = Realm.GetInstance())
            {
                var deviceConfig = realm.All<Device>().FirstOrDefault();

                if(deviceConfig != null)
                {
                    realm.Write(() =>
                    {
                        deviceConfig.Vhi = vhi;
                        realm.Add(deviceConfig, update: true);
                    });
                }
            }
        }
    }
}