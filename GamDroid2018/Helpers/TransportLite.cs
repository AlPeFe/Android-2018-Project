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
using GamDroid2018.Utils;
using Realms;
using AutoMapper;
using Serilog;

namespace GamDroid2018.Helpers
{
    public class TransportLite : ITransportLite
    {
        private static object _lockObject = new object();

        public void DeleteTransport(string idTransport)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    var trp = realm.All<Transport>().FirstOrDefault(x => x.Id == idTransport);
                    realm.Write(() =>
                    {
                        if (trp != null)
                            realm.Remove(trp);
                    });
                }
            }catch(Exception ex)
            {
                Log.Error($"DeleteTransport REALM {ex.ToString()}");
            }
        }

        public List<TransportDto> GetAllTransports()
        {
            using (var realm = Realm.GetInstance())
            {
                var listTransports = realm.All<Transport>().Where(x => x.DateFinished == null && x.CancellationDate == null).OrderBy(x => x.StartDate).ToList();

                return Mapper.Map<List<TransportDto>>(listTransports);
            }        
        }

        public bool GetTransportByServiceNumber(string serviceNumber)
        {
            using (var realm = Realm.GetInstance())
            {
                return realm.All<Transport>().Where(x => x.ServiceNumber == serviceNumber && x.DateFinished == null && x.CancellationDate == null).Any();
            }
        }

        public TransportDto GetTransport(string idTransport)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    return Mapper.Map<TransportDto>(realm.All<Transport>().Where(x => x.Id == idTransport).First());
                }

            }catch(Exception ex)
            {
                Log.Error($"GetTransport REALM {ex.ToString()}");
            }

            return null; 
        }

        public TransportDto UpdateTransportStatus(string id, int status)
        {
            TransportDto trp = null;

            try
            {

                using (var realm = Realm.GetInstance())
                {

                    var transport = realm.All<Transport>().FirstOrDefault(x => x.Id == id);

                    if (transport != null)
                    {
                        realm.Write(() =>
                        {
                            transport.Status = status;

                            switch (status)
                            {
                                case 700:

                                    transport.Dateconfirmation = DateTime.Now;
                                    break;
                                case 705:
                                    transport.DateActive = DateTime.Now;
                                    break;
                                case 710:
                                    transport.DateAtOrigin = DateTime.Now;
                                    break;
                                case 715:
                                    transport.DateEvacuation = DateTime.Now;
                                    break;
                                case 720:
                                    transport.DateAtDestination = DateTime.Now;
                                    break;
                                case 725:
                                    transport.DateTransfer = DateTime.Now;
                                    break;
                                case 730:
                                    transport.DateRelease = DateTime.Now;
                                    break;
                                case 735:
                                    transport.DateFinished = DateTime.Now;
                                    break;
                                case 740:
                                    transport.DateReceived = DateTime.Now;
                                    break;
                            }

                            realm.Add(transport, update: true);

                        });


                        trp = Mapper.Map<TransportDto>(transport);
                    }
                }

            }catch(Exception ex) { Log.Error($"UpdateTransportStatus {ex.ToString()}"); }

            return trp;
        }

        public List<TransportDto> NewTransport(List<Transport> newTransport, List<TransportDto> currentTransports)
        {
            Log.Information("NEW TRANSPORT REALM");
            try
            {
                lock (_lockObject)
                {
                    currentTransports.Select(x => x.ServiceNumber).ToList()
                        .ForEach(x =>
                        {
                            var trp = newTransport.FirstOrDefault(c => c.ServiceNumber == x);

                            if (trp != null)
                            {
                                newTransport.Remove(trp);
                            }
                        });

                    newTransport.ForEach(x => { x.DateReceived = DateTime.Now; x.Id = Guid.NewGuid().ToString(); }); //marcar recibido en dispositivo

                    using (var realm = Realm.GetInstance())
                    {
                        newTransport.ForEach(x => realm.Write(() => { realm.Add(x); }));

                        return Mapper.Map<List<TransportDto>>(newTransport);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error($"{ex.ToString()} NEW TRANSPORT REALM");
            }

            return new List<TransportDto>();
        }      

        public void SetTransportReceived(string id, bool received)
        {
            using (var realm = Realm.GetInstance())
            {
                var transport = realm.All<Transport>().FirstOrDefault(x => x.Id == id);

                if (transport != null)
                {
                    transport.TransportReceived = true;
                    realm.Write(() => { realm.Add(transport, update: true); });
                }
            }
        }

        public void SetTransportStatusReceived(string idStatus)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    var status = realm.All<TransportStatus>().FirstOrDefault(c => c.Id == idStatus);

                    if (status != null)
                    {
                        realm.Write(() =>
                        {
                            status.Received = true;
                            realm.Add(status, update: true);
                        });
                    };
                }
            }catch(Exception ex)
            {
                Log.Error($"{ex.ToString()} SetTransportStatusReceived REALM");
            }
        }

        public void SetTransportStatusResponse(string idStatus, bool responseStatus)
        {
            using (var realm = Realm.GetInstance())
            {
                var status = realm.All<TransportStatus>().FirstOrDefault(c => c.Id == idStatus);

                if (status != null)
                {
                    realm.Write(() =>
                    {
                        status.Received = responseStatus;
                        realm.Add(status, update: true);
                    });
                };
            }
        }

        public List<TransportDto> GetNotSentTransports()
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<TransportDto>>(realm.All<Transport>().Where(x => x.TransportReceived == false).ToList());
            }
        }

        public void CancelTransportFromGam(string transportNumber)
        {
            try
            {
                lock (_lockObject)
                {
                    using (var realm = Realm.GetInstance())
                    {
                        var transportToCancel = realm.All<Transport>().FirstOrDefault(x => x.ServiceNumber == transportNumber);

                        realm.Write(() =>
                        {
                            if (transportToCancel != null)
                                realm.Remove(transportToCancel);
                        });
                    }
                }

            }catch(Exception ex)
            {
                Log.Error($"{ex.ToString()} CancelTransportFromGam REALM");
            }
        }

        public void CancelTransportFromGamDroid(string idTransport)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    var transportToCancel = realm.All<Transport>().FirstOrDefault(x => x.Id == idTransport);

                    if (transportToCancel != null)
                    {
                        realm.Write(() =>
                        {
                            transportToCancel.CancellationDate = DateTime.Now;
                            realm.Add(transportToCancel, update: true);
                        });
                    }
                }
            }catch(Exception ex)
            {
                Log.Error($"{ex.ToString()} CancelTransportFromGamDroid REALM");
            }
        }

        public TransportStatusDto NewTransportStatus(TransportStatus status)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    var statusId = Guid.NewGuid().ToString();

                    realm.Write(() =>
                    {
                        status.Id = statusId;
                        realm.Add(status);
                    });

                    return Mapper.Map<TransportStatusDto>(status);
                }
            }
            catch(Exception ex) { Log.Error($"NewTransportStatus Realm ERROR {ex.Message}_{ex?.InnerException} "); }

            return new TransportStatusDto();
        }

        public TransportDto NewTransport(Transport trp)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    var transportId = Guid.NewGuid().ToString();
                    realm.Write(() =>
                    {
                        trp.Id = transportId;
                        realm.Add(trp);
                    });

                    return Mapper.Map<TransportDto>(trp);
                }
            }catch(Exception ex)
            {
                Log.Error($"NewTransport Realm ERROR {ex.Message}_{ex?.InnerException} ");
            }

            return null;
        }

        public List<TransportStatusDto> GetNotSentTransportStatus()
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<TransportStatusDto>>(realm.All<TransportStatus>()
                    .Where(c => c.Received == false))
                    .ToList();
            }
        }
    }
}