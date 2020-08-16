using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Services
{
    public class ValidationService : IValidationService
    {
        private static ValidationService _instance;
        private Regex _messageSectionsRegex;
        
        public static ValidationService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ValidationService();
            }
            return _instance;
        }

        private ValidationService()
        {
            _messageSectionsRegex = new Regex(@"\""(.*?)\""");
        }

        public bool IsValidEncoding(string text)
        {
            bool isValidEncoding = Encoding.UTF8.GetByteCount(text) == text.Length;
            return isValidEncoding;
        }

        public bool HasCorrectNumberOfDoubleQuotes(string messageBody, int num)
        {
            bool hasCorrectNumber = messageBody.Count(x => x == '"') == num;
            return hasCorrectNumber;
        }

        public string[] GetMessageSections(string messageBody)
        {
            string[] messageSections = _messageSectionsRegex.Matches(messageBody).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();
            return messageSections;
        }

        public bool IsSignificantIncidentSubject(string messageBody)
        {
            string[] messageSections = GetMessageSections(messageBody);
            if (messageSections[0].Length == 12
                && messageSections[0].Substring(0, 3) == "SIR"
                && DateTime.TryParseExact(messageSections[0].Substring(4, 8), "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
