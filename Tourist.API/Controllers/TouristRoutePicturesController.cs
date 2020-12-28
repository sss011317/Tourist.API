using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Services;

namespace Tourist.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;

        public TouristRoutePicturesController(
                ITouristRouteRepository touristRouteRepository,
                IMapper mapper
            )
        {
            _touristRouteRepository = touristRouteRepository ??
                throw new ArgumentException(nameof(touristRouteRepository));
            _mapper = mapper ??
                throw new ArgumentException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetPictureListForTouristRotue(Guid touristRouteId)
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅遊路線不存在");
            }
            var pictureFromRepo = _touristRouteRepository.GetPicturesByTouristRouteId(touristRouteId);
            if(pictureFromRepo == null || pictureFromRepo.Count() <= 0)
            {
                return NotFound("照片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(pictureFromRepo));
        }
        [HttpGet("{pictureId}")]
        //實際上在設計RESTful API 處理向這種有父子關係或嵌套關係的資源時，我們首先要取得父資源，在父資源的基礎上再獲得子資源
        //如果連父資源都不存在，最好不要暴露子資源
        public IActionResult GetPicture(Guid touristRouteId, int pictureId) 
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅遊路線不存在");
            }

            var pictureFromRepo = _touristRouteRepository.GetPicture(pictureId);
            if(pictureFromRepo == null)
            {
                return NotFound("相片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }
    }
}
