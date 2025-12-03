using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PedidosManejo.Startup))]
namespace PedidosManejo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
