using Brand_Clothes_Store.Controllers;
using Brand_Clothes_Store.Helpers;
using Brand_Clothes_Store.Models;
using Brand_Clothes_Store.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class ShopController : BaseController
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShopController(AppDbContext context, UserManager<ApplicationUser> userManager) : base(userManager, context)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Browse()
    {
        var userId = _userManager.GetUserId(User);

        // Get all products
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        // Get user's favorite product IDs
        var favoriteIds = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();

        // Mark products as favorite
        foreach (var product in products)
        {
            product.IsFavorite = favoriteIds.Contains(product.Id);
        }

        return View(products);
    }


    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants) 
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }


    [HttpPost]
    public IActionResult AddToCart(int variantId)
    {
        var variant = _context.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefault(v => v.Id == variantId);

        if (variant == null) return NotFound();

        // نجيب الكارت من السيشن
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        // نشوف لو موجود نزود الكمية
        var existing = cart.FirstOrDefault(c => c.VariantId == variantId);
        if (existing != null)
            existing.Quantity++;
        else
            cart.Add(new CartItemDto
            {
                VariantId = variant.Id,
                ProductName = variant.Product.Name,
                Price = variant.Price ?? 0,
                ImageUrl = variant.ImageUrl
            });


        // نرجع نحفظ الكارت
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Cart");
    }

    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        // نحولها لـ OrderItem عشان الـ View
        var items = cart.Select(c => new OrderItem
        {
            VariantId = c.VariantId,
            Quantity = c.Quantity,
            UnitPrice = c.Price ?? 0,
            TotalPrice = (c.Price ?? 0) * c.Quantity,
            Variant = _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefault(v => v.Id == c.VariantId)!
        }).ToList();

        return View(items);
    }


    [HttpPost]
    public IActionResult PlaceOrder()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart");
        if (cart == null || !cart.Any())
            return RedirectToAction("Cart");

        var order = new Order
        {
            CreatedAt = DateTime.Now,
            TotalPrice = cart.Sum(c => c.TotalPrice),
            OrderItems = cart
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        HttpContext.Session.Remove("Cart"); // نمسح الكارت بعد الحفظ
        return RedirectToAction("OrderConfirmation", new { id = order.Id });
    }

    [HttpGet]
    public IActionResult GetCartSummary()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();
        var total = cart.Sum(c => (c.Price ?? 0) * c.Quantity);

        return Json(new { total });
    }



    [HttpPost]
    public IActionResult IncreaseQuantity(int variantId)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        var item = cart.FirstOrDefault(c => c.VariantId == variantId);
        if (item != null)
        {
            item.Quantity++;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult DecreaseQuantity(int variantId)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        var item = cart.FirstOrDefault(c => c.VariantId == variantId);
        if (item != null)
        {
            if (item.Quantity > 1)
                item.Quantity--;
            else
                cart.Remove(item);

            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult RemoveItem(int variantId)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        cart.RemoveAll(c => c.VariantId == variantId);

        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return Json(new { success = true });
    }


    [HttpPost]
    public async Task<IActionResult> OrderNow()
    {
        // هات المستخدم الحالي
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account"); // مش داخل بحسابه

        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart");

        if (cart == null || !cart.Any())
            return RedirectToAction("Cart");

        // إنشاء طلب جديد
        var order = new Order
        {
            UserId = user.Id,
            CreatedAt = DateTime.Now,
            Payment = PaymentMethod.CreditCard,
            orderStatus = OrderStatus.Processing,
            TotalPrice = cart.Sum(c => (c.Price ?? 0) * c.Quantity),
            OrderItems = new List<OrderItem>()
        };

        foreach (var item in cart)
        {
            var orderItem = new OrderItem
            {
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                UnitPrice = item.Price ?? 0,
                TotalPrice = (item.Price ?? 0) * item.Quantity
            };
            order.OrderItems.Add(orderItem);
        }

        _context.Orders.Add(order);
        _context.SaveChanges();

        // بعد الحفظ نفضي الكارت
        HttpContext.Session.Remove("Cart");

        TempData["Message"] = "✅ Your order has been placed successfully!";
        return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
    }

    [HttpPost]
    public IActionResult CashOnDelivery()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return RedirectToAction("Login", "Account"); // مش داخل بحسابه
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemDto>>("Cart");

        if (cart == null || !cart.Any())
            return RedirectToAction("Cart");

        var order = new Order
        {
            UserId=userId,
            CreatedAt = DateTime.Now,
            Payment = PaymentMethod.Cash,
            orderStatus = OrderStatus.Processing,
            TotalPrice = cart.Sum(c => (c.Price ?? 0) * c.Quantity),
            OrderItems = new List<OrderItem>()
        };

        foreach (var item in cart)
        {
            var orderItem = new OrderItem
            {
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                UnitPrice = item.Price ?? 0,
                TotalPrice = (item.Price ?? 0) * item.Quantity
            };
            order.OrderItems.Add(orderItem);
        }

        _context.Orders.Add(order);
        _context.SaveChanges();

        HttpContext.Session.Remove("Cart");

        TempData["Message"] = "🛍️ Order placed successfully — you'll pay on delivery.";
        return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
    }

    public IActionResult OrderConfirmation(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Variant)
            .ThenInclude(v => v.Product)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
            return NotFound();

        return View(order);
    }


    public async Task<IActionResult> MyOrders()
    {
        // هات المستخدم الحالي
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account"); // لو مش عامل لوجين

        // هات الطلبات الخاصة بالمستخدم بس
        var orders = await _context.Orders
            .Where(o => o.UserId == user.Id)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
                    .ThenInclude(v => v.Product)
            .OrderByDescending(o => o.CreatedAt) // الأحدث أولاً
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequest request)
    {
        var userId = _userManager.GetUserId(User);
        var favorite = await _context.Favorites
                            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == request.ProductId);

        if (favorite == null)
        {
            // لو مش موجود، نضيفه
            favorite = new Favorite { UserId = userId, ProductId = request.ProductId };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return Json(new { isFavorited = true });
        }
        else
        {
            // لو موجود، نحذفه
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Json(new { isFavorited = false });
        }
    }

    public class FavoriteRequest
    {
        public int ProductId { get; set; }
    }

    public async Task<IActionResult> MyFavourite()
    {
        var userId = _userManager.GetUserId(User);

        var favouriteProducts = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Product)
            .ThenInclude(p => p.Category)
            .Select(f => f.Product)
            .ToListAsync();

        ViewBag.FavCount = await _context.Favorites
    .Where(f => f.UserId == userId)
    .CountAsync();

        foreach (var product in favouriteProducts)
        {
            product.IsFavorite = true;
        }

        return View(favouriteProducts);
    }


}
