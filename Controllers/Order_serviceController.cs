using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Controllers
{
    public class Order_serviceController : Controller
    {
        private readonly AutoserviceContext _context;

        public Order_serviceController(AutoserviceContext context)
        {
            _context = context;
        }
        // GET: Order_serviceController
        public async Task<IActionResult> Index()
        {
            var orders = _context.Order_service.Include(o => o.Services).Include(o => o.Orders);
            return View(await orders.ToListAsync());
        }

        // GET: Order_serviceController/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Order_service == null)
            {
                return NotFound();
            }

            var order_service = _context.Order_service
                .Include(o => o.Services)
                .Include(o => o.Orders)
                .Include(o => o.Orders.Employees)
                .Where(m => m.order_id == id);
            if (order_service == null)
            {
                return NotFound();
            }

            return View(order_service);
        }

        // GET: Order_serviceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order_serviceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Order_serviceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Order_serviceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Order_serviceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order_serviceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
