using System.Collections.Generic;

namespace EustonLeisureMessaging.Services
{
    public interface IHashtagMonitoringService
    {
        void CountHashtags(string message);
        IDictionary<string, int> GetTrendingList();
    }
}