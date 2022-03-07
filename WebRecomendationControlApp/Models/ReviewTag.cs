namespace WebRecomendationControlApp.Models
{
    public class ReviewTag
    {
        public int Id { get; set; }
        public string Tag { get; set; }

        public int ReviewId { get; set; }
    }
}
