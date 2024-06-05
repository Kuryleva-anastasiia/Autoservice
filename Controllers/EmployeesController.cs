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
    public class EmployeesController : Controller
    {
        private readonly AutoserviceContext _context;
        private readonly INotyfService _toastNotification;
        private readonly ILogger<EmployeesController> _logger;
        IWebHostEnvironment _appEnvironment;

        public EmployeesController(AutoserviceContext context, INotyfService toastNotification, ILogger<EmployeesController> logger, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        public IActionResult ReportNotify()
        {
            _toastNotification.Custom("Отчет создан в папке \"Отчеты\"!", 6, "#602AC3", "fa fa-user");
            return RedirectToAction("Analize");
        }   

        public IActionResult LoginNotify()
        {
            _toastNotification.Custom("Необходимо войти в свой аккаунт!", 6, "#602AC3", "fa fa-user");
            return RedirectToAction("Login");
        }

        // GET: EmployeesController
        public async Task<IActionResult> Index()
        {

            return _context.Employees != null ?
                        View(await _context.Employees.Include(x => x.Orders).ToListAsync()) :
                        Problem("Entity set 'AutoserviceContext.Employees'  is nul.");

        }

        //[Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var users = await _context.Employees.Include(x => x.Orders)
                .FirstOrDefaultAsync(m => m.id == id);
            //ViewData["CartCount"] = _context.Cart.Where(x => x.userId == id).Count();


            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,login,password,name")] Employees users)
        {
            if (ModelState.IsValid)
            {
                if (users.login != null && users.password != null)
                {
                    var count = _context.Employees.Where(u => u.login == users.login).Count();

                    if (count == 0)
                    {
                        try
                        {

                            users.password = Crypto.Hash(users.password.ToString(), "SHA-256");

                            _context.Add(users);
                            await _context.SaveChangesAsync();
                            _toastNotification.Success("Вы успешно зарегистрированы!");
                            return Redirect($"~/Employees/SignIn/{users.id}");
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

        // GET: EmployeesController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var users = await _context.Employees.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            //ViewData["role"] = new SelectList(new List<string> { "client", "admin" }, "client");
            return View(users);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("login,password")] Employees model)
        {


            if (model.login != null && model.password != null)
            {


                var p = Crypto.Hash(model.password, "SHA-256");



                var user = _context.Employees.FirstOrDefaultAsync(u => u.login == model.login && u.password == p);

                if (user.Result != null)
                {

                    int id = Convert.ToInt32(user.Result.id);

                    model.id = id;
                    _toastNotification.Success("Вы успешно вошли в аккаунт!");
                    return Redirect($"~/Employees/SignIn/{id}");
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

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,login,password,name")] Employees users)
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
                    if (!EmployeesExists(users.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"~/Employees/Index");
            }
            return Redirect($"~/Employees/Index");
        }

        // GET: EmployeesController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var users = await _context.Employees
                .FirstOrDefaultAsync(m => m.id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: EmployeesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'AutoserviceContext.Employees'  is null.");
            }
            var users = await _context.Employees.FindAsync(id);
            if (users != null)
            {
                _context.Employees.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeesExists(int id)
        {
            return (_context.Employees?.Any(e => e.id == id)).GetValueOrDefault();
        }

        // GET: Users/Analize
        public IActionResult Analize()
        {
            _toastNotification.Warning("\nОтчет создан в папке Отчеты!\n", 10);

            var orders = _context.Orders.Include(x => x.Order_service).Include(x => x.Employees).Where(x => x.date.Month == DateTime.Today.Month && x.date.Year == DateTime.Today.Year).ToList();

            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Users/Analize
        [HttpPost, ActionName("Analize")]
        [ValidateAntiForgeryToken]
        public IActionResult Analize(DateTime start, DateTime end, string file)
        {


            Excel.Application winword = new Excel.Application()
            {
                //Отобразить Excel
                Visible = true,
                //Количество листов в рабочей книге
                SheetsInNewWorkbook = 1
            };
            Excel.Application app = new Excel.Application();

            //Добавить рабочую книгу
            Excel.Workbook workBook = app.Workbooks.Add(Type.Missing);

            //Отключить отображение окон с сообщениями
            app.DisplayAlerts = false;

            //Получаем первый лист документа (счет начинается с 1)
            Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);

            //Название листа (вкладки снизу).
            sheet.Name = string.Concat("Отчет ", start.ToString("dd.MM.yyyy"), " - ", end.ToString("dd.MM.yyyy"));

            var orders = _context.Orders.Include(x => x.Order_service).Include(x => x.Employees).Where(x => x.date.Date >= start.Date && x.date.Date <= end.Date).ToList();


            sheet.Cells[1, 1] = string.Concat("Промежуток времени: ");
            sheet.Cells[1, 2] = string.Concat(start.ToString("dd.MM.yyyy"), " - ", end.ToString("dd.MM.yyyy"));

            //заполнение имен столбцов в excel

            sheet.Cells[3, 1] = "Дата";
            sheet.Cells[3, 2] = "Номер заказа";
            sheet.Cells[3, 3] = "Сотрудник";
            sheet.Cells[3, 4] = "Статус";
            sheet.Cells[3, 5] = "Сумма";

            decimal sum = 0;
            int j = 4;

            foreach (var order in orders)
            {

                sheet.Cells[j, 1] = order.date.ToString("dd.MM.yyyy");
                sheet.Cells[j, 2] = order.id;
                sheet.Cells[j, 3] = order.Employees.name;
                sheet.Cells[j, 4] = order.status;
                sheet.Cells[j, 5] = order.sum;
                j++;
                sum += order.sum.Value;
            }

            sheet.Cells[j, 4] = "Итог:";
            sheet.Cells[j, 5] = sum;
            sheet.Cells[j, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            sheet.Cells[j, 4].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

            sheet.Columns.AutoFit();

            // и места где его нужно сохранить*/
            app.Application.ActiveWorkbook.SaveAs($"{_appEnvironment.WebRootPath}/Отчеты/{file}.xlsx", Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            app.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

            winword.Quit();


            return Redirect("~/Employees/ReportNotify");
        }
    }
}
