using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Models;

namespace Tourist.API.Services
{
    public interface ITouristRouteRepository
    {
        //返回一組的touristRoute
        IEnumerable<TouristRoute> GetTouristRoutes(string keyword,string ratingOperator,int? ratingValue);
        //返回一個的touristRoute
        TouristRoute GetTouristRoute(Guid touristRouteId);

        bool TouristRouteExists(Guid touristRouteId);
        IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId);
        IEnumerable<TouristRoute> GetTouristRoutesByIDList(IEnumerable<Guid> ids);

        TouristRoutePicture GetPicture(int pictureId);

        void AddTouristRoute(TouristRoute touristRoute);
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);

        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        
        bool Save();
    }
}
