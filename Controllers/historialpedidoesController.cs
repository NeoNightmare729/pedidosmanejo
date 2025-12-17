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
    public class historialpedidoesController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: historialpedidoes
        public ActionResult Index()
        {
            var historialpedido = db.historialpedido.Include(h => h.pedido).Include(h => h.usuario);
            return View(historialpedido.ToList());
        }

        // GET: historialpedidoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            historialpedido historialpedido = db.historialpedido.Find(id);
            if (historialpedido == null)
            {
                return HttpNotFound();
            }
            return View(historialpedido);
        }

        // GET: historialpedidoes/Create
        public ActionResult Create()
        {
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado");
            ViewBag.UsuarioModificadorID = new SelectList(db.usuario, "UsuarioID", "Nombre");
            return View();
        }

        // POST: historialpedidoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HistorialID,PedidoID,EstadoAnterior,EstadoNuevo,FechaCambio,UsuarioModificadorID")] historialpedido historialpedido)
        {
            if (ModelState.IsValid)
            {
                db.historialpedido.Add(historialpedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", historialpedido.PedidoID);
            ViewBag.UsuarioModificadorID = new SelectList(db.usuario, "UsuarioID", "Nombre", historialpedido.UsuarioModificadorID);
            return View(historialpedido);
        }

        // GET: historialpedidoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            historialpedido historialpedido = db.historialpedido.Find(id);
            if (historialpedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", historialpedido.PedidoID);
            ViewBag.UsuarioModificadorID = new SelectList(db.usuario, "UsuarioID", "Nombre", historialpedido.UsuarioModificadorID);
            return View(historialpedido);
        }

        // POST: historialpedidoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HistorialID,PedidoID,EstadoAnterior,EstadoNuevo,FechaCambio,UsuarioModificadorID")] historialpedido historialpedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(historialpedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PedidoID = new SelectList(db.pedido, "PedidoID", "Estado", historialpedido.PedidoID);
            ViewBag.UsuarioModificadorID = new SelectList(db.usuario, "UsuarioID", "Nombre", historialpedido.UsuarioModificadorID);
            return View(historialpedido);
        }

        // GET: historialpedidoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            historialpedido historialpedido = db.historialpedido.Find(id);
            if (historialpedido == null)
            {
                return HttpNotFound();
            }
            return View(historialpedido);
        }

        // POST: historialpedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            historialpedido historialpedido = db.historialpedido.Find(id);
            db.historialpedido.Remove(historialpedido);
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
