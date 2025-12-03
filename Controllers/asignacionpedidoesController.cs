using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PedidosManejo.Models;
using PedidosManejo.Filters;

namespace PedidosManejo.Controllers
{
    [Prohibido]
    public class asignacionpedidoesController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: asignacionpedidoes
        public ActionResult Index()
        {
            var asignacionpedido = db.asignacionpedido.Include(a => a.pedido).Include(a => a.repartidor);
            return View(asignacionpedido.ToList());
        }

        // GET: asignacionpedidoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asignacionpedido asignacionpedido = db.asignacionpedido.Find(id);
            if (asignacionpedido == null)
            {
                return HttpNotFound();
            }
            return View(asignacionpedido);
        }

        // GET: asignacionpedidoes/Create
        public ActionResult Create()
        {
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado");
            ViewBag.RepartidorID = new SelectList(db.repartidor, "RepartidorID", "Estado");
            return View();
        }

        // POST: asignacionpedidoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsignacionID,PedidoID,RepartidorID,Estado")] asignacionpedido asignacionpedido)
        {
            if (ModelState.IsValid)
            {
                db.asignacionpedido.Add(asignacionpedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", asignacionpedido.PedidoID);
            ViewBag.RepartidorID = new SelectList(db.repartidor, "RepartidorID", "Estado", asignacionpedido.RepartidorID);
            return View(asignacionpedido);
        }

        // GET: asignacionpedidoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asignacionpedido asignacionpedido = db.asignacionpedido.Find(id);
            if (asignacionpedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", asignacionpedido.PedidoID);
            ViewBag.RepartidorID = new SelectList(db.repartidor, "RepartidorID", "Estado", asignacionpedido.RepartidorID);
            return View(asignacionpedido);
        }

        // POST: asignacionpedidoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsignacionID,PedidoID,RepartidorID,Estado")] asignacionpedido asignacionpedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asignacionpedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", asignacionpedido.PedidoID);
            ViewBag.RepartidorID = new SelectList(db.repartidor, "RepartidorID", "Estado", asignacionpedido.RepartidorID);
            return View(asignacionpedido);
        }

        // GET: asignacionpedidoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asignacionpedido asignacionpedido = db.asignacionpedido.Find(id);
            if (asignacionpedido == null)
            {
                return HttpNotFound();
            }
            return View(asignacionpedido);
        }

        // POST: asignacionpedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            asignacionpedido asignacionpedido = db.asignacionpedido.Find(id);
            db.asignacionpedido.Remove(asignacionpedido);
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
