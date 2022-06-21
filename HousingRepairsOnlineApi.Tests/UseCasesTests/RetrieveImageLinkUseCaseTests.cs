using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class RetrieveImageLinkUseCaseTests
    {
        private readonly RetrieveImageLinkUseCase sytemUndertest;
        private readonly Mock<IBlobStorageGateway> mockAzureStorageGateway;

        public RetrieveImageLinkUseCaseTests()
        {
            mockAzureStorageGateway = new Mock<IBlobStorageGateway>();
            sytemUndertest = new RetrieveImageLinkUseCase(mockAzureStorageGateway.Object, 100);
        }

        [Fact]
        public void GivenAPhotoUrl_WhenExecute_ThenGatewayIsCalled()
        {
            const string BlobName = "//https://housingrepairsonline.blob.core.windows.net/housing-repairs-online/0f6780dd-ce73-44f9-b64c-b56c061560ea.png";
            mockAzureStorageGateway.Setup(x =>
                x.GetUriForBlob(BlobName, 100, null)).Returns("uri");

            var _ = sytemUndertest.Execute(BlobName);

            mockAzureStorageGateway.Verify(x => x.GetUriForBlob("0f6780dd-ce73-44f9-b64c-b56c061560ea.png", 100, null), Times.Once);
        }
    }
}
