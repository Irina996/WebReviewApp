using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.ViewModels
{
    public class ReviewListViewModel
    { 
        public IEnumerable<Review> Reviews { get; set; }   
        public SelectList Groups { get; set; }
        public string Name { get; set; }

    }
}
