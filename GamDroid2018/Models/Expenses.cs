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
    public class Expense : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Description { get; set; }

        public string DescriptionInputValue { get; set; }

        public bool HasKm { get; set; }

        public string ExpenseCode { get; set; }
    }

    public class ExpenseDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string DescriptionInputValue { get; set; }

        public bool HasKm { get; set; }

        public string ExpenseCode { get; set; }
    }


    public class ReportExpenseRealm : RealmObject
    {
        public string Id { get; set; }
        public DateTimeOffset Date { get; set; }
        
        public string Code { get; set; }

        public string InputValue { get; set; }

        public string Card { get; set; }

        public string VhiCode { get; set; }

        public string Km { get; set; }

        public string Tes { get; set; }

        public string AssistantCode { get; set; }

        public string Amount { get; set; }
    }

    public class ReportExpense
    {
        public string Id { get; set; }
        public DateTimeOffset Date { get; set; }

        public string Code { get; set; }

        public string InputValue { get; set; }

        public string Card { get; set; }

        public string VhiCode { get; set; }

        public string Km { get; set; }

        public string Tes { get; set; }

        public string AssistantCode { get; set; }

        public string Amount { get; set; }
    }

    public class GasExpense 
    {
        public DateTime Date { get; set; }

        public string Value { get; set; }

        public string Km { get; set; }

        public string Liters { get; set; }

        public string Amount { get; set; }

        public string TesCode { get; set; }

        public string AssistantCode { get; set; }

        public string VhiCode { get; set; }

        public string Code { get; set; }
    }
}