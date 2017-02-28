using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Vibe.Startup))]
namespace Vibe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
