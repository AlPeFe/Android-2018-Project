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
    public class ShiftLite : IShiftLite
    {
        public ShiftDto AddShift(ShiftDto shiftDto)
        {
            var shift = Mapper.Map<Shift>(shiftDto);

            using (var realm = Realm.GetInstance())
            {
                realm.Write(() => 
                {
                    shift.Id = Guid.NewGuid().ToString();
                    realm.Add(shift);
                });

                return Mapper.Map<ShiftDto>(shift);
            } 
        }

        public ShiftDto EndShift(string id)
        {
            using(var realm = Realm.GetInstance())
            {
                var shift = realm.All<Shift>().Where(x => x.Id == id).FirstOrDefault();

                if (shift != null)
                {
                    realm.Write(() =>
                    {
                        shift.EndDate = DateTime.Now;
                        realm.Add(shift, update: true);
                    });
                }

                return Mapper.Map<ShiftDto>(shift);
            }
        }

        public List<ShiftDto> GetAllShifts()
        {
            using(var realm = Realm.GetInstance())
            {
                var listShift = realm.All<Shift>().Where(x => x.EndDate == null).ToList();
                return Mapper.Map<List<ShiftDto>>(listShift);
            }
        }

        public bool PendingShift()
        {
            using(var realm = Realm.GetInstance())
            {
                return realm.All<Shift>().Where(x => x.ShiftReceived == false).Any();
            }
        }

        public bool GetShift(ShiftDto shift)
        {
            using(var realm = Realm.GetInstance())
            {
                return realm.All<Shift>().Any(x => (x.ShiftType == shift.ShiftType && x.EndDate == null) || (x.User == shift.User && x.EndDate == null));
            }
        }

        public void UpdateShiftName(string id, string name)
        {
            using(var realm = Realm.GetInstance())
            {
                var shiftToUpdate = realm.All<Shift>().FirstOrDefault(x => x.Id == id);

                if(shiftToUpdate != null)
                {
                    realm.Write(() =>
                    {
                        shiftToUpdate.Name = name;
                        realm.Add(shiftToUpdate, update: true);
                    });
                }
            }
        }

        public void UpdateShiftStatus(string id, bool status)
        {
            using(var realm = Realm.GetInstance())
            {
                var shift = realm.All<Shift>().FirstOrDefault(x => x.Id == id);

                if(shift != null)
                {
                    realm.Write(() =>
                    {
                        shift.ShiftReceived = status;
                        realm.Add(shift, update: true);
                    });
                }
            }
        }

        public List<ShiftDto> GetNotSentShift()
        {
            using (var realm = Realm.GetInstance())
            {
                var listShifts = realm.All<Shift>().Where(x => x.ShiftReceived == false).ToList();

                return Mapper.Map<List<ShiftDto>>(listShifts);
            }   
        }

        public List<ShiftDto> GetActiveShifts()
        {
            using(var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<ShiftDto>>(realm.All<Shift>().Where(x => x.EndDate == null).ToList());
            }
        }
    }
}