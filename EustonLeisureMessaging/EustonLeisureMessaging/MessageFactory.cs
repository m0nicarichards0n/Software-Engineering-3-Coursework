using EustonLeisureMessaging.MessageTypes;
using EustonLeisureMessaging.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EustonLeisureMessaging
{
    public class MessageFactory
    {
        /// <summary>
        /// Instantiates the correct type of message based on the first character of the ID passed in.
        /// </summary>
        /// <param name="id">A 10 character Message ID consisting of a letter, followed by 9 numeric characters.</param>
        public static IMessage CreateMessage(string id, string body, IValidationService validationService)
        {
            // Get first character of ID
            char firstLetterOfId = id.ToCharArray()[0];
            
            // Check ID character encoding is valid ASCII
            if (validationService.IsValidEncoding(id))
            {
                // Check Body character encoding is valid ASCII
                if (validationService.IsValidEncoding(body))
                {
                    switch (char.ToUpper(firstLetterOfId))
                    {
                        case 'S':
                            if (validationService.HasCorrectNumberOfDoubleQuotes(body, 4))
                            {
                                // Get message sections from body
                                string[] messageSections = validationService.GetMessageSections(body);
                                if (messageSections.Count() == 2)
                                {
                                    return new SMS(id, messageSections[0], messageSections[1], TextSpeakService.GetInstance());
                                }
                                else
                                {
                                    throw new Exception("Error in message " + id + ": Invalid message format - text must contain a message and a phone number in the format \"{message}\" \"{phone number}\"");
                                }
                            }
                            else
                            {
                                throw new Exception("Error in message " + id + ": Invalid message format - text must contain a message and a phone number in the format \"{message}\" \"{phone number}\"");
                            }
                        case 'E':
                            if (validationService.HasCorrectNumberOfDoubleQuotes(body, 6))
                            {
                                string[] messageSections = validationService.GetMessageSections(body);
                                if (messageSections.Count() == 3)
                                {
                                    // Check if subject is in the format "SIR dd/mm/yy"
                                    if (validationService.IsSignificantIncidentSubject(body))
                                    {
                                        return new SignificantIncidentReport(id, messageSections[0], messageSections[1], messageSections[2], QuarantineUrlService.GetInstance(), SignificantIncidentService.GetInstance());
                                    }
                                    else
                                    {
                                        return new Email(id, messageSections[0], messageSections[1], messageSections[2], QuarantineUrlService.GetInstance());
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error in message " + id + ": Invalid message format - email must contain a subject, message and email address in the format \"{subject}\" \"{message}\" \"{email address}\"");
                                }
                            }
                            else
                            {
                                throw new Exception("Error in message " + id + ": Invalid message format - email must contain a subject, message and email address in the format \"{subject}\" \"{message}\" \"{email address}\"");
                            }
                        case 'T':
                            if (validationService.HasCorrectNumberOfDoubleQuotes(body, 4))
                            {
                                string[] messageSections = validationService.GetMessageSections(body);
                                if (messageSections.Count() == 2)
                                {
                                    return new Tweet(id, messageSections[0], messageSections[1], TextSpeakService.GetInstance(), HashtagMonitoringService.GetInstance());
                                }
                                else
                                {
                                    throw new Exception("Error in message " + id + ": Invalid message format - text must contain a message and a phone number in the format \"{message}\" \"{phone number}\"");
                                }
                            }
                            else
                            {
                                throw new Exception("Error in message " + id + ": Invalid message format - tweet must contain a message and a Twitter ID in the format \"{message}\" \"@{Twitter ID}\"");
                            }
                        default:
                            throw new Exception("Error in message " + id + ": Unrecognised Message Type.");
                    }
                }
                else
                {
                    throw new Exception("Error in message " + id + ": Body contains unsupported characters.");
                }
            }
            else
            {
                throw new Exception("Error in message " + id + ": ID contains unsupported characters.");
            }
        }
    }
}
