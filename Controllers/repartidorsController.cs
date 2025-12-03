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
    public class repartidorsController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: repartidors
        public ActionResult Index()
        {
            var repartidor = db.repartidor.Include(r => r.usuario);
            return View(repartidor.ToList());
        }

        // GET: repartidors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            repartidor repartidor = db.repartidor.Find(id);
            if (repartidor == null)
            {
                return HttpNotFound();
            }
            return View(repartidor);
        }

        // GET: repartidors/Create
        public ActionResult Create()
        {
            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre");
            return View();
        }

        // POST: repartidors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RepartidorID,UsuarioID,Estado,Vehiculo")] repartidor repartidor)
        {
            if (ModelState.IsValid)
            {
                db.repartidor.Add(repartidor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", repartidor.UsuarioID);
            return View(repartidor);
        }

        // GET: repartidors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            repartidor repartidor = db.repartidor.Find(id);
            if (repartidor == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", repartidor.UsuarioID);
            return View(repartidor);
        }

        // POST: repartidors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RepartidorID,UsuarioID,Estado,Vehiculo")] repartidor repartidor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repartidor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", repartidor.UsuarioID);
            return View(repartidor);
        }

        // GET: repartidors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            repartidor repartidor = db.repartidor.Find(id);
            if (repartidor == null)
            {
                return HttpNotFound();
            }
            return View(repartidor);
        }

        // POST: repartidors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            repartidor repartidor = db.repartidor.Find(id);
            db.repartidor.Remove(repartidor);
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
