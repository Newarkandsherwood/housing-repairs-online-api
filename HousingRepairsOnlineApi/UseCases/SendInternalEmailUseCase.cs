using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HousingRepairsOnlineApi.Gateways;

namespace HousingRepairsOnlineApi.UseCases
{
    public class SendInternalEmailUseCase : ISendInternalEmailUseCase
    {
        private readonly INotifyGateway notifyGateway;
        private readonly string internalEmail;

        public SendInternalEmailUseCase(INotifyGateway notifyGateway, string internalEmail)
        {
            this.notifyGateway = notifyGateway;
            this.internalEmail = internalEmail;
        }
        public void Execute(Dictionary<string, dynamic> personalisation, string imagelink, string templateId)
        {
            ValidateEmail(internalEmail);

            personalisation.Add("image_1", imagelink);

            notifyGateway.SendEmail(internalEmail, templateId, personalisation);
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
