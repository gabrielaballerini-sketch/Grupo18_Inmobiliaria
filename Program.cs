
using Grupo18_Inmobiliaria;
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Globalization;

var builder = WebApplication.CreateBuilder(args);






// Forzamos a que el servidor acepte el punto (.) como separador decimal
var defaultCulture = new CultureInfo("es-AR");
defaultCulture.NumberFormat.NumberDecimalSeparator = ".";
defaultCulture.NumberFormat.CurrencyDecimalSeparator = ".";

CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

//indicamos q va haber controller y vistas
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IRepositorioPropietario, RepositorioPropietarioMySql>();
builder.Services.AddScoped<IRepositorioInquilino, RepositorioInquilinoMySql>();
builder.Services.AddScoped<IRepositorio<TipoInmueble>, RepositorioTipoInmuebleMySql>();
builder.Services.AddScoped<IRepositorioInmueble, RepositorioInmuebleMySql>();
builder.Services.AddScoped<IRepositorioReserva, RepositorioReservaMySql>();
builder.Services.AddScoped<IRepositorioPago, RepositorioPagoMySql>();
builder.Services.AddScoped<IRepositorioImagen, RepositorioImagenMySql>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioMySql>();

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
});



// construimos la aplicacion
var app = builder.Build();

// inicializador de  bd , seeder
using (var scope = app.Services.CreateScope())
{
DbSeeder.Seed(scope.ServiceProvider);
}


//Si NO estoy trabajando en desarrollo.
//Entonces configura determinadas cosas para producción.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Si alguien intenta entrar por HTTP, redirigilo a HTTPS.

app.UseHttpsRedirection();

app.UseStaticFiles();
//habilitamos rutas
app.UseRouting();


//Esto tiene que ver con qué puede hacer un usuario dependiendo de sus permisos.
app.UseAuthentication();
app.UseAuthorization();



//Esto permite manejar recursos estáticos de la aplicación.
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//arranca la aplicacion
app.Run();
