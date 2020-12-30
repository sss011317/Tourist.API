using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Models;

namespace Tourist.API.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}
