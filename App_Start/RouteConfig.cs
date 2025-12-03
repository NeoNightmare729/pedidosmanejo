using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PedidosManejo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // -----------------------------------------------------
            // 🔒 Bloquear acceso directo a controladores auto-generados
            // -----------------------------------------------------
            routes.MapRoute(
                name: "BlockEFControllers",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "NotFound", id = UrlParameter.Optional },
                constraints: new
                {
                    controller = @"^(asignacionpedidos|clientes|detallepedidos|facturas|historialpedidos|menus|pedidos|permisos|repartidores|restaurantes|rols|usuariosController)$"
                }
            );

            // -----------------------------------------------------
            // 🏠 Ruta general por defecto
            // -----------------------------------------------------
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
