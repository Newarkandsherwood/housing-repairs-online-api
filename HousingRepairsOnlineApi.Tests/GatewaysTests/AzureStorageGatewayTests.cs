using System;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using HousingRepairsOnlineApi.Gateways;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.GatewaysTests
{
    public class AzureStorageGatewayTests
    {
        private readonly AzureStorageGateway azureStorageGateway;
        private readonly Mock<BlobContainerClient> mockStorageContainerClient;

        public AzureStorageGatewayTests()
        {
            mockStorageContainerClient = new Mock<BlobContainerClient>()
            {
                Name = "BlobContainerClientName"
            };

            azureStorageGateway = new AzureStorageGateway(mockStorageContainerClient.Object);
        }

        [Fact]
        public async void AFileIsUploaded()
        {
            // Arrange
            const string base64Img = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw1AUhU9TpaJVB4uIOGSoThZERRy1CkWoEGqFVh1MXvojNGlIUlwcBdeCgz+LVQcXZ10dXAVB8AfE0clJ0UVKvC8ptIjxwuN9nHfP4b37AKFWYprVNgZoum2mEnExk10RQ68IIIQe9KNLZpYxK0lJ+NbXPXVT3cV4ln/fn9Wt5iwGBETiGWaYNvE68dSmbXDeJ46woqwSnxOPmnRB4keuKx6/cS64LPDMiJlOzRFHiMVCCystzIqmRjxJHFU1nfKFjMcq5y3OWqnCGvfkLwzn9OUlrtMaQgILWIQEEQoq2EAJNmK066RYSNF53Mc/6Polcink2gAjxzzK0CC7fvA/+D1bKz8x7iWF40D7i+N8DAOhXaBedZzvY8epnwDBZ+BKb/rLNWD6k/RqU4seAb3bwMV1U1P2gMsdYODJkE3ZlYK0hHweeD+jb8oCfbdA56o3t8Y5Th+ANM0qeQMcHAIjBcpe83l3R+vc/u1pzO8H+I9yds6VEEcAAAAJcEhZcwAALiMAAC4jAXilP3YAAAAHdElNRQfmAQcOFjXsyx/IAAAAGXRFWHRDb21tZW50AENyZWF0ZWQgd2l0aCBHSU1QV4EOFwAAAAxJREFUCNdj0HiTBAACtgF3wqeo5gAAAABJRU5ErkJggg==";
            const string fileExtension = "png";

            var mockBlobClient = new Mock<BlobClient>();

            mockBlobClient.Setup(x => x.Upload(It.IsAny<MemoryStream>()));
            mockBlobClient.Setup(x => x.Uri).Returns(new Uri("http://some.thing"));


            mockStorageContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(mockBlobClient.Object);

            var actual = await azureStorageGateway.UploadBlob(base64Img, fileExtension);

            // Assert
            mockStorageContainerClient.Verify(foo => foo.GetBlobClient(It.IsRegex(@$"^[0-9A-Fa-f\-]*\.{fileExtension}")), Times.Once());
            mockBlobClient.Verify(x => x.Upload(It.IsAny<MemoryStream>()));
        }

        [Fact]
        public void GivenABlob_WhenGetBlobSasUri_ThenASasUriIsGenerated()
        {
            // Arrange
            const string blobName = "something.png";

            var mockBlobClient = new Mock<BlobClient>();

            mockBlobClient.Setup(x => x.CanGenerateSasUri).Returns(true);
            mockBlobClient.Setup(x => x.GenerateSasUri(It.IsAny<BlobSasBuilder>())).Returns(new Uri("http://some.thing"));
            mockBlobClient.Setup(x => x.Uri).Returns(new Uri("http://some.thing"));
            mockBlobClient.Setup(x => x.Name).Returns("name");


            mockBlobClient.Setup(x => x.Name).Returns("BlobContainerClientName");


            mockStorageContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(mockBlobClient.Object);


            var actual = azureStorageGateway.GetUriForBlob(blobName, 100, "antything");

            // Assert
            mockStorageContainerClient.Verify(foo => foo.GetBlobClient(It.IsRegex(blobName)), Times.Once());
            mockBlobClient.Verify(x => x.GenerateSasUri(It.IsAny<BlobSasBuilder>()), Times.Once);
        }
        [Fact]
        public void GivenInvalidPermissions_WhenGetBlobSasUri_ThenExceptionIsThrown()
        {
            // Arrange
            const string blobName = "something.png";

            var mockBlobClient = new Mock<BlobClient>();

            mockBlobClient.Setup(x => x.CanGenerateSasUri).Returns(false);


            mockStorageContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(mockBlobClient.Object);

            Assert.Throws<Exception>(() =>
            {
                azureStorageGateway.GetUriForBlob(blobName, 100, "anything");
            });

            // Assert
            mockBlobClient.Verify(x => x.GenerateSasUri(It.IsAny<BlobSasBuilder>()), Times.Never);
        }
    }
}
