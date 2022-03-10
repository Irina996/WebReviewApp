using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRecomendationControlApp.Data;
using Microsoft.EntityFrameworkCore;
using WebRecomendationControlApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebRecomendationControlApp.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

        public CommentController(UserManager<IdentityUser> userManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index(int id=1)
        {
            ViewBag.ReviewId = id;
            var comments = _context.ReviewComments
                .Include(c => c.Commentator)
                .Include(c => c.CommentedReview)
                .Where(c => c.CommentedReview.Id == id)
                .OrderByDescending(c => c.Id);
            return View(comments);
        }

        [Route("Comment/Create")]
        public async Task<IActionResult> Create(int ReviewId, string CommentText)
        {
            var review = _context.Reviews
                .Where(r => r.Id == ReviewId)
                .FirstOrDefault();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ReviewComment comment = new ReviewComment
            {
                Text = CommentText,
                Commentator = user,
                CommentedReview = review
            };
            _context.ReviewComments.Add(comment);
            _context.SaveChanges();
            return StatusCode(200);
        }

        public PartialViewResult GetComments(int ReviewId)
        {
            ViewBag.ReviewId = ReviewId;
            var comments = _context.ReviewComments
                .Include(c => c.Commentator)
                .Include(c => c.CommentedReview)
                .Where(c => c.CommentedReview.Id == ReviewId)
                .OrderByDescending(c => c.Id);
            return PartialView("_GetComments", comments);
        }
    }
}
