using Autoservice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Autoservice.Data;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Autoservice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AutoserviceContext _context;
        private readonly INotyfService _toastNotification;
        public HomeController(ILogger<HomeController> logger, AutoserviceContext context, INotyfService toastNotification)
        {
            _logger = logger;
            _context = context;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Services != null ?
                         View(await _context.Categories.Include(o => o.Services).ToListAsync()) :
                         Problem("Entity set 'AutoserviceContext.Services'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> LoginAsync()
        {
            return _context.Clients != null ?
                         View(await _context.Services.Include(o => o.Categories).ToListAsync()) :
                         Problem("Entity set 'AutoserviceContext.Services'  is null.");
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
    }
}