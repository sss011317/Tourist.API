using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tourist.API.Models
{
    public class TouristRoutePicture
    {
        //資料庫自己新建ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        [MaxLength(100)]
        public string Url { get; set; }
        //ForeignKey為關聯KEY，要注意 EntityFrameWork 會將class名稱與id加起來變成主鍵，所以要與TouristRoute內的ID進行關聯 就要如此輸入
        [ForeignKey("TouristRouteId")]
       
        public Guid TouristRouteId { get; set; }
        public TouristRoute TouristRoute { get; set; }
    }
}
