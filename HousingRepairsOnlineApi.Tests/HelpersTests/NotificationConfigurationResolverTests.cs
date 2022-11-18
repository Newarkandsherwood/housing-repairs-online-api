using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class NotificationConfigurationResolverTests
{
    private readonly Mock<TenantNotificationConfigurationProvider> tenantNotificationConfigurationProvider;
    private readonly NotificationConfigurationResolver systemUnderTest;

    public NotificationConfigurationResolverTests()
    {
        tenantNotificationConfigurationProvider = new Mock<TenantNotificationConfigurationProvider>("", "", "");

        Dictionary<string, INotificationConfigurationProvider> notificationConfigurationProviders =
            new Dictionary<string, INotificationConfigurationProvider>();
        notificationConfigurationProviders.Add("TENANT", tenantNotificationConfigurationProvider.Object);

        systemUnderTest = new NotificationConfigurationResolver(notificationConfigurationProviders);
    }

    [Fact]
    public void GivenInvalidRepairType_WhenResolving_ThenExceptionIsReturned()
    {
        // Act
        Action actual = () => systemUnderTest.Resolve("INVALIDTYPE");

        // Assert
        actual.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void GivenUnregisteredRepairType_WhenResolving_ThenExceptionIsReturned()
    {
        // Act
        Action actual = () => systemUnderTest.Resolve("COMMUNAL");

        // Assert
        actual.Should().ThrowExactly<NotSupportedException>();
    }

    [Fact]
    public void GivenValidRepairType_WhenResolving_ThenTenantNotificationConfigurationProviderIsReturned()
    {
        // Act
        var actual = systemUnderTest.Resolve("TENANT");

        // Assert
        actual.Should().BeEquivalentTo(tenantNotificationConfigurationProvider.Object);
    }




}
