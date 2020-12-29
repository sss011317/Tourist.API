using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tourist.API.Dtos;
using Tourist.API.Models;

namespace Tourist.API.Profiles
{
    public class TouristRouteProfile : Profile
    {
        public TouristRouteProfile()
        {
            //第一個參數為模型的對象，第二個參數是映射的目標對象
            //默認情況下AutoMapper會自動映射兩個對象相同名稱的字段，而找不到的字段將被忽略 會使用null來代替
            CreateMap<TouristRoute, TouristRouteDto>()
                .ForMember(
                    dest => dest.Price,
                    //src.DiscountPresent??1 為判斷 dsicount 為空的時候讓他輸出1 不為空的時候輸出本身的值
                    opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                )
                .ForMember(
                    //ForMember第一個對象為dest 投影的目標對象也就是TouristRouteDto
                    dest => dest.TravelDays,
                    //ForMember第二個對象為opt 原始對象 也就是TouristRoute 而對於計算投影的變化就是在這裡執行
                    opt => opt.MapFrom(src => src.TravelDays.ToString())
                )
                .ForMember(
                    dest => dest.TripType,
                    opt => opt.MapFrom(src => src.TripType.ToString())
                )
                .ForMember(
                    dest => dest.DepartureCity,
                    opt => opt.MapFrom(src => src.DepartureCity.ToString())
                );

            
            CreateMap<TouristRouteForCreationDto, TouristRoute>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => Guid.NewGuid())
                );
            CreateMap<TouristRouteForUpdateDto, TouristRoute>();

            CreateMap<TouristRoute, TouristRouteForUpdateDto>();
        }
    }
}
