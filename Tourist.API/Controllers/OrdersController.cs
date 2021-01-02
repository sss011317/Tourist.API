using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Models;
using Tourist.API.ResourceParameters;
using Tourist.API.Services;

namespace Tourist.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet(Name = "GetOrders")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] PaginationResourceParamaters paramaters //分頁參數處理器
            )
        {
            //1.獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2.使用userId獲得訂單紀錄
            var orders = await _touristRouteRepository.GetOrdersByUserId(userId,paramaters.PageNumber,paramaters.PageSize);
            //3.return
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute]Guid orderId)
        {
            //1.獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //2.使用orderId取得訂單詳細資料
            var order = await _touristRouteRepository.GetOrderById(orderId);
            //3.return
            return Ok(_mapper.Map<OrderDto>(order));
        }
        [HttpPost("{orderId}/placeOrder")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PlaceOrder([FromRoute]Guid orderId)
        {
            //1.獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //2.開始處理支付
            var order = await _touristRouteRepository.GetOrderById(orderId);
            order.PaymentProcessing();
            //目前訂單狀態的改變依然停留在內存中，希望在交由第三方處理前，先進行持久化，以免由於第三方問題導致訂單狀態不能即時保存
            await _touristRouteRepository.SaveAsync();
            //3.向第三方提交支付請求，等待第三方回應
            var httpClient = _httpClientFactory.CreateClient();
            string url = @"https://localhost:5001/api/FakeVanderPaymentProcess?orderNumber={0}&returnFault={1}";
            var response=await httpClient.PostAsync(
                string.Format(url, order.Id, false),
                null ///請求主體
                );
            //4.提取支付結果，以及支付訊息
            bool isApproved = false;
            string transactionMetadata = "";
            if (response.IsSuccessStatusCode)
            {
                transactionMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject =(JObject)JsonConvert.DeserializeObject(transactionMetadata);
                isApproved = jsonObject["approved"].Value<bool>();
            }
            //5.return
            if (isApproved)
            {
                order.PaymentApprove();
            }
            else
            {
                order.PaymentReject();
            }
            order.TransactionMetadata = transactionMetadata;
            await _touristRouteRepository.SaveAsync();
            return Ok(_mapper.Map<OrderDto>(order));
           
        }
    }
}
