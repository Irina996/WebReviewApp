using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebRecomendationControlApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set;}

        [Display(Name = "Description")]
        public string Description { get; set;}

        public int GroupId { get; set; }

        [Display(Name = "Group")]
        public virtual ReviewGroup Group { get; set; }

        [Display(Name = "Tags")]
        public virtual List<ReviewTag> Tags { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Creator")]
        public virtual IdentityUser Creator { get; set; }

        [Display(Name = "Rating")]
        public int Rating { get; set; }
    }
}
