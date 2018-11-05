using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel CartsVM { get; set; }

        public ShoppingCartsController(ApplicationDbContext db)
        {
            _db = db;
            CartsVM = new ShoppingCartViewModel
            {
                Products = new List<Products>()
            };
        }

        public async Task<IActionResult> Index()
        {
            List<string> lstShopingCart = HttpContext.Session.Get<List<string>>(SD.SessionShoppingCartName);
            if (lstShopingCart != null && lstShopingCart.Count > 0)
            {
                foreach (var cartItem in lstShopingCart)
                {
                    var productFromDB = await _db.Products
                            .Include(p => p.ProductTypes)
                            .Include(p => p.SpecialTags)
                            .Where(p => p.Id == cartItem)
                            .FirstOrDefaultAsync();

                    CartsVM.Products.Add(productFromDB);
                }
            }

            return View(CartsVM);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost()
        {
            List<string> lstShoppingCart = HttpContext.Session.Get<List<string>>(SD.SessionShoppingCartName);
            if (lstShoppingCart != null && lstShoppingCart.Count > 0)
            {
                CartsVM.Appointment.AppointmentDate = CartsVM.Appointment.AppointmentDate
                        .AddHours(CartsVM.Appointment.AppointmentTime.Hour)
                        .AddMinutes(CartsVM.Appointment.AppointmentTime.Minute);

                Appointments appointment = CartsVM.Appointment;
                _db.Appointments.Add(appointment);
                await _db.SaveChangesAsync();

                string appId = appointment.Id;

                foreach(string productId in lstShoppingCart)
                {
                    ProductsSelectedForAppointment selectedProduct = new ProductsSelectedForAppointment()
                    {
                        AppointmentId = appId,
                        ProductId = productId
                    };

                    _db.ProductsSelectedForAppointment.Add(selectedProduct);
                }

                await _db.SaveChangesAsync();

                lstShoppingCart = new List<string>();
                HttpContext.Session.Set(SD.SessionShoppingCartName, lstShoppingCart);

                return RedirectToAction("AppointmentConfirmation", "ShoppingCarts", new { id = appId });
            }

            return View(CartsVM);
        }

        public IActionResult Remove(string id)
        {
            List<string> lstShoppingCart = HttpContext.Session.Get<List<string>>(SD.SessionShoppingCartName);
            if (lstShoppingCart != null && lstShoppingCart.Count > 0)
            {
                if (lstShoppingCart.Contains(id))
                {
                    lstShoppingCart.Remove(id);
                }

                HttpContext.Session.Set(SD.SessionShoppingCartName, lstShoppingCart);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AppointmentConfirmation(string id)
        {
            CartsVM.Appointment = await _db.Appointments
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

            List<ProductsSelectedForAppointment> selectedProducts = await _db.ProductsSelectedForAppointment
                    .Where(p => p.AppointmentId == id)
                    .ToListAsync();

            foreach (ProductsSelectedForAppointment productItem in selectedProducts)
            {
                var productFromDB = await _db.Products
                        .Include(p => p.ProductTypes)
                        .Include(p => p.SpecialTags)
                        .Where(p => p.Id == productItem.ProductId)
                        .FirstOrDefaultAsync();

                CartsVM.Products.Add(productFromDB);
            }

            return View(CartsVM);
        }

    }
}