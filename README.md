# housing-repairs-online-api

## Local Development
1. `mv sample.env .env`
2. Set values in `.env`
3. `make run`

## Authentication
Requests to the API require authentication.
The API implements JSON Web Tokens (JWT) for authentication.

A unique, secret identifier is required to generate a JWT.
This should be set in an `AUTHENTICATION_IDENTIFIER` environment variable which will be consumed during startup.

A JWT can be generated using a POST request to the `Authentication` endpoint, i.e.
```http request
POST https://localhost:5001/Authentication?identifier=<AUTHENTICATION_IDENTIFIER>
```
The body of the response will contain a JWT which will expire after 1 minute.

All other requests require a valid JWT to be sent in the `Authorization` header with a value of
`Bearer <JWT TOKEN>`, i.e.
```http request
GET https://localhost:5001/Addresses?postcode=1
Authorization: Bearer <JWT TOKEN>
```
## Running the API locally with other Services
In order to run the API locally, the following steps may need to be taken to exclude some functionality.

Only make the changes necessary to enable you to work on functionality locally and bear in mind the adjustments you make locally when diagnosing issues.
Take care to not commit any of these local-only changes.

In your local .env file, if you wish to connect to the Housing Management API, set
AUTHENTICATION_IDENTIFIER to the same value as the front end and the Housing Management API.
Set ADDRESSES_API_URL to the location of your locally running Housing Management API:
```
export ADDRESSES_API_URL=https://localhost:6001/
```

1. In Program.cs, if you do not have Sentry set up, then comment out the Sentry section:
```
   // webBuilder.UseSentry(o =>
   // {
   //     o.Dsn = EnvironmentVariableHelper.GetEnvironmentVariable("SENTRY_DSN");
   //
   //     var environment = EnvironmentVariableHelper.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
   //     if (environment == Environments.Development)
   //     {
   //         o.Debug = true;
   //         o.TracesSampleRate = 1.0;
   //     }
   // });
```
2. In Startup.cs, to bypass Azure Storage Gateway, make the following changes:
```
    //var blobContainerClient = GetBlobContainerClient();

    services.AddTransient<IBlobStorageGateway, AzureStorageGateway>(s =>
    {
        return new AzureStorageGateway(
            null//blobContainerClient
        );
    });
```

3. In Startup.cs, disable Health Checks
```
   // services.AddHealthChecks()
   //     .AddUrlGroup(new Uri(@$"{addressesApiUrl}/health"), "Addresses API")
   //     .AddUrlGroup(new Uri(@$"{schedulingApiUrl}/health"), "Scheduling API")
   //     .AddCosmosDb($"AccountEndpoint={cosmosEndpointUrl};AccountKey={cosmosAuthorizationKey};", cosmosDatabaseId, name: "Azure CosmosDb")
   //     .AddAzureBlobStorage(storageConnectionString, blobContainerName, name: "Azure Blob Storage");

   // endpoints.MapHealthChecks("/health");
 ```

4. In Startup.cs, disable Https Redirection if you wish to use Http.
```
//app.UseHttpsRedirection();
```
5. In Startup.cs, disable Sentry Tracing
```
   //  app.UseSentryTracing();
```

