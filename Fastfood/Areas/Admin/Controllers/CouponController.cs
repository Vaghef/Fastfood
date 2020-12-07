using Fastfood.Data;
using Fastfood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupons.ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if(files.Count > 0)
                {
                    byte[] byteArray = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using(var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            byteArray = ms1.ToArray();
                        }
                    }
                    coupon.Picture = byteArray;
                }

                _db.Coupons.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupon);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.FindAsync(id); ;

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (coupon == null)
                return NotFound();

            var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(woak => woak.Id == coupon.Id);

            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;


                if (files.Count > 0)
                {
                    byte[] byteArray = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            byteArray = ms1.ToArray();
                        }
                    }
                    couponFromDb.Picture = byteArray;
                }
                couponFromDb.MiniAmount = coupon.MiniAmount;
                couponFromDb.Name = coupon.Name;
                couponFromDb.Discount = coupon.Discount;
                couponFromDb.Coupontype = coupon.Coupontype;
                couponFromDb.IsActive = coupon.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupon);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.FindAsync(id);

            if (coupon == null)
                return NotFound(0);

            return View(coupon);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.FindAsync(id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var coupon = await _db.Coupons.SingleOrDefaultAsync(c => c.Id == id);

            _db.Coupons.Remove(coupon);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
