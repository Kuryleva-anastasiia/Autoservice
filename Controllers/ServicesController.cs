using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autoservice.Data;
using Autoservice.Models;


namespace Autoservice.Controllers
{
    public class ServicesController : Controller
    {
        private readonly AutoserviceContext _context;
        private readonly INotyfService _toastNotification;
        public ServicesController(AutoserviceContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }


        // GET: Books
        public async Task<IActionResult> Index(string? name, string? category, string? sort, SortState sortOrder = SortState.NameAsc)
        {

            if (_context.Services != null)
            {
                IQueryable<Services>? Services = _context.Services.Include(o => o.Categories);

                //сортировка по названию и цене 
                switch (sort)
                {
                    case "По названию А-Я":
                        Services = Services.OrderBy(s => s.name);
                        break;
                    case "По названию Я-А":
                        Services = Services.OrderByDescending(s => s.name);
                        break;
                    case "По возрастанию цены":
                        Services = Services.OrderBy(s => s.price);
                        break;
                    case "По убыванию цены":
                        Services = Services.OrderByDescending(s => s.price);
                        break;
                    default:
                        Services = Services.OrderBy(s => s.name);
                        break;
                }

                List<string> sortList = new List<string>() { "По названию А-Я", "По названию Я-А", "По возрастанию цены", "По убыванию цены" };


                List<Categories> categoriesList = _context.Categories.ToList();
                // устанавливаем начальный элемент, который позволит выбрать всех
                categoriesList.Insert(0, new Categories { name = "Все", id = 0 });

                //Поиск по названию и автору и фильтр по жанру
                if (category != null && Convert.ToInt32(category) != 0)
                {
                    Services = Services.Where(p => p.category_id == Convert.ToInt32(category));
                    ViewData["category"] = new SelectList(categoriesList, "id", "name", category);
                }
                else ViewData["category"] = new SelectList(categoriesList, "id", "name");

                if (!string.IsNullOrEmpty(name))
                {
                    Services = Services.Where(p => p.name!.Contains(name));
                    ViewData["name"] = name;
                }

                if (sort != null)
                {
                    ViewData["sort"] = new SelectList(sortList, sort);
                }
                else { ViewData["sort"] = new SelectList(sortList, "По названию А-Я"); }

                return View(await Services.ToListAsync());

            }
            else
            {
                Problem("Entity set 'AutoserviceContext.Services'  is null.");
            }

            ViewData["category"] = new SelectList(_context.Categories, "id", "name");

            return View();

        }


        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var Services = await _context.Services.Include(o => o.Categories)
                .FirstOrDefaultAsync(m => m.id == id);
            if (Services == null)
            {
                return NotFound();
            }

            return View(Services);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["id_category"] = new SelectList(_context.Categories, "id", "name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,description,price,duration,category_id")] Services Services)
        {
            try
            {
                
                _context.Services.Add(Services);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Книга добавлена!\n", 10);
                return Redirect("~/Services/Index");
            }
            catch (Exception ex)
            {
                _toastNotification.Error("Ошибка!\n" + ex.Message, 10);
                return View(Services);
            }
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var services = await _context.Services.Include(o => o.Categories)
                .FirstOrDefaultAsync(m => m.id == id);

            if (services == null)
            {
                return NotFound();
            }

            ViewData["id_category"] = new SelectList(_context.Categories, "id", "name", service.Categories.id);
            return View(service);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,description,price,duration,category_id")] Services services)
        {
            if (id != services.id)
            {
                return NotFound();
            }

            try
            {
                
                _context.Update(services);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Информация изменена успешно!\n", 10);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BooksExists(services.id))
                {
                    _toastNotification.Error("Ошибка!\n" + ex.Message, 10);
                    return NotFound();
                }
                else
                {
                    _toastNotification.Error("Ошибка!\n" + ex.Message, 10);
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var services = await _context.Services
                .FirstOrDefaultAsync(m => m.id == id);
            if (services == null)
            {
                return NotFound();
            }

            return View(services);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'AutoserviceContext.Services'  is null.");
            }
            var services = await _context.Services.FindAsync(id);
            if (services != null)
            {
                _context.Services.Remove(services);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BooksExists(int id)
        {
            return (_context.Services?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
