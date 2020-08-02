using System.Collections.Generic;

namespace EustonLeisureMessaging.Services
{
    public interface IQuarantineUrlService
    {
        List<Url> GetQuarantinedUrls();
        string QuarantineURLs(string id, string message);
    }
}