using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Models;

namespace Tourist.API.Dtos
{
    public class LineItemDto
    {
        public int Id { get; set; }
        public Guid TouristRouteId { get; set; }
        public TouristRouteDto touristRoute { get; set; }
        public Guid? ShoppingCartId { get; set; }
        //public Guid? OrderId { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPresent { get; set; }
    }
}
