using EustonLeisureMessaging.MessageTypes;
using EustonLeisureMessaging.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;

namespace EustonLeisureMessaging
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Email> Emails;
        private List<SignificantIncidentReport> SignificantIncidentReports;
        private List<Tweet> Tweets;
        private List<SMS> SMSMessages;
        private List<JObject> MessagesAsJson;
        private IDictionary<string, string> QuarantinedUrls;
        private IDictionary<string, int> TrendingList;
        private List<Tweet> MentionsList;
        private IQuarantineUrlService _quarantinedUrlService = QuarantineUrlService.GetInstance();
        private IHashtagMonitoringService _hashtagMonitoringService = HashtagMonitoringService.GetInstance();
        private IMentionMonitoringService _mentionMonitoringService = MentionMonitoringService.GetInstance();
        private static readonly string _username = "EustonLeisure";

        public MainWindow()
        {
            QuarantinedUrls = _quarantinedUrlService.GetQuarantinedUrls();
            TrendingList = _hashtagMonitoringService.GetTrendingList();
            
            InitializeComponent();
            
            Emails = new List<Email>();
            SignificantIncidentReports = new List<SignificantIncidentReport>();
            Tweets = new List<Tweet>();
            SMSMessages = new List<SMS>();
            MessagesAsJson = new List<JObject>();
            MentionsList = new List<Tweet>();

            lst_Emails.ItemsSource = Emails;
            lst_Tweets.ItemsSource = Tweets;
            lst_SMS.ItemsSource = SMSMessages;
            lstView_URLQuarantine.ItemsSource = QuarantinedUrls;
            lstView_TrendingList.ItemsSource = TrendingList;
            lstView_MentionsList.ItemsSource = MentionsList;
            lstView_SIRList.ItemsSource = SignificantIncidentReports;

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
                    if (MessagesAsJson.Exists(x => x["Id"].ToString() == txt_MessageHeader.Text))
                    {
                        throw new Exception("This Message ID is already in use. Please enter a unique message ID.");
                    }

                    string messageHeader = txt_MessageHeader.Text;
                    string messageBody = txt_MessageBody.Text;

                    var newMessage = MessageFactory.CreateMessage(messageHeader, messageBody, ValidationService.GetInstance());
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
            else if (messageType == typeof(SignificantIncidentReport))
            {
                SignificantIncidentReports.Add((SignificantIncidentReport)message);
                lstView_SIRList.Items.Refresh();
                DisplaySignificantIncidents();
            }
            else if (messageType == typeof(Tweet))
            {
                Tweets.Add((Tweet)message);
                lst_Tweets.Items.Refresh();

                lstView_TrendingList.Items.Refresh();
                lstView_TrendingList.SelectedItems.Clear();
                lstView_MentionsList.Items.Refresh();

                if (_mentionMonitoringService.ContainsMention(_username, message.Body))
                {
                    MentionsList.Add((Tweet)message);
                }
                lstView_MentionsList.Items.Refresh();
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
                    Email sourceEmail = Emails.Find(x => x.Id == messageSourceIds[i]);
                    if (sourceEmail == null)
                    {
                        SignificantIncidentReport sourceSIR = SignificantIncidentReports.Find(x => x.Id == messageSourceIds[i]);
                        lstView_URLSourceMessages.Items.Add(sourceSIR);
                    }
                    else
                    {
                        lstView_URLSourceMessages.Items.Add(sourceEmail);
                    }
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
                
                List<Tweet> sourceTweets = Tweets.FindAll(x => Regex.IsMatch(x.Body, @"\" + selectedHashtag.Key.ToString() + @"\b", RegexOptions.IgnoreCase));
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
        
        public void DisplaySignificantIncidents()
        {
            if (SignificantIncidentReports.Count > 0)
            {
                lst_Emails.Height = 250;
                lst_SMS.Height = 250;
                lst_Tweets.Height = 250;

                stackPanel_SIR.Visibility = Visibility.Visible;
            }
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            txtBlock_ImportExportError.Text = "";
            scrollView_ImportExportError.Visibility = Visibility.Hidden;

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {  
                Title = "Select A Text File",
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string[] messages = File.ReadAllLines(openFileDialog.FileName);
                txtBlock_ImportExportError.Text = "Some messages failed to import:\n";
                foreach (string message in messages)
                {
                    try
                    {
                        string header = message.Substring(0, 10);
                        string body = message.Substring(message.IndexOf(',') + 1, message.Length - 11);
                        var newMessage = MessageFactory.CreateMessage(header, body, ValidationService.GetInstance());
                        if (!MessagesAsJson.Exists(x => x["Id"].ToString() == newMessage.Id))
                        {
                            CategoriseMessage(newMessage);
                            MessagesAsJson.Add(newMessage.GetMessageAsJObject());
                        }
                        else
                        {
                            txtBlock_ImportExportError.Text += "\n" + newMessage.Id + " - this ID is already in use.";
                            scrollView_ImportExportError.Visibility = Visibility.Visible;
                        }
                    }
                    catch (Exception exception)
                    {
                        txtBlock_ImportExportError.Text += "\n---\n" + exception.Message + "\n---";
                        scrollView_ImportExportError.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            scrollView_ImportExportError.Visibility = Visibility.Hidden;
            if (MessagesAsJson.Count > 0)
            {
                string selectedPath = "";
                
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select output folder...";
                if (folderDialog.ShowDialog().ToString().Equals("OK"))
                {
                    selectedPath = folderDialog.SelectedPath;
                }

                using (StreamWriter file = File.CreateText(selectedPath + "\\ELM-Message-Export.json"))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.Indented;
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, MessagesAsJson);
                    }
                }

                Process.Start(selectedPath + "\\ELM-Message-Export.json");
            }
            else
            {
                txtBlock_ImportExportError.Text = "No messages to export.";
                scrollView_ImportExportError.Visibility = Visibility.Visible;
            }
        }
    }
}
