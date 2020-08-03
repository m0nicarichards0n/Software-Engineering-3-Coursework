using System.Text.RegularExpressions;

namespace EustonLeisureMessaging.Services
{
    public class MentionMonitoringService : IMentionMonitoringService
    {
        private static MentionMonitoringService _instance;

        public static MentionMonitoringService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MentionMonitoringService();
            }
            return _instance;
        }

        public bool ContainsMention(string username, string message)
        {
            bool mentionsUsername = Regex.IsMatch(message, @"(?i)\@" + username + @"\b");
            return mentionsUsername;
        }
    }
}
