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
        private List<Email> _emails;
        private List<SignificantIncidentReport> _significantIncidentReports;
        private List<Tweet> _tweets;
        private List<SMS> _smsMessages;
        private List<JObject> _messagesAsJson;
        private IDictionary<string, string> _quarantinedUrls;
        private IDictionary<string, int> _trendingList;
        private List<Tweet> _mentionsList;
        private IQuarantineUrlService _quarantinedUrlService = QuarantineUrlService.GetInstance();
        private IHashtagMonitoringService _hashtagMonitoringService = HashtagMonitoringService.GetInstance();
        private IMentionMonitoringService _mentionMonitoringService = MentionMonitoringService.GetInstance();
        private static readonly string _username = "EustonLeisure";

        public MainWindow()
        {
            _quarantinedUrls = _quarantinedUrlService.GetQuarantinedUrls();
            _trendingList = _hashtagMonitoringService.GetTrendingList();
            
            InitializeComponent();
            
            _emails = new List<Email>();
            _significantIncidentReports = new List<SignificantIncidentReport>();
            _tweets = new List<Tweet>();
            _smsMessages = new List<SMS>();
            _messagesAsJson = new List<JObject>();
            _mentionsList = new List<Tweet>();

            lst_Emails.ItemsSource = _emails;
            lst_Tweets.ItemsSource = _tweets;
            lst_SMS.ItemsSource = _smsMessages;
            lstView_URLQuarantine.ItemsSource = _quarantinedUrls;
            lstView_TrendingList.ItemsSource = _trendingList;
            lstView_MentionsList.ItemsSource = _mentionsList;
            lstView_SIRList.ItemsSource = _significantIncidentReports;

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
                    if (_messagesAsJson.Exists(x => x["Id"].ToString() == txt_MessageHeader.Text))
                    {
                        throw new Exception("This Message ID is already in use. Please enter a unique message ID.");
                    }

                    string messageHeader = txt_MessageHeader.Text;
                    string messageBody = txt_MessageBody.Text;

                    var newMessage = MessageFactory.CreateMessage(messageHeader, messageBody, ValidationService.GetInstance());
                    _messagesAsJson.Add(newMessage.GetMessageAsJObject());

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
                _emails.Add((Email)message);
                lst_Emails.Items.Refresh();
                lstView_URLQuarantine.Items.Refresh();
            }
            else if (messageType == typeof(SignificantIncidentReport))
            {
                _significantIncidentReports.Add((SignificantIncidentReport)message);
                lstView_SIRList.Items.Refresh();
                DisplaySignificantIncidents();
            }
            else if (messageType == typeof(Tweet))
            {
                _tweets.Add((Tweet)message);
                lst_Tweets.Items.Refresh();

                lstView_TrendingList.Items.Refresh();
                lstView_TrendingList.SelectedItems.Clear();
                lstView_MentionsList.Items.Refresh();

                if (_mentionMonitoringService.ContainsMention(_username, message.Body))
                {
                    _mentionsList.Add((Tweet)message);
                }
                lstView_MentionsList.Items.Refresh();
            }
            else if (messageType == typeof(SMS))
            {
                _smsMessages.Add((SMS)message);
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
                    Email sourceEmail = _emails.Find(x => x.Id == messageSourceIds[i]);
                    if (sourceEmail == null)
                    {
                        SignificantIncidentReport sourceSIR = _significantIncidentReports.Find(x => x.Id == messageSourceIds[i]);
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
                
                List<Tweet> sourceTweets = _tweets.FindAll(x => Regex.IsMatch(x.Body, @"\" + selectedHashtag.Key.ToString() + @"\b", RegexOptions.IgnoreCase));
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
            if (_significantIncidentReports.Count > 0)
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
                        if (!_messagesAsJson.Exists(x => x["Id"].ToString() == header))
                        {
                            var newMessage = MessageFactory.CreateMessage(header, body, ValidationService.GetInstance());
                            CategoriseMessage(newMessage);
                            _messagesAsJson.Add(newMessage.GetMessageAsJObject());
                        }
                        else
                        {
                            txtBlock_ImportExportError.Text += "\n" + header + " - this ID is already in use.";
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
            if (_messagesAsJson.Count > 0)
            {
                string selectedPath = "";
                
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select output folder...";
                if (folderDialog.ShowDialog().ToString().Equals("OK"))
                {
                    selectedPath = folderDialog.SelectedPath;
                }

                if (selectedPath != "")
                {
                    using (StreamWriter file = File.CreateText(selectedPath + "\\ELM-Message-Export.json"))
                    {
                        using (JsonTextWriter writer = new JsonTextWriter(file))
                        {
                            writer.Formatting = Formatting.Indented;
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(writer, _messagesAsJson);
                        }
                    }

                    Process.Start(selectedPath + "\\ELM-Message-Export.json");
                }
            }
            else
            {
                txtBlock_ImportExportError.Text = "No messages to export.";
                scrollView_ImportExportError.Visibility = Visibility.Visible;
            }
        }
    }
}
