using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tourist.API.Dtos;
using Tourist.API.Models;

namespace Tourist.API.Profiles
{
    public class TouristRoutePictureProfile : Profile
    {
        public TouristRoutePictureProfile()
        {
            //第一個參數為模型的對象，第二個參數是映射的目標對象
            //默認情況下AutoMapper會自動映射兩個對象相同名稱的字段，而找不到的字段將被忽略 會使用null來代替
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>();

            CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
            CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();
        }
    }
}
