using System.Threading.Tasks;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Domain
{
    public static class RepairExtensions
    {
        public static async Task<string> GetImageLink(this Repair repair, IRetrieveImageLinkUseCase retrieveImageLinkUseCase)
        {
            var imageLink = "None";
            if (!string.IsNullOrEmpty(repair.Description?.PhotoUrl))
            {
                await Task.Run(() =>
                {
                    imageLink = retrieveImageLinkUseCase.Execute(repair.Description?.PhotoUrl);
                });
            }
            return imageLink;
        }

    }
}
