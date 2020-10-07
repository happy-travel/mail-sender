using System;
using System.Net.Http;
using System.Threading.Tasks;
using HappyTravel.MailSender;
using HappyTravel.MailSender.Infrastructure;
using HappyTravel.MailSender.Models;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using Xunit;

namespace HappyTravel.MailSenderTests
{
    public class MailSenderTests
    {
        [Fact]
        public async Task Test1()
        {
            var options = Options.Create(new SenderOptions
            {
                ApiKey = "key",
                BaseUrl = new Uri("https://base-url"),
                SenderAddress = new EmailAddress("sender@happytravel.com")
            });
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var sendGridClientMock = new Mock<ISendGridClient>();
            var sender = new SendGridMailSender(httpClientFactoryMock.Object, null, /*sendGridClientMock.Object, */options);
            var message = new TestMessage
            {
                ClassProp = new TestMessage
                {
                    ClassProp = new TestMessage
                    {
                        IntProp = 14,
                        StringProp = "super inline test"
                    },
                    IntProp = 4,
                    StringProp = null
                },
                IntProp = 2,
                StringProp = "test"
            };

            var result = await sender.Send("template-id", new []{new EmailAddress("email", "name")}, message);
        }


        public class TestMessage
        {
            public TestMessage ClassProp { get; set; }
            public int IntProp { get; set; }
            public string StringProp { get; set; }
        }
    }
}
