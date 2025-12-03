using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PedidosManejo.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace PedidosManejo.Controllers
{
    public class AccountController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Buscar usuario por correo electrónico
                var usuario = db.usuario.FirstOrDefault(u => u.CorreoElectronico == model.Email);
                if (usuario != null)
                {
                    // Verificar contraseña (texto plano por ahora)
                    if (usuario.Contraseña == model.Password)
                    {
                        if (usuario.Estado?.ToLower() != "activo")
                        {
                            ModelState.AddModelError("", "La cuenta no está activa.");
                            return View(model);
                        }

                        // Crear identidad del usuario
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                            new Claim(ClaimTypes.Name, usuario.Nombre),
                            new Claim(ClaimTypes.Email, usuario.CorreoElectronico),
                            new Claim(ClaimTypes.Role, usuario.RolID?.ToString() ?? "0")
                        }, "ApplicationCookie");

                        var authenticationManager = HttpContext.GetOwinContext().Authentication;
                        authenticationManager.SignIn(new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        }, identity);

                        return RedirectToLocal(returnUrl);
                    }
                }

                ModelState.AddModelError("", "Correo electrónico o contraseña incorrectos.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al iniciar sesión: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el correo ya existe
                    var usuarioExistente = db.usuario.FirstOrDefault(u => u.CorreoElectronico == model.Email);
                    if (usuarioExistente != null)
                    {
                        ModelState.AddModelError("Email", "Este correo electrónico ya está registrado.");
                        return View(model);
                    }

                    // Crear nuevo usuario con RolID = 2 (Cliente)
                    var nuevoUsuario = new usuario
                    {
                        Nombre = model.Nombre,
                        CorreoElectronico = model.Email,
                        Contraseña = model.Password, // TODO: Hashear la contraseña en producción
                        RolID = 2, // ✅ Automáticamente asigna RolID = 2
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    };

                    db.usuario.Add(nuevoUsuario);
                    db.SaveChanges();

                    // Crear el registro en la tabla Cliente
                    var nuevoCliente = new cliente
                    {
                        UsuarioID = nuevoUsuario.UsuarioID,
                        Telefono = model.Telefono,
                        Direccion = model.Direccion
                    };

                    db.cliente.Add(nuevoCliente);
                    db.SaveChanges();

                    // Autenticar automáticamente después del registro
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, nuevoUsuario.UsuarioID.ToString()),
                        new Claim(ClaimTypes.Name, nuevoUsuario.Nombre),
                        new Claim(ClaimTypes.Email, nuevoUsuario.CorreoElectronico),
                        new Claim(ClaimTypes.Role, "2")
                    }, "ApplicationCookie");

                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, identity);

                    // Redirigir al home después del registro exitoso
                    TempData["SuccessMessage"] = "¡Bienvenido! Tu cuenta ha sido creada exitosamente.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar: " + ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut("ApplicationCookie");
            return RedirectToAction("Index", "Home");
        }
        [Authorize] // Solo usuarios autenticados
        public ActionResult RegisterRepartidor()
        {
            // Verificar que el usuario tenga PermisoID = 1 (Administrador/Gerente)
            var usuarioId = int.Parse(User.Identity.GetUserId());
            var usuario = db.usuario.Include("rol.permisos").FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (usuario == null || usuario.rol == null || usuario.rol.PermisoID != 1)
            {
                TempData["ErrorMessage"] = "No tienes permisos para registrar repartidores.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Account/RegisterRepartidor
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterRepartidor(RegisterRepartidorViewModel model)
        {
            // Verificar permisos nuevamente
            var usuarioId = int.Parse(User.Identity.GetUserId());
            var usuarioActual = db.usuario.Include("rol.permisos").FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (usuarioActual == null || usuarioActual.rol == null || usuarioActual.rol.PermisoID != 1)
            {
                TempData["ErrorMessage"] = "No tienes permisos para registrar repartidores.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el correo ya existe
                    var usuarioExistente = db.usuario.FirstOrDefault(u => u.CorreoElectronico == model.Email);
                    if (usuarioExistente != null)
                    {
                        ModelState.AddModelError("Email", "Este correo electrónico ya está registrado.");
                        return View(model);
                    }

                    // Buscar el RolID de "Repartidor"
                    var rolRepartidor = db.rol.FirstOrDefault(r => r.NombreRol == "Repartidor");
                    if (rolRepartidor == null)
                    {
                        ModelState.AddModelError("", "Error: No se encontró el rol de Repartidor en el sistema.");
                        return View(model);
                    }

                    // Crear nuevo usuario con rol de Repartidor
                    var nuevoUsuario = new usuario
                    {
                        Nombre = model.Nombre,
                        CorreoElectronico = model.Email,
                        Contraseña = model.Password, // TODO: Hashear la contraseña en producción
                        RolID = rolRepartidor.RolID, // Asigna el RolID de Repartidor
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    };

                    db.usuario.Add(nuevoUsuario);
                    db.SaveChanges();

                    // Crear el registro en la tabla Repartidor
                    var nuevoRepartidor = new repartidor
                    {
                        UsuarioID = nuevoUsuario.UsuarioID,
                        Vehiculo = model.Vehiculo,
                        Estado = model.Estado ?? "Disponible"
                    };

                    db.repartidor.Add(nuevoRepartidor);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = $"Repartidor {model.Nombre} registrado exitosamente.";
                    return RedirectToAction("Index", "repartidors");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar repartidor: " + ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        // Métodos auxiliares
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}