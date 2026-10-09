using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VehicleServiceManager.Data;
using VehicleServiceManager.Models;

namespace VehicleServiceManager.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VehicleController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var vehicles = _db.Vehicles
                .Include(vehicle => vehicle.Customer)
                .ToList();

            return View(vehicles);
        }

        public IActionResult Create()
        {
            LoadCustomers();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vehicle vehicle)
        {
            NormalizeVehicle(vehicle);
            CheckForDuplicateRegistration(vehicle);

            if (!ModelState.IsValid)
            {
                LoadCustomers(vehicle.CustomerId);
                return View(vehicle);
            }

            _db.Vehicles.Add(vehicle);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var vehicle = _db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            LoadCustomers(vehicle.CustomerId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vehicle vehicle)
        {
            NormalizeVehicle(vehicle);
            CheckForDuplicateRegistration(vehicle);

            if (!ModelState.IsValid)
            {
                LoadCustomers(vehicle.CustomerId);
                return View(vehicle);
            }

            _db.Vehicles.Update(vehicle);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var vehicle = _db.Vehicles
                .Include(item => item.Customer)
                .FirstOrDefault(item => item.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var vehicle = _db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            _db.Vehicles.Remove(vehicle);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        private void LoadCustomers(int? selectedCustomerId = null)
        {
            ViewBag.CustomerId = new SelectList(
                _db.Customers.OrderBy(customer => customer.Name),
                "Id",
                "Name",
                selectedCustomerId);
        }

        private void NormalizeVehicle(Vehicle vehicle)
        {
            vehicle.RegistrationNumber = (vehicle.RegistrationNumber ?? string.Empty)
                .Trim()
                .ToUpperInvariant();
            vehicle.Make = (vehicle.Make ?? string.Empty).Trim();
            vehicle.Model = (vehicle.Model ?? string.Empty).Trim();
        }

        private void CheckForDuplicateRegistration(Vehicle vehicle)
        {
            var registrationAlreadyExists = _db.Vehicles.Any(existingVehicle =>
                existingVehicle.RegistrationNumber.ToUpper() == vehicle.RegistrationNumber &&
                existingVehicle.Id != vehicle.Id);

            if (registrationAlreadyExists)
            {
                ModelState.AddModelError(
                    nameof(vehicle.RegistrationNumber),
                    "A vehicle with this registration number already exists.");
            }
        }
    }
}
