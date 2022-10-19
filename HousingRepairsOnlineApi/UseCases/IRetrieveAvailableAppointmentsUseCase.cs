using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IRetrieveAvailableAppointmentsUseCase
    {
        public Task<List<AppointmentTime>> Execute(string repairType, string repairLocation, string repairProblem, string repairIssue,
            string locationId, DateTime? fromDate = null);
    }
}
