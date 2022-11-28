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
    public class SendAppointmentConfirmationSmsUseCaseTests
    {
        private readonly Mock<INotifyGateway> govNotifyGatewayMock;
        private readonly SendAppointmentConfirmationSmsUseCase systemUnderTest;
        private readonly Dictionary<string, dynamic> personalisation = new()
        {
            { "repair_ref", "2" },
            { "appointment_time", "" }
        };
        private readonly string templateId = "456";
        private readonly string phoneNumber = "07415300544";

        public SendAppointmentConfirmationSmsUseCaseTests()
        {
            govNotifyGatewayMock = new Mock<INotifyGateway>();
            systemUnderTest = new SendAppointmentConfirmationSmsUseCase(govNotifyGatewayMock.Object);
        }

        //Arrange
        public static IEnumerable<object[]> InvalidNumberArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), "074353000554" };
        }

        [Theory]
        [MemberData(nameof(InvalidNumberArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnInvalidNumber_WhenExecute_ThenExceptionIsThrown<T>(T exception, string number) where T : Exception
#pragma warning restore xUnit1026
        {
            //Act
            Action act = () => systemUnderTest.Execute(number, personalisation, templateId);

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
            systemUnderTest.Execute(phoneNumber, personalisation, templateId);

            //Assert
            govNotifyGatewayMock.Verify(x => x.SendSms(phoneNumber, templateId, It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
        }
    }
}
