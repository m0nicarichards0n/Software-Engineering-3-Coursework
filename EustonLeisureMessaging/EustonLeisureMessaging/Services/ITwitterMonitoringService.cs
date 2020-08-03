using System.Collections.Generic;

namespace EustonLeisureMessaging.Services
{
    public interface ITwitterMonitoringService
    {
        void CountHashtags(string message);
        IDictionary<string, int> GetTrendingList();
        bool ContainsMention(string username, string message);
    }
}