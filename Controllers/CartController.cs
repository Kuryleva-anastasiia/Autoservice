using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Controllers
{
    public class CartController : Controller
    {
        private readonly AutoserviceContext _context;
        private readonly INotyfService _toastNotification;
        public CartController(AutoserviceContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public IActionResult Notify()
        {
            var id = User.FindFirst("ID").Value;
            _toastNotification.Warning("Книга удалена!");
            return Redirect($"~/cart/details/{id}");
        }

        public IActionResult CartAddNotify()
        {
            var cart = _context.Cart.OrderBy(x => x.id).Last();
            _toastNotification.Success("Услуга добавлена в корзину!");
            return Redirect($"~/Services/index");
        }

        public IActionResult OrderNotify()
        {
            var id = User.FindFirst("ID").Value;

            _toastNotification.Success("Заказ создан! Менеджер свяжется с Вами для подтверждения заказа", 15);

            if (User.IsInRole("admin"))
            {
                return Redirect($"~/Orders/Details/{id}");
            }
            else
            {
                return Redirect($"~/Clients/Details/{id}");
            }
        }

        // GET: CartController
        public async Task<IActionResult> Index()
        {
            var cart = _context.Cart.Include(c => c.Services).Include(c => c.Clients);
            return View(await cart.ToListAsync());
        }

        // GET: CartController/Details/5
        public IActionResult Details(int id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = _context.Cart
                .Include(c => c.Services)
                .Include(c => c.Clients).Where(x => x.Clients.id == id).ToList();
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: CartController/Create
        public IActionResult Create()
        {
            ViewData["service_id"] = new SelectList(_context.Services, "id", "name");
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "name");
            return View();
        }

        // POST: CartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,client_id,service_id")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["service_id"] = new SelectList(_context.Services, "id", "name", cart.service_id);
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "name", cart.client_id);
            return View(cart);
        }

        // GET: CartController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["service_id"] = new SelectList(_context.Services, "id", "name", cart.service_id);
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "name", cart.client_id);
            return View(cart);
        }


        // POST: CartController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,client_id,service_id")] Cart cart)
        {
            if (id != cart.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["service_id"] = new SelectList(_context.Services, "id", "name", cart.service_id);
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "name", cart.client_id);
            return View(cart);
        }

        private bool CartExists(int id)
        {
            return (_context.Cart?.Any(e => e.id == id)).GetValueOrDefault();
        }

        // GET: CartController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Services)
                .Include(c => c.Clients)
                .FirstOrDefaultAsync(m => m.id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: CartController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            if (_context.Cart == null)
            {
                return Problem("Entity set 'AutoserviceContext.Cart'  is null.");
            }
            var cart = await _context.Cart.FindAsync(id);
            var userId = User.FindFirst("ID").Value;
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return Redirect(returnUrl ?? $"~/Cart/Details/{userId}");
        }
    }
}
