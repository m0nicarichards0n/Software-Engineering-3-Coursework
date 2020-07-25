using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging.MessageTypes
{
    public class Email : MessageUtilities, IMessage
    {
        private string _id = "";
        private string _subject = "";
        private string _body = "";
        private string _sender = "";
        
        public Email (string id, string body)
        {
            Id = id;

            // Get message, subject and email address from message body structured: "Example subject", "Example message.", "Example email address"
            Regex messageBodySectionsRegex = new Regex(@"\""([^\""""]*?)\""");
            string[] messageBodySections = messageBodySectionsRegex.Matches(body).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();

            // Check ony 3 items are identified
            if (messageBodySections.Count() == 3)
            {
                Subject = messageBodySections[0];
                Body = messageBodySections[1];
                Sender = messageBodySections[2];
            }
            else
            {
                throw new Exception("Invalid message format - your email must contain a subject, message and email address in the format \"{subject}\", \"{message}\", \"{email address}\".");
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
                else
                {
                    throw new Exception("Message ID is invalid - contains unsupported characters.");
                }
            }
        }
        public string Subject
        {
            get { return _subject; }
            set
            {
                // Check character encoding is valid
                if (IsAsciiEncoding(value))
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
                else
                {
                    throw new Exception("Message subject is invalid - contains unsupported characters.");
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
                else
                {
                    throw new Exception("Email address is invalid - contains unsupported characters.");
                }
            }
        }

        public JObject GetMessageAsJObject()
        {
            JObject output = new JObject(
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
