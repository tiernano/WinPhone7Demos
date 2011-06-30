using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using System.Xml.Linq;
using System.Diagnostics;
namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            
            HttpNotificationChannel pushChannel;
            string channelName = "TiernanoTestChannel";
            InitializeComponent();
            pushChannel = HttpNotificationChannel.Find(channelName);
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(pushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(pushChannel_ErrorOccurred);
                pushChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(pushChannel_HttpNotificationReceived);
                pushChannel.Open();
            }
            else
            {
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(pushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(pushChannel_ErrorOccurred);
                pushChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(pushChannel_HttpNotificationReceived);
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
            }
        }

        void pushChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            string message;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(e.Notification.Body))
            {
                message = reader.ReadToEnd();
            }
            
            XElement m;
            try
            {
                m = XElement.Parse(message);
                var result = (from x in m.DescendantsAndSelf("message")
                                select new
                                {
                                    machineName = x.Element("machineName").Value,
                                    dateTime = x.Element("dateTime").Value,
                                    text = x.Element("text").Value
                                }).SingleOrDefault();
                if (result != null)
                {
                    Dispatcher.BeginInvoke(() =>
                       listBox1.Items.Insert(0, string.Format("{0} - {1}", result.machineName, result.text))

                   );
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
           
        }

        void pushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                System.Diagnostics.Debug.WriteLine(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }

        void pushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
           {
               // Display the new URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
               System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
           });
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}