using Fastfood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Utilities
{
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";

        public const string ssShopingCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "In Process";
        public const string StatusReady = "StatusReady";
        public const string StatusComplete = "Complete";
        public const string StatusCanceled = "Canceled";


        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";
        public static double DiscountPrice(Coupon coupon, double OriginalOrderTotal)
        {
            if (coupon == null)
                return OriginalOrderTotal;
            else
            {
                if (coupon.MiniAmount > OriginalOrderTotal)
                    return OriginalOrderTotal;
                else
                {
                    if (Convert.ToInt32(coupon.Coupontype) == (int)Coupon.ECouponType.Money)
                    {
                        return Math.Round(OriginalOrderTotal - coupon.Discount, 2);
                    }
                    if (Convert.ToInt32(coupon.Coupontype) == (int)Coupon.ECouponType.Percent)
                    {
                        return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * coupon.Discount / 100), 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }
    }
}
