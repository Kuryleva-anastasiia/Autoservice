using System.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autoservice.Data;
using Autoservice.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.AspNetCore.Hosting;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Autoservice.Controllers
{
    public class ClientsController : Controller
    {
        private readonly AutoserviceContext _context;
        private readonly INotyfService _toastNotification;
        private readonly ILogger<ClientsController> _logger;
        IWebHostEnvironment _appEnvironment;

        public ClientsController(AutoserviceContext context, INotyfService toastNotification, ILogger<ClientsController> logger, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        public IActionResult LoginNotify()
        {
            _toastNotification.Custom("Необходимо войти в свой аккаунт!", 6, "#602AC3", "fa fa-user");
            return RedirectToAction("Login");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            //return _context.Users != null ? 
            //            View(await _context.Users.Include(x => x.Carts).Include(x => x.Order_Rent).Include(x => x.Order_Buy).ToListAsync()) :
            //            Problem("Entity set 'PoolOfBooksContext.Users'  is null.");
            return _context.Clients != null ?
                        View(await _context.Clients.Include(x => x.Orders).ToListAsync()) :
                        Problem("Entity set 'AutoserviceContext.Clients'  is null.");
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var users = await _context.Clients.Include(x => x.Orders)
                .FirstOrDefaultAsync(m => m.id == id);
            //ViewData["CartCount"] = _context.Cart.Where(x => x.userId == id).Count();


            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }


        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,login,password,name")] Clients users)
        {
            if (ModelState.IsValid)
            {
                if (users.login != null && users.password != null)
                {
                    var count = _context.Clients.Where(u => u.login == users.login).Count();

                    if (count == 0)
                    {
                        try
                        {

                            users.password = Crypto.Hash(users.password.ToString(), "SHA-256");

                            _context.Add(users);
                            await _context.SaveChangesAsync();
                            _toastNotification.Success("Вы успешно зарегистрированы!");
                            return Redirect($"~/Clients/SignIn/{users.id}");
                        }
                        catch (Exception ex)
                        {
                            _toastNotification.Error("Ошибка регистрации!\n" + ex.Message);
                        }
                    }
                    else
                    {
                        _toastNotification.Error("Аккаунт с таким логином уже существует!");
                    }


                }
                else
                {
                    _toastNotification.Error("Логин и пароль обязательны для заполнения!");

                }
            }
            return View(users);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("login,password")] Clients model)
        {


            if (model.login != null && model.password != null)
            {


                var p = Crypto.Hash(model.password, "SHA-256");



                var user = _context.Clients.FirstOrDefaultAsync(u => u.login == model.login && u.password == p);

                if (user.Result != null)
                {

                    int id = Convert.ToInt32(user.Result.id);

                    model.id = id;
                    _toastNotification.Success("Вы успешно вошли в аккаунт!");
                    return Redirect($"~/Clients/SignIn/{id}");
                }
                else
                {
                    _toastNotification.Error("Аккаунт не найден!");
                    return View(model);
                }
            }
            else
            {
                _toastNotification.Error("Введите логин и пароль!");
            }
            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var users = await _context.Clients.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            //ViewData["role"] = new SelectList(new List<string> { "client", "admin" }, "client");
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,login,password,name")] Clients users)
        {
            if (id != users.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"~/Clients/Details/{users.id}");
            }
            return Redirect($"~/Clients/Details/{users.id}");
        }

        // GET: Users/DitailsForAdmin/5
        public async Task<IActionResult> DetailsForAdmin(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var users = await _context.Clients
                .FirstOrDefaultAsync(m => m.id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var users = await _context.Clients
                .FirstOrDefaultAsync(m => m.id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'AutoserviceContext.Autoservice'  is null.");
            }
            var users = await _context.Clients.FindAsync(id);
            if (users != null)
            {
                _context.Clients.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return (_context.Clients?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
