

// using=import

//Herramientas para diagnosticar el sistema (acá se usa para capturar el ID de un error)
using System.Diagnostics;

//Toda la magia de .NET MVC (para usar la clase Controller, IActionResult, View(), etc.).
using Microsoft.AspNetCore.Mvc;

// mi carpeta de modelos 
using Grupo18_Inmobiliaria.Models;

//simil package
namespace Grupo18_Inmobiliaria.Controllers;

// define clase HomeController e indica que hereda de Controller
public class HomeController : Controller
{

    // IActionResult: Es el tipo de dato que devuelven
    //  los métodos en un controlador MVC (en Java dirías algo como el tipo de retorno). Significa "Resultado de una Acción".
   

    public IActionResult Index()
{
    return View();
}






//Para Index(), busca el archivo Views/Home/Index.cshtml.
//Para Privacy(), busca el archivo Views/Home/Privacy.cshtml.




    public IActionResult Privacy()
    {
        return View();
    }

public IActionResult Hola()
    {

 return View();
    }



// * aparte


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
