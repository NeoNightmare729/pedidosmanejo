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
    public class pedidoesController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();

        // GET: pedidoes
        public ActionResult Index()
        {
            var pedido = db.pedido.Include(p => p.usuario).Include(p => p.restaurante);
            return View(pedido.ToList());
        }

        // GET: pedidoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // GET: pedidoes/Create
        public ActionResult Create()
        {
            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre");
            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre");
            return View();
        }

        // POST: pedidoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PedidoID,UsuarioID,RestauranteID,FechaHora,Estado,Total")] pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.pedido.Add(pedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", pedido.UsuarioID);
            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre", pedido.RestauranteID);
            return View(pedido);
        }

        // GET: pedidoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", pedido.UsuarioID);
            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre", pedido.RestauranteID);
            return View(pedido);
        }

        // POST: pedidoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PedidoID,UsuarioID,RestauranteID,FechaHora,Estado,Total")] pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UsuarioID = new SelectList(db.usuario, "UsuarioID", "Nombre", pedido.UsuarioID);
            ViewBag.RestauranteID = new SelectList(db.restaurante, "RestauranteID", "Nombre", pedido.RestauranteID);
            return View(pedido);
        }

        // GET: pedidoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            pedido pedido = db.pedido.Find(id);
            db.pedido.Remove(pedido);
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
