using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Services;
using AutoMapper;
using System.Text.RegularExpressions;
using Tourist.API.ResourceParameters;
using Tourist.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Tourist.API.Helper;

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
        // api/touristRoutes?keyword=傳入的參數
        [HttpGet, HttpHead]
        public IActionResult GetTouristRoutes(
            [FromQuery] TouristRouteResourceParamaters paramaters
            //[FromQuery] string keyword,
            //string rating //小於lessThan,大於largerThan,等於 equalTo lessThan3, largerThan2,equalTo5
            ) // FromQuery(負責接收YRL的參數) vs FromBody(負責接收請求主體)
        {
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes(paramaters.Keyword, paramaters.RatingOperator, paramaters.RatingValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
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
        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead]
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
            //var 變數 = _mapper.Map<投影數據>(原始數據);
            var touristRotueDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRotueDto);
        }
        [HttpPost]
        //DTO是一種複雜的對象，而ASP.net自帶反序列化的功能，所以會將請求的主體內容解析，並且加載進入參數touristRouteForCreationDto中
        //接下來就可以使用此參數，並用此參數映射進入touristRoute的模型，最後通過數據倉庫來添加數據寫入資料庫
        public IActionResult CreatTouristRote([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            _touristRouteRepository.Save();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = touristRouteToReturn.Id },
                touristRouteToReturn
                );
        }

        [HttpPut("{touristRouteId}")]
        public IActionResult UPdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
            )
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅遊訊息找不到");
            }
            //在數據倉庫中我們調用的是ORM框架在entityFrameWork中
            //在entityFrameWork中，touristRouteFromRepo是根據上下文對象context追中的
            //當我們在執行_mapper.Map這句程式碼時，資料模型的數據其實已經被修改了，而這個時候資料模型的追蹤狀態也就相應發生了變化
            //模型的追蹤狀態是由entity上下文關係對象context自我管理的，當我們使用資料庫保存時模型的追蹤狀態就會隨著context的保存寫入資料庫
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            //1.我們必須把touristRouteFromRepo所有的資料都提取出來，然後映射為DTO
            //2.更新DTO的資料
            //3.把更新後的DTO所有的資料再映射回到資料模型
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            _touristRouteRepository.Save();
            //對於更新的響應，可以回傳200並在響應主體中包含更新後的數據資源，或者回傳204(No Content)返回一個完全不包含任何數據的響應
            return NoContent();
        }
        [HttpPatch("{touristRouteId}")]
        public IActionResult PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
            )
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅遊訊息找不到");
            }

            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            var touristRouteToPacth = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPacth, ModelState);
            //ModelState是通過JSON PATCH的ApplyTo函數與DTO函數進行綁定，至於資料的驗證規則則是由DTO的data annotation來定義
            if (!TryValidateModel(touristRouteToPacth))
            {
                return ValidationProblem(ModelState);
            }
            //輸入數據touristRouteToPacth，輸出數據touristRouteFromRepo
            _mapper.Map(touristRouteToPacth, touristRouteFromRepo);
            _touristRouteRepository.Save();

            return NoContent();
        }
        [HttpDelete("{touristRouteId}")]
        public IActionResult DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅遊訊息找不到");
            }
            var touristRoute = _touristRouteRepository.GetTouristRoute(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            _touristRouteRepository.Save();
            return NoContent();
        }
        [HttpDelete("({touristIDs})")]
        public IActionResult DeleteByIDs(
            //[ModelBinder(binderType:typeof(ArrayModelBinder))] 解析并匹配 GUID
            [ModelBinder(binderType:typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> touristIDs)
        {
            if(touristIDs == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutesByIDList(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            _touristRouteRepository.Save();
            return NoContent();
        }
    }
}
