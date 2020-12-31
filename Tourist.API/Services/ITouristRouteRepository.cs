using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Helper;
using Tourist.API.Models;
namespace Tourist.API.Services
{
    public interface ITouristRouteRepository
    {
        //返回一組的touristRoute
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword,string ratingOperator,int? ratingValue,
            int pageNumber , int pageSize,string orderBy);
        //返回一個的touristRoute
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);

        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids);

        Task<TouristRoutePicture> GetPictureAsync(int pictureId);

        void AddTouristRoute(TouristRoute touristRoute);
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);

        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);

        Task<ShoppingCart> GetShoppingCartByUserId(string userId);
        Task CreateShoppingCart(ShoppingCart shoppingCart);
        Task AddShoppingCartItem(LineItem lineItem);
        Task<LineItem> GetShoppingCartItemByItemId(int lineItemId);
        void DeleteShoppingCartItem(LineItem lineItem);
        Task<IEnumerable<LineItem>> GetshoppingCartsByIdListAsync(IEnumerable<int> ids);
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
        Task AddOrderAsync(Order order);
        Task<PaginationList<Order>> GetOrdersByUserId(string userId, int pageNumber,int pageSize);
        Task<Order> GetOrderById(Guid orderId);
        Task<bool> SaveAsync();
    }
}
