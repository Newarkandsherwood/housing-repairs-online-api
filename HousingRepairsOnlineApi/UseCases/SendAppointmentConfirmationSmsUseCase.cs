using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;

namespace HousingRepairsOnlineApi.UseCases
{
    public class SendAppointmentConfirmationSmsUseCase : ISendAppointmentConfirmationSmsUseCase
    {
        private readonly INotifyGateway notifyGateway;

        public SendAppointmentConfirmationSmsUseCase(INotifyGateway notifyGateway)
        {
            this.notifyGateway = notifyGateway;
        }

        public void Execute(string number, IDictionary<string, dynamic> personalisation, string templateId)
        {
            Guard.Against.NullOrWhiteSpace(number, nameof(number), "The email provided is invalid");

            ValidatePhoneNumber(number);
            notifyGateway.SendSms(number, templateId, new Dictionary<string, dynamic>(personalisation));
        }

        private static bool ValidatePhoneNumber(string number)
        {
            var result = new Regex(@"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$");
            if (!result.IsMatch(number))
            {
                throw new ArgumentException("The phone number provided is invalid", nameof(number));
            }
            return true;
        }
    }
}
