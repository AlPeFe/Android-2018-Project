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
    public class CheckList : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Code { get; set; }

        public string GamId { get; set; } //sys215chk

        public int Index { get; set; } //ordre

        public string StockUnit { get; set; } //codi mat

        public string Type { get; set; } //tipus resposta

        public string Description { get; set; } //pregunta

        public int Value { get; set; } //selected value 

        public int MinValue { get; set; } //unitats minimes

        public bool IsRequired { get; set; } //SiNo

        public string Observations { get; set; }

        public string CheckListName { get; set; }
    }

    public class CheckListDto
    {

        public string Id { get; set; }

        public string Code { get; set; } //codiCheck 

        public string GamId { get; set; } //sys215chk

        public int Index { get; set; } //ordre

        public string StockUnit { get; set; } //codi mat

        public string Type { get; set; } //tipus resposta

        public string Description { get; set; } //pregunta

        public int Value { get; set; } //selectedValue

        public int MinValue { get; set; } //unitats minimes

        public bool IsRequired { get; set; } //SiNo

        public string Observations { get; set; }

        public string CheckListName { get; set; }
    }

    public class CheckListCompleted
    {
        public List<CheckListDto> CheckList { get; set; }

        public string VhiCode { get; set; }

        public string User { get; set; }

        public string SecondaryUser { get; set; }

        public int Km { get; set; }
    }
}