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
using Microsoft.AspNet.Identity;

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

        [Authorize]
        public ActionResult MisPedidos()
        {
            bool esAdministrador = EsAdministrador();

            if (!esAdministrador && !TryGetUsuarioActualId(out var usuarioId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var pedidosQuery = db.pedido
                .Include(p => p.usuario)
                .Include(p => p.restaurante)
                .Include(p => p.historialpedido);

            if (!esAdministrador)
            {
                pedidosQuery = pedidosQuery.Where(p => p.UsuarioID == usuarioId);
            }

            var pedidos = pedidosQuery
                .OrderByDescending(p => p.FechaHora ?? DateTime.MinValue)
                .ThenByDescending(p => p.PedidoID)
                .ToList();

            ViewBag.EsAdministrador = esAdministrador;
            return View("MisPedidos", pedidos);
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
                int? usuarioActualId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var userIdString = User.Identity.GetUserId();
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out var parsedUserId))
                    {
                        usuarioActualId = parsedUserId;
                        pedido.UsuarioID = parsedUserId;
                    }
                }

                db.pedido.Add(pedido);
                db.SaveChanges();

                var historial = new historialpedido
                {
                    PedidoID = pedido.PedidoID,
                    EstadoAnterior = "Creado",
                    EstadoNuevo = string.IsNullOrWhiteSpace(pedido.Estado) ? "Creado" : pedido.Estado,
                    FechaCambio = DateTime.Now,
                    UsuarioModificadorID = usuarioActualId ?? pedido.UsuarioID
                };

                db.historialpedido.Add(historial);
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

        private bool TryGetUsuarioActualId(out int usuarioId)
        {
            usuarioId = 0;
            var userIdString = User?.Identity?.GetUserId();
            return !string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out usuarioId);
        }

        private bool EsAdministrador()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var rolClaim = claimsIdentity?.FindFirst(ClaimTypes.Role);

                if (rolClaim != null && int.TryParse(rolClaim.Value, out var rolId))
                {
                    var rol = db.rol.FirstOrDefault(r => r.RolID == rolId);
                    return rol?.PermisoID == 1;
                }
            }

            return false;
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
