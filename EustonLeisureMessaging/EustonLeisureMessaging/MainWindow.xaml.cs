using EustonLeisureMessaging.MessageTypes;
using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EustonLeisureMessaging
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Email> Emails;
        private List<Tweet> Tweets;
        private List<SMS> SMSMessages;
        public List<JObject> MessagesAsJson;
        private List<Url> Urls;
        private IQuarantineUrlService _quarantinedUrlService = QuarantineUrlService.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
            
            Emails = new List<Email>();
            Tweets = new List<Tweet>();
            SMSMessages = new List<SMS>();
            MessagesAsJson = new List<JObject>();
            Urls = _quarantinedUrlService.GetQuarantinedUrls();

            lst_Emails.ItemsSource = Emails;
            lst_Tweets.ItemsSource = Tweets;
            lst_SMS.ItemsSource = SMSMessages;
            lst_URLQuarantine.ItemsSource = Urls;
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            txtBlock_ErrorMessage.Visibility = Visibility.Hidden;
            txtBlock_ErrorMessage.Padding = new Thickness(0);
            txtBlock_ErrorMessage.Text = "";
            try
            {
                if (txt_MessageHeader.Text.Length > 0 && txt_MessageBody.Text.Length > 0)
                {
                    string messageHeader = txt_MessageHeader.Text;
                    string messageBody = txt_MessageBody.Text;

                    var newMessage = MessageFactory.CreateMessage(messageHeader, messageBody);
                    MessagesAsJson.Add(newMessage.GetMessageAsJObject());

                    CategoriseMessage(newMessage);
                }
                else
                {
                    throw new Exception("Please complete both header and body fields.");
                }
            }
            catch (Exception exception)
            {
                txtBlock_ErrorMessage.Visibility = Visibility.Visible;
                txtBlock_ErrorMessage.Padding = new Thickness(10);
                txtBlock_ErrorMessage.Text = exception.Message;
            }
        }

        private void CategoriseMessage(IMessage message)
        {
            Type messageType = message.GetType();
            if (messageType == typeof(Email))
            {
                Emails.Add((Email)message);
                lst_Emails.Items.Refresh();
                lst_URLQuarantine.Items.Refresh();
            }
            else if (messageType == typeof(Tweet))
            {
                Tweets.Add((Tweet)message);
                lst_Tweets.Items.Refresh();
            }
            else if (messageType == typeof(SMS))
            {
                SMSMessages.Add((SMS)message);
                lst_SMS.Items.Refresh();
            }
        }
    }
}
