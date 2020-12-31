using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Database;
using Tourist.API.Helper;
using Tourist.API.Models;

namespace Tourist.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;

        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }

        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword, 
            string ratingOperator, 
            int? ratingValue,
            int pageNumber,
            int pageSize,
            string orderBy
            )
        {
            //IQueryable 實際上是c# LINQ to SQL 語句的返回類型 簡單來說 IQueryable可以疊加處理LINQ語句，最後統一訪問資料庫，處理過程也較延遲執行
            //每次進行資料庫操作實際上都是1次IO操作，而系統中的IO操作開銷是最大也是最浪費時間
            //而IQueryable就是為SQL設計的，他在創建時是以Expression Tree(表達式目錄形式構成)，有就是可以不斷地給這棵樹新的表達式，之後再一次表所有表達式轉換給SQL語言，統一進行
            IQueryable<TouristRoute> result = _context
                .TouristRoutes
                .Include(t => t.TouristRoutePictures);
            //上述該行只是生成了SQL語句，並沒有執行查詢操作
            if(!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                if(orderBy.ToLowerInvariant() == "originalprice")
                {
                    result = result.OrderBy(t => t.OriginalPrice);
                }
            }
            if (ratingValue >= 0)
            {
                switch (ratingOperator)
                {
                    case "largerThan":
                        result = result.Where(t => t.Rating >= ratingValue);
                        break;
                    case "lessThan":
                        result = result.Where(t => t.Rating <= ratingValue);
                        break;
                    case "equalTo":
                        result = result.Where(t => t.Rating == ratingValue);
                        break;
                }
            }

            //include函數為entityFramework中連接兩張表的方法 表示兩張表通過外部進行連接
            //join函數為不通過外界而是手動表連接的屬性
            //通過上述兩種方法可以進行立即加載(Eager Load)
            //另外entityFramework也提供另一種加載方式延遲加載(Lazy Load)，也就是不用join或include進行表連接
            return await PaginationList<TouristRoute>.CreateAsync(pageNumber,pageSize,result);
            //ToList為IQueryable內建函數，通過調用此函數，就會執行資料庫的訪問，而查詢出來的數據類型則不是IQueryable而是<TouristRoute>類型
        }

        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            //只要輸入的ID能有任何返回的數據，則這個ANY就返回true

            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);
        }
        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId).ToArrayAsync();
        }
        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.Where(p => p.Id ==pictureId).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToArrayAsync();
        }
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
            //_context.SaveChanges();  保存至資料庫
        }
        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if(touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRouteId));
            }
            if(touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }
        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }
        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }
        public void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture)
        {
            _context.TouristRoutePictures.Remove(touristRoutePicture);
        }

        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User) //shopping cart表 與 user表 連接
                .Include(s => s.ShoppingCartItems) //shopping car表 與 lineItem表 連接
                .ThenInclude(li => li.touristRoute) //獲得旅遊路線的具體數據
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }
        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }
        public async Task AddShoppingCartItem(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }
        public async Task<LineItem> GetShoppingCartItemByItemId(int lineItemId)
        {
           return await _context.LineItems
                .Where(li=>li.Id==lineItemId)
                .FirstOrDefaultAsync();
        }
        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }
        public async Task<IEnumerable<LineItem>> GetshoppingCartsByIdListAsync(IEnumerable<int> ids)
        {
            //ids.Contain(li.Id) 用li.Id在ids列表中判斷是否存在
            return await _context.LineItems
                .Where(li => ids.Contains(li.Id))
                .ToListAsync();
        }
        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }
        public async Task AddOrderAsync(Order order)
        {
             await _context.AddAsync(order);
        }
        public async Task<PaginationList<Order>> GetOrdersByUserId(string userId, int pageNumber, int pageSize)
        {
            IQueryable<Order> result = _context.Orders.Where(o => o.UserId == userId);
            return await PaginationList<Order>.CreateAsync(pageNumber,pageSize,result);
            //return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        }
        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)//獲得訂單內產品列表
                .ThenInclude(oi => oi.touristRoute) //在獲得產品列表的基礎上，進一步連結旅遊路線table獲取旅遊路線具體資料
                .Where(o => o.Id == orderId)//最後過濾數據
                .FirstOrDefaultAsync();
        }
        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >=0);
        }
    }
}
