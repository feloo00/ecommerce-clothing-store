using Brand_Clothes_Store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Brand_Clothes_Store.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly AppDbContext _context;

        public BaseController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.FavCount = _context.Favorites.Count(f => f.UserId == userId);
            }
            else
            {
                ViewBag.FavCount = 0;
            }

            base.OnActionExecuting(context);
        }
    }

}
