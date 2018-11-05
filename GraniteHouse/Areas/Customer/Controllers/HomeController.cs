using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraniteHouse.Models;
using GraniteHouse.Data;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.Extensions;
using GraniteHouse.Utility;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var productsFromDB = await _db.Products
                    .Include(p => p.ProductTypes)
                    .Include(p => p.SpecialTags)
                    .ToListAsync();

            return View(productsFromDB);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ViewData["Message"] = "Your Product description page.";

            var productFromDB = await _db.Products
                    .Include(p => p.ProductTypes)
                    .Include(p => p.SpecialTags)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            if (productFromDB == null)
            {
                return NotFound();
            }

            return View(productFromDB);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(string id)
        {
            List<string> lstShoppingCarts = HttpContext.Session.Get<List<string>>(SD.SessionShoppingCartName);
            if (lstShoppingCarts == null)
            {
                lstShoppingCarts = new List<string>();
            }

            lstShoppingCarts.Add(id);
            HttpContext.Session.Set(SD.SessionShoppingCartName, lstShoppingCarts);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult Remove(string id)
        {
            List<string> lstShoppingCarts = HttpContext.Session.Get<List<string>>(SD.SessionShoppingCartName);
            if (lstShoppingCarts == null)
            {
                lstShoppingCarts = new List<string>();
            }
            else
            {
                if (lstShoppingCarts.Count > 0)
                {
                    if (lstShoppingCarts.Contains(id))
                    {
                        lstShoppingCarts.Remove(id);
                    }
                }
            }

            HttpContext.Session.Set(SD.SessionShoppingCartName, lstShoppingCarts);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }




        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
