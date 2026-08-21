
//creamos el objeto builder simil constructor
//configuramos
using Grupo18_Inmobiliaria.Models;

var builder = WebApplication.CreateBuilder(args);

//indicamos q va haber controller y vistas
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<RepositorioPropietarioMySql>();
builder.Services.AddScoped<RepositorioInquilinoMySql>();
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
