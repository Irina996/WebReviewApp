using Microsoft.AspNetCore.Identity;

namespace WebRecomendationControlApp.Models
{
    public class ReviewComment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual Review CommentedReview { get; set; }

        public virtual IdentityUser Commentator { get; set; }
    }
}
