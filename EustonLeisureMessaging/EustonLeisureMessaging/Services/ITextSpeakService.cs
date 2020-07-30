namespace EustonLeisureMessaging.Services
{
    public interface ITextSpeakService
    {
        string ExpandTextSpeakAbbreviations(string message);
    }
}