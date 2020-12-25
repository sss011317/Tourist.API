using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tourist.API.Models
{
    public class TouristRoutePicture
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public Guid ToureistRouteId { get; set; }
        public TouristRoute TouristRoute { get; set; }
    }
}
