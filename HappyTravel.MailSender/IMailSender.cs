using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HappyTravel.MailSender.Models;

namespace HappyTravel.MailSender
{
    public interface IMailSender
    {
        Task<Result> Send(string templateId, string recipientAddress);

        Task<Result> Send(string templateId, EmailAddress recipientAddress);

        Task<Result> Send<TMessageData>(string templateId, string recipientAddress, TMessageData messageData);

        Task<Result> Send<TMessageData>(string templateId, EmailAddress recipientAddress, TMessageData messageData);

        Task<Result> Send(string templateId, IEnumerable<string> recipientAddresses);

        Task<Result> Send(string templateId, IEnumerable<EmailAddress> recipientAddresses);

        Task<Result> Send<TMessageData>(string templateId, IEnumerable<string> recipientAddresses, TMessageData messageData);

        Task<Result> Send<TMessageData>(string templateId, IEnumerable<EmailAddress> recipientAddresses, TMessageData messageData);
    }
}
