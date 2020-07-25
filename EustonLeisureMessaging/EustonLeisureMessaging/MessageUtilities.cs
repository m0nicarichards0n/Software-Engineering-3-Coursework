using System.Text;

namespace EustonLeisureMessaging
{
    public abstract class MessageUtilities
    {
        protected bool IsAsciiEncoding(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }
    }
}
