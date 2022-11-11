using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class AppointmentConfirmationSenderTests
    {
        private readonly Mock<ISendAppointmentConfirmationEmailUseCase> sendAppointmentConfirmationEmailUseCaseMock;
        private readonly Mock<ISendAppointmentConfirmationSmsUseCase> sendAppointmentConfirmationSmsUseCaseMock;
        private Mock<INotificationConfigurationResolver> sendNotificationResolver;
        private readonly AppointmentConfirmationSender systemUnderTest;

        public AppointmentConfirmationSenderTests()
        {
            sendAppointmentConfirmationEmailUseCaseMock = new Mock<ISendAppointmentConfirmationEmailUseCase>();
            sendAppointmentConfirmationSmsUseCaseMock = new Mock<ISendAppointmentConfirmationSmsUseCase>();
            sendNotificationResolver = new Mock<INotificationConfigurationResolver>();
            systemUnderTest = new AppointmentConfirmationSender(sendAppointmentConfirmationEmailUseCaseMock.Object, sendAppointmentConfirmationSmsUseCaseMock.Object, sendNotificationResolver.Object);
        }

        [Fact]
        public void GivenEmailContact_WhenExecute_ThenSendAppointmentConfirmationEmailUseCaseIsCalled()
        {
            var repair = new Repair() { Id = "id", ContactDetails = new RepairContactDetails { Type = AppointmentConfirmationSendingTypes.Email, Value = "abc@defg.hij" }, Time = new RepairAvailability() { Display = "some time" } };
            systemUnderTest.Execute(repair);
            sendAppointmentConfirmationEmailUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>()), Times.Once);
            sendAppointmentConfirmationSmsUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GivenSmsContact_WhenExecute_ThenSendAppointmentConfirmationSmsUseCaseIsCalled()
        {
            var repair = new Repair() { Id = "id", ContactDetails = new RepairContactDetails { Type = AppointmentConfirmationSendingTypes.Sms, Value = "0754325678" }, Time = new RepairAvailability() { Display = "some time" } };
            systemUnderTest.Execute(repair);
            sendAppointmentConfirmationEmailUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>()), Times.Never);
            sendAppointmentConfirmationSmsUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>()), Times.Once);
        }
    }
}
