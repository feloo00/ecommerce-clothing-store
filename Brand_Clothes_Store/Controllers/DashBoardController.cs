using Brand_Clothes_Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brand_Clothes_Store.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashBoardController> _logger;

        public DashBoardController(AppDbContext context, ILogger<DashBoardController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var dashboard = new 
            {
                Categories = await _context.Categories.CountAsync(),
                Products = await _context.Products.CountAsync(),
                Variants = await _context.ProductVariants.CountAsync(),
                Orders = await _context.Orders.CountAsync(),
                OrderItems = await _context.OrderItems.CountAsync(),
                Users = await _context.Users.CountAsync()
            };
            ViewBag.Dashboard = dashboard;
            return View(dashboard);
        }
    }
}
