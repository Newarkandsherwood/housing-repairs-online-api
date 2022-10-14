using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.Extensions.Logging;

namespace HousingRepairsOnlineApi.UseCases
{
    public class RetrieveRepairsUseCase : IRetrieveRepairsUseCase
    {
        private readonly IRepairStorageGateway cosmosGateway;
        private readonly ISoREngine sorEngine;

        public RetrieveRepairsUseCase(IRepairStorageGateway cosmosGateway, ISoREngine sorEngine)

        {
            this.cosmosGateway = cosmosGateway;
            this.sorEngine = sorEngine;
        }

        public async Task<IEnumerable<Repair>> Execute(string propertyReference)
        {
            return new []{ new Repair()};
        }
    }
}
