using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Gateways
{
    public interface IRepairStorageGateway
    {
        Task<Repair> AddRepair(Repair repair);
        Task<IEnumerable<RepairRequestSummary>> SearchByPropertyReference(string repairType, string propertyReference);
        Task<IEnumerable<RepairRequestSummary>> SearchByPostcodeAndId(IEnumerable<string> repairTypes, string id, string repairId);
    }
}
