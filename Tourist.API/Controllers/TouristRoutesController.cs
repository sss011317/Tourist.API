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
using Microsoft.AspNetCore.Authorization;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using System.Dynamic;

namespace Tourist.API.Controllers
{
    [Route("api/[controller]")]  //api/TouristRoutes
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        //透過構建函數注入數據倉庫服務
        private readonly IMapper _mapper;
        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            //IUrlHelperFactory,IActionContextAccessor都是為了urlHelper
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService
            )
        {
            //在構建函數的參數中 通過傳入旅遊倉庫的街口 來注入 旅遊倉庫的實例
            //最後在構建函數中 給私有倉庫賦值
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }
        private string GeneratieTouristRouteResourceURL( //分頁導航
            TouristRouteResourceParamaters paramaters,
            PaginationResourceParamaters paramaters2,
            ResourceUrlType type
            )
        {
            return type switch
            {
                //asp.net中urlhelp專門管理url，urlHelp裡面有個函數link來生成絕對路徑，而名稱就是在action 函數中http內定義的字串
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = paramaters.Fields,
                        orderBy = paramaters.OrderBy,
                        keyword = paramaters.Keyword,
                        rating = paramaters.Rating,
                        pageNumber = paramaters2.PageNumber - 1,
                        pageSize = paramaters2.PageSize
                    }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = paramaters.Fields,
                        orderBy = paramaters.OrderBy,
                        keyword = paramaters.Keyword,
                        rating = paramaters.Rating,
                        pageNumber = paramaters2.PageNumber + 1,
                        pageSize = paramaters2.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = paramaters.Fields,
                        keyword = paramaters.Keyword,
                        rating = paramaters.Rating,
                        pageNumber = paramaters2.PageNumber,
                        pageSize = paramaters2.PageSize
                    })
            };
        }

        // api/touristRoutes?keyword=傳入的參數(from query用法)
        // 1.application/json -> 旅遊路線資源
        // http行業規定自定義媒體類型 名稱為 供應商特定媒體類型(Vendor-specific media type)
        // 2.application/vnd.{公司名稱}.hateoas + json
        // 3.application/vnd.tourist.simplify+json ->輸出簡化版資源數據
        // 4.application/vnd.tourist.simplify.hateoas+json ->輸出簡化版hateoas超媒體資源數據
        [Produces(
            ".application/json",
            "application/vnd.tourist.hateoas+json",
            "application/vnd.tourist.simplify+json",
            "application/vnd.tourist.simplify.hateoas+json"
            )]
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParamaters paramaters, //關鍵字參數處理
            [FromQuery] PaginationResourceParamaters paramaters2, //分頁參數處理器
            [FromHeader(Name ="Accept")] string mediaType
            //[FromQuery] string keyword,
            //string rating //小於lessThan,大於largerThan,等於 equalTo lessThan3, largerThan2,equalTo5
            ) // FromQuery(負責接收YRL的參數) vs FromBody(負責接收請求主體)
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            //如果解析成功，解析後的對象將會通過out關鍵詞 保存在mediaTypeHeaderValue
            {
                return BadRequest();
            }
            //如果字串不合法，不可排序的話 返回badrequest
            if(!_propertyMappingService
                .IsMappingExists<TouristRouteDto,TouristRoute>
                (paramaters.OrderBy))
            {
                return BadRequest("請輸入正確的排序參數");
            }
            if (!_propertyMappingService
                .IsPropertiesExists<TouristRouteDto>(paramaters.Fields))
            {
                return BadRequest("請輸入正確的塑型參數");  
            };

            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(
                paramaters.Keyword, 
                paramaters.RatingOperator, 
                paramaters.RatingValue,
                paramaters2.PageNumber,
                paramaters2.PageSize,
                paramaters.OrderBy
                );
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("沒有旅遊路線");
            }



            var previousPageLink = touristRoutesFromRepo.HasPrevious    //判斷touristRoutesFromRepo是否存在上一頁
                ? GeneratieTouristRouteResourceURL(   //如果存在使用GeneratieTouristRouteResourceURL生成器來創建字串
                    paramaters, paramaters2, ResourceUrlType.PreviousPage) //三個參數 資料過濾,分頁參數,以及URL類型
                : null; //如果不存在上一頁的資料 previousPageLink 就為null

            var nextPageLink = touristRoutesFromRepo.HasNext
               ? GeneratieTouristRouteResourceURL(
                   paramaters, paramaters2, ResourceUrlType.NextPage)
               : null;
            //在頭部創建訊息 x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };
            Response.Headers.Add("x-pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            bool isHateoas = parsedMediaType.SubTypeWithoutSuffix //withoutSuffix 忽略返回類型，也就是忽略+json字串的部份
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase); //EndsWith獲得結尾字串
            //StringComparison.InvariantCultureIgnoreCase 忽略字串大小寫

            //判斷要獲得的isHateoas數據
            var primaryMediaType = isHateoas
                ? parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            //MAP支持列表映射，所以使用IEnumerable，而類型就是TouristRouteDto，最後加上數據源touristRoutesFromRepo
            // var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            //touristRoutesDtod輸出數據為IEnumerable，所以就可以直接使用shapeData拓展組件數據塑型
            // var shapeDtoList = touristRoutesDto.ShapeData(paramaters.Fields);

            //因為目前Dto返回的資料有兩種可能，所以要以object為基礎類型，創建一個可以動態識別的實例
            IEnumerable<object> touristRoutesDto;
            IEnumerable<ExpandoObject> shapedDtoList;

            if(primaryMediaType == "vnd.tourist.simplify")
            {
                touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteSimplifyDto>>(touristRoutesFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteSimplifyDto>)touristRoutesDto)
                    .ShapeData(paramaters.Fields);
            }
            else
            {
                touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteDto>)touristRoutesDto)
                    .ShapeData(paramaters.Fields);
            }

            if(isHateoas)
            {
                var linkDto = CreateLinksForTouristRouteList(paramaters, paramaters2);

                //C# LINQ 函數式編成處理forloop循環 
                var shapeDtoWtihLinkList = shapedDtoList.Select(t => {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute(
                        (Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });

                var reslut = new
                {
                    value = shapeDtoWtihLinkList,
                    links = linkDto
                };
                return Ok(reslut);
            }
            return Ok(shapedDtoList);
        }
        
        private IEnumerable<LinkDto> CreateLinksForTouristRouteList(
            TouristRouteResourceParamaters paramaters,
            PaginationResourceParamaters paramaters2
            )
        {
            var links = new List<LinkDto>();
            //添加self 自我連接
            links.Add(new LinkDto(
                    GeneratieTouristRouteResourceURL(paramaters,paramaters2,ResourceUrlType.CurrentPage),
                    "self",
                    "GET"
                    ));

            // "api/touristRotues"
            //添加創建旅遊路線
            links.Add(new LinkDto(
                    Url.Link("CreateTouristRoute",null),
                    "Create_tourist_route",
                    "POST"
                    ));

            return links;
        }

        //使用RESTful思想設計一個單一資源，最佳的實踐方式是在URL中使用名詞狀態的複數型，緊接著協槓ID.

        // api/touristroutes/{touristRouteId}
        //控制器的URL會映射到這個的前半部分也就是api/touristroutes
        //而我們的action函數只要後半部分
        //由於該這個參數要映射URL的動態變量所以要加上大括號，而該參數會被映射到GetTouristRouteById的參數中
        //另外需要注意touristRouteId可能會被輸入各種不同類型，不僅僅限於GUID，所以為了避免奇異，可在路由中加入類型的匹配 ":Guid"
        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(
            Guid touristRouteId,
            string fileds)
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
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
            //touristRoutesDtod輸出數據為Object，所以就可以直接使用shapeData拓展組件數據塑型

            //return Ok(touristRotueDto.ShapeData(fileds));
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fileds);

            var reslut = touristRotueDto.ShapeData(fileds)
                as IDictionary<string,object>;
            reslut.Add("links", linkDtos);
            return Ok(reslut);
        }
        private IEnumerable<LinkDto> CreateLinkForTouristRoute(
             Guid touristRouteId,
             string fileds)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(
                    Url.Link("GetTouristRouteById", new { touristRouteId, fileds }), //引用action函數的字串名稱
                    "self",
                    "GET"
                    )
                );
            //更新
            links.Add(
                new LinkDto(
                  Url.Link("UPdateTouristRoute", new { touristRouteId }),
                  "update",
                  "PUT"
                    )
                );
            //局部更新
            links.Add(
                new LinkDto(
                  Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                  "partially_update",
                  "PATCH"
                    )
                );
            //刪除
            links.Add(
                new LinkDto(
                  Url.Link("DeleteTouristRoute", new { touristRouteId }),
                  "delete",
                  "DELETE"
                    )
                );
            //獲取當前旅遊路線圖片
            links.Add(
                new LinkDto(
                  Url.Link("GetPictureListForTouristRotue", new { touristRouteId }),
                  "get_pictures",
                  "GET"
                    )
                );
            //添加新圖片
            links.Add(
                new LinkDto(
                  Url.Link("CreatTouristRoutePicture", new { touristRouteId }),
                  "create_pictures",
                  "POST"
                    )
                );
            return links;
        }



        [HttpPost(Name = "CreateTouristRoute")]
        //我們再用Identity框架的多角色驗證的默認中間件並不是JWT TOKEN，所以必須使用指定Bearer
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Authorize(Roles ="Admin")]
        //DTO是一種複雜的對象，而ASP.net自帶反序列化的功能，所以會將請求的主體內容解析，並且加載進入參數touristRouteForCreationDto中
        //接下來就可以使用此參數，並用此參數映射進入touristRoute的模型，最後通過數據倉庫來添加數據寫入資料庫
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);

            //轉化Dto到字典類型，首先要將Dto轉化成ExpandoObject
            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string,object>;
            result.Add("links", links);

            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = result["Id"] },
                result
                );

            //return CreatedAtRoute(
            //    "GetTouristRouteById",
            //    new { touristRouteId = touristRouteToReturn.Id },
            //    touristRouteToReturn
            //    );
        }

        [HttpPut("{touristRouteId}" , Name = "UPdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UPdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
            )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊訊息找不到");
            }
            //在數據倉庫中我們調用的是ORM框架在entityFrameWork中
            //在entityFrameWork中，touristRouteFromRepo是根據上下文對象context追中的
            //當我們在執行_mapper.Map這句程式碼時，資料模型的數據其實已經被修改了，而這個時候資料模型的追蹤狀態也就相應發生了變化
            //模型的追蹤狀態是由entity上下文關係對象context自我管理的，當我們使用資料庫保存時模型的追蹤狀態就會隨著context的保存寫入資料庫
            var touristRouteFromRepo =await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            //1.我們必須把touristRouteFromRepo所有的資料都提取出來，然後映射為DTO
            //2.更新DTO的資料
            //3.把更新後的DTO所有的資料再映射回到資料模型
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            //對於更新的響應，可以回傳200並在響應主體中包含更新後的數據資源，或者回傳204(No Content)返回一個完全不包含任何數據的響應
            return NoContent();
        }
        [HttpPatch("{touristRouteId}",Name = "PartiallyUpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
            )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊訊息找不到");
            }

            var touristRouteFromRepo =await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPacth = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPacth, ModelState);
            //ModelState是通過JSON PATCH的ApplyTo函數與DTO函數進行綁定，至於資料的驗證規則則是由DTO的data annotation來定義
            if (!TryValidateModel(touristRouteToPacth))
            {
                return ValidationProblem(ModelState);
            }
            //輸入數據touristRouteToPacth，輸出數據touristRouteFromRepo
            _mapper.Map(touristRouteToPacth, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
        [HttpDelete("{touristRouteId}",Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊訊息找不到");
            }
            var touristRoute =await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs(
            //[ModelBinder(binderType:typeof(ArrayModelBinder))] 解析并匹配 GUID (將route的數值轉換成GUID的類型)
            [ModelBinder(binderType:typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> touristIDs)
        {
            if(touristIDs == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
