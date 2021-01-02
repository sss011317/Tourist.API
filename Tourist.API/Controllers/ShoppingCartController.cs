using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Helper;
using Tourist.API.Models;
using Tourist.API.Services;

namespace Tourist.API.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetShoppingCart")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            //1.獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2.使用userId獲得購物車
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem(
            [FromBody] AddShoppingCartDto addShoppingCartDto
            )
        {
            //1.獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2.使用userId獲得購物車
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);
            //3.創建lineItem
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("旅遊路線不存在");
            }
            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCartDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };

            //4.添加lineItem並保存資料庫
            await _touristRouteRepository.AddShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem(
            [FromRoute] int itemId
            )
        {
            //1.獲取lineitem數據
            var lineItem = await _touristRouteRepository.GetShoppingCartItemByItemId(itemId);
            if (lineItem == null)
            {
                return NotFound("購物車找不到該商品");
            }
            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("items/({itemIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItems(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            [FromRoute] IEnumerable<int> itemIds
            )
        {
            var lineItems = await _touristRouteRepository.GetshoppingCartsByIdListAsync(itemIds);
            if (lineItems == null)
            {
                return NotFound("購物車找不到該商品");
            }
            _touristRouteRepository.DeleteShoppingCartItems(lineItems);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Checkout()
        {
            //1.獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2.使用userId獲得購物車
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            //3.創建訂單
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                OrderItems = shoppingCart.ShoppingCartItems,
                CreateDateUTC = DateTime.UtcNow
            };

            shoppingCart.ShoppingCartItems = null;
            //4.保存數據
            await _touristRouteRepository.AddOrderAsync(order);
            await _touristRouteRepository.SaveAsync();
            //5.return
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }

}
