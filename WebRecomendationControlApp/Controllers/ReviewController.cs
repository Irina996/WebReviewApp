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

        public ReviewController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
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
            var reviews = _context.Reviews.Include(x => x.Group).Include(x => x.Tags).Include(x => x.Creator);
            return View(reviews);
        }

        public IActionResult Edit(int id)
        {
            SelectList groups = new SelectList(_context.reviewGroups, "Id", "Name");
            ViewBag.ReviewGroups = groups;
            var review = _context.Reviews.Where(r => r.Id == id).Include(x => x.Group)
                .Include(x => x.Tags).Include(x => x.Creator).FirstOrDefault();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Review review)
		{
            _context.Entry(review).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("List");
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
            if (model.ReviewTitle == null)
                ModelState.AddModelError("ReviewTitle", "Tittle is required");
            if (model.ReviewDescription == null)
                ModelState.AddModelError("ReviewDescription", "Description is required");
            if (model.ReviewTags[0] == null)
                ModelState.AddModelError("ReviewTags", "Input tag");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                ReviewGroup group = _context.reviewGroups.Find(model.ReviewGroupId);
                List<ReviewTag> tagList = new List<ReviewTag>();
                foreach (var tag in model.ReviewTags)
                {
                    if (tag != null)
                        tagList.Add(new ReviewTag { Tag = tag });
                }
                /*var tagList = getTagList(model.ReviewTags);*/
                Review review = new Review
                {
                    Title = model.ReviewTitle,
                    Description = model.ReviewDescription,
                    Group = group,
                    Tags = tagList,
                    Creator = user,
                    Rating = model.ReviewRating
                };
                _context.Reviews.Add(review);
                _context.SaveChanges();
                return RedirectToAction("List", "Review");
            }
            else
            {
                SelectList groups = new SelectList(_context.reviewGroups, "Id", "Name");
                ViewBag.ReviewGroups = groups;
                return View();
            }
        }

        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Where(r => r.Id == id).Include(x => x.Group)
                .Include(x => x.Tags).Include(x => x.Creator).FirstOrDefault();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = _context.Reviews.Where(r => r.Id == id).Include(x => x.Group)
                .Include(x => x.Tags).Include(x => x.Creator).FirstOrDefault();
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        public IActionResult Details(int id)
        {
            ViewBag.AllowEdit = false;
            var review = _context.Reviews.Where(r => r.Id == id).Include(x => x.Group)
                .Include(x => x.Tags).Include(x => x.Creator).FirstOrDefault();
            if (review.Creator.UserName == this.User.Identity.Name)
            {
                ViewBag.AllowEdit = true;
            }
            return View(review);
        }

        public IActionResult UserReviews()
        {
            string user = this.User.Identity.Name;
            var reviews = _context.Reviews.Include(x => x.Group).Include(x => x.Tags)
                .Include(x => x.Creator).Where(x => x.Creator.UserName == user);
            return View(reviews);
        }
    }
}
