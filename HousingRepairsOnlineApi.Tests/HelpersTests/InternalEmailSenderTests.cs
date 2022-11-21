using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class InternalEmailSenderTests
    {
        private readonly InternalEmailSender systemUnderTest;
        private readonly Mock<IRetrieveImageLinkUseCase> retrieveImageLinkUseCase;
        private readonly Mock<ISendInternalEmailUseCase> sendInternalEmailUseCase;
        private readonly Mock<INotificationConfigurationResolver> notificationConfigurationResolver;
        private readonly Mock<INotificationConfigurationProvider> notificationConfigurationProvider;
        private readonly Dictionary<string, dynamic> personalisation = new()
        {
            { "repair_ref", "2" },
            { "appointment_time", "" }
        };

        private readonly string templateId = "123";

        public InternalEmailSenderTests()
        {
            retrieveImageLinkUseCase = new Mock<IRetrieveImageLinkUseCase>();
            sendInternalEmailUseCase = new Mock<ISendInternalEmailUseCase>();
            notificationConfigurationResolver = new Mock<INotificationConfigurationResolver>();
            notificationConfigurationProvider = new Mock<INotificationConfigurationProvider>();

            systemUnderTest = new InternalEmailSender(retrieveImageLinkUseCase.Object, sendInternalEmailUseCase.Object, notificationConfigurationResolver.Object);
            notificationConfigurationProvider.Setup(x => x.GetPersonalisationForInternalEmailTemplate(It.IsAny<Repair>(), It.IsAny<IRetrieveImageLinkUseCase>())).Returns(Task.FromResult<IDictionary<string, dynamic>>(personalisation));
            notificationConfigurationProvider.Setup(x => x.InternalEmailTemplateId).Returns(templateId);

            notificationConfigurationResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(notificationConfigurationProvider.Object);
        }

        [Fact]
        public async Task GivenARetrieveImageLink_WhenExecute_ThenSendInternalEmailUseCaseIsCalled()
        {
            var repair = new Repair
            {
                Id = "1AB2C3D4",
                ContactDetails = new RepairContactDetails { Value = "07465087654" },
                Address = new RepairAddress { Display = "address", LocationId = "uprn" },
                Description = new RepairDescription { Text = "repair description", Base64Image = "image", PhotoUrl = "x/Url.png" },
                Location = new RepairLocation { Value = "location" },
                Problem = new RepairProblem { Value = "problem" },
                Issue = new RepairIssue { Value = "issue" },
                SOR = "sor"
            };

            await systemUnderTest.Execute(repair);

            sendInternalEmailUseCase.Verify(x => x.Execute(personalisation, templateId), Times.Once);
        }
    }
}
