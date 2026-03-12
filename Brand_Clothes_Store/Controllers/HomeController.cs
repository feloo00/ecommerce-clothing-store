using Brand_Clothes_Store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Diagnostics;

namespace Brand_Clothes_Store.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(UserManager<ApplicationUser> userManager,
                              AppDbContext context,
                              ILogger<HomeController> logger)
            : base(userManager, context) 
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            
            return RedirectToAction("Browse","Shop");
        }
        public IActionResult AccessDenied()
        {
            return View(); 
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult ContactUs()
        {
            var model = new
            {
                PhonePrimary = "+20 1554129293",
                PhoneSecondary = "+20 1279999601",
                WhatsAppNumber = "201279999601",
                FacebookUrl = "https://www.facebook.com/felo.adel.75",
                InstagramUrl = "https://www.instagram.com/fel0adel/?__pwa=1",
                Email = "felopateradel@gmail.com",
                Address = "Haram Street, Cairo, Egypt"
            };

            return View(model);
        }
        public ActionResult AboutUs()
        {
            var model = new
            {
                BrandName = "YourBrand", 
                YearsActive = 6,
                ShortDescription = "We are an Egyptian clothing brand focused on comfort, style and lasting quality.",
                LongDescription = "We started our journey 6 years ago with a simple goal: deliver comfortable, stylish clothes made to last. From day one we committed to using only 100% authentic Egyptian cotton — the fabric the world admires. Over the years our designs stayed clean and modern while keeping prices accessible so everyone can enjoy high-quality local clothing.",
                Vision = "To become the leading Egyptian brand known for comfort, quality and pride in local craftsmanship.",
                Branches = new List<string>
                {
                    "Al-Haram Branch|https://maps.app.goo.gl/RVXYV4i4twSsvNcRA",
                    "Faisal Branch|https://maps.app.goo.gl/RVXYV4i4twSsvNcRA",
                    "Helwan Branch|https://maps.app.goo.gl/RVXYV4i4twSsvNcRA",
                    "Alexandria Branch|https://maps.app.goo.gl/RVXYV4i4twSsvNcRA",
                    "Assiut Branch|https://maps.app.goo.gl/RVXYV4i4twSsvNcRA"
                },
                    
                WhyChooseUs = new List<string>
                {
                    "100% Egyptian cotton",
                    "Durable, high-quality materials",
                    "Simple and tasteful designs",
                    "Proudly local production",
                    "Excellent customer service"
                }
            };

            return View(model);
        }
    }
}
