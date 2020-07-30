using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Services
{
    public class TextSpeakService : ITextSpeakService
    {
        private static TextSpeakService _instance;
        private static readonly IDictionary<string, string> _abbreviations = new Dictionary<string, string>();

        public static TextSpeakService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TextSpeakService();
            }
            return _instance;
        }
        
        private TextSpeakService()
        {
            using (var reader = new StreamReader("textwords.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    _abbreviations.Add(values[0], values[1]);
                }
            }
        }
        private bool ContainsTextSpeakAbbreviations(string message)
        {
            bool containsAbbreviations = _abbreviations.Keys.Any(x => message.Contains(x));
            return containsAbbreviations;
        }
        public string ExpandTextSpeakAbbreviations(string message)
        {
            if (ContainsTextSpeakAbbreviations(message))
            {
                string[] abbreviationsFound = _abbreviations.Keys.Where(x => message.Contains(x)).ToArray();

                for (int i = 0; i < abbreviationsFound.Length; i++)
                {
                    string expandedAbbreviation = abbreviationsFound[i] + " <" + _abbreviations[abbreviationsFound[i]] + ">";
                    message = message.Replace(abbreviationsFound[i], expandedAbbreviation);
                }

                return message;
            }
            else
            {
                return message;
            }
        }
    }
}
