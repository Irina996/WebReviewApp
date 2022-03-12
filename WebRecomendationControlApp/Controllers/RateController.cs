using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRecomendationControlApp.Data;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.Controllers
{
    public class RateController : Controller
    {

        ApplicationDbContext _context;

        public RateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void ResetRate(int starCount, int reviewId, string userId)
        {
            var rate = _context.ReviewRates
                .Where(r => r.UserId == userId && r.RatedReviewId == reviewId)
                .FirstOrDefault();
            if (rate != null)
            {
                rate.Rate = starCount;
                _context.Update(rate);
            }
            else
            {
                rate = new ReviewRate
                {
                    Rate = starCount,
                    RatedReviewId = reviewId,
                    UserId = userId
                };
                _context.Add(rate);
            }
            _context.SaveChanges();
        }
    }
}
