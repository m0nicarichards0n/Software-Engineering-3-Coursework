using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace EustonLeisureMessaging.MessageTypes
{
    public class Tweet : MessageUtilities, IMessage
    {
        private string _id;
        private string _body;
        private string _sender;
        
        public Tweet(string id, string body, ITextSpeakService textSpeakService)
        {
            // Check ID character encoding is valid
            if (IsAsciiEncoding(id))
            {
                Id = id;
                // Check message body character encoding is valid
                if (IsAsciiEncoding(body))
                {
                    // Check there are exactly 4 double quotes in the message body
                    if (body.Count(x => x == '"') == 4)
                    {
                        // Get each section in double quotes from message body
                        string[] messageSections = GetMessageSections(body);

                        // Check ony 2 sections are identified
                        if (messageSections.Count() == 2)
                        {
                            Body = textSpeakService.ExpandTextSpeakAbbreviations(messageSections[0]);
                            Sender = messageSections[1];
                        }
                        else
                        {
                            throw new Exception("Invalid message format - your tweet must contain a message and a Twitter ID in the format \"{message}\" \"@{Twitter ID}\"");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid message format - your tweet must contain a message and a Twitter ID in the format \"{message}\" \"@{Twitter ID}\"");
                    }
                }
                else
                {
                    throw new Exception("Message Body contains unsupported characters.");
                }
            }
            else
            {
                throw new Exception("Message ID contains unsupported characters.");
            }
        }
        public string Id
        {
            get { return _id; }
            set
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
        }
        public string Body
        {
            get { return _body; }
            set
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
        }
        public string Sender
        {
            get { return _sender; }
            set
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
        }

        public JObject GetMessageAsJObject()
        {
            JObject output = new JObject(
                            new JProperty("Type", "Tweet"),
                            new JProperty("Id", Id),
                            new JProperty("Body", Body),
                            new JProperty("Sender", Sender)
                            );

            return output;
        }
    }
}
