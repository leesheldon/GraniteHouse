using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductsViewModel
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Product = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTags);

            return View(await products.ToListAsync());
        }

        // GET Products Create
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

        // POST Products Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }

            _db.Products.Add(ProductsVM.Product);
            await _db.SaveChangesAsync();

            // Image being saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productFromDB = _db.Products.Find(ProductsVM.Product.Id);

            if (files.Count > 0 && files[0] != null)
            {
                // Image has been uploaded                
                var imageExtension = Path.GetExtension(files[0].FileName);
                var imageFileName = ProductsVM.Product.Id + imageExtension;
                var imageFilePath = SD.ImageFolder + @"\" + imageFileName;
                var uploadLocation = Path.Combine(webRootPath, imageFilePath);

                using (var fileStream = new FileStream(uploadLocation, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productFromDB.Image = @"\" + imageFilePath;
            }
            else
            {
                // When user does NOT upload image
                string defaultImageFilePath = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                string imageFilePathInDB = SD.ImageFolder + @"\" + productFromDB.Id + ".png";
                var uploadLocation = Path.Combine(webRootPath, imageFilePathInDB);
                System.IO.File.Copy(defaultImageFilePath, uploadLocation);

                productFromDB.Image = @"\" + imageFilePathInDB;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET Products Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ProductsVM.Product = await _db.Products
                    .Include(p => p.SpecialTags)
                    .Include(p => p.ProductTypes)
                    .SingleOrDefaultAsync(p => p.Id == id);

            if (ProductsVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        // POST Products Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDB = await _db.Products
                        .Where(p => p.Id == ProductsVM.Product.Id)
                        .FirstOrDefaultAsync();

                if (productFromDB == null)
                {
                    return NotFound();
                }

                if (files.Count > 0 && files[0] != null)
                {
                    // If user upload a new image
                    var imageExtension_old = Path.GetExtension(productFromDB.Image);
                    var imageFileName_old = ProductsVM.Product.Id + imageExtension_old;
                    var imageFilePath_old = SD.ImageFolder + @"\" + imageFileName_old;

                    var imageExtension_new = Path.GetExtension(files[0].FileName);
                    var imageFileName_new = ProductsVM.Product.Id + imageExtension_new;
                    var imageFilePath_new = SD.ImageFolder + @"\" + imageFileName_new;

                    var uploadLocation_old = Path.Combine(webRootPath, imageFilePath_old);
                    var uploadLocation_new = Path.Combine(webRootPath, imageFilePath_new);

                    if (System.IO.File.Exists(uploadLocation_old))
                    {
                        System.IO.File.Delete(uploadLocation_old);
                    }

                    using (var fileStream = new FileStream(uploadLocation_new, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    ProductsVM.Product.Image = @"\" + imageFilePath_new;
                }

                if (ProductsVM.Product.Image != null)
                {
                    productFromDB.Image = ProductsVM.Product.Image;
                }

                productFromDB.Name = ProductsVM.Product.Name;
                productFromDB.Price = ProductsVM.Product.Price;
                productFromDB.Available = ProductsVM.Product.Available;
                productFromDB.ProductTypeId = ProductsVM.Product.ProductTypeId;
                productFromDB.SpecialTagsId = ProductsVM.Product.SpecialTagsId;
                productFromDB.ShadeColor = ProductsVM.Product.ShadeColor;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ProductsVM);
        }

        // GET Products Details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ProductsVM.Product = await _db.Products
                    .Include(p => p.SpecialTags)
                    .Include(p => p.ProductTypes)
                    .SingleOrDefaultAsync(p => p.Id == id);

            if (ProductsVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        // GET Products Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ProductsVM.Product = await _db.Products
                    .Include(p => p.SpecialTags)
                    .Include(p => p.ProductTypes)
                    .SingleOrDefaultAsync(p => p.Id == id);

            if (ProductsVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        // POST Products Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var productFromDB = await _db.Products
                        .Where(p => p.Id == id)
                        .FirstOrDefaultAsync();

            if (productFromDB == null)
            {
                return NotFound();
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var imageExtension = Path.GetExtension(productFromDB.Image);
            var imageFileName = productFromDB.Id + imageExtension;
            var imageFilePath = SD.ImageFolder + @"\" + imageFileName;
            var uploadLocation = Path.Combine(webRootPath, imageFilePath);

            if (System.IO.File.Exists(uploadLocation))
            {
                System.IO.File.Delete(uploadLocation);
            }

            _db.Products.Remove(productFromDB);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));            
        }

    }
}