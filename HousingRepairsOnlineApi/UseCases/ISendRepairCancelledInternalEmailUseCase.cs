using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases;

public interface ISendRepairCancelledInternalEmailUseCase
{
    void Execute(Repair repair);
}
