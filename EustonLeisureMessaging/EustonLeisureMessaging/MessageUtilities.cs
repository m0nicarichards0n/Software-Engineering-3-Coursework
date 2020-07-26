using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging
{
    public abstract class MessageUtilities
    {
        protected bool IsAsciiEncoding(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        protected string[] GetMessageSections(string message)
        {
            Regex messageSectionsRegex = new Regex(@"\""(.*?)\""");
            string[] messageSections = messageSectionsRegex.Matches(message).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();

            return messageSections;
        }
    }
}
