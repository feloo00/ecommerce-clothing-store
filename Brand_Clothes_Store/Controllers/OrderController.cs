using Brand_Clothes_Store.Models;
using Brand_Clothes_Store.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brand_Clothes_Store.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .ToListAsync();
            return View(orders);
        }


        // GET: Orders/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User) // المستخدم اللي عامل الطلب
                .Include(o => o.OrderItems) // عناصر الطلب
                .ThenInclude(oi => oi.Variant) // الـ Variant المرتبط بكل عنصر
                .ThenInclude(v => v.Product) // المنتج اللي جواه الـ Variant
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        // GET: Orders/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "Name"); // لو User هو الجدول
            ViewBag.Products = _context.Products
                .Select(p => new { id = p.Id, name = p.Name })
                .ToList();
            return View();
        }


        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Order order)
        {
            // تأكيد إن البيانات اللي جت من الـ View صح
            if (!ModelState.IsValid)
            {
                Console.WriteLine("⚠️ ModelState invalid!");
                foreach (var err in ModelState)
                {
                    if (err.Value.Errors.Count > 0)
                        Console.WriteLine($"Field: {err.Key} => {err.Value.Errors[0].ErrorMessage}");
                }

                ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "Name", order.UserId);
                ViewBag.Products = _context.Products.Select(p => new { id = p.Id, name = p.Name }).ToList();
                return View(order);
            }

            try
            {
                // ✅ احسب الـ total من الـ items
                order.TotalPrice = order.OrderItems.Sum(i => i.TotalPrice);

                // ✅ أضف الـ Order أولًا علشان ياخد Id
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // ✅ اربط كل OrderItem بالـ Order الجديد
                foreach (var item in order.OrderItems)
                {
                    item.OrderId = order.Id;
                    _context.OrderItems.Add(item);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Order created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error while creating order: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong while saving the order.");

                ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "Name", order.UserId);
                ViewBag.Products = _context.Products.Select(p => new { id = p.Id, name = p.Name }).ToList();
                return View(order);
            }
        }



        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", order.UserId);
            // ✅ نمرر enum list للـ View
            ViewData["OrderStatusList"] = new SelectList(Enum.GetValues(typeof(OrderStatus)));

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CreatedAt,TotalPrice,orderStatus")] Order order)
        {
            if (id != order.Id)
                return NotFound();

            // 🧩 هنا تحط كود الـ debug
            foreach (var kvp in ModelState)
            {
                foreach (var error in kvp.Value.Errors)
                {
                    Console.WriteLine($"❌ Field: {kvp.Key} | Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get current logged-in user ID
                    var userId = _userManager.GetUserId(User);
                    order.UserId = userId;

                    _context.Update(order);
                    await _context.SaveChangesAsync();

                    // ✅ رسالة نجاح
                    TempData["SuccessMessage"] = "✅ Order updated successfully.";
                    return View(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Orders.Any(e => e.Id == order.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            // ❌ رسالة خطأ في الـ Validation
            TempData["ErrorMessage"] = "⚠️ Please correct the errors and try again.";
            ViewData["OrderStatusList"] = new SelectList(Enum.GetValues(typeof(OrderStatus)));
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", order.UserId);
            return View(order);
        }





        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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
                    v.Price
                })
                .ToList();

            return Json(variants);
        }

    }
}
