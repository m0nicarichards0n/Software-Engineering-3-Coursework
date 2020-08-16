using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;

namespace EustonLeisureMessaging.MessageTypes
{
    public class SignificantIncidentReport : Email
    {
        private string _date;
        private string _subject;
        private string _sportCentreCode;
        private string _natureOfIncident;

        public SignificantIncidentReport (string id, string subject, string body, string sender, 
                                            IQuarantineUrlService quarantineUrlService, 
                                            ISignificantIncidentService significantIncidentService) : base (id, subject, body, sender, quarantineUrlService)
        {
            Id = id;
            Date = GetDate(subject);
            Subject = subject;
            Body = significantIncidentService.GetMessageBody(quarantineUrlService.QuarantineUrls(Id, body));
            Sender = sender;
            SportCentreCode = significantIncidentService.GetSportCentreCode(Id, body);
            NatureOfIncident = significantIncidentService.GetNatureOfIncident(Id, body);
        }

        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public override string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string SportCentreCode
        {
            get { return _sportCentreCode; }
            set { _sportCentreCode = value; }
        }
        public string NatureOfIncident
        {
            get { return _natureOfIncident; }
            set { _natureOfIncident = value; }
        }

        private string GetDate(string subject)
        {
            string date = subject.Substring(4, 8);
            return date;
        }

        public override JObject GetMessageAsJObject()
        {
            JObject output = new JObject(
                            new JProperty("Type", "SignificantIncidentReport"),
                            new JProperty("Id", Id),
                            new JProperty("Date", Date),
                            new JProperty("Subject", Subject),
                            new JProperty("SportCentreCode", SportCentreCode),
                            new JProperty("NatureOfIncident", NatureOfIncident),
                            new JProperty("Body", Body),
                            new JProperty("Sender", Sender));

            return output;
        }
    }
}
