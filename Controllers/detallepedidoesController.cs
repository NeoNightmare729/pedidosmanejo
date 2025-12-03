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
    public class detallepedidoesController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: detallepedidoes
        public ActionResult Index()
        {
            var detallepedido = db.detallepedido.Include(d => d.pedido).Include(d => d.menu);
            return View(detallepedido.ToList());
        }

        // GET: detallepedidoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detallepedido detallepedido = db.detallepedido.Find(id);
            if (detallepedido == null)
            {
                return HttpNotFound();
            }
            return View(detallepedido);
        }

        // GET: detallepedidoes/Create
        public ActionResult Create()
        {
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado");
            ViewBag.MenuID = new SelectList(db.menu, "MenuID", "NombrePlato");
            return View();
        }

        // POST: detallepedidoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DetallePedidoID,PedidoID,MenuID,Cantidad,PrecioUnitario,Subtotal")] detallepedido detallepedido)
        {
            if (ModelState.IsValid)
            {
                db.detallepedido.Add(detallepedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", detallepedido.PedidoID);
            ViewBag.MenuID = new SelectList(db.menu, "MenuID", "NombrePlato", detallepedido.MenuID);
            return View(detallepedido);
        }

        // GET: detallepedidoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detallepedido detallepedido = db.detallepedido.Find(id);
            if (detallepedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", detallepedido.PedidoID);
            ViewBag.MenuID = new SelectList(db.menu, "MenuID", "NombrePlato", detallepedido.MenuID);
            return View(detallepedido);
        }

        // POST: detallepedidoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DetallePedidoID,PedidoID,MenuID,Cantidad,PrecioUnitario,Subtotal")] detallepedido detallepedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detallepedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", detallepedido.PedidoID);
            ViewBag.MenuID = new SelectList(db.menu, "MenuID", "NombrePlato", detallepedido.MenuID);
            return View(detallepedido);
        }

        // GET: detallepedidoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detallepedido detallepedido = db.detallepedido.Find(id);
            if (detallepedido == null)
            {
                return HttpNotFound();
            }
            return View(detallepedido);
        }

        // POST: detallepedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            detallepedido detallepedido = db.detallepedido.Find(id);
            db.detallepedido.Remove(detallepedido);
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
