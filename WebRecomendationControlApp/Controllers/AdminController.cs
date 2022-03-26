using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRecomendationControlApp.Data;

namespace WebRecomendationControlApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        UserManager<IdentityUser> _userManager;
        IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _config = configuration;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await createRoleAdmin();
            IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (User.IsInRole("admin"))
            {
                return View(getUsers());
            }
            var user = await _userManager.FindByEmailAsync(_config.GetSection("AdminEmail").Value);
            if (currentUser == user)
            {
                await _userManager.AddToRoleAsync(user, "admin");
                return View(getUsers());
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private async Task createRoleAdmin()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            }
        }

        private IEnumerable<IdentityUser> getUsers()
        {
            return _context.Users.ToList();
        }
    }
}
