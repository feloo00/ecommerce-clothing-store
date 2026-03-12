using Brand_Clothes_Store.Models;
using Brand_Clothes_Store.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brand_Clothes_Store.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            // إزالة الـ Favorites عشان الفورم مش هيرسله
            ModelState.Remove("Favorites");

            // Validation: Category required
            if (product.CategoryId == null || product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Category must be selected");
            }

            if (!ModelState.IsValid)
            {
                // طبع كل الأخطاء في Console
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewData["Categories"] = _context.Categories.ToList();
                return View(product); // هرجع الفورم مع الأخطاء
            }

            try
            {
                product.CreatedAt = DateTime.Now;
                product.IsFavorite = false;
                product.Status = ProductStatus.Active;
                product.Favorites ??= new List<Favorite>();

                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // لو فيه Exception يبان في Console
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["Categories"] = _context.Categories.ToList();
                ModelState.AddModelError("", "Error saving product: " + ex.Message);
                return View(product);
            }
        }






        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }


        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            // حذف الـ Favorites من الـ ModelState عشان مش موجودة في الفورم
            ModelState.Remove("Favorites");

            if (ModelState.IsValid)
            {
                var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (existing == null) return NotFound();

                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.BasePrice = product.BasePrice;
                existing.ImageUrl = product.ImageUrl;
                existing.CategoryId = product.CategoryId;
                existing.Status = product.Status; // لو عايز تعدل Status

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }







        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
