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
        public static IMessage CreateMessage(string id, string body)
        {
            Regex messageSectionsRegex = new Regex(@"\""(.*?)\""");
            // Get first character of ID
            char firstLetterOfId = id.ToCharArray()[0];
            
            // Check ID character encoding is valid ASCII
            if (Encoding.UTF8.GetByteCount(id) == id.Length)
            {
                // Check Body character encoding is valid ASCII
                if (Encoding.UTF8.GetByteCount(body) == body.Length)
                {
                    switch (char.ToUpper(firstLetterOfId))
                    {
                        case 'S':
                            if ((body.Count(x => x == '"') == 4))
                            {
                                // Get message sections from body
                                string[] messageSections = messageSectionsRegex.Matches(body).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();
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
                            if (body.Count(x => x == '"') == 6)
                            {
                                string[] messageSections = messageSectionsRegex.Matches(body).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();
                                if (messageSections.Count() == 3)
                                {
                                    // Check if subject is in the format "SIR dd/mm/yy"
                                    if (messageSections[0].Length == 12
                                        && messageSections[0].Substring(0, 3) == "SIR"
                                        && DateTime.TryParseExact(messageSections[0].Substring(4, 8), "dd/mm/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate))
                                    {
                                        return new SignificantIncidentReport(id, validDate, messageSections[0], messageSections[1], messageSections[2], QuarantineUrlService.GetInstance());
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
                            if ((body.Count(x => x == '"') == 4))
                            {
                                string[] messageSections = messageSectionsRegex.Matches(body).Cast<Match>().Select(m => m.Value).Select(x => x.Trim('"')).ToArray();
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
