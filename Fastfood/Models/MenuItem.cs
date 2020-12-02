using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "نام منو")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        public string Spicyness { get; set; }
        public enum ESpicy
        {
            [Display(Name = " ")]
            NA = 0,
            [Display(Name = "ساده")]
            NotSpicy = 1,
            [Display(Name = "تند")]
            Spicy = 2,
            [Display(Name = "خیلی تند")]
            VerySpicy = 3
        }
        [Display(Name = "تصویر")]
        public string Image { get; set; }
        [Display(Name = "فهرست")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Display(Name = "زیرگروه")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
        [Display(Name = "قیمت")]
        public double Price { get; set; }
    }
}
