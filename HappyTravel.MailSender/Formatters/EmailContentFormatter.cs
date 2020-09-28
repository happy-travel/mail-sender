using System;

namespace HappyTravel.MailSender.Formatters
{
    public static class EmailContentFormatter
    {
        public static string FromDateTime(DateTime dateTime) => 
            $"{dateTime:yyyy.MM.dd hh:mm} UTC";


        public static string FromEnumDescription<T>(T value) where T : Enum 
            => EnumFormatter.ToDescriptionString(value);


        public static string FromPassengerName(string firstName, string lastName, string? title = null)
        {
            var maskedName = firstName.Length == 1 ? "*" : firstName.Substring(0, 1);

            return title is null 
                ? $"{maskedName}. {lastName}" 
                : $"{title}. {maskedName} {lastName}";
        }
    }
}
