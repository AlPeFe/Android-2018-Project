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
using Realms;
using AutoMapper;

namespace GamDroid2018.Helpers
{
    public class AuxTableLite : IAuxTableLite
    {
        public void SetCancellation(List<Cancellation> cancellationList)
        {
            using(var realm = Realm.GetInstance())
            {
                realm.Write(() =>
                {
                    cancellationList.ForEach(c =>
                    {
                        var canc = realm.All<Cancellation>().FirstOrDefault(x => x.CancellationCode == c.CancellationCode);

                        if (canc == null)
                        {
                            c.Id = Guid.NewGuid().ToString();
                            realm.Add(c);
                        }
                    });
                });
            }
        }

        public void ClearData()
        {
            using (var realm = Realm.GetInstance())
            {
                realm.Write(() =>
                {
                    realm.RemoveAll<Shift>();
                    realm.RemoveAll<Transport>();
                });
            }
        }

        public List<CancellationDto> GetCancellationList()
        {
            using(var realm = Realm.GetInstance())
            {
                var listCancellations = realm.All<Cancellation>().ToList();
                return Mapper.Map<List<CancellationDto>>(listCancellations);
            }
        }

        public CancellationDto GetCancellation(string idCancellation)
        {
            using(var realm = Realm.GetInstance())
            {
                return Mapper.Map<CancellationDto>(realm.All<Cancellation>().First(x => x.Id == idCancellation));
            }
        }

        public void SetExpenses(List<Expense> expenseList)
        {
            using(var realm = Realm.GetInstance())
            {
                expenseList
                .ForEach(x => 
                {
                    var expense = realm.All<Expense>().Where(c => c.ExpenseCode == x.ExpenseCode).FirstOrDefault();

                    if (expense == null)
                    {
                        realm.Write(() =>
                        {
                            x.Id = Guid.NewGuid().ToString();
                            realm.Add(x);
                        });
                    }
                });            
            }
        }

        public List<ExpenseDto> GetListExpenses()
        {
            using(var realm = Realm.GetInstance())
            {
               return Mapper.Map<List<ExpenseDto>>(realm.All<Expense>().ToList());
            }
        }

        public List<ReportExpense> GetPendingExpense()
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<ReportExpense>>(realm.All<ReportExpenseRealm>().ToList());
            }
        }

        public void SaveReportExpense(ReportExpense expense)
        {
            using (var realm = Realm.GetInstance())
            {             
                if (expense != null)
                {
                    realm.Write(() =>
                    {
                        expense.Id = Guid.NewGuid().ToString();
                        realm.Add(Mapper.Map<ReportExpenseRealm>(expense));
                    });
                }  
            }
        }

        public void DeletePendingExpense(string id)
        {
            using(var realm = Realm.GetInstance())
            {
                var expense = realm.All<ReportExpenseRealm>().FirstOrDefault(x => x.Id == id);

                if (expense != null) {

                    using (var trans = realm.BeginWrite())
                    {
                        realm.Remove(expense);
                        trans.Commit();
                    }
                }

            }
        }
    }
}