using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging.MessageTypes
{
    public class Tweet : MessageUtilities, IMessage
    {
        private string _id;
        private string _body;
        private string _sender;
        
        public Tweet(string id, string body)
        {
            Id = id;

            // Get message and twitter ID from message body structured: "Example message" - @TwitterID
            Regex messageBodySectionsRegex = new Regex(@"\""([^\""]*?)\""|(\@[^\@]+$)");
            string[] messageBodySections = messageBodySectionsRegex.Matches(body).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();

            // Check ony 2 items are identified
            if (messageBodySections.Count() == 2)
            {
                Body = messageBodySections[0];
                Sender = messageBodySections[1];
            }
            else
            {
                throw new Exception("Invalid message format - your tweet must contain a message and a Twitter ID in the format \"{message}\" - @{Twitter ID}\"");
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
                        // Check first character of ID is 'T'
                        if (char.ToUpper(value.First()) == 'T')
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
                    throw new Exception("Message ID is invalid - ID contains unsupported characters.");
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
                    // Check Tweet body is at least 1 character long, but no longer than 140 characters
                    if (value.Count() >= 1 && value.Count() <= 140)
                    {
                        _body = value;
                    }
                    else
                    {
                        throw new Exception("Message body is invalid - Tweet must be between 1 and 140 characters long.");
                    }
                }
                else
                {
                    throw new Exception("Message body is invalid - Tweet contains unsupported characters.");
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
                    // Check Twitter username is at least 1 character long, but no longer than 16 characters (including '@' symbol)
                    if (value.Count() >= 2 && value.Count() <= 16)
                    {
                        // Check first character is '@' symbol
                        if (value.First() == '@')
                        {
                            // Check Twitter username does not contain spaces
                            string username = value.Substring(1, value.IndexOf(value.Last()));
                            if (!username.Contains(" "))
                            {
                                _sender = value;
                            }
                            else
                            {
                                throw new Exception("Sender information is invalid - Twitter username cannot contain spaces.");
                            }
                        }
                        else
                        {
                            throw new Exception("Sender information is invalid - Twitter username must begin with '@' symbol.");
                        }
                    }
                    else
                    {
                        throw new Exception("Sender information is invalid - Twitter username must be no longer than 15 characters.");
                    }
                }
                else
                {
                    throw new Exception("Sender information is invalid - Twitter username contains unsupported characters.");
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
