using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام زیر گروه الزامیست")]
        [Display(Name = "نام زیر گروه")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "نام فهرست")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
