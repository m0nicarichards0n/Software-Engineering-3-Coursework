namespace EustonLeisureMessaging.Services
{
    public interface IValidationService
    {
        string[] GetMessageSections(string messageBody);
        bool HasCorrectNumberOfDoubleQuotes(string messageBody, int num);
        bool IsValidEncoding(string text);
        bool IsSignificantIncidentSubject(string messageBody);
    }
}