using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using TCC.Data;
using TCC.Filters;
using TCC.Libs.EmailContato;
using TCC.Libs.Login;
using TCC.Libs.Sessao;
using TCC.Repositories;
using TCC.Repositories.Interfaces;

namespace TCC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Google Recaptcha
            services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

            // Tratamento da string de conexão:
            string _defaultConnection = Configuration.GetConnectionString("DefaultConnection");
            _defaultConnection.Replace("//", "");

            char[] delimiterChars = { '/', ':', '@', '?' };
            string[] strConn = _defaultConnection.Split(delimiterChars);
            strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            string _user = strConn[1];
            string _pass = strConn[2];
            string _server = strConn[3];
            string _database = strConn[5];
            string _port = strConn[4];

            string _connectionString = "host=" + _server + ";port=" + _port + ";database=" + _database + ";uid=" + _user + ";pwd=" + _pass + ";sslmode=Require;Trust Server Certificate=true;Timeout=1000";

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<Sessao>();
            services.AddScoped<LoginUsuario>();
            services.AddScoped<UsuarioFilter>();
            services.AddScoped<AdminFilter>();
            services.AddScoped<ContatoEmail>();

            // Injeção de dependência para as views:
            services.AddTransient<LoginUsuario>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();

            // Configurando o Entity FrameworkCore 
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_connectionString));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSession(options => { });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/home/Error404";
                    await next();
                }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}