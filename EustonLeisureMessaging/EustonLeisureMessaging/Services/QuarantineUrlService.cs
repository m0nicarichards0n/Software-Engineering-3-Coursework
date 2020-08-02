using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging.Services
{
    public class QuarantineUrlService : IQuarantineUrlService
    {
        private static QuarantineUrlService _instance;
        private Regex urlRegex = new Regex(@"(https\:\/{2}|http\:\/{2}|www\.)(([a-zA-Z0-9-_~]+\.)+)([a-zA-Z0-9-_~]+)((\/[a-zA-Z0-9\.\-_~;\/?:@&=+$,]+)?)");
        private List<Url> _quaratinedUrls;

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
            _quaratinedUrls = new List<Url>();
        }

        private bool ContainsUrls(string message)
        {
            bool containsURLs = urlRegex.IsMatch(message);
            return containsURLs;
        }

        public string QuarantineURLs(string id, string message)
        {
            string processedMessage = message;
            if (ContainsUrls(message))
            {
                MatchCollection urls = urlRegex.Matches(message);
                
                foreach(Match m in urls)
                {
                    Url newUrl = new Url();
                    newUrl.SourceMessageId = id;
                    newUrl.Path = m.ToString();
                    _quaratinedUrls.Add(newUrl);
                    processedMessage = processedMessage.Replace(m.ToString(), "<URL Quarantined>");
                }
            }
            return processedMessage;
        }

        public List<Url> GetQuarantinedUrls()
        {
            return _quaratinedUrls;
        }
    }
}
