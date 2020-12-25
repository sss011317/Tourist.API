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

namespace Tourist.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<ITouristRouteRepository,MockTouristRouteRepository>();
            //services.AddTransient  �C�@���o�_�ШD�A�Ыؤ@�ӥ��s���ƾڭܮw�A�ШD�����|���P�ӭܮw
            //services.AddSingleton  �t�αҥήɡA�u�Ыؤ@�Ӽƾڭܮw�A���᭫�ƨϥ�(²�����Τ�K�޲z�A���s�e�Τ֮Ĳv��)
            //services.AddScoped     ��X�H�W��ӡA�i���w�@�t�C�ާ@��X�_�ӨϥΡA���������A�۹�ϥΤW������
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
