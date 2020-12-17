using Fastfood.Data;
using Fastfood.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Fastfood.Utilities;
using Fastfood.Models;

namespace Fastfood.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.shoppingCarts.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
                detailCart.listCart = cart.ToList();

            foreach(var list in detailCart.listCart)
            {
                list.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(woak => woak.Id == list.MenuItemId);

                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);

                if (list.MenuItem.Description.Length > 100)
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                    
            }

            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;

            if(HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponDb = await _db.Coupons.Where(c => c.Name == detailCart.OrderHeader.CouponCode).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountPrice(couponDb, detailCart.OrderHeader.OrderTotalOriginal);
            }

            return View(detailCart);
        }

        public async Task<IActionResult> Summary()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = await _db.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _db.shoppingCarts.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
                detailCart.listCart = cart.ToList();

            foreach (var list in detailCart.listCart)
            {
                list.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(woak => woak.Id == list.MenuItemId);

                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);

            }

            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;
            detailCart.OrderHeader.PickupName = applicationUser.Name;
            detailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            detailCart.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponDb = await _db.Coupons.Where(c => c.Name == detailCart.OrderHeader.CouponCode).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountPrice(couponDb, detailCart.OrderHeader.OrderTotalOriginal);
            }

            return View(detailCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailCart.listCart = await _db.shoppingCarts.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();
            
            detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claim.Value;
            detailCart.OrderHeader.State = SD.PaymentStatusPending;
            detailCart.OrderHeader.PickUpTime = DateTime.Now.AddHours(2);


            List<OrderDetail> orderDetailList = new List<OrderDetail>();
            _db.OrderHeaders.Add(detailCart.OrderHeader);
            await _db.SaveChangesAsync();

            detailCart.OrderHeader.OrderTotalOriginal = 0;

            foreach (var item in detailCart.listCart)
            {
                item.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);

                OrderDetail orderDetails = new OrderDetail()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = detailCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Price =item.MenuItem.Price,
                    Count = item.Count
                    
                };
                detailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);
            }

            if(HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupons.Where(c => c.Name == detailCart.OrderHeader.CouponCode).FirstOrDefaultAsync();

                detailCart.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotalOriginal;
            }
            detailCart.OrderHeader.CouponDiscount = detailCart.OrderHeader.OrderTotalOriginal - detailCart.OrderHeader.OrderTotal;

            _db.shoppingCarts.RemoveRange(detailCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShopingCartCount, 0);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddCoupon()
        {
            if (detailCart.OrderHeader.CouponCode == null)
                detailCart.OrderHeader.CouponCode = string.Empty;
            HttpContext.Session.SetString(SD.ssCouponCode, detailCart.OrderHeader.CouponCode);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.shoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);

            cart.Count += 1;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.shoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);

            if(cart.Count == 1)
            {
                _db.shoppingCarts.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.shoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;

                HttpContext.Session.SetInt32(SD.ssShopingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.shoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);

            _db.shoppingCarts.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.shoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShopingCartCount, cnt);


            return RedirectToAction(nameof(Index));
        }
    }
}
