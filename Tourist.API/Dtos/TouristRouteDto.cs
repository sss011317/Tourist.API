using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tourist.API.Dtos
{
    public class TouristRouteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //計算方式: 原價 X 折扣
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPresent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }
        public string TravelDays { get; set; }
        public string TripType { get; set; }
        public string DepartureCity { get; set; }

        //底下名詞TouristRoutePictures 一定要跟TouristRoute模型內的TouristRoutePictures名稱相同
        //這邊表示當autoMapper在兩個對象名詞完全一致的時候，會自動進行映射，而映射字段的類型是已經在profile註冊過的對象，那也不需做任何配置，autoMapper會接管所有的映射
        public ICollection<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}
