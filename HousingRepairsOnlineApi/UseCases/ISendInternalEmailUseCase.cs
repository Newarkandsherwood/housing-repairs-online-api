using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISendInternalEmailUseCase
    {
        public void Execute(Dictionary<string, dynamic> personalisation, string templateId);

    }
}
