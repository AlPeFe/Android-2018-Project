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
using GamDroid2018.Utils;
using AutoMapper;
using Realms;

namespace GamDroid2018.Helpers
{
    public class CheckListLite : ICheckListLite
    {
        public List<CheckListDto> GetCheckListIndex(string checkListCode)
        {
            using(var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<CheckListDto>>(
                    realm.All<CheckList>()
                    .Where(x => x.Code == checkListCode)
                    .OrderBy(x => x.Index).ToList());
            }
        }

        public void InsertCheckList(List<CheckList> listChk)
        {
            using (var realm = Realm.GetInstance())
            {
                realm.Write(() =>
                {
                    listChk.ForEach(x => 
                    {
                        x.Id = Guid.NewGuid().ToString();
                        realm.Add(x);
                    });
                });
            }
        }

        public void DeleteCheckList()
        {
            using (var realm = Realm.GetInstance())
            {
                realm.Write(() =>
                {
                    realm.RemoveAll<CheckList>();
                });
            }
        }

        public List<CheckListDto> GetCheckListType()
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<CheckListDto>>(realm.All<CheckList>().ToList());
            }
        }
    }
}