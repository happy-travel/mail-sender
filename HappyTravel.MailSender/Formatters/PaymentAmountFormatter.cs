using System.Globalization;
using HappyTravel.EdoContracts.General.Enums;

namespace HappyTravel.MailSender.Formatters
{
    public static class PaymentAmountFormatter
    {
        public static string ToCurrencyString(decimal amount, Currencies currency)
        {
            return currency switch
            {
                Currencies.USD => Format(amount, "en-US"),
                Currencies.EUR => Format(amount, "de-DE"),
                Currencies.AED => Format(amount, "ar-SA"),
                Currencies.SAR => Format(amount, "ar-AE"),
                Currencies.NotSpecified => $"{amount:F2}",
                _ => $"{amount:F2}"
            };


            static string Format(decimal value, string culture) 
                => string.Format(new CultureInfo(culture), "{0:C}", value);
        }
    }
}
