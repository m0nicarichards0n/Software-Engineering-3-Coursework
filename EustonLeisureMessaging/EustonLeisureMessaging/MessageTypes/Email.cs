using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Mail;

namespace EustonLeisureMessaging.MessageTypes
{
    public class Email : MessageUtilities, IMessage
    {
        private string _id;
        private string _subject;
        private string _body;
        private string _sender;

        public Email (string id, string body, IQuarantineUrlService quarantineUrlService)
        {
            // Check ID character encoding is valid
            if (IsAsciiEncoding(id))
            {
                Id = id;
                // Check message body character encoding is valid
                if (IsAsciiEncoding(body))
                {
                    // Check there are exactly 6 double quotes in the message body
                    if (body.Count(x => x == '"') == 6)
                    {
                        // Get each section in double quotes from message body
                        string[] messageSections = GetMessageSections(body);

                        // Check ony 3 sections are identified
                        if (messageSections.Count() == 3)
                        {
                            Subject = messageSections[0];
                            Body = quarantineUrlService.QuarantineURLs(Id, messageSections[1]);
                            Sender = messageSections[2];
                        }
                        else
                        {
                            throw new Exception("Invalid message format - your email must contain a subject, message and email address in the format \"{subject}\" \"{message}\" \"{email address}\"");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid message format - your email must contain a subject, message and email address in the format \"{subject}\" \"{message}\" \"{email address}\"");
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
                    // Check first character of ID is 'E'
                    if (char.ToUpper(value.First()) == 'E')
                    {
                        // Check last 9 characters of ID are numeric
                        string lastNineChars = value.Substring(1, 9);
                        if (lastNineChars.All(char.IsDigit))
                        {
                            _id = value;
                        }
                        else
                        {
                            throw new Exception("Message was assigned an invalid ID - last 9 character must be numeric.");
                        }
                    }
                    else
                    {
                        throw new Exception("Message was assigned an invalid ID - first character must refer to a valid message type.");
                    }
                }
                else
                {
                    throw new Exception("Message was assigned an invalid ID - ID must be 10 characters long.");
                }
            }
        }
        public string Subject
        {
            get { return _subject; }
            set
            {
                // Check Email subject is at least 1 character long, but no longer than 20 characters
                if (value.Count() >= 1 && value.Count() <= 20)
                {
                    _subject = value;
                }
                else
                {
                    throw new Exception("Message subject is invalid - Email subject must be between 1 and 20 characters long.");
                }
            }
        }
        public string Body
        {
            get { return _body; }
            set
            {
                // Check Email message body is at least 1 character long, but no longer than 1028 characters
                if (value.Count() >= 1 && value.Count() <= 1028)
                {
                    _body = value;
                }
                else
                {
                    throw new Exception("Message body is invalid - Email message must be between 1 and 1028 characters long.");
                }
            }
        }
        public string Sender
        {
            get { return _sender; }
            set
            {
                // Check email address is in valid format
                if (IsValidEmailAddress(value))
                {
                    _sender = value;
                }
                else
                {
                    throw new Exception("Email address is invalid - please enter a valid email address.");
                }
            }
        }

        public JObject GetMessageAsJObject()
        {
            JObject output = new JObject(
                            new JProperty("Type", "Email"),
                            new JProperty("Id", Id),
                            new JProperty("Subject", Subject),
                            new JProperty("Body", Body),
                            new JProperty("Sender", Sender)
                            );

            return output;
        }

        private bool IsValidEmailAddress(string emailAddress)
        {
            try
            {
                var addr = new MailAddress(emailAddress);
                return addr.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }
    }
}
