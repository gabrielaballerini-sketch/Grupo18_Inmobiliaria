
using Grupo18_Inmobiliaria.Models;
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

builder.Services.AddScoped<RepositorioPropietarioMySql>();
builder.Services.AddScoped<RepositorioInquilinoMySql>();
builder.Services.AddScoped<RepositorioTipoInmuebleMySql>();
builder.Services.AddScoped<RepositorioInmuebleMySql>();
builder.Services.AddScoped<RepositorioReservaMySql>();



// construimos la aplicacion
var app = builder.Build();

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

//habilitamos rutas
app.UseRouting();


//Esto tiene que ver con qué puede hacer un usuario dependiendo de sus permisos.
app.UseAuthorization();


//Esto permite manejar recursos estáticos de la aplicación.
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//arranca la aplicacion
app.Run();
