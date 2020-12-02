using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "نام کوپن")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "نوع کوپن")]
        public string Coupontype { get; set; }
        public enum ECouponType
        {
            Percent = 0,
            Money = 1
        }
        [Required]
        [Display(Name = "تخفیف")]
        public double Discount { get; set; }
        [Required]
        [Display(Name = "حداقل قیمت")]
        public double MiniAmount { get; set; }
        [Display(Name ="تصویر")]
        public byte[] Picture { get; set; }
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

    }
}
