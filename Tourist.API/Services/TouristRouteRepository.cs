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

        public IEnumerable<TouristRoute> GetTouristRoutes()
        {
            //include函數為entityFramework中連接兩張表的方法 表示兩張表通過外部進行連接
            //join函數為不通過外界而是手動表連接的屬性
            //通過上述兩種方法可以進行立即加載(Eager Load)
            //另外entityFramework也提供另一種加載方式延遲加載(Lazy Load)，也就是不用join或include進行表連接
            return _context.TouristRoutes.Include(t => t.TouristRoutePictures);
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
    }
}
