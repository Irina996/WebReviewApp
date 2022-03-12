using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRecomendationControlApp.Data;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.Controllers
{
    public class LikeController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

        public LikeController(UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async void ResetLike(int reviewId, string userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var like = _context.ReviewLikes
                .Where(l => l.LikedReviewId == reviewId && l.User == user)
                .FirstOrDefault();
            if (like == null)
            {
                like = new ReviewLike { LikedReviewId = reviewId, User = user };
                _context.Add(like);
            }
            else
            {
                _context.Remove(like);
            }
            _context.SaveChanges();
        }
    }
}
