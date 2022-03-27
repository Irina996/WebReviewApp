using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebRecomendationControlApp.Data;
using WebRecomendationControlApp.Models;
using WebRecomendationControlApp.ViewModels;

namespace WebRecomendationControlApp.Controllers
{
    public class ReviewController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ReviewController(UserManager<IdentityUser> userManager,
            ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(List<int>? ids = null, bool empty = false)
        {
            if (!empty && ids.Count == 0)
            {
                var revs = _context.Reviews.Include(x => x.Group)
                    .Include(x => x.Tags)
                    .Include(x => x.Creator)
                    .OrderByDescending(x => x.Id).ToList();
                return View(revs);
            }
            List<Review> reviews = new List<Review>();
            foreach (var id in ids)
            {
                var rev = GetReview(id);
                reviews.Add(rev);
            }
            return View(reviews);
        }

        private Review? GetReview(int id)
        {
            return _context.Reviews.Where(r => r.Id == id)
                .Include(x => x.Group)
                .Include(x => x.Tags)
                .Include(x => x.Creator)
                .FirstOrDefault();
        }

        private List<string> getTagList(List<ReviewTag> reviewTags)
        {
            List<string> tagList = new List<string>();
            foreach (var tag in reviewTags)
            {
                tagList.Add(tag.Tag);
            }
            return tagList;
        }
        public IActionResult Edit(int id)
        {
            ViewBag.ReviewGroups = new SelectList(_context.reviewGroups, "Id", "Name"); ;
            var review = GetReview(id);
            List<string> tagList = getTagList(review.Tags);
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

        private void ChangeReviewTags(int reviewId, List<ReviewTag> reviewTags, List<ReviewTag> existingTags)
        {
            foreach (var tag in reviewTags)
            {
                tag.ReviewId = reviewId;
                var existingTag = existingTags
                    .FirstOrDefault(t => t.Tag == tag.Tag && t.ReviewId == reviewId);
                if (existingTag == null)
                {
                    existingTags.Add(tag);
                }
                else
                {
                    tag.Id = existingTag.Id;
                    _context.Entry(existingTag).CurrentValues.SetValues(tag);
                }
            }
            for (int i = 0; i < existingTags.Count; i++)
            {
                var existingTag = reviewTags
                    .FirstOrDefault(t => t.Tag == existingTags[i].Tag && t.ReviewId == reviewId);
                if (existingTag == null)
                {
                    existingTags.Remove(existingTags[i]);
                    i--;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReviewViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Review existingReview = GetReview(model.Id);
            var review = getReviewFromModel(model, user);
            review.ImageUrl = user.Id + "/" + model.Id;
            _context.Entry(existingReview).CurrentValues.SetValues(review);
            ChangeReviewTags(review.Id, review.Tags, existingReview.Tags);
            if (model.ImageFiles != null)
            {
                await SaveImages(model.ImageFiles, review.Creator.Id, review.Id);
            }
            _context.SaveChanges();
            return RedirectToAction("Details", new { review.Id });
        }

        [Authorize]
        public async Task<IActionResult> Create(string userId)
        {
            if (userId == null)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                userId = user.Id;
            }
            ViewBag.userId = userId;
            SelectList groups = new SelectList(_context.reviewGroups, "Id", "Name");
            ViewBag.ReviewGroups = groups;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewViewModel model, string userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).First();
            Review review = getReviewFromModel(model, user);
            _context.Reviews.Add(review);
            _context.SaveChanges();
            if (model.ImageFiles != null)
            {
                await SaveImages(model.ImageFiles, review.Creator.Id, review.Id);
                review.ImageUrl = review.Creator.Id + "/" + review.Id;
                _context.SaveChanges();
            }
            return RedirectToAction("List", "Review");
        }

        private async Task SaveImages(IFormFile[] images, string userId, int reviewId)
        {
            string imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
            imagesPath = Path.Combine(
                    imagesPath,
                    userId,
                    reviewId.ToString()
                );
            Directory.CreateDirectory(imagesPath);
            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    string filePath = Path.Combine(imagesPath, image.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }
            }
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
            var review = GetReview(id);
            ViewBag.Images = GetImages(review.ImageUrl);
            ViewBag.Description = getMarkdownDescription(review.Description);
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = GetReview(id);
            deleteLikes(review.Id);
            deleteRates(review.Id);
            deleteImages(review.ImageUrl);
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        private void deleteImages(string UrlPath)
        {
            if (UrlPath != null)
            {
                string imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
                imagesPath = Path.Combine(imagesPath, UrlPath);
                if (Directory.Exists(imagesPath))
                {
                    var dir = new DirectoryInfo(imagesPath);
                    dir.Delete(true);
                }
            }
        }

        private void deleteLikes(int reviewId)
        {
            var likes = _context.ReviewLikes
                .Where(l => l.LikedReviewId == reviewId);
            foreach (var like in likes)
                _context.ReviewLikes.Remove(like);
            _context.SaveChanges();
        }

        private void deleteRates(int reviewId)
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
            var review = GetReview(id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.AllowEdit = IsAllowEdit(user, review.Creator.Id);
            ViewBag.UserId = user.Id;
            ViewBag.Liked = IsLiked(user, review.Id);
            ViewBag.StarCount = GetStarCount(user, review.Id);
            ViewBag.Images = GetImages(review.ImageUrl);
            ViewBag.Rating = GetRating(review.Id);
            ViewBag.Description = getMarkdownDescription(review.Description);
            return View(review);
        }

        private string getMarkdownDescription(string description)
		{
            // Uses all extensions except the BootStrap, Emoji, SmartyPants and soft line as hard line breaks extensions.
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(description, pipeline);
        }

        private int GetRating(int reviewId)
        {
            var rates = _context.ReviewRates.Where(r => r.RatedReviewId == reviewId).ToList();
            if (rates.Count == 0)
                return 0;
            int rating = 0;
            foreach (var rate in rates)
            {
                rating += rate.Rate;
            }
            return (int)rating / rates.Count();
        }

        private FileInfo[] GetImages(string imageUrl)
        {
            if (imageUrl == null)
                return null;
            string imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
            imagesPath = Path.Combine(imagesPath, imageUrl);
            if (Directory.Exists(imagesPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(imagesPath);
                FileInfo[] imageFiles = directoryInfo.GetFiles();
                return imageFiles;
            }
            return null;
        }

        private bool IsAllowEdit(IdentityUser user, string creatorId)
        {
            if (creatorId == user.Id)
            {
                return true;
            }
            return false;
        }

        private bool IsLiked(IdentityUser user, int reviewId)
        {
            var like = _context.ReviewLikes
                .Where(l => l.User == user && l.LikedReviewId == reviewId)
                .FirstOrDefault();
            if (like != null)
            {
                return true;
            }
            return false;
        }

        private int GetStarCount(IdentityUser user, int reviewId)
        {
            var starCount = _context.ReviewRates
                .Where(r => r.RatedReviewId == reviewId && r.UserId == user.Id)
                .FirstOrDefault()?.Rate;
            if (starCount == null)
                starCount = 0;
            return (int)starCount;
        }

        private IEnumerable<Review> GetUserReviews(string creatorId, int? group, string name)
        {
            IQueryable<Review> reviews = _context.Reviews.Where(x => x.Creator.Id == creatorId)
                .Include(x => x.Group)
                .OrderByDescending(x => x.Id);
            if (group != null && group != 0)
            {
                reviews = reviews.Where(r => r.GroupId == group);
            }
            if (!String.IsNullOrEmpty(name))
            {
                reviews = reviews.Where(p => p.Title.Contains(name));
            }
            return reviews;
        }

        public async Task<IActionResult> UserPage(string userId, int? group, string name)
        {
            if (userId == null)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                userId = user.Id;
            }
            ViewBag.userId = userId;
            var reviews = GetUserReviews(userId, group, name);

            List<ReviewGroup> reviewGroups = _context.reviewGroups.ToList();
            reviewGroups.Insert(0, new ReviewGroup { Name = "Все", Id = 0 });

            ReviewListViewModel viewModel = new ReviewListViewModel
            {
                Reviews = reviews.ToList(),
                Groups = new SelectList(reviewGroups, "Id", "Name"),
                Name = name
            };

            return View(viewModel);
        }

        public IActionResult ReviewsSearch(string search)
        {
            var ids = getResultIds(search);
            var empty = false;
            if (ids.Count == 0)
            {
                empty = true;
            }
            return RedirectToAction("List", new { ids, empty });
        }

        private List<int> getResultIds(string search)
        {
            var reviewIds1 = _context.Reviews
                .Where(r => EF.Functions.FreeText(r.Description, search))
                .Union(_context.Reviews
                .Where(r => EF.Functions.FreeText(r.Title, search)))
                .Select(r => r.Id);
            var reviewIds2 = _context.Reviews.Join(_context.ReviewComments
                .Where(c => EF.Functions.FreeText(c.Text, search)),
                r => r.Id,
                c => c.CommentedReview.Id,
                (r, c) => new
                {
                    r.Id
                }).Select(r => r.Id);
            return reviewIds1.Concat(reviewIds2).ToHashSet().ToList();
        }
    }
}
