using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tourist.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Tourist.API.Database
{
    //<IdentityUser>  指的是身分認證的資料庫結構，相當於usermodel(用戶模型)，他定義了用戶的基本訊息，像用戶的ID、姓名、hash過的密碼等等
    //這些內容的定義都是由dotnetCore自動完成，也同樣會被映射到資料庫中，有了資料庫結構以後，IdentityDbContext還會自動為我們的系統添加
    //與資料庫用戶表相對應的映射關係，如果資料庫的用戶表UserTable還不存在的話，IdentityDbContext還可以幫我們更新資料庫，自動添加資料表
    public class AppDbContext : IdentityDbContext<ApplicationUser> //DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //添加種子數據
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //    Id = new Guid("1953b1f1-72ef-49fa-b63f-6fd228dcbe5a"),
            //    Title = "Test",
            //    Description = "Description",
            //    OriginalPrice = 0,
            //    CreateTime = DateTime.UtcNow,
            //    //TripType=null,
            //    //TravelDays=null,
            //    //DepartueCity=null,

            //});
            //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) <= 此為查詢該專案的文件夾地址
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + (@"/Database/touristRoutesMockData.json"));
            //反序列化的類型就是列表的類型
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);




            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + (@"/Database/touristRoutePicturesMockData.json"));
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            //初始化用戶與角色的種子數據
            //1.更新用戶與角色的鍵
            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles)  //u.HasMany 表示一對多的關係
                .WithOne().HasForeignKey(ur => ur.UserId).IsRequired()
            );
            //2.添加管理員角色
            var adminRoleId = "308660dc-ae51-480f-824d-7dca6714c3e2";
            modelBuilder.Entity<IdentityRole>().HasData(
                    new IdentityRole()
                    {
                        Id = adminRoleId,
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper()
                    });
            //3.添加用戶
            var adminUserId = "90184155-dee0-40c9-bb1e-b5ed07afc04e";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@tourist.com",
                NormalizedUserName = "admin@tourist.com".ToUpper(),
                Email = "admin@tourist.com",
                NormalizedEmail = "admin@tourist.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false
            };
            var ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            //4.給用戶加入管理員角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                    new IdentityUserRole<string>()
                    {
                        RoleId = adminRoleId,
                        UserId = adminUserId
                    }
                );



            base.OnModelCreating(modelBuilder);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .EnableSensitiveDataLogging()
        //        .UseSqlServer(@"Data Source = localhost; Persist Security Info = True; User ID = SA; Password = PaSSword12!; ");
        //}
    }
}

