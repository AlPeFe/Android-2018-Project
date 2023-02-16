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
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Utils
{
    public class AutoMaperConfig
    {
        //aqui se definen todos los mapping de los objetos de la capa de negocio
        public static void RegisterMapping()
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Transport, TransportDto>().ReverseMap();
                    cfg.CreateMap<Message, Message>().ReverseMap();
                    cfg.CreateMap<TransportStatus, TransportStatusDto>().ReverseMap();
                    cfg.CreateMap<CheckList, CheckListDto>().ReverseMap();
                    cfg.CreateMap<Shift, ShiftDto>().ReverseMap();
                    cfg.CreateMap<Device, DeviceDto>().ReverseMap();
                    cfg.CreateMap<Cancellation, CancellationDto>().ReverseMap();
                    cfg.CreateMap<Expense, ExpenseDto>().ReverseMap();
                    cfg.CreateMap<ReportExpense, ReportExpenseRealm>().ReverseMap();
                });
            }
            catch { }
        }
    }
}