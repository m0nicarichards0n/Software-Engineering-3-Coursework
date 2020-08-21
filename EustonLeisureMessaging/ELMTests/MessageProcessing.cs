using EustonLeisureMessaging;
using EustonLeisureMessaging.MessageTypes;
using EustonLeisureMessaging.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ELMTests
{
    [TestClass]
    public class MessageProcessing
    {
        private const string _emailHeader = "E123456789";
        private const string _emailBody = "\"Test Subject\" \"Test message body.\" \"test@address.com\"";
        [TestMethod]
        public void CreateMessage_Email_ReturnsEmail()
        {
            IMessage message = MessageFactory.CreateMessage(_emailHeader, _emailBody, ValidationService.GetInstance());
            Assert.IsInstanceOfType(message, typeof(Email));
        }

        private const string _smsHeader = "S123456789";
        private const string _smsBody = "\"Test message body.\" \"07565490298\"";
        [TestMethod]
        public void CreateMessage_SMS_ReturnsSMS()
        {
            IMessage message = MessageFactory.CreateMessage(_smsHeader, _smsBody, ValidationService.GetInstance());
            Assert.IsInstanceOfType(message, typeof(SMS));
        }

        private const string _tweetHeader = "T123456789";
        private const string _tweetBody = "\"Test message body.\" \"@TestUser\"";
        [TestMethod]
        public void CreateMessage_Tweet_ReturnTweet()
        {
            IMessage message = MessageFactory.CreateMessage(_tweetHeader, _tweetBody, ValidationService.GetInstance());
            Assert.IsInstanceOfType(message, typeof(Tweet));
        }

        private const string _sirHeader = "E123456789";
        private const string _sirBody = "\"SIR 21/08/20\" \"Sport Centre Code: 12-345-67 Nature of Incident: Bomb Threat, Test message body.\" \"test@address.com\"";
        [TestMethod]
        public void CreateMessage_SIR_ReturnSIR()
        {
            IMessage message = MessageFactory.CreateMessage(_sirHeader, _sirBody, ValidationService.GetInstance());
            Assert.IsInstanceOfType(message, typeof(SignificantIncidentReport));
        }
    }
}
