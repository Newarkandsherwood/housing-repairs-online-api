using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class SendAppointmentConfirmationEmailUseCaseTests
    {
        private readonly Mock<INotifyGateway> govNotifyGatewayMock;
        private readonly SendAppointmentConfirmationEmailUseCase systemUnderTest;
        private readonly Dictionary<string, dynamic> personalisation = new()
        {
            { "repair_ref", "2" },
            { "appointment_time", "" }
        };
        private readonly string templateId = "456";

        public SendAppointmentConfirmationEmailUseCaseTests()
        {
            govNotifyGatewayMock = new Mock<INotifyGateway>();
            systemUnderTest = new SendAppointmentConfirmationEmailUseCase(govNotifyGatewayMock.Object);
        }

        //Arrange
        public static IEnumerable<object[]> InvalidEmailArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), "notanemail.com" };
        }

        [Theory]
        [MemberData(nameof(InvalidEmailArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnInvalidEmail_WhenExecute_ThenExceptionIsThrown<T>(T exception, string email) where T : Exception
#pragma warning restore xUnit1026
        {
            //Act
            Action act = () => systemUnderTest.Execute(email, personalisation, templateId);

            //Assert
            act.Should().ThrowExactly<T>();
        }

        //Arrange
        public static IEnumerable<object[]> InvalidBookingRefArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
        }

        [Fact]
        public void GivenValidParameters_WhenExecute_ThenGovNotifyGateWayIsCalled()
        {
            //Act
            systemUnderTest.Execute("dr.who@tardis.com", personalisation, templateId);

            //Assert
            govNotifyGatewayMock.Verify(x => x.SendEmail("dr.who@tardis.com", templateId, It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
        }
    }
}
