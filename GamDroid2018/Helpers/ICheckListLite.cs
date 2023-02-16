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
    public interface ICheckListLite
    {
        List<CheckListDto> GetCheckListIndex(string checkListCode);

        void InsertCheckList(List<CheckList> listChk);

        void DeleteCheckList();

        List<CheckListDto> GetCheckListType();
    }
}