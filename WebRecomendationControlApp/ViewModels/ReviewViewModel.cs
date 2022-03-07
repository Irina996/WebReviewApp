using System.ComponentModel.DataAnnotations;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.ViewModels
{
    public class ReviewViewModel
    {
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
        [Display(Name = "Tags(put each tag in different field)")]
        public List<string> ReviewTags { get; set; }

        /*[Display(Name = "Images")]
        public List<string>? ImageUrl { get; set; }*/

        [Required]
        [Display(Name = "Rating(0-5)")]
        public int ReviewRating { get; set; }

    }
}
