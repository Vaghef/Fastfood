using Fastfood.Data;
using Fastfood.Models;
using Fastfood.Models.ViewModels;
using Fastfood.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fastfood.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItems = await _context.MenuItems.Include(s => s.Category).Include(s => s.SubCategory).ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Coupons = await _context.Coupons.Where(woak => woak.IsActive == true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var count = _context.shoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;

                HttpContext.Session.SetInt32(SD.ssShopingCartCount, count);
            }
            return View(IndexVM);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemDb = await _context.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                MenuItem = menuItemDb,
                MenuItemId = menuItemDb.Id
            };

            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                cartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartDb = await _context.shoppingCarts.Where(c => c.ApplicationUserId == cartObj.ApplicationUserId && c.MenuItemId == cartObj.MenuItemId).FirstOrDefaultAsync();

                if (cartDb == null)
                    await _context.shoppingCarts.AddAsync(cartDb);
                else
                    cartDb.Count = cartDb.Count + cartObj.Count;

                await _context.SaveChangesAsync();

                var count = _context.shoppingCarts.Where(c => c.ApplicationUserId == cartObj.ApplicationUserId).ToList().Count;

                HttpContext.Session.SetInt32(SD.ssShopingCartCount, count);
                return RedirectToAction("Index");
            }
            else
            {
                var menuItemFromDb = await _context.MenuItems.Include(m => m.Category).Include(s => s.SubCategory).Where(m => m.Id == cartObj.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart cartObject = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(cartObject);
            }
        }
    }
}
