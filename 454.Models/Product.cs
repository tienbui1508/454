﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace _454.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public string Author { get; set; }
        public string Description { get; set; }
  

        [Required]
        [DisplayName("List price")]
        [Range(1, 1000)]
        public Double ListPrice { get; set; }

        [Required]
        [DisplayName("Price for 1-4")]
        [Range(1, 1000)]
        public Double Price { get; set; }

        [Required]
        [DisplayName("Price for 5-9")]
        [Range(1, 1000)]
        public Double Price5 { get; set; }

        [Required]
        [DisplayName("Price for 10+")]
        [Range(1, 1000)]
        public Double Price10 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]

        public Category Category { get; set; }
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }


    }
}
