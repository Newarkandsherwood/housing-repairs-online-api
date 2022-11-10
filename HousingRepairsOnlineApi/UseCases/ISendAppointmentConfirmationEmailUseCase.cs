using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISendAppointmentConfirmationEmailUseCase
    {
        public void Execute(string email, Dictionary<string, dynamic> personalisation, string templateId);
    }
}
