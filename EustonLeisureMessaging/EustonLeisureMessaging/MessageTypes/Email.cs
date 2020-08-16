using EustonLeisureMessaging.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Mail;

namespace EustonLeisureMessaging.MessageTypes
{
    public class Email : IMessage
    {
        private string _id;
        private string _subject;
        private string _body;
        private string _sender;

        public Email(string id, string subject, string body, string sender, IQuarantineUrlService quarantineUrlService)
        {
            Id = id;
            Subject = subject;
            Body = quarantineUrlService.QuarantineUrls(Id, body);
            Sender = sender;
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
                            throw new Exception("Error in message " + value + ": Message was assigned an invalid ID - last 9 character must be numeric.");
                        }
                    }
                    else
                    {
                        throw new Exception("Error in message " + value + ": Message was assigned an invalid ID - first character must refer to a valid message type.");
                    }
                }
                else
                {
                    throw new Exception("Error in message " + value + ": Message was assigned an invalid ID - ID must be 10 characters long.");
                }
            }
        }
        public virtual string Subject
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
                    throw new Exception("Error in message " + Id + ": Message subject is invalid - Email subject must be between 1 and 20 characters long.");
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
                    throw new Exception("Error in message " + Id + ": Message body is invalid - Email message must be between 1 and 1028 characters long.");
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
                    throw new Exception("Error in message " + Id + ": Email address is invalid - please enter a valid email address.");
                }
            }
        }

        public virtual JObject GetMessageAsJObject()
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
