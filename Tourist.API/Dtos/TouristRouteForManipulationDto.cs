using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Tourist.API.ValidationAttributes;

namespace Tourist.API.Dtos
{
    //考慮這個父類不會被直接調用 所以給他申明變為抽象類abstract
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "title 不可為空")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        //計算方式: 原價 X 折扣
        public decimal Price { get; set; }
        //public decimal OriginalPrice { get; set; }
        //public double? DiscountPresent { get; set; }
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

        //使父子資源可同時創建
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
            = new List<TouristRoutePictureForCreationDto>();


        //public IEnumerable<ValidationResult> Validate(
        //    ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        //yield return 確保 return完 下次調用程式碼還會自動執行ValidationResult，而ValidationResult是可以提供錯誤訊息並返回類型或成員變量的對象
        //        yield return new ValidationResult(
        //            "路線名稱必須與路線描述不同",  //錯誤訊息
        //            new[] { "TouristRouteForCreationDto" }  //錯誤路徑
        //            );
        //    }
        //}
    }
}
