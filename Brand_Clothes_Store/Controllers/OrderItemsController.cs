using Brand_Clothes_Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brand_Clothes_Store.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orderItems = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Variant)
                .ThenInclude(v => v.Product)
                .ToListAsync();

            return View(orderItems);
        }

        // GET: OrderItems/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Orders = _context.Orders.ToList();
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Variants = _context.ProductVariants.Include(v => v.Product).ToList();
            return View();
        }

        // AJAX: Get Variants for selected product
        [Authorize(Roles = "Admin")]
        public JsonResult GetVariantsByProduct(int productId)
        {
            var variants = _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .Select(v => new
                {
                    v.Id,
                    v.Color,
                    v.Size,
                    v.Price,
                    v.Stock
                })
                .ToList();

            return Json(variants);
        }
        [Authorize(Roles = "Admin")]
        // POST: OrderItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                orderItem.TotalPrice = orderItem.UnitPrice * orderItem.Quantity;
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Orders = _context.Orders.ToList();
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Variants = _context.ProductVariants.Include(v => v.Product).ToList();

            return View(orderItem);
        }
    }
}
