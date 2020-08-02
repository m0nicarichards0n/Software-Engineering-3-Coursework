using System.Collections.Generic;

namespace EustonLeisureMessaging.Services
{
    public interface ITrendingHashtagService
    {
        void CountHashtags(string message);
        IDictionary<string, int> GetTrendingList();
    }
}