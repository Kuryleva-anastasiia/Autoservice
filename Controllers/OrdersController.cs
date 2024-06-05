using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autoservice.Data;
using Autoservice.Models;
using Microsoft.Office.Interop.Word;

namespace Autoservice.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AutoserviceContext _context;

        public OrdersController(AutoserviceContext context)
        {
            _context = context;
        }

        // GET: OrdersController
        public async Task<ActionResult> Index(string? name, SortState sortOrder = SortState.NameAsc)
        {
            if (_context.Orders != null)
            {
                IQueryable<Orders>? orders = _context.Orders.Include(o => o.Clients).Include(o => o.Employees);

                List<Employees> employeesList = _context.Employees.ToList();
                // устанавливаем начальный элемент, который позволит выбрать всех
                employeesList.Insert(0, new Employees { name = "Все", id = 0 });

                //Поиск по названию и автору и фильтр по жанру
                if (name != null && Convert.ToInt32(name) != 0)
                {
                    orders = orders.Where(p => p.employee_id == Convert.ToInt32(name));
                    ViewData["employees"] = new SelectList(employeesList, "id", "name", name);
                }
                else ViewData["employees"] = new SelectList(employeesList, "id", "name");


                return View(await orders.ToListAsync());
            }
            else
            {
                Problem("Entity set 'AutoserviceContext.Orders'  is null.");
            }
            ViewData["employees"] = new SelectList(_context.Employees, "id", "name");

            return View();
        }

        // GET: OrdersController/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var Orders = await _context.Orders
                .Include(o => o.Clients)
                .FirstOrDefaultAsync(m => m.id == id);
            if (Orders == null)
            {
                return NotFound();
            }

            return View(Orders);
        }

        // GET: OrdersController/Create
        public ActionResult Create()
        {
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "login");
            ViewData["employee_id"] = new SelectList(_context.Employees, "id", "name");
            return View();
        }

        // POST: OrdersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,client_id,employee_id,date,completeInDays,sum,status")] Orders order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "login", order.client_id);
            ViewData["employee_id"] = new SelectList(_context.Employees, "id", "name", order.employee_id);
            ViewData["status"] = new SelectList(new List<string> { "Создан", "Собран", "Доставлен", "Выполнен", "Продлен" }, order.status);
            return View(order);
        }

        // GET: OrdersController/Edit/5
        public async Task<ActionResult> EditAsync(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["client_id"] = new SelectList(_context.Clients, "id", "login", order.client_id);
            ViewData["employee_id"] = new SelectList(_context.Employees, "id", "name", order.employee_id);
            ViewData["status"] = new SelectList(new List<string> { "Создан", "Собран", "Доставлен", "Выполнен", "Продлен" }, order.status);
            return View(order);
        }

        // POST: OrdersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,client_id,employee_id,date,completeInDays,sum,status")] Orders order)
        {
            if (id != order.id)
            {
                return NotFound();
            }


            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.id))
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

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.id == id)).GetValueOrDefault();
        }

        // GET: OrdersController/Delete/5
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Clients)
                .Include(o => o.Employees)
                .FirstOrDefaultAsync(m => m.id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: OrdersController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AutoserviceContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
