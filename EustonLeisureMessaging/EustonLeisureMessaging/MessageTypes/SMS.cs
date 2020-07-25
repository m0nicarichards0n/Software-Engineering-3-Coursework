using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging.MessageTypes
{
    public class SMS : MessageUtilities, IMessage
    {
        private string _id;
        private string _body;
        private string _sender;
        
        public SMS (string id, string body)
        {
            Id = id;

            // Get message and phone number from message body structured: "Example message" - 01234567
            Regex messageBodySectionsRegex = new Regex(@"\""([^\""]*?)\""|([0-9]{7,15})");
            string[] messageBodySections = messageBodySectionsRegex.Matches(body).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();

            // Check ony 2 items are identified
            if (messageBodySections.Count() == 2)
            {
                Body = messageBodySections[0];
                Sender = messageBodySections[1];
            }
            else
            {
                throw new Exception("Invalid message format - your text must contain a message and a phone number in the format \"{message}\" - {phone number}\"");
            }
        }
        public string Id
        {
            get { return _id; }
            set
            {
                // Check character encoding is valid
                if (IsAsciiEncoding(value))
                {
                    // Check ID is 10 characters long
                    if (value.Count() == 10)
                    {
                        // Check first character of ID is 'S'
                        if (char.ToUpper(value.First()) == 'S')
                        {
                            // Check last 9 characters of ID are numeric
                            string lastNineChars = value.Substring(1, 9);
                            if (lastNineChars.All(char.IsDigit))
                            {
                                _id = value;
                            }
                            else
                            {
                                throw new Exception("Message ID is invalid - last 9 character must be numeric.");
                            }
                        }
                        else
                        {
                            throw new Exception("Message ID is invalid - first character must refer to a valid message type.");
                        }
                    }
                    else
                    {
                        throw new Exception("Message ID is invalid - ID must be 10 characters long.");
                    }
                }
                else
                {
                    throw new Exception("Message ID is invalid - contains unsupported characters.");
                }

            }
        }
        public string Body
        {
            get { return _body; }
            set
            {
                // Check character encoding is valid
                if (IsAsciiEncoding(value))
                {
                    // Check SMS message body is at least 1 character long, but no longer than 140 characters
                    if (value.Count() >= 1 && value.Count() <= 140)
                    {
                        _body = value;
                    }
                    else
                    {
                        throw new Exception("Message body is invalid - SMS message must be between 1 and 140 characters long.");
                    }
                }
                else
                {
                    throw new Exception("Message body is invalid - contains unsupported characters.");
                }
            }
        }
        public string Sender
        {
            get { return _sender; }
            set
            {
                // Check character encoding is valid
                if (IsAsciiEncoding(value))
                {
                    // Check telephone number is made of entirely numeric values
                    if (value.All(char.IsDigit))
                    {
                        // Check telephone number is between 7 and 15 digits long
                        if (value.Count() >= 7 && value.Count() <= 15)
                        {
                            _sender = value;
                        }
                        else
                        {
                            throw new Exception("Telephone number is invalid - telephone number must be between 7 and 15 characters long.");
                        }
                    }
                    else
                    {
                        throw new Exception("Telephone number is invalid - you must enter a valid numeric telephone number.");
                    }
                }
                else
                {
                    throw new Exception("Telephone number is invalid - contains unsupported characters.");
                }
            }
        }

        public JObject GetMessageAsJObject()
        {
            JObject output = new JObject(
                            new JProperty("Id", Id),
                            new JProperty("Body", Body),
                            new JProperty("Sender", Sender)
                            );

            return output;
        }
    }
}
