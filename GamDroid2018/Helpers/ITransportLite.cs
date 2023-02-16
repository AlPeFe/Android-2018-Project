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
using GamDroid2018.Models;

namespace GamDroid2018.Helpers
{
    public interface ITransportLite
    {
        List<TransportDto> GetAllTransports();

        TransportDto GetTransport(string idTransport);

        void DeleteTransport(string idTransport);

        TransportDto NewTransport(Transport trp);

        List<TransportDto> NewTransport(List<Transport> newTransport, List<TransportDto> currentTransports);

        TransportDto UpdateTransportStatus(string id, int status);

        void SetTransportReceived(string id, bool received);

        List<TransportDto> GetNotSentTransports();

        void CancelTransportFromGam(string transportNumber);

        void CancelTransportFromGamDroid(string idTransport);

        TransportStatusDto NewTransportStatus(TransportStatus status);

        void SetTransportStatusReceived(string idStatus);

        bool GetTransportByServiceNumber(string serviceNumber);

        void SetTransportStatusResponse(string idStatus, bool responseStatus);

        List<TransportStatusDto> GetNotSentTransportStatus();

        
    }
}