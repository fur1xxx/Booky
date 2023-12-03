using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Street adress")]
        public string? StreetAdress { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        [DisplayName("Postal code")]
        public string? PostalCode { get; set; }

        [DisplayName("Phone number")]
        public string? PhoneNumber { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }   
    }
}
