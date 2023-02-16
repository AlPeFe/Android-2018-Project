using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GamDroid2018.Models;
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Helpers
{
    public interface IGamDroidService
    {
        Task<List<Transport>> GetTransportsAsync();
        Task<bool> PostLocationAsync(Location location);
        Task<ShiftResponse> PostShiftAsync(ShiftDto shift);
        //Task<TransportResponse> PutTransportStatus(Transport transport);
        Task<TransportResponse> PostTransportStatus(TransportStatusDto transportStatus);
        Task<List<Message>> GetMessagesAsync();
        Task<MessageReceivedStatus> PostMessageAsync(MessageDto message);
        Task<TransportResponse> PutPendingTransportAsync(Transport transport);
        Task<List<Cancellation>> GetCancellationAsync();
        Task<MessageReceivedStatus> PostCancellationAsync(CancellationToken cancellationToken);
        Task<bool> GetVhiAsync(string vhi);
        Task<List<Breaks>> GetBreaksAsync();
        Task<bool> PostBreak(Break breakVhi);
        //Task<bool> PostExpensesAsync(Expense expense);
        //Task<TransportDetail> GetTransportDetailAsync(string numSrv);
        Task PostGasExpensesAsync(GasExpense expense);
        Task PostLogAsync(LogItem log);
        Task<List<CheckList>> GetCheckList();
        Task PostCheckList(CheckListCompleted checkList);
        Task<List<CancelTransport>> GetTransportsToCancel(List<string> listTransportToCheck);
        Task<List<Dictionary<string, string>>> GetDataTransportApplication();
        Task<List<Expense>> GetExpensesAsync();
        Task<GamDroidServiceResponse> PostReportExpenseAsync(ReportExpense expense);
    }
}