using Brand_Clothes_Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brand_Clothes_Store.Controllers
{
    public class ProductVariantsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductVariantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProductVariants
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .ToListAsync();

            return View(variants);
        }

        // GET: ProductVariants/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductVariants == null)
                return NotFound();

            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (variant == null)
                return NotFound();

            return View(variant);
        }

        // GET: ProductVariants/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Products = new SelectList(_context.Products.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ProductVariant variant)
        {
            Console.WriteLine(">>> POST request received!");

            if (ModelState.IsValid)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == variant.ProductId);
                if (product != null)
                {
                    variant.Price = product.BasePrice; // automatic price
                }

                _context.Add(variant);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("ModelState invalid!");
            ViewBag.Products = new SelectList(_context.Products.ToList(), "Id", "Name");

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"❌ Validation Error: {error.ErrorMessage}");
            }


            return View(variant);
        }





        // GET: ProductVariants/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            ViewData["Products"] = _context.Products.ToList();
            return View(variant);
        }

        // POST: ProductVariants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ProductVariant variant)
        {
            if (id != variant.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ProductVariants.Any(e => e.Id == variant.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Products"] = _context.Products.ToList();
            return View(variant);
        }

        // GET: ProductVariants/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (variant == null) return NotFound();

            return View(variant);
        }

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant != null)
            {
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
