using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using HousingRepairsOnline.Authentication.DependencyInjection;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Notify.Client;
using ServiceCollectionExtensions = HousingRepairsOnlineApi.Helpers.ServiceCollectionExtensions;

namespace HousingRepairsOnlineApi
{
    public class Startup
    {
        private const string HousingRepairsOnlineApiIssuerId = "Housing Repairs Online Api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSorEngines(
                new[]
                {
                    new EnvironmentVariableRepairTypeSorConfigurationProvider(RepairType.Tenant),
                    new EnvironmentVariableRepairTypeSorConfigurationProvider(RepairType.Communal),
                    new EnvironmentVariableRepairTypeSorConfigurationProvider(RepairType.Leasehold),
                });

            var sendTenantNotification = new TenantNotificationConfigurationProvider(EnvironmentVariableHelper.GetEnvironmentVariable("TENANT_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID"),
                EnvironmentVariableHelper.GetEnvironmentVariable("TENANT_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID"),
                EnvironmentVariableHelper.GetEnvironmentVariable("TENANT_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID"));

            var sendCommunalNotification = new CommunalNotificationConfigurationProvider(EnvironmentVariableHelper.GetEnvironmentVariable("COMMUNAL_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID"),
                EnvironmentVariableHelper.GetEnvironmentVariable("COMMUNAL_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID"),
                EnvironmentVariableHelper.GetEnvironmentVariable("COMMUNAL_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID"));

            var sendLeaseholdNotification = new LeaseholdNotificationConfigurationProvider(EnvironmentVariableHelper.GetEnvironmentVariable("LEASEHOLD_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID"),
                EnvironmentVariableHelper.GetEnvironmentVariable("LEASEHOLD_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID"),
                EnvironmentVariableHelper.GetEnvironmentVariable("LEASEHOLD_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID"));

            var sendNotificationDictionary = new Dictionary<string, INotificationConfigurationProvider>
            {
                { RepairType.Tenant, sendTenantNotification },
                { RepairType.Communal, sendCommunalNotification },
                { RepairType.Leasehold, sendLeaseholdNotification },
            };

            services.AddTransient<IDictionary<string, INotificationConfigurationProvider>>(_ => sendNotificationDictionary);

            services.AddTransient<INotificationConfigurationResolver, NotificationConfigurationResolver>();

            var cancellationInternalNotificationTemplateId =
                EnvironmentVariableHelper.GetEnvironmentVariable("CANCELLATION_INTERNAL_NOTIFY_TEMPLATE_ID");
            services
                .AddTransient<ICancellationNotificationConfigurationProvider,
                    CancellationInternalNotificationConfigurationProvider>(_ =>
                    new CancellationInternalNotificationConfigurationProvider(
                        cancellationInternalNotificationTemplateId));

            var environmentVariable = EnvironmentVariableHelper.GetEnvironmentVariable("ALLOWED_APPOINTMENT_SLOTS");
            var allowedAppointmentSlots = ServiceCollectionExtensions.ParseAppointmentSlotsConfigurationJson(environmentVariable);
            services.AddTransient(_ => allowedAppointmentSlots);
            services.AddTransient<IAppointmentSlotsFilter, LargestAppointmentSlotFilter>();
            services.AddTransient<IDictionary<string, IAppointmentSlotsFilter>>(_ => new Dictionary<string, IAppointmentSlotsFilter>
            {
                { RepairType.Communal, new LargestAppointmentSlotFilter() }
            });

            var repairPriorityToDays = EnvironmentVariableHelper.GetEnvironmentVariable("REPAIR_PRIORITY_TO_DAYS");
            var repairDays = ServiceCollectionExtensions.ParseRepairPriorityToDaysConfigurationJson(repairPriorityToDays);
            services.AddTransient(_ => repairDays);
            services.AddTransient<IRepairPriorityDaysHelper, RepairPriorityDaysHelper>();
            services.AddTransient<IRepairToRepairBookingResponseMapper, RepairToRepairBookingResponseMapper>();

            services.AddTransient<IAllowedAppointmentsFactory, AllowedAppointmentsFactory>();

            services.AddTransient<IRetrieveAddressesUseCase, RetrieveAddressesUseCase>();
            services.AddTransient<IRetrieveAvailableAppointmentsUseCase, RetrieveAvailableAppointmentsUseCase>();
            services.AddTransient<IBookAppointmentUseCase, BookAppointmentUseCase>();
            services.AddTransient<IRetrieveAvailableCommunalAppointmentUseCase, RetrieveAvailableCommunalAppointmentUseCase>();
            services.AddTransient<IAppointmentTimeToRepairAvailabilityMapper, AppointmentTimeToRepairAvailabilityMapper>();
            services.AddTransient<IRepairToFindRepairResponseMapper, RepairToFindRepairResponseMapper>();
            services.AddTransient<IRetrieveJourneyTriageOptionsUseCase, RetrieveJourneyTriageOptionsUseCase>();
            services.AddTransient<IEarlyExitRepairTriageOptionMapper, EarlyExitRepairTriageOptionMapper>();
            services.AddTransient<ICancelAppointmentUseCase, CancelAppointmentUseCase>();
            services.AddTransient<ICancelRepairRequestUseCase, CancelRepairRequestUseCase>();
            services.AddTransient<ISendRepairCancelledInternalEmailUseCase, SendRepairCancelledInternalEmailUseCase>();

            var addressesApiUrl = EnvironmentVariableHelper.GetEnvironmentVariable("ADDRESSES_API_URL");
            var schedulingApiUrl = EnvironmentVariableHelper.GetEnvironmentVariable("SCHEDULING_API_URL");
            var authenticationIdentifier = EnvironmentVariableHelper.GetEnvironmentVariable("AUTHENTICATION_IDENTIFIER");
            services.AddHttpClient();

            services.AddTransient<IAddressGateway, AddressGateway>(s =>
            {
                var httpClient = s.GetService<HttpClient>();
                httpClient.BaseAddress = new Uri(addressesApiUrl);
                return new AddressGateway(httpClient, authenticationIdentifier);
            });

            services.AddTransient<IAppointmentsGateway, AppointmentsGateway>(s =>
            {
                var httpClient = s.GetService<HttpClient>();
                httpClient.BaseAddress = new Uri(schedulingApiUrl);
                return new AppointmentsGateway(httpClient, authenticationIdentifier);
            });

            var notifyApiKey = EnvironmentVariableHelper.GetEnvironmentVariable("GOV_NOTIFY_KEY");

            services.AddTransient<INotifyGateway, NotifyGateway>(s =>
                {
                    var notifyClient = new NotificationClient(notifyApiKey);
                    return new NotifyGateway(notifyClient);
                }
            );

            var internalEmail = EnvironmentVariableHelper.GetEnvironmentVariable("INTERNAL_EMAIL");

            var daysUntilImageExpiry = EnvironmentVariableHelper.GetEnvironmentVariable("DAYS_UNTIL_IMAGE_EXPIRY");

            services.AddTransient<ISendAppointmentConfirmationSmsUseCase, SendAppointmentConfirmationSmsUseCase>(s =>
            {
                var notifyGateway = s.GetService<INotifyGateway>();
                return new SendAppointmentConfirmationSmsUseCase(notifyGateway);
            });

            services.AddTransient<ISendAppointmentConfirmationEmailUseCase, SendAppointmentConfirmationEmailUseCase>(s =>
            {
                var notifyGateway = s.GetService<INotifyGateway>();
                return new SendAppointmentConfirmationEmailUseCase(notifyGateway);
            });

            services.AddTransient<IAppointmentConfirmationSender, AppointmentConfirmationSender>();

            services.AddTransient<IRetrieveImageLinkUseCase, RetrieveImageLinkUseCase>(s =>
            {
                var azureStorageGateway = s.GetService<IBlobStorageGateway>();
                return new RetrieveImageLinkUseCase(azureStorageGateway, int.Parse(daysUntilImageExpiry));
            });

            services.AddTransient<ISendInternalEmailUseCase, SendInternalEmailUseCase>(s =>
            {
                var notifyGateway = s.GetService<INotifyGateway>();
                return new SendInternalEmailUseCase(notifyGateway, internalEmail);
            });
            services.AddTransient<IRepairRequestToRepairMapper, RepairRequestToRepairMapper>();
            services
                .AddTransient<IRepairDescriptionRequestToRepairDescriptionMapper,
                    RepairDescriptionRequestToRepairDescriptionMapper>();

            services.AddHousingRepairsOnlineAuthentication(HousingRepairsOnlineApiIssuerId);
            services.AddTransient<ISaveRepairRequestUseCase, SaveRepairRequestUseCase>();
            services.AddTransient<IRetrieveRepairsUseCase, RetrieveRepairsUseCase>();
            services.AddTransient<IInternalEmailSender, InternalEmailSender>();

            var cosmosContainer = GetCosmosContainer();

            services.AddTransient<IIdGenerator, IdGenerator>();
            services.AddTransient<IRepairQueryHelper, RepairQueryHelper>(s => new RepairQueryHelper(
                cosmosContainer
            ));

            services.AddTransient<IRepairStorageGateway, CosmosGateway>(s =>
            {
                var idGenerator = s.GetService<IIdGenerator>();
                var repairQueryHelper = s.GetService<IRepairQueryHelper>();
                return new CosmosGateway(
                    cosmosContainer, idGenerator, repairQueryHelper
                );
            });

            var blobContainerClient = GetBlobContainerClient();

            services.AddTransient<IBlobStorageGateway, AzureStorageGateway>(s =>
            {
                return new AzureStorageGateway(
                    blobContainerClient
                );
            });

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HousingRepairsOnlineApi", Version = "v1" });
                c.AddJwtSecurityScheme();
            });

            var cosmosEndpointUrl = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_ENDPOINT_URL");
            var cosmosAuthorizationKey = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_AUTHORIZATION_KEY");
            var cosmosDatabaseId = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_DATABASE_ID");
            var storageConnectionString = EnvironmentVariableHelper.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var blobContainerName = EnvironmentVariableHelper.GetEnvironmentVariable("STORAGE_CONTAINER_NAME");

            services.AddHealthChecks()
                .AddUrlGroup(new Uri(@$"{addressesApiUrl}/health"), "Addresses API")
                .AddUrlGroup(new Uri(@$"{schedulingApiUrl}/health"), "Scheduling API")
                .AddCosmosDb($"AccountEndpoint={cosmosEndpointUrl};AccountKey={cosmosAuthorizationKey};", cosmosDatabaseId, name: "Azure CosmosDb")
                .AddAzureBlobStorage(storageConnectionString, blobContainerName, name: "Azure Blob Storage");
        }

        private static BlobContainerClient GetBlobContainerClient()
        {
            string storageConnectionString = EnvironmentVariableHelper.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string blobContainerName = EnvironmentVariableHelper.GetEnvironmentVariable("STORAGE_CONTAINER_NAME");

            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            return blobContainerClient;
        }

        private static ContainerResponse GetCosmosContainer()
        {
            var endpointUrl = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_ENDPOINT_URL");
            var authorizationKey = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_AUTHORIZATION_KEY");
            var databaseId = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_DATABASE_ID");
            var containerId = EnvironmentVariableHelper.GetEnvironmentVariable("COSMOS_CONTAINER_ID");

            var cosmosClient = new CosmosClient(endpointUrl, authorizationKey);

            Task<DatabaseResponse> databaseResponseTask = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            _ = databaseResponseTask.GetAwaiter().GetResult();
            ;

            Task<ContainerResponse> cosmosContainerResponse =
                cosmosClient.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(containerId, "/RepairID");
            ContainerResponse cosmosContainer = cosmosContainerResponse.GetAwaiter().GetResult();
            ;
            return cosmosContainer;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HousingRepairsOnlineApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSentryTracing();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
