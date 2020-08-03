namespace EustonLeisureMessaging.Services
{
    public interface IMentionMonitoringService
    {
        bool ContainsMention(string username, string message);
    }
}