
namespace HousingRepairsOnlineApi.Domain
{
    public class RepairDescriptionRequest
    {
        public string Base64Img { get; set; }
        public string FileExtension { get; set; }
        public string Text { get; set; }
        public string LocationText { get; set; }
    }
}
