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
    public class Transport : RealmObject
    {
        
        public string Id { get; set; }

        [Realms.PrimaryKey]
        public string ServiceNumber { get; set; }
        public string Reference { get; set; }
        public string Observations { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DestinationDate { get; set; }
        public bool RoundTrip { get; set; }
        public bool Oxigen { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Age { get; set; }
        public int Gender { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string Dni { get; set; }
        public string Cip { get; set; }
        public string Nass { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Number { get; set; }
        public string Door { get; set; }
        public string DestinationStreet { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationProvince { get; set; }
        public string DestinationNumber { get; set; }
        public string DestinationDoor { get; set; }

        public DateTimeOffset? DateReceived { get; set; } //recibido
        public DateTimeOffset? Dateconfirmation { get; set; } //confirma
        public DateTimeOffset? DateActive { get; set; } //activacio
        public DateTimeOffset? DateEvacuation { get; set; } //evacuacio
        public DateTimeOffset? DateAtDestination { get; set; } //llega destino
        public DateTimeOffset? DateTransfer { get; set; } // transfer
        public DateTimeOffset? DateRelease { get; set; }
        public DateTimeOffset? DateFinished { get; set; }
        public DateTimeOffset? DateAtOrigin { get; set; } //
        public DateTimeOffset? CancellationDate { get; set; }

        public string PhoneNumber { get; set; }

        public string PhoneNumber2 { get; set; }

        public string PhoneNumber3 { get; set; }

        public bool Stretcher { get; set; }

        public bool WheelChair { get; set; }

        public bool Ramp { get; set; }

        public bool SpecialWheelChair { get; set; }

        public bool Companion { get; set; }

        public string OxigenConcentration { get; set; }

        public bool TransportReceived { get; set; }

        public string DestinationAddressCode { get; set; }

        public string OriginAddressCode { get; set; }

        public string TransportReason { get; set; }

        public string TransportType { get; set; }

        public string IsReturn { get; set; }

        public bool Assistant { get; set; }

        public bool Due { get; set; }

        public string IndividualColectivo { get; set; }


    }

    public class TransportDto
    {
        public string Id { get; set; }   
        public string ServiceNumber { get; set; }
        public string Reference { get; set; }
        public string Observations { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DestinationDate { get; set; }
        public bool RoundTrip { get; set; }
        public bool Oxigen { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Age { get; set; }
        public int Gender { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string Dni { get; set; }
        public string Cip { get; set; }
        public string Nass { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Number { get; set; }
        public string Door { get; set; }
        public string DestinationStreet { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationProvince { get; set; }
        public string DestinationNumber { get; set; }
        public string DestinationDoor { get; set; }
        public DateTimeOffset? DateReceived { get; set; } //recibido
        public DateTimeOffset? Dateconfirmation { get; set; } //confirma
        public DateTimeOffset? DateActive { get; set; } //activacio
        public DateTimeOffset? DateEvacuation { get; set; } //evacuacio
        public DateTimeOffset? DateAtDestination { get; set; } //llega destino
        public DateTimeOffset? DateTransfer { get; set; } // transfer
        public DateTimeOffset? DateRelease { get; set; }
        public DateTimeOffset? DateFinished { get; set; }
        public DateTimeOffset? DateAtOrigin { get; set; } //
        public DateTimeOffset? CancellationDate { get; set; }

        public string PhoneNumber { get; set; }

        public string PhoneNumber2 { get; set; }

        public string PhoneNumber3 { get; set; }

        public bool Stretcher { get; set; }

        public bool WheelChair { get; set; }

        public bool Ramp { get; set; }

        public bool SpecialWheelChair { get; set; }

        public bool Companion { get; set; }

        public string OxigenConcentration { get; set; }

        public bool TransportReceived { get; set; }

        public string DestinationAddressCode { get; set; }

        public string OriginAddressCode { get; set; }

        public string IndividualColectivo { get; set; }

        public string TransportReason { get; set; }

        public string TransportType { get; set; }

        public string IsReturn { get; set; }

        public bool Assistant { get; set; }

        public bool Due { get; set; }

        public string GetPatientName()
        {
            return $"{this.Name?.Trim()} {this.Surname?.Trim()}";
        }

        public string GetTransportDatesAndBasicInfo()
        {
            return $"{this.StartDate.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm")}" +
                $"| {this.ServiceNumber} | {this.IsReturn} | {this.TransportType} " +
                $"| {this.IndividualColectivo} " +
                $"| {this.DestinationDate.Value.ToLocalTime().ToString("HH:mm")}" ;
        }

        public string GetTransportExtraInfo()
        {
            string extraInfo = "";

            extraInfo += this.WheelChair  ? "Silla ruedas" : "";
            extraInfo += this.Ramp  ? "Rampa" : "";
            extraInfo += this.Stretcher ? "Camilla" : "";
           

            return extraInfo;
        }

        public string GetOriginAddress()
        {
            return $"{this.Street?.Trim()} " +
                $"{this.Number?.Trim()}, {this.City.Trim()} --> {this.DestinationCity?.Trim()}";
        }

        public string GetOriginAdressNavigation()
        {
            return $"{this.Street?.Trim()} " +
                  $"{this.Number?.Trim()}, {this.City.Trim()}";
        }

        public string GetDestinationAddress()
        {
            return $"{this.DestinationStreet?.Trim()} " +
                $"{this.DestinationNumber}, {this.DestinationCity?.Trim()}"; ;
        }

    }

    //public enum Gender
    //{
    //    MALE, FEMALE
    //}

    public enum TransportResponse
    {
        OK = 1, ERROR = 2
    }
}