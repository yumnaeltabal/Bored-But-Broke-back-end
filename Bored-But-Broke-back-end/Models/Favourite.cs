namespace Bored_But_Broke_back_end.Models
{
    public class Favourite
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public string PlaceId { get; set; } = string.Empty;
        public Place Place { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
