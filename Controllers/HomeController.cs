using PedidosManejo.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosManejo.Models;
using System.Data.Entity;
using System.Security.Claims;

namespace PedidosManejo.Controllers
{
    public class HomeController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        public ActionResult Index()
        {
            // Traemos los últimos 5 menús activos ordenados por ID (más recientes primero)
            var menus = db.menu
                          .Include(m => m.restaurante)
                          .Where(m => m.Estado == "Disponible")
                          .OrderByDescending(m => m.MenuID)
                          .Take(5)
                          .ToList();
            return View(menus);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        // GET: /Home/Contact
        [Authorize] // Solo usuarios autenticados
        public ActionResult Contact()
        {
            var model = new ContactViewModel();

            try
            {
                // Obtener el UsuarioID del usuario autenticado
                var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int usuarioId = int.Parse(userIdClaim.Value);
                    var usuario = db.usuario
                                    .Include(u => u.cliente)
                                    .FirstOrDefault(u => u.UsuarioID == usuarioId);

                    if (usuario != null)
                    {
                        // Prellenar datos del usuario
                        model.Name = usuario.Nombre;
                        model.Email = usuario.CorreoElectronico;

                        // Si es un cliente, obtener también el teléfono
                        var cliente = usuario.cliente.FirstOrDefault();
                        if (cliente != null)
                        {
                            model.Phone = cliente.Telefono;
                        }

                        // Indicar que el formulario está prellenado
                        ViewBag.UserName = usuario.Nombre;
                        ViewBag.UsuarioID = usuarioId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Si hay error obteniendo los datos, simplemente mostrar formulario vacío
                System.Diagnostics.Debug.WriteLine($"Error al prellenar datos: {ex.Message}");
            }

            return View(model);
        }

        // POST: /Home/Contact
        [HttpPost]
        [Authorize] // Solo usuarios autenticados
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el UsuarioID (OBLIGATORIO ahora)
                    var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                    {
                        ModelState.AddModelError("", "Error: No se pudo identificar al usuario.");
                        return View(model);
                    }

                    int usuarioId = int.Parse(userIdClaim.Value);

                    // Crear el registro de contacto
                    var contacto = new Contactos
                    {
                        Nombre = model.Name,
                        Email = model.Email,
                        Telefono = model.Phone,
                        Asunto = model.Subject,
                        Mensaje = model.Message,
                        FechaEnvio = DateTime.Now,
                        Estado = "Pendiente",
                        UsuarioID = usuarioId // SIEMPRE tiene UsuarioID
                    };

                    db.Contactos.Add(contacto);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "¡Mensaje enviado correctamente! Nos pondremos en contacto pronto.";
                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar el mensaje: {ex.Message}");
                }
            }

            // Si hay errores, mantener el nombre de usuario en ViewBag
            ViewBag.UserName = User.Identity.Name;

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Cart()
        {
            return View();
        }
    }
}