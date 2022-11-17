using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISendAppointmentConfirmationSmsUseCase
    {
        public void Execute(string number, IDictionary<string, dynamic> personalisation, string templateId);
    }
}
