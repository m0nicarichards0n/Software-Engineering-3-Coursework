using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace EustonLeisureMessaging.MessageTypes
{
    public class SMS : MessageUtilities, IMessage
    {
        private string _id;
        private string _body;
        private string _sender;
        
        public SMS (string id, string body, ITextSpeakService textSpeakService)
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
                            throw new Exception("Invalid message format - your text must contain a message and a phone number in the format \"{message}\" \"{phone number}\"");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid message format - your text must contain a message and a phone number in the format \"{message}\" \"{phone number}\"");
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
        }
        public string Body
        {
            get { return _body; }
            set
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
        }
        public string Sender
        {
            get { return _sender; }
            set
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
        }

        public JObject GetMessageAsJObject()
        {
            JObject output = new JObject(
                            new JProperty("Type", "SMS"),
                            new JProperty("Id", Id),
                            new JProperty("Body", Body),
                            new JProperty("Sender", Sender)
                            );

            return output;
        }
    }
}
