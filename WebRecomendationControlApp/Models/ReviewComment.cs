using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebRecomendationControlApp.Models
{
    public class ReviewComment
    {
        public int Id { get; set; }

        [Display(Name = "Comment")]
        public string Text { get; set; }

        public virtual Review CommentedReview { get; set; }

        [Display(Name = "Commentator")]
        public virtual IdentityUser Commentator { get; set; }
    }
}
