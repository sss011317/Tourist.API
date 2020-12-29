﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.ValidationAttributes;

namespace Tourist.API.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public class TouristRouteForUpdateDto : TouristRouteForManipulationDto
    {

        [Required(ErrorMessage ="更新必備")]
        [MaxLength(1500)]
        public override string Description { get; set; }
 
    }
}
