using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.Gateways
{
    public class AzureStorageGateway : IBlobStorageGateway
    {
        private readonly BlobContainerClient storageContainerClient;

        public AzureStorageGateway(BlobContainerClient storageContainerClient)
        {
            this.storageContainerClient = storageContainerClient;
        }

        public Task<string> UploadBlob(string base64Img, string fileExtension)
        {
            string fileName = $"{Guid.NewGuid().ToString()}.{fileExtension}";

            BlobClient blobClient = storageContainerClient.GetBlobClient(fileName);

            byte[] bytes = Convert.FromBase64String(base64Img);
            using (var stream = new MemoryStream(bytes))
            {
                blobClient.Upload(stream);
            }

            return Task.FromResult(blobClient.Uri.AbsoluteUri);
        }

        public string GetUriForBlob(string blobName, int daysUntilExpiry, string storedPolicyName = null)
        {

            var blobClient = storageContainerClient.GetBlobClient(blobName);

            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = storageContainerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = AzureResourceTypes.Blob
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(daysUntilExpiry);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                var sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri.AbsoluteUri;
            }
            else
            {
                throw new Exception(
                    "BlobClient must be authorized with Shared Key credentials to create a service SAS.");
            }
        }
    }
}
