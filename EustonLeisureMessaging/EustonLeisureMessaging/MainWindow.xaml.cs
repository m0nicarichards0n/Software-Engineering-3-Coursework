using EustonLeisureMessaging.MessageTypes;
using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
        private List<JObject> MessagesAsJson;
        private IDictionary<string, string> QuarantinedUrls;
        private IDictionary<string, int> TrendingList;
        private IQuarantineUrlService _quarantinedUrlService = QuarantineUrlService.GetInstance();
        private ITrendingHashtagService _trendingHashtagService = TrendingHashtagService.GetInstance();

        public MainWindow()
        {
            QuarantinedUrls = _quarantinedUrlService.GetQuarantinedUrls();
            TrendingList = _trendingHashtagService.GetTrendingList();

            InitializeComponent();
            
            Emails = new List<Email>();
            Tweets = new List<Tweet>();
            SMSMessages = new List<SMS>();
            MessagesAsJson = new List<JObject>();
            

            lst_Emails.ItemsSource = Emails;
            lst_Tweets.ItemsSource = Tweets;
            lst_SMS.ItemsSource = SMSMessages;
            lstView_URLQuarantine.ItemsSource = QuarantinedUrls;
            lstView_TrendingList.ItemsSource = TrendingList;

            CollectionView trendingListView = (CollectionView)CollectionViewSource.GetDefaultView(lstView_TrendingList.ItemsSource);
            trendingListView.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Descending));
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
                    foreach (var message in MessagesAsJson)
                    {
                        if (message["Id"].ToString() == txt_MessageHeader.Text)
                        {
                            throw new Exception("This Message ID is already in use. Please enter a unique message ID.");
                        }
                    }

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
                lstView_URLQuarantine.Items.Refresh();
            }
            else if (messageType == typeof(Tweet))
            {
                Tweets.Add((Tweet)message);
                lst_Tweets.Items.Refresh();
                lstView_TrendingList.Items.Refresh();
                lstView_TrendingList.SelectedItems.Clear();
            }
            else if (messageType == typeof(SMS))
            {
                SMSMessages.Add((SMS)message);
                lst_SMS.Items.Refresh();
            }
        }

        private void lstView_URLQuarantine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstView_URLQuarantine.SelectedItems.Count > 0)
            {
                List<JObject> sourceMessages = new List<JObject>();
                lstView_URLSourceMessages.Items.Clear();
                lbl_URLSources.Visibility = Visibility.Visible;
                lstView_URLSourceMessages.Visibility = Visibility.Visible;

                KeyValuePair<string, string> selectedUrl = (KeyValuePair<string, string>)lstView_URLQuarantine.SelectedItems[0];
                string[] messageSourceIds = selectedUrl.Value.Split(',');

                for (int i = 0; i < messageSourceIds.Length; i++)
                {
                    Email sourceMessage = Emails.Find(x => x.Id == messageSourceIds[i]);
                    lstView_URLSourceMessages.Items.Add(sourceMessage);
                }
            }
            else
            {
                lbl_URLSources.Visibility = Visibility.Hidden;
                lstView_URLSourceMessages.Visibility = Visibility.Hidden;
            }
        }

        private void lstView_TrendingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstView_TrendingList.SelectedItems.Count > 0)
            {
                List<JObject> sourceMessages = new List<JObject>();
                lstView_TrendingSourceMessages.Items.Clear();

                KeyValuePair<string, int> selectedHashtag = (KeyValuePair<string, int>)lstView_TrendingList.SelectedItems[0];

                lbl_TrendingSourceMessages.Content = "Tweets Mentioning " + selectedHashtag.Key;
                lbl_TrendingSourceMessages.Visibility = Visibility.Visible;
                lstView_TrendingSourceMessages.Visibility = Visibility.Visible;
                
                List<Tweet> sourceTweets = Tweets.FindAll(x => Regex.IsMatch(x.Body, @"\" + selectedHashtag.Key.ToString() + @"\b"));
                foreach (Tweet tweet in sourceTweets)
                {
                    lstView_TrendingSourceMessages.Items.Add(tweet);
                }
            }
            else
            {
                lbl_TrendingSourceMessages.Visibility = Visibility.Hidden;
                lstView_TrendingSourceMessages.Visibility = Visibility.Hidden;
            }
        }
    }
}
