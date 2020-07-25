using EustonLeisureMessaging.MessageTypes;
using System;

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
            char firstLetter = id.ToCharArray()[0];

            switch (char.ToUpper(firstLetter))
            {
                case 'S':
                    return new SMS(id, body);
                case 'E':
                    return new Email(id, body);
                case 'T':
                    return new Tweet(id, body);
                default:
                    throw new Exception("Unrecognised Message Type.");
            }
        }
    }
}
