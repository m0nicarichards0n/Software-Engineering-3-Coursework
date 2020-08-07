using System;

namespace EustonLeisureMessaging.Services
{
    public interface ISignificantIncidentService
    {
        string GetNatureOfIncident(string id, string body);
        string GetSportCentreCode(string id, string body);
    }
}