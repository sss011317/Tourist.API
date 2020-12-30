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
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Tourist.API.Models;

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
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        //����token���o���� �u���ڭ̫�ݵo����token�~�|�Q����
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        //����token��������
                        ValidateAudience = true,
                        ValidAudience = Configuration["Authentication:Audience"],
                        //����token�O�_�L��
                        ValidateLifetime = true,
                        //�N�t�m��󪺨p�_�Ƕi�ӡA�öi��[�K
                        IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                    };
                });
            //���U�ڭ̦b�Ы�API���ɭԦVIOC���e�����K�[�@�Ӯج[�A��
            //�ڭ̻ݭn�baddControllers�Ѽƪ��ӹﱱ��i��t�m�A�q�ӱҰʹ�media Type���B�z
            services.AddControllers(setupAction => {
                //setupAction.ReturnHttpNotAcceptable = false;  //�q�{�]�m�O�o�� �ڭ̩Ҧ���API���|�����ШD��header MediaType���w�q�A���|�^�_�Τ@���ƾڵ��c(JSON)
                setupAction.ReturnHttpNotAcceptable = true;

                //�ڭ̥i�H�z�LOutputFormatters ������K�[��XML����� �Ъ`�N������X���榡�A���o�Odotnet Core�ª����ϥΤ�k�A�ثe�D�y�N�u�ݭn�bConfigutrServices �� class�s�W �N�i�H�P�ɺ�����J�P��X�\��
                //setupAction.OutputFormatters.Add( 
                //    new XmlDataContractSerializerOutputFormatter()
                //    );
            })
            .AddNewtonsoftJson(setupAction =>
            {
                //�Njson patch�ج[�w�˶i�t��
                //�ݭn�t�m���N�Ojson�ǦC�Ƴ]�m
                setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction=> //��return 400�����~��令422(������ҥ��� 422 unprocessable entity)
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetail = new ValidationProblemDetails(context.ModelState) 
                    {
                        Type="�L�ҿ�",
                        Title="������ҥ���",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail="�ЬݸԲӤ��e",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetail) 
                    {
                        ContentTypes = { "application/problem+json"}
                    };
                };
            }); //�D�k�ҫ��T���u�t�A²��ӻ��N�O���Ҹ�ƬO�_�D�k���@�ӹL�{

            services.AddTransient<ITouristRouteRepository,TouristRouteRepository>();
            //services.AddTransient  �C�@���o�_�ШD�A�Ыؤ@�ӥ��s���ƾڭܮw�A�ШD�����|���P�ӭܮw
            //services.AddSingleton  �t�αҥήɡA�u�Ыؤ@�Ӽƾڭܮw�A���᭫�ƨϥ�(²�����Τ�K�޲z�A���s�e�Τ֮Ĳv��)
            //services.AddScoped     ��X�H�W��ӡA�i���w�@�t�C�ާ@��X�_�ӨϥΡA���������A�۹�ϥΤW������
            services.AddDbContext<AppDbContext>(option => {
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });
            //AutoMapper�A�Ȩ̿�`�J����: AutoMapper�|�۰ʱ��y�{���X�̩Ҧ��]�t�M�g���Y��profile���[����AppDomain.CurrentDomain.GetAssemblies()
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //UseRouting�A�b��
            app.UseRouting();
            //UseAuthentication�A�O��
            app.UseAuthentication();
            //UseAuthorization�A���v���O����
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
