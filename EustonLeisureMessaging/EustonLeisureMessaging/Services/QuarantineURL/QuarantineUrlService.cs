using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging.Services
{
    public class QuarantineUrlService : IQuarantineUrlService
    {
        private static QuarantineUrlService _instance;
        private Regex _urlRegex = new Regex(@"(https\:\/{2}|http\:\/{2}|www\.)(([a-zA-Z0-9-_~]+\.)+)([a-zA-Z0-9-_~]+)((\/[a-zA-Z0-9\.\-_~;\/?:@&=+$,]+)?)");
        private IDictionary<string,string> _quaratinedUrls;

        public static QuarantineUrlService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new QuarantineUrlService();
            }
            return _instance;
        }

        private QuarantineUrlService()
        {
            _quaratinedUrls = new Dictionary<string,string>();
        }

        private bool ContainsUrls(string message)
        {
            bool containsURLs = _urlRegex.IsMatch(message);
            return containsURLs;
        }

        public string QuarantineURLs(string id, string message)
        {
            if (ContainsUrls(message))
            {
                MatchCollection urls = _urlRegex.Matches(message);
                
                foreach(Match m in urls)
                {
                    if (!_quaratinedUrls.Keys.Contains(m.ToString()))
                    {
                        _quaratinedUrls.Add(m.ToString(), id);
                    }
                    else
                    {
                        string[] existingSourceIDs = _quaratinedUrls[m.ToString()].Split(',');
                        for (int i = 0; i < existingSourceIDs.Length; i++)
                        {
                            if(existingSourceIDs[i] == id)
                            {
                                message = _urlRegex.Replace(message, "<URL Quarantined>");
                                return message;
                            }
                        }
                        _quaratinedUrls[m.ToString()] += "," + id;
                    }
                }

                message = _urlRegex.Replace(message, "<URL Quarantined>");
            }
            return message;
        }

        public IDictionary<string, string> GetQuarantinedUrls()
        {
            return _quaratinedUrls;
        }
    }
}
