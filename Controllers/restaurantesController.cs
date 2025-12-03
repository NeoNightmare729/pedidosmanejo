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

namespace PedidosManejo.Controllers
{
    [Prohibido]
    public class restaurantesController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: restaurantes
        public ActionResult Index()
        {
            var restaurante = db.restaurante.Include(r => r.usuario);
            return View(restaurante.ToList());
        }

        // GET: restaurantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            restaurante restaurante = db.restaurante.Find(id);
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // GET: restaurantes/Create
        public ActionResult Create()
        {
            ViewBag.PropietarioID = new SelectList(db.usuario, "UsuarioID", "Nombre");
            return View();
        }

        // POST: restaurantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RestauranteID,Nombre,Direccion,Descripcion,Categoria,Imagen,PropietarioID")] restaurante restaurante)
        {
            if (ModelState.IsValid)
            {
                db.restaurante.Add(restaurante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PropietarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", restaurante.PropietarioID);
            return View(restaurante);
        }

        // GET: restaurantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            restaurante restaurante = db.restaurante.Find(id);
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            ViewBag.PropietarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", restaurante.PropietarioID);
            return View(restaurante);
        }

        // POST: restaurantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RestauranteID,Nombre,Direccion,Descripcion,Categoria,Imagen,PropietarioID")] restaurante restaurante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restaurante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PropietarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", restaurante.PropietarioID);
            return View(restaurante);
        }

        // GET: restaurantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            restaurante restaurante = db.restaurante.Find(id);
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // POST: restaurantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            restaurante restaurante = db.restaurante.Find(id);
            db.restaurante.Remove(restaurante);
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
