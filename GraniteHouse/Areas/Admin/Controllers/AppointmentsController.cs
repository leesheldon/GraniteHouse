using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.NormalEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string searchName, string searchEmail,
            string searchPhone, string searchDate)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var loggedInUser = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVM = new AppointmentViewModel();

            appointmentVM.Appointments = await _db.Appointments
                        .Include(p => p.SalesPerson)
                        .ToListAsync();

            //if(User.IsInRole(SD.AdminEndUser))
            //{
            //appointmentVM.Appointments = appointmentVM.Appointments
            //        .Where(p => p.SalesPersonId == loggedInUser.Value)
            //        .ToList();
            //}

            // Search criteria
            if (!string.IsNullOrEmpty(searchName))
            {
                appointmentVM.Appointments = appointmentVM.Appointments
                        .Where(p => p.CustomerName.ToLower().Contains(searchName.ToLower()))
                        .ToList();
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                appointmentVM.Appointments = appointmentVM.Appointments
                        .Where(p => p.CustomerEmail.ToLower().Contains(searchEmail.ToLower()))
                        .ToList();
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                appointmentVM.Appointments = appointmentVM.Appointments
                        .Where(p => p.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower()))
                        .ToList();
            }

            if (!string.IsNullOrEmpty(searchDate))
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);

                    appointmentVM.Appointments = appointmentVM.Appointments
                            .Where(p => p.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString()))
                            .ToList();
                }
                catch (Exception ex)
                {
                    return View(appointmentVM);
                }                                
            }

            return View(appointmentVM);
        }

        // GET Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }


            var productsList = (IEnumerable<Products>)(from p in _db.Products
                                                       join a in _db.ProductsSelectedForAppointment
                                                       on p.Id equals a.ProductId
                                                       where a.AppointmentId == id
                                                       select p).Include("ProductTypes");

            AppointmentDetailsViewModel appDetailsVM = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefaultAsync(),
                SalesPerson = await _db.ApplicationUser.ToListAsync(),
                Products = productsList.ToList()
            };

            return View(appDetailsVM);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppointmentDetailsViewModel appDetailsVM)
        {
            if (ModelState.IsValid)
            {
                appDetailsVM.Appointment.AppointmentDate = appDetailsVM.Appointment.AppointmentDate
                        .AddHours(appDetailsVM.Appointment.AppointmentTime.Hour)
                        .AddMinutes(appDetailsVM.Appointment.AppointmentTime.Minute);

                var appFromDB = await _db.Appointments
                        .Where(p => p.Id == appDetailsVM.Appointment.Id)
                        .FirstOrDefaultAsync();

                appFromDB.CustomerName = appDetailsVM.Appointment.CustomerName;
                appFromDB.CustomerEmail = appDetailsVM.Appointment.CustomerEmail;
                appFromDB.CustomerPhoneNumber = appDetailsVM.Appointment.CustomerPhoneNumber;
                appFromDB.AppointmentDate = appDetailsVM.Appointment.AppointmentDate;
                appFromDB.isConfirmed = appDetailsVM.Appointment.isConfirmed;

                if (User.IsInRole(SD.AdminEndUser))
                {
                    appFromDB.SalesPersonId = appDetailsVM.Appointment.SalesPersonId;
                }

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(appDetailsVM);
        }

        //GET Detials
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel appDetailsVM = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefaultAsync(),
                SalesPerson = await _db.ApplicationUser.ToListAsync(),
                Products = productList.ToList()
            };

            return View(appDetailsVM);
        }

        //GET Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel appDetailsVM = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefaultAsync(),
                SalesPerson = await _db.ApplicationUser.ToListAsync(),
                Products = productList.ToList()
            };

            return View(appDetailsVM);
        }

        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var appsList = await _db.ProductsSelectedForAppointment
                    .Where(p => p.AppointmentId == id)
                    .ToListAsync();

            if (appsList != null && appsList.Count > 0)
            {
                _db.ProductsSelectedForAppointment.RemoveRange(appsList);
            }

            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}