﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title {  get; set; }

        public string Description { get; set; }

        [Required]
        public string ISBN {  get; set; }

        [Required]
        public string Author {  get; set; }

        [Required]
        [Display(Name = "List price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50-100")]
        [Range(1, 1000)]
        public double PriceForFifty {  get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double PriceForOneHundred { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public IList<ProductImage> ProductImages { get; set; }
    }
}
