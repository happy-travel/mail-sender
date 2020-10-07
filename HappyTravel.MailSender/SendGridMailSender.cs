using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HappyTravel.MailSender.Infrastructure;
using HappyTravel.MailSender.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SendGrid;
using SendGrid.Helpers.Mail;
using EmailAddress = HappyTravel.MailSender.Models.EmailAddress;

namespace HappyTravel.MailSender
{
    public class SendGridMailSender : IMailSender
    {
        public SendGridMailSender(IHttpClientFactory httpClientFactory, ILogger<SendGridMailSender>? logger, IOptions<SenderOptions> senderOptions)
        {
            _senderOptions = senderOptions.Value;
            _logger = logger ?? new NullLogger<SendGridMailSender>();
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            if (string.IsNullOrWhiteSpace(_senderOptions.ApiKey))
                throw new ArgumentNullException(nameof(_senderOptions.ApiKey));

            if (_senderOptions.BaseUrl is null)
                throw new ArgumentNullException(nameof(_senderOptions.BaseUrl));

            if (_senderOptions.SenderAddress.Equals(default))
                throw new ArgumentNullException(nameof(_senderOptions.SenderAddress));
        }


        public Task<Result> Send(string templateId, string recipientAddress)
            => Send<object>(templateId, new[] {recipientAddress}, new {});


        public Task<Result> Send(string templateId, EmailAddress recipientAddress)
            => Send<object>(templateId, new[] {recipientAddress}, new {});


        public Task<Result> Send<TMessageData>(string templateId, string recipientAddress, TMessageData messageData)
            => Send(templateId, new[] {recipientAddress}, messageData);


        public Task<Result> Send<TMessageData>(string templateId, EmailAddress recipientAddress, TMessageData messageData)
            => Send(templateId, new[] {recipientAddress}, messageData);


        public Task<Result> Send(string templateId, IEnumerable<string> recipientAddresses)
            => Send<object>(templateId, recipientAddresses, new {});


        public Task<Result> Send(string templateId, IEnumerable<EmailAddress> recipientAddresses)
            => Send<object>(templateId, recipientAddresses, new {});


        public Task<Result> Send<TMessageData>(string templateId, IEnumerable<string> recipientAddresses, TMessageData messageData)
        {
            var emails = recipientAddresses
                .Select(a => new EmailAddress(a));

            return Send(templateId, emails, messageData);
        }


        public async Task<Result> Send<TMessageData>(string templateId, IEnumerable<EmailAddress> recipientAddresses, TMessageData messageData)
        {
            var enumerable = recipientAddresses as EmailAddress[] ?? recipientAddresses.ToArray();
            if (!enumerable.Any())
                return Result.Failure("No recipient addresses provided");

            var templateData = GetTemplateData(messageData);
            using var httpClient = _httpClientFactory.CreateClient(HttpClientName);
            var client = new SendGridClient(httpClient, _senderOptions.ApiKey);
            try
            {
                var result = Result.Success();
                foreach (var address in enumerable)
                {
                    var message = new SendGridMessage
                    {
                        TemplateId = templateId,
                        From = new SendGrid.Helpers.Mail.EmailAddress(_senderOptions.SenderAddress.Email, _senderOptions.SenderAddress.Name)
                    };

                    message.SetTemplateData(templateData);
                    message.AddTo(new SendGrid.Helpers.Mail.EmailAddress(address.Email, address.Name));

                    var response = await client.SendEmailAsync(message);
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        _logger.LogSendMailInformation($"{templateId} successfully e-mailed to {address}");
                    }
                    else
                    {
                        var error = await response.Body.ReadAsStringAsync();
                        var failure =
                            $"Could not send an e-mail {templateId} to {address}, a server responded: '{error}' with the status code '{response.StatusCode}'";
                        result = Result.Combine(result, Result.Failure(failure));

                        _logger.LogSendMailError(failure);
                    }

                    result = Result.Combine(result, Result.Success());
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogSendMailException(ex);
                return Result.Failure("Unhandled error occurred while sending an e-mail.");
            }
        }


        private IDictionary<string, object?> GetTemplateData<TMessageData>(TMessageData messageData)
        {
            var templateData = new ExpandoObject() as IDictionary<string, object?>;
            templateData[_senderOptions.BaseUrlTemplateName] = _senderOptions.BaseUrl;
            if (messageData != null)
            {
                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);
                using var jsonWriter = new JsonTextWriter(writer);

                var serializer = new JsonSerializer {ContractResolver = ContractResolver};
                serializer.Serialize(jsonWriter, messageData);
                jsonWriter.Flush();
                
                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(reader);
                templateData = serializer.Deserialize<ExpandoObject>(jsonReader);
            }
            
            templateData[_senderOptions.BaseUrlTemplateName] = _senderOptions.BaseUrl;
            return templateData;
        }


        public static string HttpClientName = "SendGrid";


        private static readonly IContractResolver ContractResolver = new CamelCasePropertyNamesContractResolver();


        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendGridMailSender> _logger;
        private readonly SenderOptions _senderOptions;
    }
}