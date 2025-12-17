using System.Web.Mvc;

namespace PedidosManejo.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: /Checkout
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}
