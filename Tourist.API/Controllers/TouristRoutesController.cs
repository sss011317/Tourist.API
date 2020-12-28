using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Services;
using AutoMapper;
namespace Tourist.API.Controllers
{
    [Route("api/[controller]")]  //api/TouristRoutes
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        //透過構建函數注入數據倉庫服務
        private readonly IMapper _mapper;
        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
            )
        {
            //在構建函數的參數中 通過傳入旅遊倉庫的街口 來注入 旅遊倉庫的實例
            //最後在構建函數中 給私有倉庫賦值
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet,HttpHead]
        
        public IActionResult GetTouristRoutes()
        {
           var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes();
            if(touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("沒有旅遊路線");
            }

            //MAP支持列表映射，所以使用IEnumerable，而類型就是TouristRouteDto，最後加上數據源touristRoutesFromRepo
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesDto);
        }
        

        //使用RESTful思想設計一個單一資源，最佳的實踐方式是在URL中使用名詞狀態的複數型，緊接著協槓ID.

        // api/touristroutes/{touristRouteId}
        //控制器的URL會映射到這個的前半部分也就是api/touristroutes
        //而我們的action函數只要後半部分
        //由於該這個參數要映射URL的動態變量所以要加上大括號，而該參數會被映射到GetTouristRouteById的參數中
        //另外需要注意touristRouteId可能會被輸入各種不同類型，不僅僅限於GUID，所以為了避免奇異，可在路由中加入類型的匹配 ":Guid"
        [HttpGet("{touristRouteId:Guid}"),HttpHead]
       
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"沒有旅遊路線{touristRouteId}");
            }

            //var touristRotueDto = new TouristRouteDto()
            //{
            //    Id = touristRouteFromRepo.Id,
            //    Title = touristRouteFromRepo.Title,
            //    Description = touristRouteFromRepo.Description,
            //    Price = touristRouteFromRepo.OriginalPrice * (decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
            //    CreateTime = touristRouteFromRepo.CreateTime,
            //    UpdateTime = touristRouteFromRepo.UpdateTime,
            //    Features = touristRouteFromRepo.Features,
            //    Fees = touristRouteFromRepo.Fees,
            //    Notes = touristRouteFromRepo.Notes,
            //    Rating = touristRouteFromRepo.Rating,
            //    TravelDays = touristRouteFromRepo.TravelDays.ToString(),
            //    TripType = touristRouteFromRepo.TripType.ToString(),
            //    DepartureCity = touristRouteFromRepo.DepartureCity.ToString()
            //};
            var touristRotueDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRotueDto);

        }
    }
}
