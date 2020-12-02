using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام فهرست الزامیست")]
        [Display(Name ="نام فهرست")]
        public string Name { get; set; }
    }
}
