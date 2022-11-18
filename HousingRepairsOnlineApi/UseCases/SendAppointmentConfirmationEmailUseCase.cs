using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;

namespace HousingRepairsOnlineApi.UseCases
{
    public class SendAppointmentConfirmationEmailUseCase : ISendAppointmentConfirmationEmailUseCase
    {
        private readonly INotifyGateway notifyGateway;

        public SendAppointmentConfirmationEmailUseCase(INotifyGateway notifyGateway)
        {
            this.notifyGateway = notifyGateway;
        }
        public void Execute(string email, IDictionary<string, dynamic> personalisation, string templateId)
        {
            Guard.Against.NullOrWhiteSpace(email, nameof(email), "The email provided is invalid");

            ValidateEmail(email);
            notifyGateway.SendEmail(email, templateId, new Dictionary<string, dynamic>(personalisation));
        }

        private static bool ValidateEmail(string email)
        {
            var result = new Regex(@"^\w+([\.-]?\w+)*([\+\.-]?\w+)?@\w+([\.-]?\w+)*(\.\w{2,3})+");
            if (!result.IsMatch(email))
            {
                throw new ArgumentException("The email provided is invalid", nameof(email));
            }
            return true;
        }
    }

}
