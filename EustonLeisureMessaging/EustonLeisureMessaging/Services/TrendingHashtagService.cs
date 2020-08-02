using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Services
{
    public class TrendingHashtagService : ITrendingHashtagService
    {
        private static TrendingHashtagService _instance;
        private Regex _hashtagRegex = new Regex(@"(\#[a-zA-Z0-9]+)");
        private IDictionary<string, int> _trendingList;

        public static TrendingHashtagService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TrendingHashtagService();
            }
            return _instance;
        }

        private TrendingHashtagService()
        {
            _trendingList = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        private bool ContainsHashtags(string message)
        {
            bool containsHashtags = _hashtagRegex.IsMatch(message);
            return containsHashtags;
        }

        public void CountHashtags(string message)
        {
            if (ContainsHashtags(message))
            {
                MatchCollection hashtags = _hashtagRegex.Matches(message);
                foreach (Match m in hashtags)
                {
                    if (!_trendingList.ContainsKey(m.ToString()))
                    {
                        _trendingList.Add(m.ToString(), 1);
                    }
                    else
                    {
                        _trendingList[m.ToString()]++;
                    }
                }
            }
        }

        public IDictionary<string, int> GetTrendingList()
        {
            return _trendingList;
        }
    }
}
