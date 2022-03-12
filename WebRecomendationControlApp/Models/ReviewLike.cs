using Microsoft.AspNetCore.Identity;

namespace WebRecomendationControlApp.Models
{
    public class ReviewLike
    {
        public int Id { get; set; }

        public int LikedReviewId { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
