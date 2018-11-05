using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ProductTypes.ToList());
        }

        // GET Create Action method
        public IActionResult Create()
        {
            return View();
        }

        // POST Create Action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productType)
        {
            if (ModelState.IsValid)
            {                
                _db.ProductTypes.Add(productType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productType);
        }

        // GET Edit Action method
        public async Task<IActionResult> Edit(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var productTypeFromDB = await _db.ProductTypes.FindAsync(id);
            if (productTypeFromDB == null)
            {
                return NotFound();
            }
            
            return View(productTypeFromDB);
        }

        // POST Edit Action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductTypes productType)
        {
            if (id != productType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(productType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productType);
        }

        // GET Details Action method
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var productTypeFromDB = await _db.ProductTypes.FindAsync(id);
            if (productTypeFromDB == null)
            {
                return NotFound();
            }

            return View(productTypeFromDB);
        }

        // GET Delete Action method
        public async Task<IActionResult> Delete(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var productTypeFromDB = await _db.ProductTypes.FindAsync(id);
            if (productTypeFromDB == null)
            {
                return NotFound();
            }

            return View(productTypeFromDB);
        }

        // POST Delete Action method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var productTypeFromDB = await _db.ProductTypes.FindAsync(id);
            if (productTypeFromDB == null)
            {
                return NotFound();
            }

            _db.ProductTypes.Remove(productTypeFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));            
        }

    }
}