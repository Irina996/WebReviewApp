namespace WebRecomendationControlApp.Models
{
    public class ReviewRate
    {
        public int Id { get; set; }

        public int Rate { get; set; }

        public string UserId { get; set; }

        public int RatedReviewId { get; set; } 
    }
}
