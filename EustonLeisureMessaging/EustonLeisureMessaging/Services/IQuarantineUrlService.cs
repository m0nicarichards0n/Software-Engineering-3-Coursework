using System.Collections.Generic;

namespace EustonLeisureMessaging.Services
{
    public interface IQuarantineUrlService
    {
        IDictionary<string, string> GetQuarantinedUrls();
        string QuarantineURLs(string id, string message);
    }
}