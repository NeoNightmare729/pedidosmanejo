using PedidosManejo.Filters;
using PedidosManejo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace PedidosManejo.Controllers
{
    public class menusController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: menus
        public ActionResult Index()
        {
            var menus = db.menu
                          .Include(m => m.restaurante)
                          .Where(m => m.Estado == "Disponible")
                          .OrderBy(m => m.NombrePlato)
                          .ToList();

            ViewBag.EsAdministrador = EsAdministrador();
            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre");

            return View(menus);
        }

        // Método para verificar si el usuario actual es administrador
        private bool EsAdministrador()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var rolClaim = claimsIdentity?.FindFirst(ClaimTypes.Role);

                if (rolClaim != null && rolClaim.Value == "1")
                {
                    return true;
                }
            }
            return false;
        }

        // GET: menus/Create
        [Authorize(Roles = "1")]
        public ActionResult Create()
        {
            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre");
            return View();
        }

        // POST: menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public ActionResult Create([Bind(Include = "MenuID,RestauranteID,NombrePlato,Descripcion,Precio,Imagen,Estado")] menu menu, HttpPostedFileBase imagenArchivo)
        {
            if (ModelState.IsValid)
            {
                if (imagenArchivo != null && imagenArchivo.ContentLength > 0)
                {
                    using (var binaryReader = new System.IO.BinaryReader(imagenArchivo.InputStream))
                    {
                        menu.Imagen = binaryReader.ReadBytes(imagenArchivo.ContentLength);
                    }
                }

                if (string.IsNullOrEmpty(menu.Estado))
                {
                    menu.Estado = "Disponible";
                }

                db.menu.Add(menu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre", menu.RestauranteID);
            return View(menu);
        }

        // GET: menus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            menu menu = db.menu.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // GET: menus/Edit/5
        [Authorize(Roles = "1")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            menu menu = db.menu
                          .Include(m => m.restaurante)
                          .FirstOrDefault(m => m.MenuID == id);

            if (menu == null)
            {
                return HttpNotFound();
            }

            ViewBag.RestauranteID = new SelectList(
                db.restaurante.OrderBy(r => r.Nombre),
                "RestauranteID",
                "Nombre",
                menu.RestauranteID
            );

            return View(menu);
        }

        // POST: menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public ActionResult Edit(int id, HttpPostedFileBase imagenArchivo)
        {
            var menuExistente = db.menu.Find(id);
            if (menuExistente == null)
            {
                return HttpNotFound();
            }

            if (TryUpdateModel(menuExistente, new string[] {
                "RestauranteID",
                "NombrePlato",
                "Descripcion",
                "Precio",
                "Estado"
            }))
            {
                try
                {
                    // Procesar nueva imagen si se subió
                    if (imagenArchivo != null && imagenArchivo.ContentLength > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = System.IO.Path.GetExtension(imagenArchivo.FileName).ToLower();

                        if (extensionesPermitidas.Contains(extension))
                        {
                            if (imagenArchivo.ContentLength <= 5 * 1024 * 1024)
                            {
                                using (var binaryReader = new System.IO.BinaryReader(imagenArchivo.InputStream))
                                {
                                    menuExistente.Imagen = binaryReader.ReadBytes(imagenArchivo.ContentLength);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "La imagen no debe superar los 5MB.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Solo se permiten archivos de imagen (jpg, jpeg, png, gif).");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        db.Entry(menuExistente).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["SuccessMessage"] = "El menú se actualizó correctamente.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
                }
            }

            ViewBag.RestauranteID = new SelectList(
                db.restaurante.OrderBy(r => r.Nombre),
                "RestauranteID",
                "Nombre",
                menuExistente.RestauranteID
            );

            return View(menuExistente);
        }

        // GET: menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            menu menu = db.menu.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            menu menu = db.menu.Find(id);
            db.menu.Remove(menu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}