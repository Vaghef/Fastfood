using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        [Key]
        public int Id { get; set; }
        [Display(Name = "مشتری")]
        public string ApplicationUserId { get; set; }
        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Display (Name = "منو")]
        public int MenuItemId { get; set; }
        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "حداقل آیتم خریداری یک می باشد")]
        [Display(Name = "تعداد")]
        public int Count { get; set; }
    }
}
