using Brand_Clothes_Store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Brand_Clothes_Store.Models;

public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var claims = await _userManager.GetClaimsAsync(user);

        ViewBag.GoogleName = claims.FirstOrDefault(c => c.Type == "name")?.Value;
        ViewBag.GooglePicture = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

        return View(user);
    }
}
