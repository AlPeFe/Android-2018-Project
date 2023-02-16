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
    public interface IShiftLite
    {
        ShiftDto AddShift(ShiftDto shiftDto);

        ShiftDto EndShift(string id);

        bool GetShift(ShiftDto shift);

        List<ShiftDto> GetAllShifts();

        void UpdateShiftName(string id, string name);

        bool PendingShift();

        void UpdateShiftStatus(string id, bool status);

        List<ShiftDto> GetNotSentShift();

        List<ShiftDto> GetActiveShifts();

    }
}