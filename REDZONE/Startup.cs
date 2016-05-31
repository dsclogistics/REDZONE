using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(REDZONE.Startup))]
namespace REDZONE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
