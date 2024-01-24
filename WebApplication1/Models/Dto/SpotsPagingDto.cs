namespace WebApplication1.Models.Dto
{
    public class SpotsPagingDto
    {
        public int TotalPages { get; set; }

        public List<SpotImagesSpot>? SpotsResult { get; set; }
    }
}
