using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISendInternalEmailUseCase
    {
        public void Execute(IDictionary<string, dynamic> personalisation, string templateId);

    }
}
