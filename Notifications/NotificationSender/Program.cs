using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace NotificationSender
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 2)
            {
                string subscriptionUri = args[0];
                string message = args[1];
                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);
                string rawMessage = String.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><message><text>{0}</text><machineName>{1}</machineName><dateTime>{2}</dateTime></message>", message, Environment.MachineName, DateTime.Now);
                sendNotificationRequest.Method = "POST";
                byte[] notificationMessage = Encoding.Default.GetBytes(rawMessage);

                // Set the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-NotificationClass", "3");
                

                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                // Send the notification and get the response.
                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                Console.WriteLine(String.Format("{0} | {1} | {2}", notificationStatus, deviceConnectionStatus, notificationChannelStatus));
            }
            else
            {
                Console.WriteLine("Need URL for Device");
            }
        }
    }
}
