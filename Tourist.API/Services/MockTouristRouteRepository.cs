//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Tourist.API.Models;

//namespace Tourist.API.Services
//{
//    public class MockTouristRouteRepository : ITouristRouteRepository
//    {
//        private List<TouristRoute> _routes;

//        public MockTouristRouteRepository()
//        {
//           if(_routes == null)
//            {
//                InitializeTouristRoutes();
//            }
//        }
//        public void InitializeTouristRoutes()
//        {
//            _routes = new List<TouristRoute>
//            {
//                new TouristRoute
//            {
//                Id = Guid.NewGuid(),
//                Title = "台北",
//                Description = "台北真好玩",
//                OriginalPrice = 5999,
//                Features = "<p>吃住行</p>",
//                Fees = "<p>交通費用自裡</p>",
//                Notes = "<p>小心危險</p>"
//            },
//            new TouristRoute
//            {
//                Id = Guid.NewGuid(),
//                Title = "基隆",
//                Description = "基隆真好玩",
//                OriginalPrice = 3999,
//                Features = "<p>吃住行</p>",
//                Fees = "<p>交通費用自裡</p>",
//                Notes = "<p>小心危險</p>"
//            }
//        };
            
//        }
//        public TouristRoute GetTouristRoute(Guid touristRouteId)
//        {
//            //linq #必學
//            return _routes.FirstOrDefault(x => x.Id == touristRouteId);
//        }

//        public IEnumerable<TouristRoute> GetTouristRoutes()
//        {
//            return _routes;
//        }
//    }
//}
