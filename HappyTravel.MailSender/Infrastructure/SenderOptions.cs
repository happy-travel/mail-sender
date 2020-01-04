using System;
using HappyTravel.MailSender.Models;

namespace HappyTravel.MailSender.Infrastructure
{
#nullable disable
    public class SenderOptions
    {
        public string ApiKey { get; set; }
        public Uri BaseUrl { get; set; }
        public string BaseUrlTemplateName { get; set; } = "baseUrl";
        public EmailAddress SenderAddress { get; set; }
    }
#nullable restore
}
