using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AutoMapper;
using GamDroid2018.Models;
using GamDroid2018.Utils;
using Newtonsoft.Json;
using Serilog;
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Helpers
{
    public class GamDroidService : IGamDroidService
    {
        private readonly HttpClient _client;

        public GamDroidService()
        {
            _client = new HttpClient
            {
                MaxResponseContentBufferSize = int.MaxValue
            };
        }

        //public async Task<TransportDetail> GetTransportDetailAsync(string numSrv)
        //{
        //    TransportDetail detail = null;

        //    try
        //    {
        //        var uri = new Uri(DeviceHelper.RestUrl + "/api/Transport/" + numSrv);

        //        var response = await _client.GetAsync(uri);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            detail = JsonConvert.DeserializeObject<TransportDetail>(content);
        //        }
        //    }
        //    catch
        //    {

        //    }

        //    return detail;
        //}

        public async Task<List<Transport>> GetTransportsAsync()
        {
            List<Transport> listTransport = new List<Transport>();

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Transport?vhi=" + DeviceHelper.Vhi);

                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    listTransport = JsonConvert.DeserializeObject<List<Transport>>(content);
                }
            }
            catch(Exception ex)
            {
                Log.Error($"GetTransportsAsync Error => {ex.Message}_{ex?.InnerException}");
            }
          
            return listTransport;
        }

        public async Task<bool> PostLocationAsync(Location location)
        {
            try
            {
                var gpsLocation = LocationHelper.GpsLocation(location);

                var uri = new Uri(DeviceHelper.RestUrl + "/api/Location");

                var content = new StringContent(JsonConvert.SerializeObject(gpsLocation), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<ShiftResponse> PostShiftAsync(ShiftDto shift)
        {
            ShiftResponse shiftResponse = new ShiftResponse();

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Shift");

                //var json = JsonConvert.SerializeObject(shift);

                var content = new StringContent(JsonConvert.SerializeObject(shift), Encoding.UTF8, "application/json");
            
                var response = await _client.PostAsync(uri, content);
       
                if (response.IsSuccessStatusCode)
                {
                    var httpResponse = await response.Content.ReadAsStringAsync();
                    shiftResponse = JsonConvert.DeserializeObject<ShiftResponse>(httpResponse);
                }

            }catch (Exception ex)
            {
                Log.Error($"{ex.ToString()}");
                shiftResponse.ShiftStatus = ShiftStatus.INTERNAL_ERROR;
            }

            return shiftResponse;
        }  

        public async Task<List<Cancellation>> GetCancellationAsync()
        {
            List<Cancellation> listCancellation = new List<Cancellation>();

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Cancellation");

                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    listCancellation = JsonConvert.DeserializeObject<List<Cancellation>>(content);
                }
            }
            catch
            {
                throw;
            }

            return listCancellation;
        }

        public async Task<List<Breaks>> GetBreaksAsync()
        {
            List<Breaks> listBreaks = new List<Breaks>();

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Breaks");

                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    listBreaks = JsonConvert.DeserializeObject<List<Breaks>>(content);
                }
            }
            catch
            {
               
            }

            return listBreaks;
        }

        public async Task<bool> PostBreak(Break breakVhi)
        {
            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Breaks");

                var content = new StringContent(JsonConvert.SerializeObject(breakVhi), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);
            
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<TransportResponse> PutPendingTransportAsync(Transport transport)
        {
            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Transport");

                var content = new StringContent(JsonConvert.SerializeObject(transport), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

            }
            catch
            {
                return TransportResponse.ERROR;
            }

            return TransportResponse.OK;
        }

        public async Task<TransportResponse> PostTransportStatus(TransportStatusDto transportStatus)
        {          
            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Transport");

                var json = JsonConvert.SerializeObject(transportStatus);

                var content = new StringContent(JsonConvert.SerializeObject(transportStatus), Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(uri, content);
              
            }
            catch(Exception ex)
            {
                return TransportResponse.ERROR;
            }

            return TransportResponse.OK;
        } 

        public async Task<List<Message>> GetMessagesAsync()
        {
            List<Message> listMessage = new List<Message>();

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Message?vhi=" + DeviceHelper.Vhi);

                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    listMessage = JsonConvert.DeserializeObject<List<Message>>(content);
                }
            }
            catch
            {
                throw;
            }

            return listMessage;
        }

        public async Task<MessageReceivedStatus> PostMessageAsync(MessageDto message)
        {
            MessageReceivedStatus status = MessageReceivedStatus.ERROR;

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Message");

                var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    status = MessageReceivedStatus.OK;
                }
            }
            catch(Exception ex)
            {
                status = MessageReceivedStatus.ERROR;
            }

            return status;
        }

        public async Task<MessageReceivedStatus> PostCancellationAsync(CancellationToken cancellationToken)
        {
            MessageReceivedStatus status = MessageReceivedStatus.ERROR;
            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Cancellation");

                var content = new StringContent(JsonConvert.SerializeObject(cancellationToken), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    status = MessageReceivedStatus.OK;
                                  
            }
            catch
            {
                return MessageReceivedStatus.ERROR;
            }

            return status;
        }

        public async Task<bool> GetVhiAsync(string vhi)
        {
            var result = false;

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/Vhi/" + vhi);
             
                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<bool>(content);
                }
            }
            catch
            {
                
            }

            return result;
        }

        public async Task PostGasExpensesAsync(GasExpense expense)
        {
            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/GasExpense");

                var content = new StringContent(JsonConvert.SerializeObject(expense), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);
             
            }
            catch
            {


            }


        }

        //public Task<bool> PostExpensesAsync(Expense expense)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<List<Expense>> GetExpensesAsync()
        {
            var uri = new Uri($"{DeviceHelper.RestUrl}/api/Expense");

            var response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                return new List<Expense>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Expense>>(content);
        }

        public async Task PostLogAsync(LogItem log)
        {
            try
            {

                var uri = new Uri(DeviceHelper.RestUrl + "/api/DevelopmentLogger");

                var content = new StringContent(JsonConvert.SerializeObject(log), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

            }
            catch
            {

            }
        }

        public async Task<List<CheckList>> GetCheckList()
        {
            List<CheckList> chkList = new List<CheckList>();

            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/CheckList");

                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    chkList = JsonConvert.DeserializeObject<List<CheckList>>(content);
                }
            }
            catch
            {

            }

            return chkList;
        }

        public async Task PostCheckList(CheckListCompleted checkList)
        {
            try
            {
                var uri = new Uri(DeviceHelper.RestUrl + "/api/CheckList");

                var json = JsonConvert.SerializeObject(checkList);

                var content = new StringContent(JsonConvert.SerializeObject(checkList), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

            }
            catch
            {

            }
        }

        public async Task<List<CancelTransport>> GetTransportsToCancel(List<string> listTransportToCheck)
        {           
            try
            {
                string url = DeviceHelper.RestUrl + "/api/Cancel?";

                foreach (var item in listTransportToCheck)
                {
                     url += "transportsToCheck=" + item + "&";
                }

                var uri = new Uri(url);

                var response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json =  JsonConvert.DeserializeObject<List<CancelTransport>>(content);

                    return json;
                }
            }
            catch
            {

            }

            return new List<CancelTransport>();
        } 

        public async Task<List<Dictionary<string, string>>> GetDataTransportApplication()
        {



            return null;
        }

        public async Task<GamDroidServiceResponse> PostReportExpenseAsync(ReportExpense expense)
        {
            //chiste
            var restPonse = new GamDroidServiceResponse();

            try
            {
                var uri = new Uri($"{ DeviceHelper.RestUrl }/api/Expense");

                var json = JsonConvert.SerializeObject(expense);

                var content = new StringContent(JsonConvert.SerializeObject(expense), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                var resContent = await response.Content.ReadAsStringAsync();
                restPonse =  JsonConvert.DeserializeObject<GamDroidServiceResponse>(resContent);
               
            }
            catch
            {
                restPonse.Response = Models.ResponseStatus.SERVER_ERROR;
            }

            return restPonse;
        }
    }
}