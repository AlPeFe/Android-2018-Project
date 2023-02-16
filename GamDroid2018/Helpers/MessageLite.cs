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
using GamDroid2018.Utils;
using Realms;
using Message = GamDroid2018.Models.Message;
using AutoMapper;
using GamDroid2018.Models;
using Serilog;

namespace GamDroid2018.Helpers
{
    public class MessageLite : IMessageLite
    {
        private static object _lockObject = new object();
        public List<Message> GetAllMessages()
        {
            // se separa el filtro ya que realm no permite con linq+

            var filterDateTime = DateTime.Now.AddHours(-24);
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<Message>>(realm.All<Message>().Where(c=> c.Time >= filterDateTime).ToList());
            }
        }    

        public void CleanMessageTable()
        {
            using (var realm = Realm.GetInstance())
            {
                var listMessagesToDelete = realm.All<Message>().Where(c => c.Time <= DateTime.Now.AddHours(-24)).ToList();

                listMessagesToDelete.ForEach(c =>
                {
                    realm.Write(() =>
                    {
                        realm.Remove(c);
                    });
                });
            }
        }
        
        public void InsertMessage(List<Message> message)
        {
            lock (_lockObject)
            {
                using (var realm = Realm.GetInstance())
                {
                    message.ForEach(x =>
                        realm.Write(() =>
                        {
                            x.Id = Guid.NewGuid().ToString();
                            realm.Add(x);
                        }));
                }
            }
        }

        public MessageDto InsertMessage(MessageDto message)
        {
            var messageToInsert = Mapper.Map<Message>(message);

            using (var realm = Realm.GetInstance())
            {          
                realm.Write(() =>
                {
                    messageToInsert.Id = Guid.NewGuid().ToString();
                    realm.Add(messageToInsert);
                    message.Id = messageToInsert.Id;
                });
            }

            return message;
        }

        public List<MessageDto> GetNotSentMessages()
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<List<MessageDto>>(realm.All<Message>().Where(x => x.MessageReceived == false).ToList());
            }
        }

        public void DeleteMessage(string id)
        {
            try
            {
                using (var realm = Realm.GetInstance())
                {
                    var msg = realm.All<Message>().FirstOrDefault(x => x.Id == id);
                    realm.Write(() =>
                    {
                        if (msg != null)
                            realm.Remove(msg);
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error($"DeleteTransport REALM {ex.ToString()}");
            }
        }

        public Message GetMessage(string idMessage)
        {
            using (var realm = Realm.GetInstance())
            {
                return Mapper.Map<Message>(realm.All<Message>().FirstOrDefault(x => x.Id == idMessage));
            }
        }

        public void SetMessageReceivedStatus(string messageId, bool status)
        {
            using (var realm = Realm.GetInstance())
            {
                var message = realm.All<Message>().FirstOrDefault(x => x.Id == messageId);

                if (message != null)
                {
                    realm.Write(() =>
                    {
                        message.MessageReceived = status;
                        realm.Add(message, update: true);
                    });
                } 
            }
        }

        public void InsertCancellationMessage(string transportNumber)
        {
            using(var realm = Realm.GetInstance())
            {
                realm.Write(() => 
                {
                    var msg = new Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        MessageNumber = 0,
                        Content = $"Se ha anulado el servicio {transportNumber}",
                        MessageCode = "",
                        MessageReceived = true,
                        Origin = 1,
                        TesCode = "",
                        Time = DateTime.Now,
                        Vhi = DeviceHelper.Vhi
                    };

                    realm.Add(msg);
                });
            }

        }

    }
}