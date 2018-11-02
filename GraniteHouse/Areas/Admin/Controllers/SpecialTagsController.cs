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
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.SpecialTags.ToList());
        }

        // GET Create Action method
        public IActionResult Create()
        {
            return View();
        }

        // POST Create Action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTags.Add(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        // GET Edit Action method
        public async Task<IActionResult> Edit(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var specialTagFromDB = await _db.SpecialTags.FindAsync(id);
            if (specialTagFromDB == null)
            {
                return NotFound();
            }

            return View(specialTagFromDB);
        }

        // POST Edit Action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SpecialTags specialTag)
        {
            if (id != specialTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        // GET Details Action method
        public async Task<IActionResult> Details(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var specialTagFromDB = await _db.SpecialTags.FindAsync(id);
            if (specialTagFromDB == null)
            {
                return NotFound();
            }

            return View(specialTagFromDB);
        }

        // GET Delete Action method
        public async Task<IActionResult> Delete(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var specialTagFromDB = await _db.SpecialTags.FindAsync(id);
            if (specialTagFromDB == null)
            {
                return NotFound();
            }

            return View(specialTagFromDB);
        }

        // POST Delete Action method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCOnfirmed(string id)
        {
            var specialTagFromDB = await _db.SpecialTags.FindAsync(id);
            if (specialTagFromDB == null)
            {
                return NotFound();
            }

            _db.SpecialTags.Remove(specialTagFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}