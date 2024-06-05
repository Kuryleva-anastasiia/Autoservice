using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
//using Autoservice.Data;
using Autoservice.Models;
using System.Security.Claims;
using Autoservice.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AutoserviceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AutoserviceContext") ?? throw new InvalidOperationException("Connection string 'AutoserviceContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Add ToastNotification
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter;
});

// Подключаю куки
builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/Clients/Login");

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNotyf();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("~/Clients/SignIn/{id:int}", async (string? returnUrl, HttpContext context, int id, AutoserviceContext _context) =>
{

    var user = _context.Clients.FirstOrDefaultAsync(u => u.id == id);

    if (user.Result != null)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Result.login), new Claim(ClaimTypes.Role, "client"), new Claim("ID", user.Result.id.ToString()) };
        // создаем объект ClaimsIdentity
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        // установка аутентификационных куки
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return Results.Redirect($"~/Clients/Details/{user.Result.id}");
    }
    return Results.Unauthorized();
});

app.MapGet("~/Employees/SignIn/{id:int}", async (string? returnUrl, HttpContext context, int id, AutoserviceContext _context) =>
{

    var user = _context.Employees.FirstOrDefaultAsync(u => u.id == id);

    if (user.Result != null)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Result.login), new Claim(ClaimTypes.Role, "admin"), new Claim("ID", user.Result.id.ToString()) };
        // создаем объект ClaimsIdentity
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        // установка аутентификационных куки
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return Results.Redirect($"~/Employees/Details/{user.Result.id}");
    }
    return Results.Unauthorized();
});


app.MapGet("/SignInCheckForAvatar", (string? returnUrl, HttpContext context) =>
{
    var user = context.User;
    if (user.Identity != null)
    {
        if (user.Identity.IsAuthenticated)
        {
            string id = user.FindFirst("ID").Value.ToString();

            if (user.IsInRole("client"))
            {
                if (id != null)
                {
                    return Results.Redirect($"~/Clients/Details/{id}");
                }
            }
            else if (user.IsInRole("admin"))
            {
                if (id != null)
                {
                    return Results.Redirect($"~/Employees/Details/{id}");
                }
            }
        }
        else { return Results.Redirect("~/Clients/LoginNotify"); }
    }
    else { return Results.Redirect("~/Clients/LoginNotify"); }
    return Results.Redirect("~/Clients/LoginNotify");
});

app.MapGet("/CartAdd/{client_id}/{service_id}", (string? returnUrl, HttpContext context, AutoserviceContext _context, int client_id, int service_id) =>
{

    Cart с = new Cart(client_id, service_id);
    _context.Cart.Add(с);
    _context.SaveChanges();
    return Results.Redirect($"~/Cart/CartAddNotify");
});

app.MapGet("/CreateOrder", (string? returnUrl, HttpContext context, AutoserviceContext _context) =>
{
    var cart = _context.Cart.Include(x => x.Services).Include(c => c.Clients).ToList();
    int client_id = Convert.ToInt32(context.User.FindFirst("ID").Value);
    var client = _context.Clients.FirstOrDefault(x => x.id == client_id);

    var services = cart.Where(x => x.client_id == client_id).ToList();
    decimal sum = 0;
    int time = 0;

    foreach (var service in services)
    {
        var b = _context.Services.FirstOrDefault(x => x.id == service.service_id);

        _context.Update(b);
        _context.SaveChanges();

        sum += Convert.ToDecimal(service.Services.price);
        time += Convert.ToInt32(service.Services.duration);
    }

    Orders order;

    if (context.User.IsInRole("admin"))
    {

        order = new Orders(client_id, client_id, DateTime.Now.Date, time, sum, "Создан");

    }
    else
    {
        var employee = _context.Employees.FirstOrDefault();
        order = new Orders(client_id, employee.id, DateTime.Now.Date, time, sum, "Создан");
    }


    _context.Orders.Add(order);
    _context.SaveChanges();

    var or = _context.Orders.OrderBy(x => x.id).Last();

    foreach (var service in services)
    {
        Order_service os = new Order_service(or.id, service.service_id);
        _context.Order_service.Add(os);
    }

    _context.SaveChanges();



    _context.Cart.RemoveRange(_context.Cart.Where(x => x.client_id == client_id));
    _context.SaveChanges();

    return Results.Redirect($"~/Cart/OrderNotify");
});


app.Run();
