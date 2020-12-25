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
        IEnumerable<TouristRoute> GetTouristRoutes();
        //返回一個的touristRoute
        TouristRoute GetTouristRoute(Guid touristRouteId);
    }
}
