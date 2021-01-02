using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;

namespace Tourist.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RouteController : ControllerBase
    {
        [HttpGet(Name ="GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();
            //自我連接
            links.Add(
                new LinkDto(
                    Url.Link("GetRoot", null),
                    "self",
                    "GET"
                ));
            //一級連接 旅遊路線 "GET api/touristRotes"
            links.Add(
                new LinkDto(
                    Url.Link("GetTouristRoutes", null),
                    "get_tourist_routes",
                    "GET"
                ));
            //一級連接 旅遊路線 "POST api/touristRotes"
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_routes",
                    "POST"
                ));
            //一級連接 購物車 "GET api/shoppingCart"
            links.Add(
                new LinkDto(
                    Url.Link("GetShoppingCart", null),
                    "get_shopping_cart",
                    "GEt"
                ));
            //一級連接 訂單 "POST api/orders"
            links.Add(
                new LinkDto(
                    Url.Link("GetOrders", null),
                    "get_orders",
                    "POST"
                ));

            return Ok(links);
        }
    }
}
