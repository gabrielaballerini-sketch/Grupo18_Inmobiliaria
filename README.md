#  Sistema de Gestión de Alquileres Temporales - 

Sistema web desarrollado en **ASP.NET Core MVC** con **MySQL** para la gestión integral de una agencia inmobiliaria, enfocado en el control de inmuebles, propietarios, inquilinos, reservas y pagos.

---

##  Integrantes del Grupo
1. **Alcaraz, Melisa Fatima**
2. **Ballerini, Gabriela**
3. **Delicia, Brian**

---

## Diagrama de Clases
Podés consultar el diagrama de clases del sistema en el siguiente enlace:
👉 [Ver Diagrama en Diagrams.net](https://app.diagrams.net/#G18Scs2MIZVE_8W8LoGQm6b49wo-_AWy0t#%7B%22pageId%22%3A%22cxBcQCD3t9tabTOSOMA3%22%7D)

---

##  Base de Datos
El código completo de creación de tablas y datos iniciales (seed data) se encuentra en el archivo **`database.sql`** dentro de este repositorio.

Para configurarla:
1. Creá una base de datos en MySQL llamada `inmobiliaria_g18`.
2. Importá o ejecutá el script contenido en `database.sql`.

---

##  Conexión a la Base de Datos
Agregá la siguiente cadena de conexión en tu archivo `appsettings.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost; Port=3306; Database=inmobiliaria_g18; User=root; Password=;"
}
```
---
## Cómo Ejecutar el Proyecto
1. Clonar el repositorio y abrir la solución en **Visual Studio**.
2. Verificar que la cadena de conexión en `appsettings.json` apunte correctamente a tu servidor local de MySQL.
3. Ejecutar la aplicación (`F5` o `Ctrl + F5`).
4. Una vez abierto el navegador, podés acceder a los módulos principales 