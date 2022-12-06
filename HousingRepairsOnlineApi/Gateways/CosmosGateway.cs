using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HACT.Dtos;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Gateways
{
    public class CosmosGateway : IRepairStorageGateway
    {
        private readonly Container cosmosContainer;
        private readonly IIdGenerator idGenerator;
        private readonly IRepairQueryHelper repairQueryHelper;

        public CosmosGateway(Container cosmosContainer, IIdGenerator idGenerator, IRepairQueryHelper repairQueryHelper)
        {
            this.cosmosContainer = cosmosContainer;
            this.idGenerator = idGenerator;
            this.repairQueryHelper = repairQueryHelper;
        }

        /// <summary>
        /// Add Repair items to the container
        /// </summary>
        public async Task<Repair> AddRepair(Repair repair)
        {
            repair.Id = idGenerator.Generate();
            try
            {
                ItemResponse<Repair> itemResponse = await cosmosContainer.CreateItemAsync(repair);

                return itemResponse.Resource;
            }
            catch (CosmosException)
            {
                var newRepair = await AddRepair(repair);
                return newRepair;
            }
        }

        public async Task<IEnumerable<RepairRequestSummary>> SearchByPropertyReference(string repairType, string propertyReference)
        {
            using var queryResultSetIterator = repairQueryHelper.GetItemQueryIterator<Repair>(repairType, propertyReference);
            var repairs = new List<RepairRequestSummary>();

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach (var result in currentResultSet)
                {
                    repairs.Add(new RepairRequestSummary()
                    {
                        Address = result.Address,
                        Description = result.Description,
                        Issue = result.Issue,
                        Location = result.Location,
                        Problem = result.Problem
                    });
                }
            }

            return repairs;
        }

        public async Task<IEnumerable<Repair>> SearchByPostcodeAndId(IEnumerable<string> repairTypes, string postcode, string repairId)
        {
            Guard.Against.NullOrEmpty(repairTypes, nameof(repairTypes));
            Guard.Against.NullOrWhiteSpace(postcode, nameof(postcode));
            Guard.Against.NullOrWhiteSpace(repairId, nameof(repairId));

            using var queryResultSetIterator = repairQueryHelper.GetRepairSearchIterator(repairTypes, postcode, repairId);
            IEnumerable<Repair> repairs = Array.Empty<Repair>();

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                repairs = currentResultSet.Select(repair => repair);
            }

            return repairs;
        }

        public async Task CancelRepair(string repairId)
        {
            Guard.Against.NullOrWhiteSpace(repairId, nameof(repairId));
            var itemResponse = await cosmosContainer.PatchItemAsync<Repair>(
                id: repairId,
                partitionKey: new PartitionKey("RepairID"),
                patchOperations: new[] { PatchOperation.Replace("/status", RepairStatus.Cancelled) }
            );
        }
    }
}
