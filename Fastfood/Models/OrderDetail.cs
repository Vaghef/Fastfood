using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [Display(Name = "سفارش")]
        public virtual OrderHeader OrderHeader { get; set; }
        [Required]
        [Display(Name = "اقلام")]
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
        [Display(Name = "تعداد")]
        public int Count { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "مبلغ")]
        public double Price { get; set; }

    }
}
