using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class SendInternalEmailUseCaseTests
    {
        private readonly Mock<INotifyGateway> govNotifyGatewayMock;
        private readonly SendInternalEmailUseCase systemUnderTest;
        private Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
        {
            {"repair_ref", "1"},
            {"uprn", "uprn"},
            {"address", "address"},
            {"sor", "SOR"},
            {"repair_desc", "Description"},
            {"contact_no", "contact_no"}
        };
        private readonly string templateId = "456";

        public SendInternalEmailUseCaseTests()
        {
            govNotifyGatewayMock = new Mock<INotifyGateway>();
            systemUnderTest = new SendInternalEmailUseCase(govNotifyGatewayMock.Object, "dr.who@tardis.com");
        }

        public static IEnumerable<object[]> InvalidImageArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
        }

        [Fact]
        public void GivenNoImage_WhenExecute_ThenGovNotifyGateWayIsCalled()
        {
            //Act

            systemUnderTest.Execute(personalisation, templateId);

            //Assert
            govNotifyGatewayMock.Verify(x => x.SendEmail("dr.who@tardis.com", templateId, It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
        }

        [Fact]
        public void GivenValidParameters_WhenExecute_ThenGovNotifyGateWayIsCalled()
        {
            //Act
            systemUnderTest.Execute(personalisation, templateId);

            //Assert
            govNotifyGatewayMock.Verify(x => x.SendEmail("dr.who@tardis.com", templateId, It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
        }

    }
}
