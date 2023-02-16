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
using Message = GamDroid2018.Models.Message;

namespace GamDroid2018.Helpers
{
    public interface IMessageLite
    {
        List<Message> GetAllMessages();

        void InsertMessage(List<Message> message);

        List<MessageDto> GetNotSentMessages();

        Message GetMessage(string idMessage);

        void SetMessageReceivedStatus(string messageId, bool status);

        MessageDto InsertMessage(MessageDto message);

        void InsertCancellationMessage(string transportNumber);

        void DeleteMessage(string id);
    }
}