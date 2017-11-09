using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProcurementErpVendor.Startup))]
namespace ProcurementErpVendor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
