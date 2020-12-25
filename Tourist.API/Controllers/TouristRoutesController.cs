using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Services;

namespace Tourist.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : Controller
    {
        private ITouristRouteRepository _touristRouteRepository;
        //透過構建函數注入數據倉庫服務
        public TouristRoutesController(ITouristRouteRepository touristRouteRepository)
        {
            //在構建函數的參數中 通過傳入旅遊倉庫的街口 來注入 旅遊倉庫的實例


            //最後在構建函數中 給私有倉庫賦值
            _touristRouteRepository = touristRouteRepository;
        }

        public IActionResult GetTouristRoutes()
        {
           var routes = _touristRouteRepository.GetTouristRoutes();
            return Ok(routes);
        }
    }
}
