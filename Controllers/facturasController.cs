using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using PedidosManejo.Models;

namespace PedidosManejo.Controllers
{
    public partial class facturasController : Controller
    {
        private SQLmanejopedidosEntities1 db = new SQLmanejopedidosEntities1();        // DTO local para recibir el carrito
        public class CartItemDto
        {
            public int id { get; set; }
            public string name { get; set; }
            public decimal price { get; set; }
            public int quantity { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(string cartJson)
        {
            if (string.IsNullOrWhiteSpace(cartJson))
                return Json(new { success = false, message = "Carrito vacío" });

            var items = JsonConvert.DeserializeObject<List<CartItemDto>>(cartJson);
            if (items == null || items.Count == 0)
                return Json(new { success = false, message = "Carrito vacío" });

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    // Calcular totales
                    decimal subtotal = items.Sum(i => i.price * i.quantity);
                    decimal shipping = subtotal > 0 ? 2.50m : 0m;
                    decimal tax = subtotal * 0.12m;
                    decimal total = subtotal + shipping + tax;

                    // Obtener usuario por nombre (ajusta según cómo guardes la identidad)
                    var usuario = db.usuario.FirstOrDefault(u => u.CorreoElectronico == User.Identity.Name);

                    // Crear pedido
                    var pedido = new pedido
                    {
                        UsuarioID = usuario?.UsuarioID,
                        FechaHora = DateTime.Now,
                        Estado = "Pendiente",
                        Total = total
                    };
                    db.pedido.Add(pedido);
                    db.SaveChanges();

                    // Crear detallepedido (si tienes MenuIDs reales, pásalos desde el cliente)
                    foreach (var it in items)
                    {
                        var detalle = new detallepedido
                        {
                            PedidoID = pedido.PedidoID,
                            MenuID = null, // si tienes menu IDs, mapéalos desde it.id
                            Cantidad = it.quantity,
                            PrecioUnitario = it.price,
                            Subtotal = it.price * it.quantity
                        };
                        db.detallepedido.Add(detalle);
                    }
                    db.SaveChanges();

                    // Crear factura
                    var factura = new factura
                    {
                        PedidoID = pedido.PedidoID,
                        UsuarioID = usuario?.UsuarioID,
                        Total = total,
                        Fecha = DateTime.Now,
                        MetodoPago = "Pago Web"
                    };
                    db.factura.Add(factura);
                    db.SaveChanges();

                    tx.Commit();

                    // Devolver URL de redirección para que el cliente la use
                    return Json(new { success = true, redirectUrl = Url.Action("Details", "facturas", new { id = factura.FacturaID }) });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    // loguear ex, devolver error amigable
                    return Json(new { success = false, message = "Error procesando el pedido" });
                }
            }
        }
    }
}
