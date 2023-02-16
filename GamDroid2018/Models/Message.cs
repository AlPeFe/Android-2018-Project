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
using Newtonsoft.Json;
using Realms;

namespace GamDroid2018.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Message : RealmObject
    {
       
        [Realms.PrimaryKey]
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Content { get; set; }

        [JsonProperty]
        public int Origin { get; set; }

        [JsonProperty]
        public DateTimeOffset Time { get; set; }

        [JsonProperty]
        public int MessageNumber { get; set; }

        [JsonProperty]
        public string MessageCode { get; set; }

        [JsonProperty]
        public string Vhi { get; set; }

        [JsonProperty]
        public string TesCode { get; set; }

        [JsonProperty]
        public string AssistantCode { get; set; }

        [JsonProperty]
        public bool MessageReceived { get; set; }
    }

    public class MessageDto
    {
        [Realms.PrimaryKey]
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Content { get; set; }

        [JsonProperty]
        public int Origin { get; set; }

        [JsonProperty]
        public DateTimeOffset Time { get; set; }

        [JsonProperty]
        public int MessageNumber { get; set; }

        [JsonProperty]
        public string MessageCode { get; set; }

        [JsonProperty]
        public string Vhi { get; set; }

        [JsonProperty]
        public string TesCode { get; set; }

        [JsonProperty]
        public string AssistantCode { get; set; }

        [JsonProperty]
        public bool MessageReceived { get; set; }
    }

    public enum Origin
    {
        Gam = 1, GamDroid = 2
    }

    public enum MessageReceivedStatus
    {
        OK = 1, ERROR = 2
    }
}