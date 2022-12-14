using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests;

public class SendRepairAppointmentChangedNotificationUseCaseTests
{
    private readonly SendRepairAppointmentChangedNotificationUseCase systemUnderTest;
    private readonly Mock<ISendAppointmentConfirmationEmailUseCase> sendAppointmentConfirmationEmailUseCaseMock = new();
    private readonly Mock<ISendAppointmentConfirmationSmsUseCase> sendAppointmentConfirmationSmsUseCaseMock = new();
    private readonly Mock<IAppointmentChangedNotificationConfigurationProvider> appointmentChangedNotificationConfigurationProviderMock = new();

    public SendRepairAppointmentChangedNotificationUseCaseTests()
    {
        systemUnderTest =
            new SendRepairAppointmentChangedNotificationUseCase(
                sendAppointmentConfirmationEmailUseCaseMock.Object,
                sendAppointmentConfirmationSmsUseCaseMock.Object,
                appointmentChangedNotificationConfigurationProviderMock.Object
            );
    }

    [Fact]
    public void GivenNullRepair_WhenExecuting_ThenArgumentNullExceptionIsThrown()
    {
        // Arrange

        // Act
        var act = () => systemUnderTest.Execute(null);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("repair");
    }

    [Fact]
    public void GivenRepairWithEmailContactDetail_WhenExecuting_ThenEmailIsSent()
    {
        // Arrange
        const string email = "tester@test.com";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Email,
                Value = email,
            }
        };
        sendAppointmentConfirmationEmailUseCaseMock.Setup(x =>
            x.Execute(email, It.IsAny<IDictionary<string, dynamic>>(), It.IsAny<string>()));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        sendAppointmentConfirmationEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepairWithEmailContactDetail_WhenExecuting_ThenEmailIsSentWithPersonalisation()
    {
        // Arrange
        const string email = "tester@test.com";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Email,
                Value = email,
            }
        };

        var personalisation = new Dictionary<string, dynamic>();

        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.GetPersonalisationForEmailTemplate(repair))
            .Returns(personalisation);
        sendAppointmentConfirmationEmailUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<string>(), personalisation, It.IsAny<string>()));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        appointmentChangedNotificationConfigurationProviderMock.VerifyAll();
        sendAppointmentConfirmationEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepairWithEmailContactDetail_WhenExecuting_ThenEmailIsSentWithTemplateId()
    {
        // Arrange
        const string email = "tester@test.com";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Email,
                Value = email,
            }
        };

        var templateId = "templateID";
        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.EmailTemplateId)
            .Returns(templateId);
        sendAppointmentConfirmationEmailUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<string>(), It.IsAny<IDictionary<string, dynamic>>(), templateId));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        appointmentChangedNotificationConfigurationProviderMock.VerifyAll();
        sendAppointmentConfirmationEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepairWithEmailContactDetail_WhenExecuting_ThenSmsIsNotSent()
    {
        // Arrange
        const string email = "tester@test.com";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Email,
                Value = email,
            }
        };

        var personalisation = new Dictionary<string, dynamic>();
        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.GetPersonalisationForEmailTemplate(repair))
            .Returns(personalisation);
        var templateId = "templateID";
        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.EmailTemplateId)
            .Returns(templateId);
        sendAppointmentConfirmationEmailUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<string>(), It.IsAny<IDictionary<string, dynamic>>(), templateId));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        appointmentChangedNotificationConfigurationProviderMock.Verify(x => x.GetPersonalisationForEmailTemplate(repair));
        appointmentChangedNotificationConfigurationProviderMock.Verify(x => x.EmailTemplateId);
        appointmentChangedNotificationConfigurationProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GivenRepairWithSmsContactDetail_WhenExecuting_ThenSmsIsSent()
    {
        // Arrange
        const string phoneNumber = "07234567890";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Sms,
                Value = phoneNumber,
            }
        };
        sendAppointmentConfirmationSmsUseCaseMock.Setup(x =>
            x.Execute(phoneNumber, It.IsAny<IDictionary<string, dynamic>>(), It.IsAny<string>()));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        sendAppointmentConfirmationSmsUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepairWithSmsContactDetail_WhenExecuting_ThenSmsIsSentWithPersonalisation()
    {
        // Arrange
        const string phoneNumber = "07234567890";

        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Sms,
                Value = phoneNumber,
            }
        };

        var personalisation = new Dictionary<string, dynamic>();

        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.GetPersonalisationForSmsTemplate(repair))
            .Returns(personalisation);
        sendAppointmentConfirmationSmsUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<string>(), personalisation, It.IsAny<string>()));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        appointmentChangedNotificationConfigurationProviderMock.VerifyAll();
        sendAppointmentConfirmationEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepairWithSmsContactDetail_WhenExecuting_ThenSmsIsSentWithTemplateId()
    {
        // Arrange
        const string phoneNumber = "07234567890";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Sms,
                Value = phoneNumber,
            }
        };

        var templateId = "templateID";
        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.SmsTemplateId)
            .Returns(templateId);
        sendAppointmentConfirmationSmsUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<string>(), It.IsAny<IDictionary<string, dynamic>>(), templateId));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        appointmentChangedNotificationConfigurationProviderMock.VerifyAll();
        sendAppointmentConfirmationEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepairWithSmsContactDetail_WhenExecuting_ThenEmailIsNotSent()
    {
        // Arrange
        const string phoneNumber = "07234567890";
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = AppointmentConfirmationSendingTypes.Sms,
                Value = phoneNumber,
            }
        };

        var templateId = "templateID";
        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.SmsTemplateId)
            .Returns(templateId);
        var personalisation = new Dictionary<string, dynamic>();

        appointmentChangedNotificationConfigurationProviderMock.Setup(
                x => x.GetPersonalisationForSmsTemplate(repair))
            .Returns(personalisation);
        sendAppointmentConfirmationSmsUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<string>(), It.IsAny<IDictionary<string, dynamic>>(), templateId));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        appointmentChangedNotificationConfigurationProviderMock.Verify(x => x.GetPersonalisationForSmsTemplate(repair));
        appointmentChangedNotificationConfigurationProviderMock.Verify(x => x.SmsTemplateId);
        appointmentChangedNotificationConfigurationProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GivenRepairWithUnknownContactDetail_WhenExecuting_TheNotSupportExceptionIsThrown()
    {
        // Arrange
        var repair = new Repair
        {
            ContactDetails = new()
            {
                Type = "Fax",
            }
        };

        // Act
        var act = () => systemUnderTest.Execute(repair);

        // Assert
        act.Should().ThrowExactly<NotSupportedException>().WithMessage("*Fax*");
    }
}
