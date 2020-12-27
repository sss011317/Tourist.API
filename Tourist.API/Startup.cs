using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Services;
using Tourist.API.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Tourist.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<AppDbContext>(optoin => {
                optoin.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });
            services.AddTransient<ITouristRouteRepository,TouristRouteRepository>();
            //services.AddTransient  每一次發起請求，創建一個全新的數據倉庫，請求結束會註銷該倉庫
            //services.AddSingleton  系統啟用時，只創建一個數據倉庫，之後重複使用(簡單應用方便管理，內存占用少效率高)
            //services.AddScoped     綜合以上兩個，可指定一系列操作整合起來使用，之後關閉，相對使用上較複雜
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
