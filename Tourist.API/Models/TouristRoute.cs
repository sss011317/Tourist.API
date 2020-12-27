using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tourist.API.Models
{
    public class TouristRoute
    {

        // 建立數據模型對資料庫的限定 (System.ComponentModel.DataAnnotations)
        // [Key] 為主鍵
        // [Required] 表示不能為空
        // [MaxLength(100)]限制字串數量
        // [Column(TypeName="decimal(18,2)")] 為加入有小數點
        // [Range(0.0,1.0)] 限制範圍

        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }
        [Column(TypeName="decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
        [Range(0.0,1.0)]
        public double? DiscountPresent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        [MaxLength]
        public string Features { get; set; }
        [MaxLength]
        public string Fees { get; set; }
        [MaxLength]
        public string Notes { get; set; }

        public ICollection<TouristRoutePicture> TouristRoutePictures { get; set; }
            = new List<TouristRoutePicture>();
        //給TouristRoutePictures初始化數值，避免一些未知的錯誤


        public double? Rating { get; set; }
        public TravelDays? TravelDays { get; set; }
        public TripType? TripType { get; set; }
        public DepartureCity? DepartureCity { get; set; }
    }
}
