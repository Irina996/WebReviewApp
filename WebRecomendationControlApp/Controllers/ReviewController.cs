using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRecomendationControlApp.Data;
using WebRecomendationControlApp.Models;
using WebRecomendationControlApp.ViewModels;

namespace WebRecomendationControlApp.Controllers
{
    public class ReviewController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

        public ReviewController(UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            var reviews = _context.Reviews.Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .OrderByDescending(x => x.Id);
            return View(reviews);
        }

        public IActionResult Edit(int id)
        {
            SelectList groups = new SelectList(_context.reviewGroups, "Id", "Name");
            ViewBag.ReviewGroups = groups;
            var review = _context.Reviews.Where(r => r.Id == id)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .FirstOrDefault();
            List<string> tagList = new List<string>();
            foreach (var tag in review.Tags)
            {
                tagList.Add(tag.Tag);
            }
            ReviewViewModel model = new ReviewViewModel
            {
                Id = review.Id,
                ReviewCreatorName = review.Creator.UserName,
                ReviewDescription = review.Description,
                ReviewTitle = review.Title,
                ReviewTags = tagList,
                ReviewGroupId = review.GroupId,
                ReviewRating = review.Rating
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReviewViewModel model)
        {
            DeleteTags(model.Id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Review existingReview = _context.Reviews.Where(r => r.Id == model.Id)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .First();
            var review = getReviewFromModel(model, user);
            _context.Entry(existingReview).CurrentValues.SetValues(review);
            foreach (var tag in review.Tags)
            {
                tag.ReviewId = review.Id;
                var existingTag = existingReview.Tags
                    .FirstOrDefault(t => t.Tag == tag.Tag);
                if (existingTag == null)
                {
                    existingReview.Tags.Add(tag);
                }
                else
                {
                    tag.Id = existingTag.Id;
                    _context.Entry(existingTag).CurrentValues.SetValues(tag);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Details", new { review.Id });
        }

        private void DeleteTags(int id)
        {
            var tags = _context.reviewTags.Where(t => t.ReviewId == id);
            foreach (var tag in tags)
            {
                _context.reviewTags.Remove(tag);
            }
            _context.SaveChanges();
        }

        [Authorize]
        public IActionResult Create()
        {
            SelectList groups = new SelectList(_context.reviewGroups, "Id", "Name");
            ViewBag.ReviewGroups = groups;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Review review = getReviewFromModel(model, user);
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return RedirectToAction("List", "Review");
        }

        private Review getReviewFromModel(ReviewViewModel model, IdentityUser user)
        {
            ReviewGroup group = _context.reviewGroups.Find(model.ReviewGroupId);
            List<ReviewTag> tagList = new List<ReviewTag>();
            foreach (var tag in model.ReviewTags)
            {
                if (tag != null)
                    tagList.Add(new ReviewTag { Tag = tag });
            }
            Review review = new Review
            {
                Id = model.Id,
                Title = model.ReviewTitle,
                Description = model.ReviewDescription,
                GroupId = model.ReviewGroupId,
                Group = group,
                Tags = tagList,
                Creator = user,
                Rating = model.ReviewRating
            };
            return review;
        }

        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Where(r => r.Id == id)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .FirstOrDefault();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = _context.Reviews.Where(r => r.Id == id)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .FirstOrDefault();
            deleteLikes(review.Id);
            deleteRates(review.Id);
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        public void deleteLikes(int reviewId)
        {
            var likes = _context.ReviewLikes
                .Where(l => l.LikedReviewId == reviewId);
            foreach (var like in likes)
                _context.ReviewLikes.Remove(like);
            _context.SaveChanges();
        }

        public void deleteRates(int reviewId)
        {
            var rates = _context.ReviewRates
                .Where(r => r.RatedReviewId == reviewId);
            foreach (var rate in rates)
                _context.ReviewRates.Remove(rate);
            _context.SaveChanges();
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var review = _context.Reviews.Where(r => r.Id == id)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .FirstOrDefault();
            ViewBag.AllowEdit = false;
            if (review.Creator.UserName == this.User.Identity.Name)
            {
                ViewBag.AllowEdit = true;
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.UserId = user.Id;
            var like = _context.ReviewLikes
                .Where(l => l.User == user && l.LikedReviewId == review.Id)
                .FirstOrDefault();
            ViewBag.Liked = false;
            if (like != null)
            {
                ViewBag.Liked = true;
            }
            ViewBag.StarCount = _context.ReviewRates
                .Where(r => r.RatedReviewId == id && r.UserId == user.Id)
                .FirstOrDefault()?.Rate;
            if (ViewBag.StarCount == null)
                ViewBag.StarCount = 0;
            return View(review);
        }

        public IActionResult UserReviews()
        {
            string user = this.User.Identity.Name;
            var reviews = _context.Reviews.Where(x => x.Creator.UserName == user)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator);
            return View(reviews);
        }
    }
}
