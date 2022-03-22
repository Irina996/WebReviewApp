using System.ComponentModel.DataAnnotations;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string ReviewTitle { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string ReviewDescription { get; set; }

        [Required]
        [Display(Name = "Group")]
        public int ReviewGroupId { get; set; }

        [Required]
        [Display(Name = "Tags")]
        public List<string> ReviewTags { get; set; }

        [Display(Name = "Image File")]
        public IFormFile[]? ImageFiles { get; set; }

        [Required]
        [Display(Name = "Rating")]
        public int ReviewRating { get; set; }

        public string ReviewCreatorName { get; set; }
    }
}
