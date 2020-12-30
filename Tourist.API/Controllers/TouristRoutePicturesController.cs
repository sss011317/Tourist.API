using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Models;
using Tourist.API.Services;

namespace Tourist.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

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
        public async Task<IActionResult> GetPictureListForTouristRotue(Guid touristRouteId)
        {
            if (!( await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }
            var pictureFromRepo =await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
            if(pictureFromRepo == null || pictureFromRepo.Count() <= 0)
            {
                return NotFound("照片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(pictureFromRepo));
        }
        [HttpGet("{pictureId}",Name = "GetPicture")]
        //實際上在設計RESTful API 處理向這種有父子關係或嵌套關係的資源時，我們首先要取得父資源，在父資源的基礎上再獲得子資源
        //如果連父資源都不存在，最好不要暴露子資源
        public async Task<IActionResult> GetPicture(Guid touristRouteId, int pictureId) 
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }

            var pictureFromRepo =await _touristRouteRepository.GetPictureAsync(pictureId);
            if(pictureFromRepo == null)
            {
                return NotFound("相片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatTouristRoutePicture(
            [FromRoute]Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId,pictureModel);
            await _touristRouteRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute(
                "GetPicture",
                new 
                { 
                    touristRouteId = pictureModel.TouristRouteId,
                    pictureId = pictureModel.Id
                },
                pictureToReturn
                );
        }
        [HttpDelete("{pictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePicture(
            [FromRoute] Guid touristRouteId,
            [FromRoute] int pictureId
            )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }
            var touristRoutePicture = await _touristRouteRepository.GetPictureAsync(pictureId);
            _touristRouteRepository.DeleteTouristRoutePicture(touristRoutePicture);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
