using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tourist.API.Models;
namespace Tourist.API.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

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

