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
using Microsoft.AspNetCore.Mvc.Formatters;
using AutoMapper;

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
            //幫助我們在創建API的時候向IOC的容器中添加一個框架服務
            //我們需要在addControllers參數的來對控制器進行配置，從而啟動對media Type的處理
            services.AddControllers(setupAction => {
                //setupAction.ReturnHttpNotAcceptable = false;  //默認設置是這樣 我們所有的API都會忽略請求的header MediaType的定義，都會回復統一的數據結構(JSON)
                setupAction.ReturnHttpNotAcceptable = true;

                //我們可以透過OutputFormatters 給控制器添加對XML的支持 請注意此為輸出的格式，但這是dotnet Core舊版的使用方法，目前主流就只需要在ConfigutrServices 的 class新增 就可以同時滿足輸入與輸出功能
                //setupAction.OutputFormatters.Add( 
                //    new XmlDataContractSerializerOutputFormatter()
                //    );
            }).AddXmlDataContractSerializerFormatters(); 

            services.AddTransient<ITouristRouteRepository,TouristRouteRepository>();
            //services.AddTransient  每一次發起請求，創建一個全新的數據倉庫，請求結束會註銷該倉庫
            //services.AddSingleton  系統啟用時，只創建一個數據倉庫，之後重複使用(簡單應用方便管理，內存占用少效率高)
            //services.AddScoped     綜合以上兩個，可指定一系列操作整合起來使用，之後關閉，相對使用上較複雜
            services.AddDbContext<AppDbContext>(option => {
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });
            //AutoMapper服務依賴注入機制: AutoMapper會自動掃描程式碼裡所有包含映射關係的profile文件加載到AppDomain.CurrentDomain.GetAssemblies()
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
