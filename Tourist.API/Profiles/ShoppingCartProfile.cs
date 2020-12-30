using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Models;

namespace Tourist.API.Profiles
{
    public class ShoppingCartProfile:Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}
