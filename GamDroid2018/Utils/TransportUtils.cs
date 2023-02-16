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

namespace GamDroid2018.Utils
{
    public class TransportUtils
    {      

        /// <summary>
        /// Devuelve la "configuración" del elemento status de la vista, color y descripción
        /// </summary>
        /// <param name="status">status del servicio</param>
        /// <returns></returns>
        public static TransportStatusItem GetStatus(int status, string clientCode = "")
        {
            if(clientCode == "CUEN") //CUENCA
            {
                return new TransportStatusItem
                {
                    Colour = GetColour(status),
                    Status = status,
                    Description = GetDescriptionCuenca(status)
                };
            }
            else
            {
                return new TransportStatusItem
                {
                    Colour = GetColour(status),
                    Status = status,
                    Description = GetDescription(status)
                };
            }
        }

        public static string GetNavigationAddress(int status, TransportDto transport)
        {
            string adress = "";

            if ((status < 715) || (status == 740))
                adress = $"{transport.Street?.Trim()} {transport.DestinationNumber?.Trim()}, {transport.City?.Trim()}";

            if ((status >= 715) && (status != 740))
                adress = $"{transport.DestinationStreet?.Trim()} {transport.DestinationNumber}, {transport.DestinationCity?.Trim()}"; //+ ", " + transport.Destination.Province;

            return adress;
        }

        /// <summary>
        /// Devuelve la dirección basada en el status del servicio
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        /// 
        public static string GetAdressByStatus(int status, TransportDto transport)
        {
            string adress = string.Empty;

            if ((status < 715) || (status == 740))
                adress = $"{transport.Street?.Trim()} {transport.Number?.Trim()}, {transport.City?.Trim()} -> {transport.DestinationCity?.Trim()}"; //+ ", " + transport.Origin.Province;

            if ((status >= 715) && (status != 740)) 
                adress = $"{transport.DestinationStreet?.Trim()} {transport.DestinationNumber}, {transport.DestinationCity?.Trim()}"; //+ ", " + transport.Destination.Province;

            return adress;
        }

        /// <summary>
        /// Qqui van los colores de cada status, para mostrar luego en el adapter de transporte
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static Android.Graphics.Color GetColour(int status)
        {
            Android.Graphics.Color color = new Android.Graphics.Color();

            switch (status)
            {
                case 700:
                    color = Android.Graphics.Color.Green;
                    break;
                case 705:
                    color = Android.Graphics.Color.LightGreen;
                    break;
                case 710:
                    color = Android.Graphics.Color.Yellow;
                    break;
                case 715:
                    color = Android.Graphics.Color.Orange;
                    break;
                case 720:
                    color = Android.Graphics.Color.OrangeRed;
                    break;
                case 725:
                    color = Android.Graphics.Color.DarkOrange;
                    break;
                case 730:
                    color = Android.Graphics.Color.Brown;
                    break;
                case 735:
                    color = Android.Graphics.Color.RosyBrown;
                    break;
                case 740:
                    color = Android.Graphics.Color.Green;
                    break;
                default:
                    color = Android.Graphics.Color.BurlyWood;
                    break;
            }

            return color;
        }

        /// <summary>
        /// el texto que se muestra en el status, aqui irian los switch también por cliente
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static string GetDescription(int status)
        {
            string description = "";

            switch (status)
            {
                case 700:
                    description = "CONFIRMA";
                    break;
                case 705:
                    description = "ACTIVACION";
                    break;
                case 710:
                    description = "LLEGA ORIGEN";
                    break;
                case 715:
                    description = "EVACUACION";
                    break;
                case 720:
                    description = "LLEGA DESTINO";
                    break;
                case 725:
                    description = "TRANSFER";
                    break;
                case 730:
                    description = "LIBRE";
                    break;
                case 735:
                    description = "FINAL";
                    break;
                case 740:
                    description = "RECIBIDO";
                    break;
                default:
                    description = "";
                    break;
            }

            return description;
        }

        private static string GetDescriptionCuenca(int status)
        {
            string description = "";

            switch (status)
            {
                case 700:
                    description = "LLEGA ORIGEN";
                    break;
                case 715:
                    description = "LLEGA DESTINO";
                    break;
                case 720:
                    description = "LLEGA DESTINO";
                    break;
                case 725:
                    description = "TRANSFER";
                    break;
                case 730:
                    description = "LIBRE";
                    break;
                case 735:
                    description = "FINAL";
                    break;
                case 740:
                    description = "RECIBIDO";
                    break;
                default:
                    description = "";
                    break;
            }

            return description;
        }

        public static int GetNextStatusByClientCode(string clientCode, int currentStatus)
        {
            //Cuando c# 8 deje de estar en BETA cambiar esto a Pattern Matching con lambda expressions por cada status
            //
            // return clientCode switch{ "VITA" => currentStatus+10 }
            //
            //
            //
            //

            //si el status es recibido automático en dispositivo devuelve 700
            if (currentStatus == 740 && clientCode != "CUEN") //CUENCA
                return 700;

            switch (clientCode)
            {
                //if clientcode == VITA status avoid status TRANSFER and LIBRE
                case "VITA":
                    switch (currentStatus)
                    {
                        case 700:
                            currentStatus += 10;
                            break;
                        case 720:
                            currentStatus += 10;
                            break;
                        default:
                            currentStatus += 5;
                            break;
                    }
                    break;
                case "VITMA":
                    switch (currentStatus)
                    {
                        case 700:
                            currentStatus += 10;
                            break;
                        case 720:
                            currentStatus += 10;
                            break;
                        default:
                            currentStatus += 5;
                            break;
                    }
                    break;
                case "TOMAS":
                    switch (currentStatus)
                    {
                        case 720:
                            currentStatus += 10;
                            break;
                        default:
                            currentStatus += 5;
                            break;
                    }
                    break;
                case "CUEN": //CUENCA
                    switch (currentStatus)
                    {
                        case 740:
                            currentStatus = 700;
                            break;
                        case 700:
                            currentStatus += 15;
                            break;
                        case 715:
                            currentStatus += 20;
                            break;
                        default:
                            currentStatus += 5;
                            break;
                    }
                    break;
                default:
                    currentStatus += 5;
                    break;
            }

            return currentStatus;
        }
    }
}