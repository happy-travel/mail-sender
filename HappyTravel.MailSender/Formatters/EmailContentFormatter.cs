using System;

namespace HappyTravel.MailSender.Formatters
{
    public static class EmailContentFormatter
    {
        public static string FromDateTime(DateTime dateTime) => 
            $"{dateTime:yyyy.MM.dd hh:mm} UTC";


        public static string FromEnumDescription<T>(T value) where T : Enum 
            => EnumFormatter.ToDescriptionString(value);
    }
}
