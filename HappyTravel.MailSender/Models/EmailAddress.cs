namespace HappyTravel.MailSender.Models
{
    public readonly struct EmailAddress
    {
        public EmailAddress(string email, string? name = null)
        {
            Email = email;
            Name = name;
        }


        public string Email { get; }
        public string? Name { get; }
    }
}
