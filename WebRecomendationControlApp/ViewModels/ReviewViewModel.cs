using System.ComponentModel.DataAnnotations;

namespace WebRecomendationControlApp.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "Title")]
        public string ReviewTitle { get; set; }

        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "Description")]
        public string ReviewDescription { get; set; }

        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "Group")]
        public int ReviewGroupId { get; set; }

        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "Tags")]
        public List<string> ReviewTags { get; set; }

        [Display(Name = "Image File")]
        public IFormFile[]? ImageFiles { get; set; }

        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "Rating")]
        public int ReviewRating { get; set; }

        public string ReviewCreatorName { get; set; }
    }
}
