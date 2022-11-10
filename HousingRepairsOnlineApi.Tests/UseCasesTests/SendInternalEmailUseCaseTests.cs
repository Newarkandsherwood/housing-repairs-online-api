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

        public SendInternalEmailUseCaseTests()
        {
            govNotifyGatewayMock = new Mock<INotifyGateway>();
            systemUnderTest = new SendInternalEmailUseCase(govNotifyGatewayMock.Object, "dr.who@tardis.com");
        }

//         public static IEnumerable<object[]> InvalidBookingRefArgumentTestData()
//         {
//             yield return new object[] { new ArgumentNullException(), null };
//             yield return new object[] { new ArgumentException(), "" };
//         }
//
//         [Theory]
//         [MemberData(nameof(InvalidBookingRefArgumentTestData))]
// #pragma warning disable xUnit1026
//         public void GivenAnInvalidBookingRef_WhenExecute_ThenExceptionIsThrown<T>(T exception, string bookingRef) where T : Exception
// #pragma warning restore xUnit1026
//         {
//             var bookingRefPersonalisation = new Dictionary<string, dynamic>
//             {
//                 {"repair_ref", bookingRef},
//                 {"uprn", "uprn"},
//                 {"address", "address"},
//                 {"sor", "SOR"},
//                 {"repair_desc", "Description"},
//                 {"contact_no", "contact_no"}
//             };
//             //Act
//             Action act = () => systemUnderTest.Execute(
//                 bookingRefPersonalisation, "image", "11");
//
//             //Assert
//             act.Should().ThrowExactly<T>();
//         }
//
//         public static IEnumerable<object[]> InvalidAddressArgumentTestData()
//         {
//             yield return new object[] { new ArgumentNullException(), null };
//             yield return new object[] { new ArgumentException(), "" };
//         }
//
//         [Theory]
//         [MemberData(nameof(InvalidAddressArgumentTestData))]
// #pragma warning disable xUnit1026
//         public void GivenAnInvalidAddress_WhenExecute_ThenExceptionIsThrown<T>(T exception, string address) where T : Exception
// #pragma warning restore xUnit1026
//         {
//             var bookingRefPersonalisation = new Dictionary<string, dynamic>
//             {
//                 {"repair_ref", "bookingRef"},
//                 {"uprn", "uprn"},
//                 {"address", "address"},
//                 {"sor", "SOR"},
//                 {"repair_desc", "Description"},
//                 {"contact_no", "contact_no"}
//             };
//             //Act
//             Action act = () => systemUnderTest.Execute(
//                 "bookingRef", address, "sor", "uprn", "repair description", "contact no", "image"
//                 );
//
//             //Assert
//             act.Should().ThrowExactly<T>();
//         }
//
//         public static IEnumerable<object[]> InvalidSorArgumentTestData()
//         {
//             yield return new object[] { new ArgumentNullException(), null };
//             yield return new object[] { new ArgumentException(), "" };
//         }
//
//         [Theory]
//         [MemberData(nameof(InvalidSorArgumentTestData))]
// #pragma warning disable xUnit1026
//         public void GivenAnInvalidSor_WhenExecute_ThenExceptionIsThrown<T>(T exception, string sor) where T : Exception
// #pragma warning restore xUnit1026
//         {
//             //Act
//             Action act = () => systemUnderTest.Execute(
//                 "bookingRef", "address", sor, "uprn", "repair description", "contact no", "image"
//             );
//
//             //Assert
//             act.Should().ThrowExactly<T>();
//         }
//
//         public static IEnumerable<object[]> InvalidUprnArgumentTestData()
//         {
//             yield return new object[] { new ArgumentNullException(), null };
//             yield return new object[] { new ArgumentException(), "" };
//         }
//
//         [Theory]
//         [MemberData(nameof(InvalidUprnArgumentTestData))]
// #pragma warning disable xUnit1026
//         public void GivenAnInvalidUprn_WhenExecute_ThenExceptionIsThrown<T>(T exception, string uprn) where T : Exception
// #pragma warning restore xUnit1026
//         {
//             //Act
//             Action act = () => systemUnderTest.Execute(
//                 "bookingRef", "address", "sor", uprn, "repair description", "07465087654", "image"
//             );
//
//             //Assert
//             act.Should().ThrowExactly<T>();
//         }
//
//         public static IEnumerable<object[]> InvalidRepairDescriptionArgumentTestData()
//         {
//             yield return new object[] { new ArgumentNullException(), null };
//             yield return new object[] { new ArgumentException(), "" };
//         }
//
//         [Theory]
//         [MemberData(nameof(InvalidRepairDescriptionArgumentTestData))]
// #pragma warning disable xUnit1026
//         public void GivenAnInvalidRepairDescription_WhenExecute_ThenExceptionIsThrown<T>(T exception, string repairDescription) where T : Exception
// #pragma warning restore xUnit1026
//         {
//             //Act
//             Action act = () => systemUnderTest.Execute(
//                 "bookingRef", "address", "sor", "uprn", repairDescription, "07465087654", "image"
//             );
//
//             //Assert
//             act.Should().ThrowExactly<T>();
//         }
//
//         public static IEnumerable<object[]> InvalidContactNumberArgumentTestData()
//         {
//             yield return new object[] { new ArgumentNullException(), null };
//             yield return new object[] { new ArgumentException(), "" };
//         }
//
//         [Theory]
//         [MemberData(nameof(InvalidContactNumberArgumentTestData))]
// #pragma warning disable xUnit1026
//         public void GivenAnInvalidContactNumber_WhenExecute_ThenExceptionIsThrown<T>(T exception, string contactNumber) where T : Exception
// #pragma warning restore xUnit1026
//         {
//             //Act
//             Action act = () => systemUnderTest.Execute(
//                 "bookingRef", "address", "sor", "uprn", "repair description", contactNumber, "image"
//             );
//
//             //Assert
//             act.Should().ThrowExactly<T>();
//         }

        public static IEnumerable<object[]> InvalidImageArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
        }

        [Fact]
        public void GivenNoImage_WhenExecute_ThenGovNotifyGateWayIsCalled()
        {
            //Act
            const string Base64Img = "";

            systemUnderTest.Execute(personalisation, Base64Img, "07465087654");

            //Assert
            govNotifyGatewayMock.Verify(x => x.SendEmail("dr.who@tardis.com", "templateId", It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
        }

        [Fact]
        public void GivenValidParameters_WhenExecute_ThenGovNotifyGateWayIsCalled()
        {
            //Act
            const string Base64Img = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw1AUhU9TpaJVB4uIOGSoThZERRy1CkWoEGqFVh1MXvojNGlIUlwcBdeCgz+LVQcXZ10dXAVB8AfE0clJ0UVKvC8ptIjxwuN9nHfP4b37AKFWYprVNgZoum2mEnExk10RQ68IIIQe9KNLZpYxK0lJ+NbXPXVT3cV4ln/fn9Wt5iwGBETiGWaYNvE68dSmbXDeJ46woqwSnxOPmnRB4keuKx6/cS64LPDMiJlOzRFHiMVCCystzIqmRjxJHFU1nfKFjMcq5y3OWqnCGvfkLwzn9OUlrtMaQgILWIQEEQoq2EAJNmK066RYSNF53Mc/6Polcink2gAjxzzK0CC7fvA/+D1bKz8x7iWF40D7i+N8DAOhXaBedZzvY8epnwDBZ+BKb/rLNWD6k/RqU4seAb3bwMV1U1P2gMsdYODJkE3ZlYK0hHweeD+jb8oCfbdA56o3t8Y5Th+ANM0qeQMcHAIjBcpe83l3R+vc/u1pzO8H+I9yds6VEEcAAAAJcEhZcwAALiMAAC4jAXilP3YAAAAHdElNRQfmAQcOFjXsyx/IAAAAGXRFWHRDb21tZW50AENyZWF0ZWQgd2l0aCBHSU1QV4EOFwAAAAxJREFUCNdj0HiTBAACtgF3wqeo5gAAAABJRU5ErkJggg==";

            systemUnderTest.Execute(personalisation, Base64Img, "07465087654");

            //Assert
            govNotifyGatewayMock.Verify(x => x.SendEmail("dr.who@tardis.com", "templateId", It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
        }

    }
}
