using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class SendRepairCancelledInternalEmailUseCaseTests
{
    private SendRepairCancelledInternalEmailUseCase systemUnderTest;
    private Mock<ISendInternalEmailUseCase> sendInternalEmailUseCaseMock = new();
    private Mock<ICancellationNotificationConfigurationProvider> cancellationNotificationConfigurationProviderMock = new();

    public SendRepairCancelledInternalEmailUseCaseTests()
    {
        systemUnderTest = new SendRepairCancelledInternalEmailUseCase(sendInternalEmailUseCaseMock.Object, cancellationNotificationConfigurationProviderMock.Object);
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
    public void GivenRepair_WhenExecuting_ThenEmailIsSent()
    {
        // Arrange
        var repair = new Repair();
        sendInternalEmailUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<IDictionary<string, dynamic>>(), It.IsAny<string>()));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        sendInternalEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepair_WhenExecuting_ThenEmailIsSentWithPersonalisation()
    {
        // Arrange
        var repair = new Repair();

        var personalisation = new Dictionary<string, dynamic>();
        cancellationNotificationConfigurationProviderMock.Setup(
                x => x.GetPersonalisationForTemplate(repair))
            .Returns(personalisation);
        sendInternalEmailUseCaseMock.Setup(x =>
            x.Execute(personalisation, It.IsAny<string>()));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        cancellationNotificationConfigurationProviderMock.VerifyAll();
        sendInternalEmailUseCaseMock.VerifyAll();
    }

    [Fact]
    public void GivenRepair_WhenExecuting_ThenEmailIsSentWithTemplateId()
    {
        // Arrange
        var repair = new Repair();

        var templateId = "templateID";
        cancellationNotificationConfigurationProviderMock.Setup(
                x => x.TemplateId)
            .Returns(templateId);
        sendInternalEmailUseCaseMock.Setup(x =>
            x.Execute(It.IsAny<IDictionary<string, dynamic>>(), templateId));

        // Act
        systemUnderTest.Execute(repair);

        // Assert
        cancellationNotificationConfigurationProviderMock.VerifyAll();
        sendInternalEmailUseCaseMock.VerifyAll();
    }
}
