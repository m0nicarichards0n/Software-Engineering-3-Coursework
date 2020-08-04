using EustonLeisureMessaging.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.MessageTypes
{
    public class SignificantIncidentReport : Email
    {
        private DateTime _date;
        private string _subject;
        private string _sportCentreCode;
        private string _natureOfIncident;
        private static readonly List<string> _incidentTypes = new List<string>();

        public SignificantIncidentReport (string id, DateTime date, string subject, string body, string sender, IQuarantineUrlService quarantineUrlService) : base (id, subject, body, sender, quarantineUrlService)
        {
            using (var reader = new StreamReader("SignificantIncidentTypes.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    _incidentTypes.Add(line);
                }
            }
            
            Id = id;
            Subject = subject;
            Body = quarantineUrlService.QuarantineURLs(Id, body);
            Sender = sender;
            _sportCentreCode = GetSportCentreCode(Body);
            _natureOfIncident = GetNatureOfIncident(Body);
            _date = date;
        }

        public override string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        private string GetSportCentreCode(string body)
        {
            Regex sportCentreRegex = new Regex(@"(?<=\ASport\sCentre\sCode:\s)([0-9]{2}\-[0-9]{3}\-[0-9]{2})");
            if (sportCentreRegex.IsMatch(body))
            {
                return sportCentreRegex.Match(body).ToString();
            }
            else
            {
                throw new Exception("Error in message " + Id + ": Significant Incident Report does not specify a valid Sport Centre Code.");
            }
        }
        
        private string GetNatureOfIncident(string body)
        {
            Regex incidentRegex = new Regex(@"(?<=Nature\sof\sIncident:\s)([a-zA-Z ]+),");
            if (incidentRegex.IsMatch(body) && _incidentTypes.Contains(incidentRegex.Match(body).ToString().TrimEnd(',')))
            {
                return incidentRegex.Match(body).ToString().TrimEnd(',');
            }
            else
            {
                throw new Exception("Error in message " + Id + ": Significant Incident Report does not specify a valid incident type.");
            }
        }
    }
}
