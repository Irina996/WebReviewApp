using Microsoft.AspNetCore.Identity;

namespace WebRecomendationControlApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string Title { get; set;}

        public string Description { get; set;}

        public int GroupId { get; set; }

        public virtual ReviewGroup Group { get; set; }

        public virtual List<ReviewTag> Tags { get; set; }

        public string? ImageUrl { get; set; }

        public virtual IdentityUser Creator { get; set; }

        public int Rating { get; set; }
    }
}
