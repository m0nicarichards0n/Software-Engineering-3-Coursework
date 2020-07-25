using Newtonsoft.Json.Linq;

namespace EustonLeisureMessaging
{
    public interface IMessage
    {
        string Id { get; set; }
        string Sender { get; set; }
        string Body { get; set; }
        JObject GetMessageAsJObject();
    }
}
