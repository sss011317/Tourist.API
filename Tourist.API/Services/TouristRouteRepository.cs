using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Database;
using Tourist.API.Models;

namespace Tourist.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;

        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
            return _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefault(n => n.Id == touristRouteId);
        }

        public IEnumerable<TouristRoute> GetTouristRoutes(
            string keyword, 
            string ratingOperator, 
            int? ratingValue)
        {
            //IQueryable 實際上是c# LINQ to SQL 語句的返回類型 簡單來說 IQueryable可以疊加處理LINQ語句，最後統一訪問資料庫，處理過程也較延遲執行
            //每次進行資料庫操作實際上都是1次IO操作，而系統中的IO操作開銷是最大也是最浪費時間
            //而IQueryable就是為SQL設計的，他在創建時是以Expression Tree(表達式目錄形式構成)，有就是可以不斷地給這棵樹新的表達式，之後再一次表所有表達式轉換給SQL語言，統一進行
            IQueryable<TouristRoute> result = _context
                .TouristRoutes
                .Include(t => t.TouristRoutePictures);
            //上述該行只是生成了SQL語句，並沒有執行查詢操作
            if(!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));
            }
            if (ratingValue >= 0)
            {
                switch (ratingOperator)
                {
                    case "largerThan":
                        result = result.Where(t => t.Rating >= ratingValue);
                        break;
                    case "lessThan":
                        result = result.Where(t => t.Rating <= ratingValue);
                        break;
                    case "equalTo":
                        result = result.Where(t => t.Rating == ratingValue);
                        break;
                }
            }
            //include函數為entityFramework中連接兩張表的方法 表示兩張表通過外部進行連接
            //join函數為不通過外界而是手動表連接的屬性
            //通過上述兩種方法可以進行立即加載(Eager Load)
            //另外entityFramework也提供另一種加載方式延遲加載(Lazy Load)，也就是不用join或include進行表連接
            return result.ToList();
            //ToList為IQueryable內建函數，通過調用此函數，就會執行資料庫的訪問，而查詢出來的數據類型則不是IQueryable而是<TouristRoute>類型
        }

        public bool TouristRouteExists(Guid touristRouteId)
        {
            //只要輸入的ID能有任何返回的數據，則這個ANY就返回true

            return _context.TouristRoutes.Any(t => t.Id == touristRouteId);
        }
        public IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId)
        {
            return _context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId).ToList();
        }
        public TouristRoutePicture GetPicture(int pictureId)
        {
            return _context.TouristRoutePictures.Where(p => p.Id ==pictureId).FirstOrDefault();
        }
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
            //_context.SaveChanges();  保存至資料庫
        }
        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if(touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRouteId));
            }
            if(touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }
        public bool Save()
        {
            return (_context.SaveChanges() >=0);
        }
    }
}
