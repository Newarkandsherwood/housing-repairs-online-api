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
        private Mock<INotificationConfigurationResolver> notificationConfigurationResolver;
        private Mock<INotificationConfigurationProvider> notificationConfigurationProvider;
        private readonly AppointmentConfirmationSender systemUnderTest;
        private readonly Dictionary<string, dynamic> personalisation = new()
        {
            {"repair_ref", "2"},
            {"appointment_time", ""}
        };
        private readonly string emailTemplateId = "123";
        private readonly string smsTemplateId = "456";
        public AppointmentConfirmationSenderTests()
        {
            sendAppointmentConfirmationEmailUseCaseMock = new Mock<ISendAppointmentConfirmationEmailUseCase>();
            sendAppointmentConfirmationSmsUseCaseMock = new Mock<ISendAppointmentConfirmationSmsUseCase>();
            notificationConfigurationResolver = new Mock<INotificationConfigurationResolver>();
            notificationConfigurationProvider = new Mock<INotificationConfigurationProvider>();
            notificationConfigurationProvider.Setup(x => x.GetPersonalisationForEmailTemplate(It.IsAny<Repair>())).Returns(personalisation);
            notificationConfigurationProvider.Setup(x => x.GetPersonalisationForSmsTemplate(It.IsAny<Repair>())).Returns(personalisation);
            notificationConfigurationProvider.Setup(x => x.ConfirmationEmailTemplateId).Returns(emailTemplateId);
            notificationConfigurationProvider.Setup(x => x.ConfirmationSmsTemplateId).Returns(smsTemplateId);

            notificationConfigurationResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(notificationConfigurationProvider.Object);

            systemUnderTest = new AppointmentConfirmationSender(sendAppointmentConfirmationEmailUseCaseMock.Object, sendAppointmentConfirmationSmsUseCaseMock.Object, notificationConfigurationResolver.Object);
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
