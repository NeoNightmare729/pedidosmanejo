using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using PedidosManejo.Models;

namespace PedidosManejo.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        //
        // GET: /Manage/Index
        public ActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.usuario.Find(userId);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var model = new ManageIndexViewModel
            {
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                FechaRegistro = usuario.FechaRegistro,
                Rol = usuario.rol?.NombreRol ?? "Sin rol"
            };

            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.usuario.Find(userId);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Verificar contraseña actual
            if (usuario.Contraseña != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "La contraseña actual es incorrecta.");
                return View(model);
            }

            // Actualizar contraseña
            usuario.Contraseña = model.NewPassword;
            db.SaveChanges();

            ViewBag.SuccessMessage = "Su contraseña se ha cambiado correctamente.";
            return View();
        }

        //
        // GET: /Manage/EditProfile
        public ActionResult EditProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.usuario.Find(userId);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        //
        // POST: /Manage/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(usuario model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.usuario.Find(userId);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Actualizar solo los campos permitidos
            usuario.Nombre = model.Nombre;
            usuario.CorreoElectronico = model.CorreoElectronico;

            db.SaveChanges();

            ViewBag.SuccessMessage = "Perfil actualizado correctamente.";
            return View(usuario);
        }

        // Método auxiliar para obtener el ID del usuario actual
        private int? GetCurrentUserId()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}