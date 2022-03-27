using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.ViewModels
{
    public class ReviewListViewModel
    { 
        public IEnumerable<Review> Reviews { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
