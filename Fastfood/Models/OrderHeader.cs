using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "تاریخ سفارش")]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "مبلغ اصلی")]
        public double OrderTotalOriginal { get; set; }
        [Required]
        [Display(Name = "مبلغ قابل پرداخت")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OrderTotal { get; set; }

        [Display(Name = "زمان ارسال")]
        public DateTime PickUpTime { get; set; }
        [Display(Name = "زمان تحویل")]
        [NotMapped]
        public DateTime PickUpDate { get; set; }
        [Display(Name = "کد کوپن")]
        public string CouponCode { get; set; }
        [Display(Name = "تخفیف کوپن")]
        public double CouponDiscount { get; set; }
        [Display(Name = "وضعیت سفارش")]
        public string State { get; set; }
        [Display(Name = "وضعیت پرداخت")]
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        [Display(Name = "نام ارسال کننده")]
        public string PickupName { get; set; }
        [Display(Name = "تلفن")]
        public string PhoneNumber { get; set; }
        public string TransactionId { get; set; }

    }
}
