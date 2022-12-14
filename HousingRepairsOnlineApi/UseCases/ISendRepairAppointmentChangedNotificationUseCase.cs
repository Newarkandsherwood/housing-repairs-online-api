using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases;

public interface ISendRepairAppointmentChangedNotificationUseCase
{
    void Execute(Repair repair);
}
