using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Services
{
    public class SignificantIncidentService : ISignificantIncidentService
    {
        private static SignificantIncidentService _instance;
        private static readonly List<string> _incidentTypes = new List<string>();

        public static SignificantIncidentService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SignificantIncidentService();
            }
            return _instance;
        }

        private SignificantIncidentService()
        {
            using (var reader = new StreamReader("SignificantIncidentTypes.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    _incidentTypes.Add(line);
                }
            }
        }

        public string GetNatureOfIncident(string id, string body)
        {
            Regex incidentRegex = new Regex(@"(?<=Nature\sof\sIncident:\s)([a-zA-Z ]+),", RegexOptions.IgnoreCase);
            if (incidentRegex.IsMatch(body) && _incidentTypes.Contains(incidentRegex.Match(body).ToString().TrimEnd(','), StringComparer.OrdinalIgnoreCase))
            {
                return incidentRegex.Match(body).ToString().TrimEnd(',');
            }
            else
            {
                throw new Exception("Error in message " + id + ": Significant Incident Report does not specify a valid incident type."
                                    + " Remember to separate message body with a commas e.g. \"Sport Centre Code: {code}, Nature of Incident: {type}, {message body}\"");
            }
        }

        public string GetSportCentreCode(string id, string body)
        {
            Regex sportCentreRegex = new Regex(@"(?<=\ASport\sCentre\sCode:\s)([0-9]{2}\-[0-9]{3}\-[0-9]{2})", RegexOptions.IgnoreCase);
            if (sportCentreRegex.IsMatch(body))
            {
                return sportCentreRegex.Match(body).ToString();
            }
            else
            {
                throw new Exception("Error in message " + id + ": Significant Incident Report does not specify a valid Sport Centre Code."
                                    + " Remember to separate message body with commas e.g. \"Sport Centre Code: {code}, Nature of Incident: {type}, {message body}\"");
            }
        }
    }
}
