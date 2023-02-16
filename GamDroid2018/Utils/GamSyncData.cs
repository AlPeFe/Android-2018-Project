using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GamDroid2018.Helpers;
using GamDroid2018.Models;

namespace GamDroid2018.Utils
{
    public class GamSyncData
    {
        private ITransportLite _transportLite;
        private IMessageLite _messageLite;
        private IAuxTableLite _auxTableLite;
        private IGamDroidService _gamDroidService;
        private ICheckListLite _checkListLite;

        public GamSyncData() : this(
            new TransportLite(),
            new MessageLite(), 
            new AuxTableLite(),
            new GamDroidService(),
            new CheckListLite()) { }

        internal GamSyncData(ITransportLite transportLite, 
            IMessageLite messageLite, 
            IAuxTableLite auxTableLite, 
            IGamDroidService gamDroidService, 
            ICheckListLite checkListLite)
        {
            _transportLite = transportLite;
            _messageLite = messageLite;
            _auxTableLite = auxTableLite;
            _gamDroidService = gamDroidService;
            _checkListLite = checkListLite;
        }

        public async Task SyncTransportsGamDroid()
        {
            var transport = await _gamDroidService.GetTransportsAsync();

            foreach (var trp in transport)
            {
                if (!_transportLite.GetTransportByServiceNumber(trp.ServiceNumber))
                {
                    var trpDto = _transportLite.NewTransport(trp);

                    if (trpDto.Status == 740) //marcar recibido en GAM 
                    {
                        var transportStatus = new TransportStatus
                        {
                            Date = DateTime.Now,
                            Status = trpDto.Status,
                            Vhi = DeviceHelper.Vhi,
                            TransportNumber = trpDto.ServiceNumber,
                            Latitude = "0",
                            Longitude = "0",
                        };

                        var trpStatus = _transportLite.NewTransportStatus(transportStatus);

                        var res = await _gamDroidService.PostTransportStatus(trpStatus);

                        if (res == TransportResponse.OK)
                            _transportLite.SetTransportStatusReceived(trpStatus.Id);

                       // Intent it = new Intent(MainActivity.context,
                       //typeof(Fragments.TransportFragment.TransportBroadcastReceiver));
                       // MainActivity.context.SendBroadcast(it);
                    }   
                }
            }

            var transportInDatabase = _transportLite.GetAllTransports();

            transportInDatabase.Where(p => transport.All(p2 => p2.ServiceNumber != p.ServiceNumber))
                .ToList()
                .ForEach(c=>
                {
                    _transportLite.DeleteTransport(c.Id);
                });
        }

        public async Task SyncAuxDataGamDroid()
        {       
           
                #region aux values

                _checkListLite.DeleteCheckList();

                var chkList = await _gamDroidService.GetCheckList();
                _checkListLite.InsertCheckList(chkList);

                var cancelationList = await _gamDroidService.GetCancellationAsync();
                _auxTableLite.SetCancellation(cancelationList);

                var expensesList = await _gamDroidService.GetExpensesAsync();
                _auxTableLite.SetExpenses(expensesList);
               
                #endregion
        }

        public async Task SyncMessages()
        {
            var messages = await _gamDroidService.GetMessagesAsync();

            if (messages.Count > 0)
            {
                messages.RemoveAll(c => string.IsNullOrWhiteSpace(c.Content));

                _messageLite.InsertMessage(messages);

                Intent it = new Intent(MainActivity.context, typeof(Fragments.MessageFragment.MessageBroadcastReceiver));
                MainActivity.context.SendBroadcast(it);
            }
        }
    }
}