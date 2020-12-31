using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Models;

namespace Tourist.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember( //列舉類型(enum)無法進行映射
                    dest => dest.State, //dest 目標數據
                    opt => //opt數據元
                    {
                        opt.MapFrom(src => src.State.ToString()); //用mapFrom做資料映射
                    }
                );
        }
    }
}
